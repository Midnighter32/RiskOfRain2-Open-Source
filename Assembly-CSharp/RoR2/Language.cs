using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using RoR2.ConVar;
using SimpleJSON;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003B5 RID: 949
	public static class Language
	{
		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x00063CB4 File Offset: 0x00061EB4
		// (set) Token: 0x060016FA RID: 5882 RVA: 0x00063CBB File Offset: 0x00061EBB
		public static string currentLanguage { get; private set; } = "";

		// Token: 0x060016FB RID: 5883 RVA: 0x00063CC4 File Offset: 0x00061EC4
		private static Dictionary<string, string> LoadLanguageDictionary([NotNull] string language)
		{
			Dictionary<string, string> dictionary;
			if (!Language.languageDictionaries.TryGetValue(language, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				Language.languageDictionaries[language] = dictionary;
			}
			return dictionary;
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x00063CF4 File Offset: 0x00061EF4
		private static KeyValuePair<string, string>[] LoadTokensFromFile([NotNull] string path)
		{
			if (File.Exists(path))
			{
				try
				{
					JSONNode jsonnode = JSON.Parse(File.ReadAllText(path, Encoding.UTF8));
					if (jsonnode != null)
					{
						JSONNode jsonnode2 = jsonnode["strings"];
						if (jsonnode2 != null)
						{
							KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[jsonnode2.Count];
							int num = 0;
							foreach (string text in jsonnode2.Keys)
							{
								array[num++] = new KeyValuePair<string, string>(text, jsonnode2[text].Value);
							}
							return array;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogFormat("Parsing error in language file \"{0}\". Error: {1}", new object[]
					{
						path,
						ex
					});
				}
			}
			return Array.Empty<KeyValuePair<string, string>>();
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x00063DE0 File Offset: 0x00061FE0
		[NotNull]
		private static string GetPathForLanguageFile([NotNull] string language, [NotNull] string fileName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/Language/{1}/{2}", Application.dataPath, language, fileName);
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x00063DF8 File Offset: 0x00061FF8
		private static bool ImportLanguageFile([NotNull] string language, [NotNull] string fileName)
		{
			Dictionary<string, string> dictionary = Language.LoadLanguageDictionary(language);
			string pathForLanguageFile = Language.GetPathForLanguageFile(language, fileName);
			if (File.Exists(pathForLanguageFile))
			{
				foreach (KeyValuePair<string, string> keyValuePair in Language.LoadTokensFromFile(pathForLanguageFile))
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x00063E54 File Offset: 0x00062054
		[NotNull]
		private static IEnumerable<FileInfo> GetFilesForLanguage([NotNull] string language)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(string.Format("{0}/Language/{1}/", Application.dataPath, language));
			if (directoryInfo.Exists)
			{
				return from file in directoryInfo.GetFiles()
				where file.Extension != ".meta"
				select file;
			}
			return Enumerable.Empty<FileInfo>();
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x00063EAF File Offset: 0x000620AF
		private static bool LanguageIsValid([NotNull] string language)
		{
			return Language.GetFilesForLanguage(language).Any<FileInfo>();
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x00063EBC File Offset: 0x000620BC
		public static bool LoadAllFilesForLanguage([NotNull] string language)
		{
			bool flag = false;
			foreach (FileInfo fileInfo in Language.GetFilesForLanguage(language))
			{
				flag |= Language.ImportLanguageFile(language, fileInfo.Name);
			}
			return flag;
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x00063F14 File Offset: 0x00062114
		private static void UnloadLanguage([NotNull] string language)
		{
			Language.languageDictionaries.Remove(language);
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x00063F24 File Offset: 0x00062124
		public static string GetString([NotNull] string token, [NotNull] string language)
		{
			Dictionary<string, string> dictionary;
			string result;
			if (Language.languageDictionaries.TryGetValue(language, out dictionary) && dictionary.TryGetValue(token, out result))
			{
				return result;
			}
			return token;
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00063F4E File Offset: 0x0006214E
		public static string GetString([NotNull] string token)
		{
			return Language.GetString(token, Language.currentLanguage);
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00063F5B File Offset: 0x0006215B
		public static string GetStringFormatted([NotNull] string token, params object[] args)
		{
			return string.Format(Language.GetString(token), args);
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x00063F69 File Offset: 0x00062169
		public static bool IsTokenInvalid(string token)
		{
			return token == Language.GetString(token);
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x00063F78 File Offset: 0x00062178
		public static void SetCurrentLanguage([NotNull] string language)
		{
			Debug.LogFormat("Setting current language to \"{0}\"", new object[]
			{
				language
			});
			Language.UnloadLanguage(Language.currentLanguage);
			Language.currentLanguage = language;
			if (!Language.LoadAllFilesForLanguage(Language.currentLanguage))
			{
				Debug.LogFormat("Could not load files for language \"{0}\". Falling back to \"en\".", new object[]
				{
					language
				});
				Language.LoadAllFilesForLanguage("en");
				Language.currentLanguage = "en";
			}
			Action action = Language.onCurrentLanguageChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06001708 RID: 5896 RVA: 0x00063FF0 File Offset: 0x000621F0
		// (remove) Token: 0x06001709 RID: 5897 RVA: 0x00064024 File Offset: 0x00062224
		public static event Action onCurrentLanguageChanged;

		// Token: 0x0600170A RID: 5898 RVA: 0x00064057 File Offset: 0x00062257
		[ConCommand(commandName = "language_reload", flags = ConVarFlags.None, helpText = "Reloads the current language.")]
		public static void CCLanguageReload(ConCommandArgs args)
		{
			Language.SetCurrentLanguage(Language.currentLanguage);
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x00064064 File Offset: 0x00062264
		[ConCommand(commandName = "language_dump_to_json", flags = ConVarFlags.None, helpText = "Combines all files for the given language into a single JSON file.")]
		private static void CCLanguageDumpToJson(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			string language = args[0];
			foreach (FileInfo fileInfo in Language.GetFilesForLanguage(language))
			{
				list.AddRange(Language.LoadTokensFromFile(Language.GetPathForLanguageFile(language, fileInfo.Name)));
			}
			StringBuilder stringBuilder = new StringBuilder();
			JSONNode jsonnode = new JSONObject();
			JSONNode jsonnode2 = jsonnode["strings"] = new JSONObject();
			foreach (KeyValuePair<string, string> keyValuePair in list)
			{
				jsonnode2[keyValuePair.Key] = keyValuePair.Value;
			}
			jsonnode.WriteToStringBuilder(stringBuilder, 0, 1, JSONTextMode.Indent);
			File.WriteAllText("output.json", stringBuilder.ToString(), Encoding.UTF8);
		}

		// Token: 0x040015EC RID: 5612
		private static readonly Dictionary<string, Dictionary<string, string>> languageDictionaries = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x020003B6 RID: 950
		private class LanguageConVar : BaseConVar
		{
			// Token: 0x0600170D RID: 5901 RVA: 0x00064186 File Offset: 0x00062386
			public LanguageConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600170E RID: 5902 RVA: 0x000641A0 File Offset: 0x000623A0
			public override void SetString(string newValue)
			{
				if (string.CompareOrdinal(newValue, "EN_US") == 0 || !Language.LanguageIsValid(newValue))
				{
					newValue = Language.LanguageConVar.platformString;
				}
				this.internalValue = newValue;
				if (string.CompareOrdinal(this.internalValue, Language.LanguageConVar.platformString) == 0)
				{
					newValue = (Language.LanguageConVar.GetPlatformLanguage() ?? "en");
				}
				Language.SetCurrentLanguage(newValue);
			}

			// Token: 0x0600170F RID: 5903 RVA: 0x000641F8 File Offset: 0x000623F8
			public override string GetString()
			{
				return this.internalValue;
			}

			// Token: 0x06001710 RID: 5904 RVA: 0x00064200 File Offset: 0x00062400
			private static string GetPlatformLanguage()
			{
				Client client = Client.Instance;
				string text = (client != null) ? client.CurrentLanguage : null;
				if (text == null)
				{
					return null;
				}
				Debug.Log(text);
				Language.LanguageConVar.SteamLanguageDef steamLanguageDef;
				if (Language.LanguageConVar.steamLanguageTable.TryGetValue(text, out steamLanguageDef))
				{
					return steamLanguageDef.webApiName;
				}
				return null;
			}

			// Token: 0x06001711 RID: 5905 RVA: 0x00064244 File Offset: 0x00062444
			// Note: this type is marked as 'beforefieldinit'.
			static LanguageConVar()
			{
				Dictionary<string, Language.LanguageConVar.SteamLanguageDef> dictionary = new Dictionary<string, Language.LanguageConVar.SteamLanguageDef>(StringComparer.OrdinalIgnoreCase);
				dictionary["arabic"] = new Language.LanguageConVar.SteamLanguageDef("Arabic", "العربية", "arabic", "ar");
				dictionary["bulgarian"] = new Language.LanguageConVar.SteamLanguageDef("Bulgarian", "български език", "bulgarian", "bg");
				dictionary["schinese"] = new Language.LanguageConVar.SteamLanguageDef("Chinese (Simplified)", "简体中文", "schinese", "zh-CN");
				dictionary["tchinese"] = new Language.LanguageConVar.SteamLanguageDef("Chinese (Traditional)", "繁體中文", "tchinese", "zh-TW");
				dictionary["czech"] = new Language.LanguageConVar.SteamLanguageDef("Czech", "čeština", "czech", "cs");
				dictionary["danish"] = new Language.LanguageConVar.SteamLanguageDef("Danish", "Dansk", "danish", "da");
				dictionary["dutch"] = new Language.LanguageConVar.SteamLanguageDef("Dutch", "Nederlands", "dutch", "nl");
				dictionary["english"] = new Language.LanguageConVar.SteamLanguageDef("English", "English", "english", "en");
				dictionary["finnish"] = new Language.LanguageConVar.SteamLanguageDef("Finnish", "Suomi", "finnish", "fi");
				dictionary["french"] = new Language.LanguageConVar.SteamLanguageDef("French", "Français", "french", "fr");
				dictionary["german"] = new Language.LanguageConVar.SteamLanguageDef("German", "Deutsch", "german", "de");
				dictionary["greek"] = new Language.LanguageConVar.SteamLanguageDef("Greek", "Ελληνικά", "greek", "el");
				dictionary["hungarian"] = new Language.LanguageConVar.SteamLanguageDef("Hungarian", "Magyar", "hungarian", "hu");
				dictionary["italian"] = new Language.LanguageConVar.SteamLanguageDef("Italian", "Italiano", "italian", "it");
				dictionary["japanese"] = new Language.LanguageConVar.SteamLanguageDef("Japanese", "日本語", "japanese", "ja");
				dictionary["koreana"] = new Language.LanguageConVar.SteamLanguageDef("Korean", "한국어", "koreana", "ko");
				dictionary["korean"] = new Language.LanguageConVar.SteamLanguageDef("Korean", "한국어", "korean", "ko");
				dictionary["norwegian"] = new Language.LanguageConVar.SteamLanguageDef("Norwegian", "Norsk", "norwegian", "no");
				dictionary["polish"] = new Language.LanguageConVar.SteamLanguageDef("Polish", "Polski", "polish", "pl");
				dictionary["portuguese"] = new Language.LanguageConVar.SteamLanguageDef("Portuguese", "Português", "portuguese", "pt");
				dictionary["brazilian"] = new Language.LanguageConVar.SteamLanguageDef("Portuguese-Brazil", "Português-Brasil", "brazilian", "pt-BR");
				dictionary["romanian"] = new Language.LanguageConVar.SteamLanguageDef("Romanian", "Română", "romanian", "ro");
				dictionary["russian"] = new Language.LanguageConVar.SteamLanguageDef("Russian", "Русский", "russian", "ru");
				dictionary["spanish"] = new Language.LanguageConVar.SteamLanguageDef("Spanish-Spain", "Español-España", "spanish", "es");
				dictionary["latam"] = new Language.LanguageConVar.SteamLanguageDef("Spanish-Latin America", "Español-Latinoamérica", "latam", "es-419");
				dictionary["swedish"] = new Language.LanguageConVar.SteamLanguageDef("Swedish", "Svenska", "swedish", "sv");
				dictionary["thai"] = new Language.LanguageConVar.SteamLanguageDef("Thai", "ไทย", "thai", "th");
				dictionary["turkish"] = new Language.LanguageConVar.SteamLanguageDef("Turkish", "Türkçe", "turkish", "tr");
				dictionary["ukrainian"] = new Language.LanguageConVar.SteamLanguageDef("Ukrainian", "Українська", "ukrainian", "uk");
				dictionary["vietnamese"] = new Language.LanguageConVar.SteamLanguageDef("Vietnamese", "Tiếng Việt", "vietnamese", "vn");
				Language.LanguageConVar.steamLanguageTable = dictionary;
			}

			// Token: 0x040015EE RID: 5614
			private static readonly string platformString = "platform";

			// Token: 0x040015EF RID: 5615
			private static Language.LanguageConVar instance = new Language.LanguageConVar("language", ConVarFlags.Archive, Language.LanguageConVar.platformString, "Which language to use.");

			// Token: 0x040015F0 RID: 5616
			private string internalValue = string.Empty;

			// Token: 0x040015F1 RID: 5617
			private static readonly Dictionary<string, Language.LanguageConVar.SteamLanguageDef> steamLanguageTable;

			// Token: 0x020003B7 RID: 951
			private struct SteamLanguageDef
			{
				// Token: 0x06001712 RID: 5906 RVA: 0x000646BC File Offset: 0x000628BC
				public SteamLanguageDef(string englishName, string nativeName, string apiName, string webApiName)
				{
					this.englishName = englishName;
					this.nativeName = nativeName;
					this.apiName = apiName;
					this.webApiName = webApiName;
				}

				// Token: 0x040015F2 RID: 5618
				public readonly string englishName;

				// Token: 0x040015F3 RID: 5619
				public readonly string nativeName;

				// Token: 0x040015F4 RID: 5620
				public readonly string apiName;

				// Token: 0x040015F5 RID: 5621
				public readonly string webApiName;
			}
		}
	}
}
