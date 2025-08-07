using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public class TranslationManager
    {
        private string apiKey;
        private readonly HttpClient httpClient;
        private const string DeepLApiUrl = "https://api-free.deepl.com/v2/translate";

        public TranslationManager()
        {
            httpClient = new HttpClient();
            LoadApiKey();
        }

        public bool HasApiKey 
        { 
            get { return !string.IsNullOrEmpty(apiKey); } 
        }

        public void SetApiKey(string key)
        {
            apiKey = key;
            SaveApiKey();
        }

        public async Task<string> TranslateAsync(string text, string targetLang = "EN")
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("DeepL APIキーが設定されていません。");
            }

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("auth_key", apiKey),
                    new KeyValuePair<string, string>("text", text),
                    new KeyValuePair<string, string>("target_lang", targetLang)
                });

                var response = await httpClient.PostAsync(DeepLApiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var translationResponse = JsonSerializer.Deserialize<DeepLResponse>(responseContent);
                    if (translationResponse != null && translationResponse.Translations != null && translationResponse.Translations.Count > 0)
                    {
                        return translationResponse.Translations[0].Text != null ? translationResponse.Translations[0].Text : text;
                    }
                    return text;
                }
                else
                {
                    throw new Exception($"翻訳エラー: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"翻訳中にエラーが発生しました: {ex.Message}");
            }
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("DeepL APIキーが設定されていません。");
            }

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("auth_key", apiKey),
                    new KeyValuePair<string, string>("text", text)
                });

                var response = await httpClient.PostAsync("https://api-free.deepl.com/v2/translate", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var translationResponse = JsonSerializer.Deserialize<DeepLResponse>(responseContent);
                    if (translationResponse != null && translationResponse.Translations != null && translationResponse.Translations.Count > 0)
                    {
                        return translationResponse.Translations[0].DetectedSourceLanguage != null ? translationResponse.Translations[0].DetectedSourceLanguage : "UNKNOWN";
                    }
                    return "UNKNOWN";
                }
                else
                {
                    throw new Exception($"言語検出エラー: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"言語検出中にエラーが発生しました: {ex.Message}");
            }
        }

        private void LoadApiKey()
        {
            var configPath = Path.Combine(Application.StartupPath, "config.json");
            if (File.Exists(configPath))
            {
                try
                {
                    var json = File.ReadAllText(configPath);
                                var config = JsonSerializer.Deserialize<Config>(json);
            apiKey = (config != null && config.DeepLApiKey != null) ? config.DeepLApiKey : "";
                }
                catch
                {
                    apiKey = "";
                }
            }
        }

        private void SaveApiKey()
        {
            var configPath = Path.Combine(Application.StartupPath, "config.json");
            try
            {
                var config = new Config { DeepLApiKey = apiKey };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定の保存に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }
        }
    }

    public class DeepLResponse
    {
        public List<Translation> Translations { get; set; }
    }

    public class Translation
    {
        public string DetectedSourceLanguage { get; set; }
        public string Text { get; set; }
    }

    public class Config
    {
        public string DeepLApiKey { get; set; }

        public Config()
        {
            DeepLApiKey = "";
        }
    }
}
