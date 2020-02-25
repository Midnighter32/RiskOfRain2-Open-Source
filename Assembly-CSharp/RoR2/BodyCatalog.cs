using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000CA RID: 202
	public static class BodyCatalog
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060003DC RID: 988 RVA: 0x0000F5A7 File Offset: 0x0000D7A7
		// (set) Token: 0x060003DD RID: 989 RVA: 0x0000F5AE File Offset: 0x0000D7AE
		public static int bodyCount { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060003DE RID: 990 RVA: 0x0000F5B6 File Offset: 0x0000D7B6
		public static IEnumerable<GameObject> allBodyPrefabs
		{
			get
			{
				return BodyCatalog.bodyPrefabs;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060003DF RID: 991 RVA: 0x0000F5BD File Offset: 0x0000D7BD
		public static IEnumerable<CharacterBody> allBodyPrefabBodyBodyComponents
		{
			get
			{
				return BodyCatalog.bodyPrefabBodyComponents;
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0000F5C4 File Offset: 0x0000D7C4
		public static GameObject GetBodyPrefab(int bodyIndex)
		{
			if ((ulong)bodyIndex < (ulong)((long)BodyCatalog.bodyPrefabs.Length))
			{
				return BodyCatalog.bodyPrefabs[bodyIndex];
			}
			return null;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0000F5DB File Offset: 0x0000D7DB
		[CanBeNull]
		public static CharacterBody GetBodyPrefabBodyComponent(int bodyIndex)
		{
			if ((ulong)bodyIndex < (ulong)((long)BodyCatalog.bodyPrefabBodyComponents.Length))
			{
				return BodyCatalog.bodyPrefabBodyComponents[bodyIndex];
			}
			return null;
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0000F5F2 File Offset: 0x0000D7F2
		public static string GetBodyName(int bodyIndex)
		{
			return HGArrayUtilities.GetSafe<string>(BodyCatalog.bodyNames, bodyIndex);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0000F600 File Offset: 0x0000D800
		public static int FindBodyIndex([NotNull] string bodyName)
		{
			int result;
			if (BodyCatalog.nameToIndexMap.TryGetValue(bodyName, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0000F620 File Offset: 0x0000D820
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

		// Token: 0x060003E5 RID: 997 RVA: 0x0000F657 File Offset: 0x0000D857
		public static int FindBodyIndex(GameObject bodyObject)
		{
			return BodyCatalog.FindBodyIndex(bodyObject ? bodyObject.GetComponent<CharacterBody>() : null);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0000F66F File Offset: 0x0000D86F
		public static int FindBodyIndex(CharacterBody characterBody)
		{
			if (characterBody == null)
			{
				return -1;
			}
			return characterBody.bodyIndex;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0000F67C File Offset: 0x0000D87C
		[CanBeNull]
		public static GameObject FindBodyPrefab([NotNull] string bodyName)
		{
			int num = BodyCatalog.FindBodyIndex(bodyName);
			if (num != -1)
			{
				return BodyCatalog.GetBodyPrefab(num);
			}
			return null;
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0000F69C File Offset: 0x0000D89C
		[CanBeNull]
		public static GameObject FindBodyPrefab(CharacterBody characterBody)
		{
			return BodyCatalog.GetBodyPrefab(BodyCatalog.FindBodyIndex(characterBody));
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0000F6A9 File Offset: 0x0000D8A9
		[CanBeNull]
		public static GameObject FindBodyPrefab(GameObject characterBodyObject)
		{
			return BodyCatalog.GetBodyPrefab(BodyCatalog.FindBodyIndex(characterBodyObject));
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0000F6B6 File Offset: 0x0000D8B6
		[CanBeNull]
		public static GenericSkill[] GetBodyPrefabSkillSlots(int bodyIndex)
		{
			return HGArrayUtilities.GetSafe<GenericSkill[]>(BodyCatalog.skillSlots, bodyIndex);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0000F6C3 File Offset: 0x0000D8C3
		public static SkinDef[] GetBodySkins(int bodyIndex)
		{
			return HGArrayUtilities.GetSafe<SkinDef[]>(BodyCatalog.skins, bodyIndex, Array.Empty<SkinDef>());
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060003EC RID: 1004 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
		// (remove) Token: 0x060003ED RID: 1005 RVA: 0x0000F70C File Offset: 0x0000D90C
		public static event Action<List<GameObject>> getAdditionalEntries;

		// Token: 0x060003EE RID: 1006 RVA: 0x0000F740 File Offset: 0x0000D940
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			IEnumerable<GameObject> first = Resources.LoadAll<GameObject>("Prefabs/CharacterBodies/");
			List<GameObject> list = new List<GameObject>();
			Action<List<GameObject>> action = BodyCatalog.getAdditionalEntries;
			if (action != null)
			{
				action(list);
			}
			BodyCatalog.SetBodyPrefabs(first.Concat(list.OrderBy((GameObject v) => v.name, StringComparer.Ordinal)).ToArray<GameObject>());
			BodyCatalog.availability.MakeAvailable();
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0000F7B4 File Offset: 0x0000D9B4
		private static void SetBodyPrefabs([NotNull] GameObject[] newBodyPrefabs)
		{
			BodyCatalog.bodyPrefabs = HGArrayUtilities.Clone<GameObject>(newBodyPrefabs);
			BodyCatalog.bodyPrefabBodyComponents = new CharacterBody[BodyCatalog.bodyPrefabs.Length];
			BodyCatalog.bodyNames = new string[BodyCatalog.bodyPrefabs.Length];
			BodyCatalog.bodyComponents = new Component[BodyCatalog.bodyPrefabs.Length][];
			BodyCatalog.skillSlots = new GenericSkill[BodyCatalog.bodyPrefabs.Length][];
			BodyCatalog.skins = new SkinDef[BodyCatalog.bodyPrefabs.Length][];
			BodyCatalog.nameToIndexMap.Clear();
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				GameObject gameObject = BodyCatalog.bodyPrefabs[i];
				string name = gameObject.name;
				BodyCatalog.bodyNames[i] = name;
				BodyCatalog.bodyComponents[i] = gameObject.GetComponents<Component>();
				BodyCatalog.skillSlots[i] = gameObject.GetComponents<GenericSkill>();
				BodyCatalog.nameToIndexMap.Add(name, i);
				BodyCatalog.nameToIndexMap.Add(name + "(Clone)", i);
				(BodyCatalog.bodyPrefabBodyComponents[i] = gameObject.GetComponent<CharacterBody>()).bodyIndex = i;
				Texture2D texture2D = Resources.Load<Texture2D>("Textures/BodyIcons/" + name);
				SkinDef[][] array = BodyCatalog.skins;
				int num = i;
				ModelLocator component = gameObject.GetComponent<ModelLocator>();
				SkinDef[] array2;
				if (component == null)
				{
					array2 = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform == null)
					{
						array2 = null;
					}
					else
					{
						ModelSkinController component2 = modelTransform.GetComponent<ModelSkinController>();
						array2 = ((component2 != null) ? component2.skins : null);
					}
				}
				array[num] = (array2 ?? Array.Empty<SkinDef>());
				if (texture2D)
				{
					BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon = texture2D;
				}
				else if (BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon == null)
				{
					BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon = Resources.Load<Texture2D>("Textures/MiscIcons/texMysteryIcon");
				}
				if (Language.IsTokenInvalid(BodyCatalog.bodyPrefabBodyComponents[i].baseNameToken))
				{
					BodyCatalog.bodyPrefabBodyComponents[i].baseNameToken = "UNIDENTIFIED";
				}
			}
			BodyCatalog.bodyCount = BodyCatalog.bodyPrefabs.Length;
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0000F96A File Offset: 0x0000DB6A
		private static IEnumerator GeneratePortraits(bool forceRegeneration)
		{
			BodyCatalog.<>c__DisplayClass33_0 CS$<>8__locals1 = new BodyCatalog.<>c__DisplayClass33_0();
			Debug.Log("Starting portrait generation.");
			CS$<>8__locals1.modelPanel = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/IconGenerator")).GetComponentInChildren<ModelPanel>();
			yield return new WaitForEndOfFrame();
			int num2;
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i = num2)
			{
				CharacterBody characterBody = BodyCatalog.bodyPrefabBodyComponents[i];
				if (characterBody && (forceRegeneration || !characterBody.portraitIcon || characterBody.portraitIcon.name == "texMysteryIcon"))
				{
					float num = 1f;
					try
					{
						Debug.LogFormat("Generating portrait for {0}", new object[]
						{
							BodyCatalog.bodyPrefabs[i].name
						});
						ModelPanel modelPanel = CS$<>8__locals1.modelPanel;
						ModelLocator component = BodyCatalog.bodyPrefabs[i].GetComponent<ModelLocator>();
						modelPanel.modelPrefab = ((component != null) ? component.modelTransform.gameObject : null);
						CS$<>8__locals1.modelPanel.SetAnglesForCharacterThumbnail(true);
						GameObject modelPrefab = CS$<>8__locals1.modelPanel.modelPrefab;
						PrintController printController;
						if ((printController = ((modelPrefab != null) ? modelPrefab.GetComponentInChildren<PrintController>() : null)) != null)
						{
							num = Mathf.Max(num, printController.printTime + 1f);
						}
						GameObject modelPrefab2 = CS$<>8__locals1.modelPanel.modelPrefab;
						TemporaryOverlay temporaryOverlay;
						if ((temporaryOverlay = ((modelPrefab2 != null) ? modelPrefab2.GetComponentInChildren<TemporaryOverlay>() : null)) != null)
						{
							num = Mathf.Max(num, temporaryOverlay.duration + 1f);
						}
					}
					catch (Exception message)
					{
						Debug.Log(message);
					}
					RoR2Application.onLateUpdate += CS$<>8__locals1.<GeneratePortraits>g__UpdateCamera|0;
					yield return new WaitForSeconds(num);
					CS$<>8__locals1.modelPanel.SetAnglesForCharacterThumbnail(true);
					yield return new WaitForEndOfFrame();
					yield return new WaitForEndOfFrame();
					try
					{
						Texture2D texture2D = new Texture2D(CS$<>8__locals1.modelPanel.renderTexture.width, CS$<>8__locals1.modelPanel.renderTexture.height, TextureFormat.ARGB32, false, false);
						RenderTexture active = RenderTexture.active;
						RenderTexture.active = CS$<>8__locals1.modelPanel.renderTexture;
						texture2D.ReadPixels(new Rect(0f, 0f, (float)CS$<>8__locals1.modelPanel.renderTexture.width, (float)CS$<>8__locals1.modelPanel.renderTexture.height), 0, 0, false);
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
					RoR2Application.onLateUpdate -= CS$<>8__locals1.<GeneratePortraits>g__UpdateCamera|0;
					yield return new WaitForEndOfFrame();
				}
				num2 = i + 1;
			}
			UnityEngine.Object.Destroy(CS$<>8__locals1.modelPanel.transform.root.gameObject);
			Debug.Log("Portrait generation complete.");
			yield break;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0000F97C File Offset: 0x0000DB7C
		[ConCommand(commandName = "body_generate_portraits", flags = ConVarFlags.None, helpText = "Generates portraits for all bodies that are currently using the default.")]
		private static void CCBodyGeneratePortraits(ConCommandArgs args)
		{
			RoR2Application.instance.StartCoroutine(BodyCatalog.GeneratePortraits(args.TryGetArgBool(0) ?? false));
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0000F9B8 File Offset: 0x0000DBB8
		[ConCommand(commandName = "body_list", flags = ConVarFlags.None, helpText = "Prints a list of all character bodies in the game.")]
		private static void CCBodyList(ConCommandArgs args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < BodyCatalog.bodyComponents.Length; i++)
			{
				stringBuilder.Append("[").Append(i).Append("]=").Append(BodyCatalog.bodyNames[i]).AppendLine();
			}
			Debug.Log(stringBuilder);
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0000FA10 File Offset: 0x0000DC10
		[ConCommand(commandName = "body_reload_all", flags = ConVarFlags.Cheat, helpText = "Reloads all bodies and repopulates the BodyCatalog.")]
		private static void CCBodyReloadAll(ConCommandArgs args)
		{
			BodyCatalog.SetBodyPrefabs(Resources.LoadAll<GameObject>("Prefabs/CharacterBodies/"));
		}

		// Token: 0x0400038B RID: 907
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x0400038C RID: 908
		private static string[] bodyNames;

		// Token: 0x0400038D RID: 909
		private static GameObject[] bodyPrefabs;

		// Token: 0x0400038E RID: 910
		private static CharacterBody[] bodyPrefabBodyComponents;

		// Token: 0x0400038F RID: 911
		private static Component[][] bodyComponents;

		// Token: 0x04000390 RID: 912
		private static GenericSkill[][] skillSlots;

		// Token: 0x04000391 RID: 913
		private static SkinDef[][] skins;

		// Token: 0x04000393 RID: 915
		private static readonly Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
	}
}
