using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace TranslateToSpeech.Util
{
    public class TextToSpeech
    {
        public static JArray ListLanguages()
        {
            var route = "/cognitiveservices/voices/list";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(Tokens.SpeechEndpoint + route);
                request.Headers.Add("Ocp-Apim-Subscription-Key", Tokens.SpeechKey);
                var response = client.SendAsync(request).Result;
                var jsonResponse = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                var languages = new JArray();
                foreach (var language in jsonResponse)
                {
                    var newLanguage = new JObject();
                    newLanguage["language"] = language["LocaleName"];
                    newLanguage["code"] = language["Locale"];
                    newLanguage["gender"] = language["Gender"];
                    newLanguage["name"] = language["DisplayName"];
                    newLanguage["namecode"] = language["ShortName"];
                    languages.Add(newLanguage);
                }
                return languages;
            }
        }

        public static async Task<byte[]> SynthesizeAudioAsync(string text, string language)
        {
            var config = SpeechConfig.FromSubscription(Tokens.SpeechKey, Tokens.SpeechRegion);
            //config.SpeechSynthesisLanguage = "tr-TR"; // For example, "de -DE"
            config.SpeechSynthesisVoiceName = language;
            using (var synthesizer = new SpeechSynthesizer(config, null))
            {
                var result = await synthesizer.SpeakTextAsync(text);
                return result.AudioData;
            }
        }
    }
}
