using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000204 RID: 516
	public static class BodyCatalog
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000A09 RID: 2569 RVA: 0x00032166 File Offset: 0x00030366
		public static IEnumerable<GameObject> allBodyPrefabs
		{
			get
			{
				return BodyCatalog.bodyPrefabs;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000A0A RID: 2570 RVA: 0x0003216D File Offset: 0x0003036D
		public static IEnumerable<CharacterBody> allBodyPrefabBodyBodyComponents
		{
			get
			{
				return BodyCatalog.bodyPrefabBodyComponents;
			}
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00032174 File Offset: 0x00030374
		public static GameObject GetBodyPrefab(int index)
		{
			if ((ulong)index < (ulong)((long)BodyCatalog.bodyPrefabs.Length))
			{
				return BodyCatalog.bodyPrefabs[index];
			}
			return null;
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x0003218B File Offset: 0x0003038B
		public static CharacterBody GetBodyPrefabBodyComponent(int index)
		{
			if ((ulong)index < (ulong)((long)BodyCatalog.bodyPrefabBodyComponents.Length))
			{
				return BodyCatalog.bodyPrefabBodyComponents[index];
			}
			return null;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x000321A4 File Offset: 0x000303A4
		public static int FindBodyIndex([NotNull] string bodyName)
		{
			int result;
			if (BodyCatalog.nameToIndexMap.TryGetValue(bodyName, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x000321C4 File Offset: 0x000303C4
		public static int FindBodyIndexCaseInsensitive([NotNull] string bodyName)
		{
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				if (string.Compare(BodyCatalog.bodyPrefabs[i].name, bodyName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x000321FB File Offset: 0x000303FB
		public static int FindBodyIndex(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return -1;
			}
			return BodyCatalog.FindBodyIndex(bodyObject.name);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00032214 File Offset: 0x00030414
		public static GameObject FindBodyPrefab([NotNull] string bodyName)
		{
			int num = BodyCatalog.FindBodyIndex(bodyName);
			if (num != -1)
			{
				return BodyCatalog.GetBodyPrefab(num);
			}
			return null;
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00032234 File Offset: 0x00030434
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			BodyCatalog.bodyPrefabs = Resources.LoadAll<GameObject>("Prefabs/CharacterBodies/");
			BodyCatalog.bodyPrefabBodyComponents = new CharacterBody[BodyCatalog.bodyPrefabs.Length];
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				BodyCatalog.nameToIndexMap.Add(BodyCatalog.bodyPrefabs[i].name, i);
				BodyCatalog.nameToIndexMap.Add(BodyCatalog.bodyPrefabs[i].name + "(Clone)", i);
				BodyCatalog.bodyPrefabBodyComponents[i] = BodyCatalog.bodyPrefabs[i].GetComponent<CharacterBody>();
				Texture2D texture2D = Resources.Load<Texture2D>("Textures/BodyIcons/" + BodyCatalog.bodyPrefabs[i].name);
				if (texture2D)
				{
					BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon = texture2D;
				}
			}
			BodyCatalog.availability.MakeAvailable();
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x000322FF File Offset: 0x000304FF
		private static IEnumerator GeneratePortraits()
		{
			ModelPanel modelPanel = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/IconGenerator")).GetComponentInChildren<ModelPanel>();
			yield return new WaitForEndOfFrame();
			int num;
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i = num)
			{
				if (BodyCatalog.bodyPrefabBodyComponents[i] && (!BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon || BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon.name == "texDifficultyNormalIcon"))
				{
					try
					{
						Debug.LogFormat("Generating portrait for {0}", new object[]
						{
							BodyCatalog.bodyPrefabs[i].name
						});
						ModelPanel modelPanel2 = modelPanel;
						ModelLocator component = BodyCatalog.bodyPrefabs[i].GetComponent<ModelLocator>();
						modelPanel2.modelPrefab = ((component != null) ? component.modelTransform.gameObject : null);
						modelPanel.SetAnglesForCharacterThumbnail(true);
					}
					catch (Exception message)
					{
						Debug.Log(message);
					}
					yield return new WaitForSeconds(1f);
					modelPanel.SetAnglesForCharacterThumbnail(true);
					yield return new WaitForEndOfFrame();
					yield return new WaitForEndOfFrame();
					try
					{
						Texture2D texture2D = new Texture2D(modelPanel.renderTexture.width, modelPanel.renderTexture.height, TextureFormat.ARGB32, false, false);
						RenderTexture active = RenderTexture.active;
						RenderTexture.active = modelPanel.renderTexture;
						texture2D.ReadPixels(new Rect(0f, 0f, (float)modelPanel.renderTexture.width, (float)modelPanel.renderTexture.height), 0, 0, false);
						RenderTexture.active = active;
						byte[] array = texture2D.EncodeToPNG();
						using (FileStream fileStream = new FileStream("Assets/RoR2/GeneratedPortraits/" + BodyCatalog.bodyPrefabs[i].name + ".png", FileMode.Create, FileAccess.Write))
						{
							fileStream.Write(array, 0, array.Length);
						}
					}
					catch (Exception message2)
					{
						Debug.Log(message2);
					}
					yield return new WaitForEndOfFrame();
				}
				num = i + 1;
			}
			UnityEngine.Object.Destroy(modelPanel.transform.root.gameObject);
			yield break;
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00032307 File Offset: 0x00030507
		[ConCommand(commandName = "body_generate_portraits", flags = ConVarFlags.None, helpText = "Generates portraits for all bodies that are currently using the default.")]
		private static void CCBodyGeneratePortraits(ConCommandArgs args)
		{
			RoR2Application.instance.StartCoroutine(BodyCatalog.GeneratePortraits());
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x0003231C File Offset: 0x0003051C
		[ConCommand(commandName = "body_list", flags = ConVarFlags.None, helpText = "Prints a list of all character bodies in the game.")]
		private static void CCBodyList(ConCommandArgs args)
		{
			string[] array = new string[BodyCatalog.bodyPrefabs.Length];
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				array[i] = BodyCatalog.bodyPrefabs[i].name;
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x04000D5E RID: 3422
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x04000D5F RID: 3423
		private static GameObject[] bodyPrefabs;

		// Token: 0x04000D60 RID: 3424
		private static CharacterBody[] bodyPrefabBodyComponents;

		// Token: 0x04000D61 RID: 3425
		private static readonly Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
	}
}
