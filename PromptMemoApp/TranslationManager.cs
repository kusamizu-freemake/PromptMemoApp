using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// DeepL APIを使用した翻訳機能を提供するクラス
    /// APIキーの管理と翻訳処理を担当
    /// </summary>
    public class TranslationManager : IDisposable
    {
        #region 定数
        private const string DEEPL_TRANSLATE_URL = "https://api-free.deepl.com/v2/translate";
        private const string CONFIG_FILE_NAME = "config.json";
        private const string UNKNOWN_LANGUAGE = "UNKNOWN";
        private const string ERROR_TITLE = "エラー";
        #endregion

        #region フィールド
        private string _apiKey;
        private readonly HttpClient _httpClient;
        private bool _disposed = false;
        #endregion

        #region プロパティ
        /// <summary>
        /// APIキーが設定されているかどうかを取得します
        /// </summary>
        public bool HasApiKey => !string.IsNullOrEmpty(_apiKey);
        #endregion

        #region コンストラクタ
        /// <summary>
        /// TranslationManager の新しいインスタンスを初期化します
        /// </summary>
        public TranslationManager()
        {
            _httpClient = new HttpClient();
            LoadApiKeyFromConfig();
        }
        #endregion

        #region パブリックメソッド
        /// <summary>
        /// APIキーを設定し、設定ファイルに保存します
        /// </summary>
        /// <param name="apiKey">設定するAPIキー</param>
        /// <exception cref="ArgumentException">APIキーが無効な場合</exception>
        public void SetApiKey(string apiKey)
        {
            ValidateApiKey(apiKey);
            _apiKey = apiKey;
            SaveApiKeyToConfig();
        }

        /// <summary>
        /// テキストを指定された言語に翻訳します
        /// </summary>
        /// <param name="text">翻訳するテキスト</param>
        /// <param name="sourceLang">翻訳元言語コード</param>
        /// <param name="targetLang">翻訳先言語コード</param>
        /// <returns>翻訳されたテキスト</returns>
        /// <exception cref="InvalidOperationException">APIキーが未設定の場合</exception>
        /// <exception cref="ArgumentException">引数が無効な場合</exception>
        /// <exception cref="TranslationException">翻訳処理でエラーが発生した場合</exception>
        public async Task<string> TranslateAsync(string text, string sourceLang, string targetLang)
        {
            EnsureApiKeyIsSet();
            ValidateTranslationParameters(text, sourceLang, targetLang);

            // 空文字列の場合はそのまま返す
            if (string.IsNullOrWhiteSpace(text))
                return text;

            try
            {
                var requestData = CreateTranslationRequestData(text, sourceLang, targetLang);
                var response = await SendTranslationRequestAsync(requestData);
                return ExtractTranslatedText(response, text);
            }
            catch (Exception ex) when (!(ex is TranslationException))
            {
                LogError("翻訳処理", ex);
                throw new TranslationException("翻訳中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// テキストの言語を自動検出します
        /// </summary>
        /// <param name="text">検出するテキスト</param>
        /// <returns>検出された言語コード</returns>
        /// <exception cref="InvalidOperationException">APIキーが未設定の場合</exception>
        /// <exception cref="ArgumentException">テキストが無効な場合</exception>
        /// <exception cref="TranslationException">言語検出でエラーが発生した場合</exception>
        public async Task<string> DetectLanguageAsync(string text)
        {
            EnsureApiKeyIsSet();

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("検出対象のテキストを指定してください。", nameof(text));

            try
            {
                // 翻訳APIを使用して言語検出（DeepLには専用の言語検出APIがないため）
                var requestData = CreateLanguageDetectionRequestData(text);
                var response = await SendTranslationRequestAsync(requestData);
                return ExtractDetectedLanguage(response);
            }
            catch (Exception ex) when (!(ex is TranslationException))
            {
                LogError("言語検出処理", ex);
                throw new TranslationException("言語検出中にエラーが発生しました。", ex);
            }
        }
        #endregion

        #region プライベートメソッド - API通信
        /// <summary>
        /// 翻訳リクエストデータを作成します
        /// </summary>
        private FormUrlEncodedContent CreateTranslationRequestData(string text, string sourceLang, string targetLang)
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("auth_key", _apiKey),
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("source_lang", sourceLang),
                new KeyValuePair<string, string>("target_lang", targetLang)
            });
        }

        /// <summary>
        /// 言語検出用のリクエストデータを作成します
        /// </summary>
        private FormUrlEncodedContent CreateLanguageDetectionRequestData(string text)
        {
            // 言語検出のため、適当なターゲット言語を指定して翻訳APIを呼び出す
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("auth_key", _apiKey),
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("target_lang", "EN") // 検出用の仮ターゲット
            });
        }

        /// <summary>
        /// 翻訳APIにリクエストを送信します
        /// </summary>
        private async Task<DeepLResponse> SendTranslationRequestAsync(FormUrlEncodedContent requestData)
        {
            LogDebugInfo($"DeepL APIリクエスト送信開始");

            var response = await _httpClient.PostAsync(DEEPL_TRANSLATE_URL, requestData);
            var responseContent = await response.Content.ReadAsStringAsync();

            LogDebugInfo($"DeepL API レスポンス: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new TranslationException($"API呼び出しが失敗しました。ステータス: {response.StatusCode}, 内容: {responseContent}");
            }

            return DeserializeResponse(responseContent);
        }

        /// <summary>
        /// APIレスポンスをデシリアライズします
        /// </summary>
        private DeepLResponse DeserializeResponse(string responseContent)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<DeepLResponse>(responseContent, options);
            }
            catch (JsonException ex)
            {
                throw new TranslationException("APIレスポンスの解析に失敗しました。", ex);
            }
        }

        /// <summary>
        /// レスポンスから翻訳テキストを抽出します
        /// </summary>
        private string ExtractTranslatedText(DeepLResponse response, string originalText)
        {
            if (response?.Translations == null || response.Translations.Count == 0)
            {
                LogDebugInfo("翻訳結果が空のため、元のテキストを返します");
                return originalText;
            }

            return response.Translations[0].Text ?? originalText;
        }

        /// <summary>
        /// レスポンスから検出された言語を抽出します
        /// </summary>
        private string ExtractDetectedLanguage(DeepLResponse response)
        {
            if (response?.Translations == null || response.Translations.Count == 0)
            {
                return UNKNOWN_LANGUAGE;
            }

            return response.Translations[0].DetectedSourceLanguage ?? UNKNOWN_LANGUAGE;
        }
        #endregion

        #region プライベートメソッド - 設定管理
        /// <summary>
        /// 設定ファイルからAPIキーを読み込みます
        /// </summary>
        private void LoadApiKeyFromConfig()
        {
            var configPath = GetConfigFilePath();
            if (!File.Exists(configPath))
            {
                _apiKey = string.Empty;
                return;
            }

            try
            {
                var jsonContent = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<Config>(jsonContent);
                _apiKey = config?.DeepLApiKey ?? string.Empty;

                LogDebugInfo("設定ファイルからAPIキーを読み込みました");
            }
            catch (Exception ex)
            {
                LogError("設定ファイル読み込み", ex);
                _apiKey = string.Empty;
            }
        }

        /// <summary>
        /// APIキーを設定ファイルに保存します
        /// </summary>
        private void SaveApiKeyToConfig()
        {
            var configPath = GetConfigFilePath();
            try
            {
                var config = new Config { DeepLApiKey = _apiKey };
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonContent = JsonSerializer.Serialize(config, options);

                File.WriteAllText(configPath, jsonContent);
                LogDebugInfo("設定ファイルにAPIキーを保存しました");
            }
            catch (Exception ex)
            {
                LogError("設定ファイル保存", ex);
                ShowErrorMessage($"設定の保存に失敗しました: {ex.Message}");
            }
        }

        /// <summary>
        /// 設定ファイルのパスを取得します
        /// </summary>
        private string GetConfigFilePath()
        {
            return Path.Combine(Application.StartupPath, CONFIG_FILE_NAME);
        }
        #endregion

        #region プライベートメソッド - バリデーション
        /// <summary>
        /// APIキーが設定されていることを確認します
        /// </summary>
        private void EnsureApiKeyIsSet()
        {
            if (!HasApiKey)
            {
                throw new InvalidOperationException("APIキーが設定されていません。先にAPIキーを設定してください。");
            }
        }

        /// <summary>
        /// APIキーの妥当性を検証します
        /// </summary>
        private void ValidateApiKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("APIキーを指定してください。", nameof(apiKey));
            }

            if (apiKey.Length < 10)
            {
                throw new ArgumentException("APIキーの形式が正しくありません。", nameof(apiKey));
            }
        }

        /// <summary>
        /// 翻訳パラメータの妥当性を検証します
        /// </summary>
        private void ValidateTranslationParameters(string text, string sourceLang, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(sourceLang))
                throw new ArgumentException("翻訳元言語を指定してください。", nameof(sourceLang));

            if (string.IsNullOrWhiteSpace(targetLang))
                throw new ArgumentException("翻訳先言語を指定してください。", nameof(targetLang));
        }
        #endregion

        #region プライベートメソッド - ユーティリティ
        /// <summary>
        /// デバッグ情報をログに出力します
        /// </summary>
        private void LogDebugInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] TranslationManager: {message}");
        }

        /// <summary>
        /// エラー情報をログに出力します
        /// </summary>
        private void LogError(string operation, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] TranslationManager {operation}: {ex}");
        }

        /// <summary>
        /// ユーザーにエラーメッセージを表示します
        /// </summary>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region IDisposable実装
        /// <summary>
        /// リソースを解放します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースを解放します
        /// </summary>
        /// <param name="disposing">マネージドリソースも解放する場合は true</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
        #endregion
    }

    #region データクラス
    /// <summary>
    /// DeepL APIのレスポンス形式
    /// </summary>
    public class DeepLResponse
    {
        public List<Translation> Translations { get; set; }
    }

    /// <summary>
    /// 翻訳結果の個別データ
    /// </summary>
    public class Translation
    {
        public string DetectedSourceLanguage { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// 設定ファイルの形式
    /// </summary>
    public class Config
    {
        public string DeepLApiKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// 翻訳処理固有の例外クラス
    /// </summary>
    public class TranslationException : Exception
    {
        public TranslationException(string message) : base(message) { }
        public TranslationException(string message, Exception innerException) : base(message, innerException) { }
    }
    #endregion
}