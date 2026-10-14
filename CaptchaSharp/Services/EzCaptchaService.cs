using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace YourNamespace.Services
{
    public class NoCaptchaAiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.nocaptchaai.com";

        /// <summary>
        /// Initializes the NoCaptchaAI service with your unique client Key.
        /// </summary>
        /// <param name="clientKey">Your NoCaptchaAI dashboard clientKey</param>
        public NoCaptchaAiService(string clientKey)
        {
            if (string.IsNullOrWhiteSpace(clientKey))
                throw new ArgumentException("API clientKey cannot be empty.", nameof(clientKey));

            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // Authentication requires the key inside the headers
            _httpClient.DefaultRequestHeaders.Add("clientKey", clientKey);
        }

        /// <summary>
        /// Submits a CAPTCHA challenge and polls until the token solution is ready.
        /// </summary>
        public async Task<string> SolveCaptchaAsync(string captchaType, string siteKey, string pageUrl, CancellationToken cancellationToken = default)
        {
            // 1. Create the task
            var taskPayload = new CaptchaTaskRequest
            {
                Type = captchaType,
                SiteKey = siteKey,
                PageUrl = pageUrl
            };

            string jsonPayload = JsonSerializer.Serialize(taskPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage createResponse = await _httpClient.PostAsync("/createTask", content, cancellationToken);
            createResponse.EnsureSuccessStatusCode();

            string createResultJson = await createResponse.Content.ReadAsStringAsync(cancellationToken);
            var taskResponse = JsonSerializer.Deserialize<CaptchaTaskResponse>(createResultJson);

            if (taskResponse == null || string.IsNullOrEmpty(taskResponse.TaskId))
            {
                throw new Exception($"Failed to create task. API Response: {createResultJson}");
            }

            // If the sync resolver immediately outputs a token, use it
            if (taskResponse.Status == "solved" && !string.IsNullOrEmpty(taskResponse.Solution?.Token))
            {
                return taskResponse.Solution.Token;
            }

            // 2. Poll for the result
            return await PollForResultAsync(taskResponse.TaskId, cancellationToken);
        }

        private async Task<string> PollForResultAsync(string taskId, CancellationToken cancellationToken)
        {
            var resultPayload = new { taskId = taskId };
            string jsonPayload = JsonSerializer.Serialize(resultPayload);

            // Poll maximum of 30 times (approx 30-45 seconds)
            for (int i = 0; i < 30; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage resultResponse = await _httpClient.PostAsync("/getTaskResult", content, cancellationToken);
                
                if (resultResponse.IsSuccessStatusCode)
                {
                    string resultJson = await resultResponse.Content.ReadAsStringAsync(cancellationToken);
                    var taskResult = JsonSerializer.Deserialize<CaptchaTaskResponse>(resultJson);

                    if (taskResult != null)
                    {
                        if (taskResult.Status == "solved" && taskResult.Solution?.Token != null)
                        {
                            return taskResult.Solution.Token;
                        }
                        
                        if (taskResult.Status == "failed")
                        {
                            throw new Exception($"NoCaptchaAI processing failed: {resultJson}");
                        }
                    }
                }

                // Wait 1.5 seconds before checking again
                await Task.Delay(1500, cancellationToken);
            }

            throw new TimeoutException("The CAPTCHA solving operation timed out.");
        }

        #region DTO Models
        private class CaptchaTaskRequest
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("sitekey")]
            public string SiteKey { get; set; } = string.Empty;

            [JsonPropertyName("url")]
            public string PageUrl { get; set; } = string.Empty;
        }

        private class CaptchaTaskResponse
        {
            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;

            [JsonPropertyName("taskId")]
            public string TaskId { get; set; } = string.Empty;

            [JsonPropertyName("solution")]
            public SolutionData Solution { get; set; }
        }

        private class SolutionData
        {
            [JsonPropertyName("gRecaptchaResponse")]
            [JsonPropertyName("token")] // Handles variations across specific endpoint responses
            public string Token { get; set; } = string.Empty;
        }
        #endregion
    }
}
