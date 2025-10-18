using Newtonsoft.Json;
using System.Text;

namespace BillOrganizerAPI.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["GeminiAPI:ApiKey"];
        }

        public async Task<string> ExtractBillDataAsync(string base64Image)
        {
            string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key=" + _apiKey;
            string prompt = "Extract merchant name, date YYYY-MM-DD, total amount, category, and items with prices from this receipt. Return only valid JSON with fields: merchantName, date, totalAmount, category, items array with name price quantity.";

            var body = new { contents = new[] { new { parts = new object[] { new { text = prompt }, new { inline_data = new { mime_type = "image/jpeg", data = base64Image } } } } } };

            string json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);
            return await response.Content.ReadAsStringAsync();
        }

        public string ParseGeminiResponse(string response)
        {
            dynamic obj = JsonConvert.DeserializeObject(response);
            string text = obj.candidates[0].content.parts[0].text.ToString();

            if (text.Contains("```json"))
            {
                int start = text.IndexOf("```json") + 7;
                int end = text.LastIndexOf("```");
                text = text.Substring(start, end - start).Trim();
            }
            else if (text.Contains("```"))
            {
                int start = text.IndexOf("```") + 3;
                int end = text.LastIndexOf("```");
                text = text.Substring(start, end - start).Trim();
            }

            return text;
        }
    }
}