using System.Text;
using System.Text.Json;

namespace Fanex.Alpha.Before.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<string> PostAsStringAsync(this HttpClient httpClient, string requestUri, object payload = null)
        {
            using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<T> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, object payload = null)
        {
            var response = await httpClient.PostAsStringAsync(requestUri, payload);
            return JsonSerializer.Deserialize<T>(response);
        }

    }
}