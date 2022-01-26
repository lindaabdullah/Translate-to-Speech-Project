using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranslateToSpeech
{
	public class Tokens
	{
		public static readonly string TranslateKey = "074650efa5a24dd0a5611397acb2483d"; // Environment.GetEnvironmentVariable("TRANSLATE_KEY")
		public static readonly string TranslateRegion = "eastus";//Environment.GetEnvironmentVariable("TRANSLATE_REGION")
		public static readonly string TranslateEndpoint = "https://api.cognitive.microsofttranslator.com";

		public static readonly string SpeechKey = "67327444b03c4eeb8f828fba9b42cf48";//Environment.GetEnvironmentVariable("SPEECH_KEY").ToString()
		public static readonly string SpeechRegion = "eastus";//Environment.GetEnvironmentVariable("SPEECH_REGION")
		public static readonly string SpeechEndpoint = $"https://{SpeechRegion}.tts.speech.microsoft.com";
	}
}