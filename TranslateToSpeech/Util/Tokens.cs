using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranslateToSpeech
{
	public class Tokens
	{		public static readonly string TranslateKey = Environment.GetEnvironmentVariable("TRANSLATE_KEY");
		public static readonly string TranslateRegion = Environment.GetEnvironmentVariable("TRANSLATE_REGION");
		public static readonly string TranslateEndpoint = "https://api.cognitive.microsofttranslator.com";

		public static readonly string SpeechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
		public static readonly string SpeechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");
		public static readonly string SpeechEndpoint = $"https://{SpeechRegion}.tts.speech.microsoft.com";
	}
}