using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TranslateToSpeech.Util;
using System.Threading.Tasks;


namespace TranslateToSpeech.Controllers
{
	public class HomeController : Controller
	{
		public static readonly List<JToken> TranslateLanguages = Translate.ListLanguages()["translation"].ToList();
		public static readonly JArray SpeechLanguages = TextToSpeech.ListLanguages();

		public ActionResult Index()
		{
			ViewBag.tl = TranslateLanguages;
			ViewBag.sl = SpeechLanguages;
			return View();
		}

		public string TranslateToLanguage(string text, string language)
		{
			var translationResult = Translate.TranslateText(text, language).Result;
			var detectedLanguage = translationResult["detectedLanguage"]["language"].ToString();
			var translation = translationResult["translations"][0]["text"].ToString();
			var json = JsonConvert.SerializeObject(new
			{
				text = text,
				detectedLanguage = detectedLanguage,
				translation = translation,
				targetLanguage = language
			});
			var jsonString = json.ToString();
			string base64string = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
			Queue.InsertMessage("texttranslate", base64string);
			return translation;
		}

		public async Task TranslationToSpeech(string text, string language)
		{
			Response.BinaryWrite(await TextToSpeech.SynthesizeAudioAsync(text, language));
		}
	}
}