using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004B9 RID: 1209
	public static class SurvivorCatalog
	{
		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06001B4A RID: 6986 RVA: 0x0007F8AC File Offset: 0x0007DAAC
		public static IEnumerable<SurvivorDef> allSurvivorDefs
		{
			get
			{
				return SurvivorCatalog._allSurvivorDefs;
			}
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x0007F8B3 File Offset: 0x0007DAB3
		private static void RegisterSurvivor(SurvivorIndex survivorIndex, SurvivorDef survivorDef)
		{
			survivorDef.survivorIndex = survivorIndex;
			SurvivorCatalog.survivorDefs[(int)survivorIndex] = survivorDef;
		}

		// Token: 0x06001B4C RID: 6988 RVA: 0x0007F8C4 File Offset: 0x0007DAC4
		public static SurvivorDef GetSurvivorDef(SurvivorIndex survivorIndex)
		{
			if (survivorIndex < SurvivorIndex.Commando || survivorIndex > SurvivorIndex.Count)
			{
				return null;
			}
			return SurvivorCatalog.survivorDefs[(int)survivorIndex];
		}

		// Token: 0x06001B4D RID: 6989 RVA: 0x0007F8D8 File Offset: 0x0007DAD8
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

		// Token: 0x06001B4E RID: 6990 RVA: 0x0007F918 File Offset: 0x0007DB18
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

		// Token: 0x06001B4F RID: 6991 RVA: 0x0007F958 File Offset: 0x0007DB58
		[SystemInitializer(new Type[]
		{
			typeof(BodyCatalog)
		})]
		private static void Init()
		{
			SurvivorCatalog.survivorDefs = new SurvivorDef[7];
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
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Engineer, new SurvivorDef
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
			SurvivorCatalog._allSurvivorDefs = (from v in SurvivorCatalog.survivorDefs
			where v != null
			select v).ToArray<SurvivorDef>();
			ViewablesCatalog.Node node = new ViewablesCatalog.Node("Survivors", true, null);
			using (IEnumerator<SurvivorDef> enumerator = SurvivorCatalog.allSurvivorDefs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SurvivorCatalog.<>c__DisplayClass10_0 CS$<>8__locals1 = new SurvivorCatalog.<>c__DisplayClass10_0();
					CS$<>8__locals1.survivorDef = enumerator.Current;
					ViewablesCatalog.Node survivorEntryNode = new ViewablesCatalog.Node(CS$<>8__locals1.survivorDef.survivorIndex.ToString(), false, node);
					survivorEntryNode.shouldShowUnviewed = ((UserProfile userProfile) => !userProfile.HasViewedViewable(survivorEntryNode.fullName) && userProfile.HasSurvivorUnlocked(CS$<>8__locals1.survivorDef.survivorIndex) && !string.IsNullOrEmpty(CS$<>8__locals1.survivorDef.unlockableName));
				}
			}
			ViewablesCatalog.AddNodeToRoot(node);
		}

		// Token: 0x04001DE8 RID: 7656
		public static int survivorMaxCount = 10;

		// Token: 0x04001DE9 RID: 7657
		private static SurvivorDef[] survivorDefs;

		// Token: 0x04001DEA RID: 7658
		private static SurvivorDef[] _allSurvivorDefs;

		// Token: 0x04001DEB RID: 7659
		public static SurvivorIndex[] idealSurvivorOrder = new SurvivorIndex[]
		{
			SurvivorIndex.Commando,
			SurvivorIndex.Toolbot,
			SurvivorIndex.Huntress,
			SurvivorIndex.Engineer,
			SurvivorIndex.Mage,
			SurvivorIndex.Merc
		};
	}
}
