using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.UI;
using SimpleJSON;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044E RID: 1102
	public static class Language
	{
		// Token: 0x0600187C RID: 6268 RVA: 0x00075E2C File Offset: 0x0007402C
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

		// Token: 0x0600187D RID: 6269 RVA: 0x00075E5C File Offset: 0x0007405C
		private static void LoadLanguageFile([NotNull] string language, [NotNull] string fileName)
		{
			Dictionary<string, string> dictionary = Language.LoadLanguageDictionary(language);
			string text = string.Format(CultureInfo.InvariantCulture, "{0}/Language/{1}/{2}", Application.dataPath, language, fileName);
			if (File.Exists(text))
			{
				try
				{
					JSONNode jsonnode = JSON.Parse(File.ReadAllText(text));
					if (jsonnode != null)
					{
						JSONNode jsonnode2 = jsonnode["strings"];
						if (jsonnode2 != null)
						{
							foreach (string text2 in jsonnode2.Keys)
							{
								dictionary[text2] = jsonnode2[text2].Value;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogFormat("Parsing error in language file \"{0}\". Error: {1}", new object[]
					{
						text,
						ex
					});
				}
			}
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x00075F3C File Offset: 0x0007413C
		public static void LoadAllFilesForLanguage([NotNull] string language)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(string.Format("{0}/Language/{1}/", Application.dataPath, language));
			if (directoryInfo.Exists)
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles())
				{
					if (fileInfo.Extension != ".meta")
					{
						Language.LoadLanguageFile(language, fileInfo.Name);
					}
				}
			}
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x00075F9E File Offset: 0x0007419E
		private static void UnloadLanguage([NotNull] string language)
		{
			Language.languageDictionaries.Remove(language);
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x00075FAC File Offset: 0x000741AC
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

		// Token: 0x06001881 RID: 6273 RVA: 0x00075FD6 File Offset: 0x000741D6
		public static string GetString([NotNull] string token)
		{
			return Language.GetString(token, Language.currentLanguage);
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x00075FE3 File Offset: 0x000741E3
		public static string GetStringFormatted([NotNull] string token, params object[] args)
		{
			return string.Format(Language.GetString(token), args);
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x00075FF1 File Offset: 0x000741F1
		public static bool IsTokenInvalid(string token)
		{
			return token == Language.GetString(token);
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x00076000 File Offset: 0x00074200
		public static void SetCurrentLanguage([NotNull] string language)
		{
			Language.UnloadLanguage(Language.currentLanguage);
			Language.currentLanguage = language;
			Language.LoadAllFilesForLanguage(Language.currentLanguage);
			LanguageTextMeshController[] array = UnityEngine.Object.FindObjectsOfType<LanguageTextMeshController>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResolveString();
			}
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x00076043 File Offset: 0x00074243
		[ConCommand(commandName = "language_reload", flags = ConVarFlags.None, helpText = "Reloads the current language.")]
		public static void CCLanguageReload(ConCommandArgs args)
		{
			Language.SetCurrentLanguage(Language.currentLanguage);
		}

		// Token: 0x04001C13 RID: 7187
		private static string currentLanguage = "";

		// Token: 0x04001C14 RID: 7188
		private static readonly Dictionary<string, Dictionary<string, string>> languageDictionaries = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04001C15 RID: 7189
		private static Language.LanguageConVar cvLanguage = new Language.LanguageConVar("language", ConVarFlags.Archive, "EN_US", "Which language to use.");

		// Token: 0x0200044F RID: 1103
		private class LanguageConVar : BaseConVar
		{
			// Token: 0x06001887 RID: 6279 RVA: 0x00037E38 File Offset: 0x00036038
			public LanguageConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001888 RID: 6280 RVA: 0x0007607F File Offset: 0x0007427F
			public override void SetString(string newValue)
			{
				Language.SetCurrentLanguage(newValue);
			}

			// Token: 0x06001889 RID: 6281 RVA: 0x00076087 File Offset: 0x00074287
			public override string GetString()
			{
				return Language.currentLanguage;
			}
		}
	}
}
