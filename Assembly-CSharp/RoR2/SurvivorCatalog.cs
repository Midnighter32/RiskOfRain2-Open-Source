using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044F RID: 1103
	public static class SurvivorCatalog
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06001AD5 RID: 6869 RVA: 0x00071BFB File Offset: 0x0006FDFB
		public static int survivorCount
		{
			get
			{
				return SurvivorCatalog.survivorDefs.Length;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x00071BFB File Offset: 0x0006FDFB
		public static SurvivorIndex endIndex
		{
			get
			{
				return (SurvivorIndex)SurvivorCatalog.survivorDefs.Length;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06001AD7 RID: 6871 RVA: 0x00071C04 File Offset: 0x0006FE04
		public static IEnumerable<SurvivorDef> allSurvivorDefs
		{
			get
			{
				return SurvivorCatalog._allSurvivorDefs;
			}
		}

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x06001AD8 RID: 6872 RVA: 0x00071C0C File Offset: 0x0006FE0C
		// (remove) Token: 0x06001AD9 RID: 6873 RVA: 0x00071C40 File Offset: 0x0006FE40
		public static event Action<List<SurvivorDef>> getAdditionalSurvivorDefs;

		// Token: 0x06001ADA RID: 6874 RVA: 0x00071C73 File Offset: 0x0006FE73
		private static void RegisterSurvivor(SurvivorIndex survivorIndex, SurvivorDef survivorDef)
		{
			if (survivorIndex < SurvivorIndex.Count)
			{
				survivorDef.name = survivorIndex.ToString();
			}
			survivorDef.survivorIndex = survivorIndex;
			SurvivorCatalog.survivorDefs[(int)survivorIndex] = survivorDef;
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00071C9C File Offset: 0x0006FE9C
		public static SurvivorDef GetSurvivorDef(SurvivorIndex survivorIndex)
		{
			return HGArrayUtilities.GetSafe<SurvivorDef>(SurvivorCatalog.survivorDefs, (int)survivorIndex);
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x00071CA9 File Offset: 0x0006FEA9
		public static SurvivorIndex GetSurvivorIndexFromBodyIndex(int bodyIndex)
		{
			return HGArrayUtilities.GetSafe<SurvivorIndex>(SurvivorCatalog.bodyIndexToSurvivorIndex, bodyIndex, SurvivorIndex.None);
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x00071CB7 File Offset: 0x0006FEB7
		public static int GetBodyIndexFromSurvivorIndex(SurvivorIndex survivorIndex)
		{
			return HGArrayUtilities.GetSafe<int>(SurvivorCatalog.survivorIndexToBodyIndex, (int)survivorIndex, -1);
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x00071CC8 File Offset: 0x0006FEC8
		public static SurvivorDef FindSurvivorDefFromBody(GameObject characterBodyPrefab)
		{
			for (int i = 0; i < SurvivorCatalog.survivorDefs.Length; i++)
			{
				SurvivorDef survivorDef = SurvivorCatalog.survivorDefs[i];
				GameObject y = (survivorDef != null) ? survivorDef.bodyPrefab : null;
				if (characterBodyPrefab == y)
				{
					return survivorDef;
				}
			}
			return null;
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x00071D08 File Offset: 0x0006FF08
		public static Texture GetSurvivorPortrait(SurvivorIndex survivorIndex)
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
			if (survivorDef.bodyPrefab != null)
			{
				CharacterBody component = survivorDef.bodyPrefab.GetComponent<CharacterBody>();
				if (component)
				{
					return component.portraitIcon;
				}
			}
			return null;
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x00071D48 File Offset: 0x0006FF48
		public static SurvivorIndex FindSurvivorIndex(string survivorName)
		{
			for (int i = 0; i < SurvivorCatalog.survivorCount; i++)
			{
				SurvivorDef survivorDef = SurvivorCatalog.survivorDefs[i];
				if (((survivorDef != null) ? survivorDef.name : null) == survivorName)
				{
					return (SurvivorIndex)i;
				}
			}
			return SurvivorIndex.None;
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x00071D84 File Offset: 0x0006FF84
		[SystemInitializer(new Type[]
		{
			typeof(BodyCatalog)
		})]
		private static void Init()
		{
			SurvivorCatalog.survivorDefs = new SurvivorDef[10];
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Commando, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("CommandoBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CommandoDisplay"),
				descriptionToken = "COMMANDO_DESCRIPTION",
				primaryColor = new Color(0.92941177f, 0.5882353f, 0.07058824f)
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Huntress, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("HuntressBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/HuntressDisplay"),
				primaryColor = new Color(0.8352941f, 0.23529412f, 0.23529412f),
				descriptionToken = "HUNTRESS_DESCRIPTION",
				unlockableName = "Characters.Huntress"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Toolbot, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("ToolbotBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/ToolbotDisplay"),
				descriptionToken = "TOOLBOT_DESCRIPTION",
				primaryColor = new Color(0.827451f, 0.76862746f, 0.3137255f),
				unlockableName = "Characters.Toolbot"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Engi, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("EngiBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/EngiDisplay"),
				descriptionToken = "ENGI_DESCRIPTION",
				primaryColor = new Color(0.37254903f, 0.8862745f, 0.5254902f),
				unlockableName = "Characters.Engineer"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Mage, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("MageBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MageDisplay"),
				descriptionToken = "MAGE_DESCRIPTION",
				primaryColor = new Color(0.96862745f, 0.75686276f, 0.99215686f),
				unlockableName = "Characters.Mage"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Merc, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("MercBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MercDisplay"),
				descriptionToken = "MERC_DESCRIPTION",
				primaryColor = new Color(0.42352942f, 0.81960785f, 0.91764706f),
				unlockableName = "Characters.Mercenary"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Treebot, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("TreebotBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/TreebotDisplay"),
				descriptionToken = "TREEBOT_DESCRIPTION",
				primaryColor = new Color(0.5254902f, 0.61960787f, 0.32941177f),
				unlockableName = "Characters.Treebot"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Loader, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("LoaderBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/LoaderDisplay"),
				descriptionToken = "LOADER_DESCRIPTION",
				primaryColor = new Color(0.40392157f, 0.4392157f, 0.87058824f),
				unlockableName = "Characters.Loader"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Croco, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("CrocoBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CrocoDisplay"),
				descriptionToken = "CROCO_DESCRIPTION",
				primaryColor = new Color(0.7882353f, 0.9490196f, 0.3019608f),
				unlockableName = "Characters.Croco"
			});
			for (SurvivorIndex survivorIndex = SurvivorIndex.Commando; survivorIndex < SurvivorIndex.Count; survivorIndex++)
			{
				if (SurvivorCatalog.survivorDefs[(int)survivorIndex] == null)
				{
					Debug.LogWarningFormat("Unregistered survivor {0}!", new object[]
					{
						Enum.GetName(typeof(SurvivorIndex), survivorIndex)
					});
				}
			}
			List<SurvivorDef> list = new List<SurvivorDef>();
			Action<List<SurvivorDef>> action = SurvivorCatalog.getAdditionalSurvivorDefs;
			if (action != null)
			{
				action(list);
			}
			Array.Resize<SurvivorDef>(ref SurvivorCatalog.survivorDefs, SurvivorCatalog.survivorDefs.Length + list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Count + i, list[i]);
			}
			SurvivorCatalog.survivorIndexToBodyIndex = new int[SurvivorCatalog.survivorCount];
			SurvivorCatalog.bodyIndexToSurvivorIndex = new SurvivorIndex[BodyCatalog.bodyCount];
			HGArrayUtilities.SetAll<int>(SurvivorCatalog.survivorIndexToBodyIndex, -1);
			HGArrayUtilities.SetAll<SurvivorIndex>(SurvivorCatalog.bodyIndexToSurvivorIndex, SurvivorIndex.None);
			for (int j = 0; j < SurvivorCatalog.survivorDefs.Length; j++)
			{
				SurvivorDef survivorDef = SurvivorCatalog.survivorDefs[j];
				int num = (survivorDef != null) ? survivorDef.bodyPrefab.GetComponent<CharacterBody>().bodyIndex : -1;
				SurvivorCatalog.survivorIndexToBodyIndex[j] = num;
				if (num != -1)
				{
					SurvivorCatalog.bodyIndexToSurvivorIndex[num] = (SurvivorIndex)j;
				}
			}
			SurvivorCatalog._allSurvivorDefs = (from v in SurvivorCatalog.survivorDefs
			where v != null
			select v).ToArray<SurvivorDef>();
			ViewablesCatalog.Node node = new ViewablesCatalog.Node("Survivors", true, null);
			using (IEnumerator<SurvivorDef> enumerator = SurvivorCatalog.allSurvivorDefs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SurvivorCatalog.<>c__DisplayClass22_0 CS$<>8__locals1 = new SurvivorCatalog.<>c__DisplayClass22_0();
					CS$<>8__locals1.survivorDef = enumerator.Current;
					ViewablesCatalog.Node survivorEntryNode = new ViewablesCatalog.Node(CS$<>8__locals1.survivorDef.survivorIndex.ToString(), false, node);
					survivorEntryNode.shouldShowUnviewed = ((UserProfile userProfile) => !userProfile.HasViewedViewable(survivorEntryNode.fullName) && userProfile.HasSurvivorUnlocked(CS$<>8__locals1.survivorDef.survivorIndex) && !string.IsNullOrEmpty(CS$<>8__locals1.survivorDef.unlockableName));
				}
			}
			ViewablesCatalog.AddNodeToRoot(node);
		}

		// Token: 0x04001860 RID: 6240
		public static int survivorMaxCount = 10;

		// Token: 0x04001861 RID: 6241
		private static SurvivorDef[] survivorDefs;

		// Token: 0x04001862 RID: 6242
		private static SurvivorDef[] _allSurvivorDefs;

		// Token: 0x04001863 RID: 6243
		private static SurvivorIndex[] bodyIndexToSurvivorIndex;

		// Token: 0x04001864 RID: 6244
		private static int[] survivorIndexToBodyIndex;

		// Token: 0x04001865 RID: 6245
		public static SurvivorIndex[] idealSurvivorOrder = new SurvivorIndex[]
		{
			SurvivorIndex.Commando,
			SurvivorIndex.Huntress,
			SurvivorIndex.Toolbot,
			SurvivorIndex.Engi,
			SurvivorIndex.Mage,
			SurvivorIndex.Merc,
			SurvivorIndex.Treebot,
			SurvivorIndex.Loader,
			SurvivorIndex.Croco
		};
	}
}
