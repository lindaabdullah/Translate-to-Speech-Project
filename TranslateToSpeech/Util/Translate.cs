using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TranslateToSpeech.Util
{
	public class Translate
    {
        public static JObject ListLanguages()
		{
            string route = "/languages?api-version=3.0";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(Tokens.TranslateEndpoint + route);
                var response = client.SendAsync(request).Result;
                var jsonResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                return jsonResponse;
            }
        }

        public static async Task<JToken> TranslateText(string text, string language)
		{
            string route = $"/translate?api-version=3.0&to={language}";
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(Tokens.TranslateEndpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", Tokens.TranslateKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", Tokens.TranslateRegion);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                return JArray.Parse(result)[0];
            }
        }
	}
}