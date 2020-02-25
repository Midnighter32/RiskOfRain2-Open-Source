using System;
using System.Collections.Generic;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E2 RID: 1506
	[RequireComponent(typeof(MPEventSystemLocator))]
	[RequireComponent(typeof(RectTransform))]
	public class LoadoutPanelController : MonoBehaviour
	{
		// Token: 0x06002399 RID: 9113 RVA: 0x0009B661 File Offset: 0x00099861
		public void SetDisplayData(LoadoutPanelController.DisplayData displayData)
		{
			if (displayData.Equals(this.currentDisplayData))
			{
				return;
			}
			this.currentDisplayData = displayData;
			this.Rebuild();
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x0009B680 File Offset: 0x00099880
		private void Update()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			UserProfile userProfile;
			if (eventSystem == null)
			{
				userProfile = null;
			}
			else
			{
				LocalUser localUser = eventSystem.localUser;
				userProfile = ((localUser != null) ? localUser.userProfile : null);
			}
			UserProfile userProfile2 = userProfile;
			MPEventSystem eventSystem2 = this.eventSystemLocator.eventSystem;
			NetworkUser networkUser;
			if (eventSystem2 == null)
			{
				networkUser = null;
			}
			else
			{
				LocalUser localUser2 = eventSystem2.localUser;
				networkUser = ((localUser2 != null) ? localUser2.currentNetworkUser : null);
			}
			NetworkUser networkUser2 = networkUser;
			int bodyIndex = networkUser2 ? networkUser2.bodyIndexPreference : -1;
			this.SetDisplayData(new LoadoutPanelController.DisplayData
			{
				userProfile = userProfile2,
				bodyIndex = bodyIndex
			});
		}

		// Token: 0x0600239B RID: 9115 RVA: 0x0009B708 File Offset: 0x00099908
		private void DestroyRows()
		{
			for (int i = this.rows.Count - 1; i >= 0; i--)
			{
				this.rows[i].Dispose();
			}
			this.rows.Clear();
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x0009B74C File Offset: 0x0009994C
		private void Rebuild()
		{
			this.DestroyRows();
			CharacterBody bodyPrefabBodyComponent = BodyCatalog.GetBodyPrefabBodyComponent(this.currentDisplayData.bodyIndex);
			if (bodyPrefabBodyComponent)
			{
				List<GenericSkill> gameObjectComponents = GetComponentsCache<GenericSkill>.GetGameObjectComponents(bodyPrefabBodyComponent.gameObject);
				int i = 0;
				int count = gameObjectComponents.Count;
				while (i < count)
				{
					GenericSkill skillSlot = gameObjectComponents[i];
					this.rows.Add(LoadoutPanelController.Row.FromSkillSlot(this, this.currentDisplayData.bodyIndex, i, skillSlot));
					i++;
				}
				int num = BodyCatalog.GetBodySkins(this.currentDisplayData.bodyIndex).Length;
				if (true)
				{
					this.rows.Add(LoadoutPanelController.Row.FromSkin(this, this.currentDisplayData.bodyIndex));
				}
			}
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x0009B7F1 File Offset: 0x000999F1
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x0009B7FF File Offset: 0x000999FF
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			LoadoutPanelController.loadoutButtonPrefab = Resources.Load<GameObject>("Prefabs/UI/Loadout/LoadoutButton");
			LoadoutPanelController.rowPrefab = Resources.Load<GameObject>("Prefabs/UI/Loadout/Row");
			LoadoutPanelController.lockedIcon = Resources.Load<Sprite>("Textures/MiscIcons/texUnlockIcon");
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x0600239F RID: 9119 RVA: 0x0009B82E File Offset: 0x00099A2E
		private RectTransform rowContainer
		{
			get
			{
				return (RectTransform)base.transform;
			}
		}

		// Token: 0x060023A0 RID: 9120 RVA: 0x0009B83B File Offset: 0x00099A3B
		private void OnDestroy()
		{
			this.DestroyRows();
		}

		// Token: 0x04002190 RID: 8592
		private LoadoutPanelController.DisplayData currentDisplayData = new LoadoutPanelController.DisplayData
		{
			userProfile = null,
			bodyIndex = -1
		};

		// Token: 0x04002191 RID: 8593
		private UIElementAllocator<RectTransform> buttonAllocator;

		// Token: 0x04002192 RID: 8594
		private readonly List<LoadoutPanelController.Row> rows = new List<LoadoutPanelController.Row>();

		// Token: 0x04002193 RID: 8595
		public static int minimumEntriesPerRow = 2;

		// Token: 0x04002194 RID: 8596
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002195 RID: 8597
		private UIElementAllocator<RectTransform> rowAllocator;

		// Token: 0x04002196 RID: 8598
		private static GameObject loadoutButtonPrefab;

		// Token: 0x04002197 RID: 8599
		private static GameObject rowPrefab;

		// Token: 0x04002198 RID: 8600
		private static Sprite lockedIcon;

		// Token: 0x020005E3 RID: 1507
		public struct DisplayData : IEquatable<LoadoutPanelController.DisplayData>
		{
			// Token: 0x060023A3 RID: 9123 RVA: 0x0009B889 File Offset: 0x00099A89
			public bool Equals(LoadoutPanelController.DisplayData other)
			{
				return this.userProfile == other.userProfile && this.bodyIndex == other.bodyIndex;
			}

			// Token: 0x060023A4 RID: 9124 RVA: 0x0009B8AC File Offset: 0x00099AAC
			public override bool Equals(object obj)
			{
				if (obj is LoadoutPanelController.DisplayData)
				{
					LoadoutPanelController.DisplayData other = (LoadoutPanelController.DisplayData)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x060023A5 RID: 9125 RVA: 0x0009B8D3 File Offset: 0x00099AD3
			public override int GetHashCode()
			{
				return ((this.userProfile != null) ? this.userProfile.GetHashCode() : 0) * 397 ^ this.bodyIndex;
			}

			// Token: 0x04002199 RID: 8601
			public UserProfile userProfile;

			// Token: 0x0400219A RID: 8602
			public int bodyIndex;
		}

		// Token: 0x020005E4 RID: 1508
		private class Row : IDisposable
		{
			// Token: 0x060023A6 RID: 9126 RVA: 0x0009B8F8 File Offset: 0x00099AF8
			private Row(LoadoutPanelController owner, int bodyIndex, string titleToken)
			{
				this.owner = owner;
				this.userProfile = owner.currentDisplayData.userProfile;
				this.rowPanelTransform = (RectTransform)UnityEngine.Object.Instantiate<GameObject>(LoadoutPanelController.rowPrefab, owner.rowContainer).transform;
				this.buttonContainerTransform = (RectTransform)this.rowPanelTransform.Find("ButtonContainer");
				this.choiceHighlightRect = (RectTransform)this.rowPanelTransform.Find("ChoiceHighlightRect");
				UserProfile.onLoadoutChangedGlobal += this.OnLoadoutChangedGlobal;
				SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(BodyCatalog.GetBodyPrefab(bodyIndex));
				if (survivorDef != null)
				{
					this.primaryColor = survivorDef.primaryColor;
				}
				float num;
				float s;
				float v;
				Color.RGBToHSV(this.primaryColor, out num, out s, out v);
				num += 0.5f;
				if (num > 1f)
				{
					num -= 1f;
				}
				this.complementaryColor = Color.HSVToRGB(num, s, v);
				RectTransform rectTransform = (RectTransform)this.rowPanelTransform.Find("SlotLabel");
				rectTransform.GetComponent<LanguageTextMeshController>().token = titleToken;
				rectTransform.GetComponent<HGTextMeshProUGUI>().color = this.primaryColor;
				this.choiceHighlightRect.GetComponent<Image>().color = this.complementaryColor;
			}

			// Token: 0x060023A7 RID: 9127 RVA: 0x0009BA2E File Offset: 0x00099C2E
			private void OnLoadoutChangedGlobal(UserProfile userProfile)
			{
				if (userProfile == this.userProfile)
				{
					this.UpdateHighlightedChoice();
				}
			}

			// Token: 0x060023A8 RID: 9128 RVA: 0x0009BA40 File Offset: 0x00099C40
			public static LoadoutPanelController.Row FromSkillSlot(LoadoutPanelController owner, int bodyIndex, int skillSlotIndex, GenericSkill skillSlot)
			{
				SkillFamily skillFamily = skillSlot.skillFamily;
				SkillLocator component = BodyCatalog.GetBodyPrefabBodyComponent(bodyIndex).GetComponent<SkillLocator>();
				bool addWIPIcons = true;
				string titleToken;
				switch (component.FindSkillSlot(skillSlot))
				{
				case SkillSlot.None:
					titleToken = "LOADOUT_SKILL_MISC";
					addWIPIcons = false;
					break;
				case SkillSlot.Primary:
					titleToken = "LOADOUT_SKILL_PRIMARY";
					addWIPIcons = false;
					break;
				case SkillSlot.Secondary:
					titleToken = "LOADOUT_SKILL_SECONDARY";
					break;
				case SkillSlot.Utility:
					titleToken = "LOADOUT_SKILL_UTILITY";
					break;
				case SkillSlot.Special:
					titleToken = "LOADOUT_SKILL_SPECIAL";
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				LoadoutPanelController.Row row = new LoadoutPanelController.Row(owner, bodyIndex, titleToken);
				for (int i = 0; i < skillFamily.variants.Length; i++)
				{
					ref SkillFamily.Variant ptr = ref skillFamily.variants[i];
					uint skillVariantIndexToAssign = (uint)i;
					row.AddButton(ptr.skillDef.icon, ptr.skillDef.skillNameToken, ptr.skillDef.skillDescriptionToken, row.primaryColor, delegate
					{
						Loadout loadout = new Loadout();
						row.userProfile.CopyLoadout(loadout);
						loadout.bodyLoadoutManager.SetSkillVariant(bodyIndex, skillSlotIndex, skillVariantIndexToAssign);
						row.userProfile.SetLoadout(loadout);
					}, ptr.unlockableName, ptr.viewableNode, false);
				}
				row.findCurrentChoice = ((Loadout loadout) => (int)loadout.bodyLoadoutManager.GetSkillVariant(bodyIndex, skillSlotIndex));
				row.FinishSetup(addWIPIcons);
				return row;
			}

			// Token: 0x060023A9 RID: 9129 RVA: 0x0009BBB8 File Offset: 0x00099DB8
			public static LoadoutPanelController.Row FromSkin(LoadoutPanelController owner, int bodyIndex)
			{
				LoadoutPanelController.Row row = new LoadoutPanelController.Row(owner, bodyIndex, "LOADOUT_SKIN");
				SkinDef[] bodySkins = BodyCatalog.GetBodySkins(bodyIndex);
				for (int i = 0; i < bodySkins.Length; i++)
				{
					SkinDef skinDef = bodySkins[i];
					uint skinToAssign = (uint)i;
					ViewablesCatalog.Node viewableNode = ViewablesCatalog.FindNode(string.Format("/Loadout/Bodies/{0}/Skins/{1}", BodyCatalog.GetBodyName(bodyIndex), skinDef.name));
					row.AddButton(skinDef.icon, skinDef.nameToken, string.Empty, row.primaryColor, delegate
					{
						Loadout loadout = new Loadout();
						row.userProfile.CopyLoadout(loadout);
						loadout.bodyLoadoutManager.SetSkinIndex(bodyIndex, skinToAssign);
						row.userProfile.SetLoadout(loadout);
					}, skinDef.unlockableName, viewableNode, false);
				}
				row.findCurrentChoice = ((Loadout loadout) => (int)loadout.bodyLoadoutManager.GetSkinIndex(bodyIndex));
				row.FinishSetup(true);
				return row;
			}

			// Token: 0x060023AA RID: 9130 RVA: 0x0009BCBC File Offset: 0x00099EBC
			private void FinishSetup(bool addWIPIcons = true)
			{
				if (addWIPIcons)
				{
					Sprite icon = Resources.Load<Sprite>("Textures/MiscIcons/texWIPIcon");
					for (int i = this.buttons.Count; i < LoadoutPanelController.minimumEntriesPerRow; i++)
					{
						this.AddButton(icon, "TOOLTIP_WIP_CONTENT_NAME", "TOOLTIP_WIP_CONTENT_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.WIP), delegate
						{
						}, "", null, true);
					}
				}
				RectTransform rectTransform = (RectTransform)this.rowPanelTransform.Find("ButtonContainer/Spacer");
				if (rectTransform)
				{
					rectTransform.SetAsLastSibling();
				}
				this.UpdateHighlightedChoice();
			}

			// Token: 0x060023AB RID: 9131 RVA: 0x0009BD60 File Offset: 0x00099F60
			private void SetButtonColorMultiplier(int i, float f)
			{
				MPButton mpbutton = this.buttons[i];
				ColorBlock colors = mpbutton.colors;
				colors.colorMultiplier = f;
				mpbutton.colors = colors;
			}

			// Token: 0x060023AC RID: 9132 RVA: 0x0009BD90 File Offset: 0x00099F90
			private void UpdateHighlightedChoice()
			{
				foreach (MPButton mpbutton in this.buttons)
				{
					ColorBlock colors = mpbutton.colors;
					colors.colorMultiplier = 0.5f;
					mpbutton.colors = colors;
				}
				for (int i = 0; i < this.buttons.Count; i++)
				{
					this.SetButtonColorMultiplier(i, 0.5f);
				}
				Loadout loadout = new Loadout();
				UserProfile userProfile = this.userProfile;
				if (userProfile != null)
				{
					userProfile.CopyLoadout(loadout);
				}
				int num = this.findCurrentChoice(loadout);
				this.choiceHighlightRect.SetParent((RectTransform)this.buttons[num].transform, false);
				this.SetButtonColorMultiplier(num, 1f);
			}

			// Token: 0x060023AD RID: 9133 RVA: 0x0009BE70 File Offset: 0x0009A070
			private void AddButton(Sprite icon, string titleToken, string bodyToken, Color tooltipColor, UnityAction callback, string unlockableName, ViewablesCatalog.Node viewableNode, bool isWIP = false)
			{
				MPButton component = UnityEngine.Object.Instantiate<GameObject>(LoadoutPanelController.loadoutButtonPrefab, this.buttonContainerTransform).GetComponent<MPButton>();
				TooltipProvider component2 = component.GetComponent<TooltipProvider>();
				UserProfile userProfile = this.userProfile;
				if (userProfile != null && userProfile.HasUnlockable(unlockableName))
				{
					component.onClick.AddListener(callback);
					component.interactable = true;
					if (viewableNode != null)
					{
						ViewableTag component3 = component.GetComponent<ViewableTag>();
						component3.viewableName = viewableNode.fullName;
						component3.Refresh();
					}
					component2.titleToken = titleToken;
					component2.bodyToken = bodyToken;
					component2.titleColor = tooltipColor;
				}
				else
				{
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
					icon = LoadoutPanelController.lockedIcon;
					component.interactable = false;
					component2.titleToken = "UNIDENTIFIED";
					component2.overrideBodyText = unlockableDef.getHowToUnlockString();
					component2.titleColor = Color.gray;
				}
				if (isWIP)
				{
					component.interactable = false;
				}
				((Image)component.targetGraphic).sprite = icon;
				this.buttons.Add(component);
			}

			// Token: 0x060023AE RID: 9134 RVA: 0x0009BF60 File Offset: 0x0009A160
			public void Dispose()
			{
				UserProfile.onLoadoutChangedGlobal -= this.OnLoadoutChangedGlobal;
				for (int i = this.buttons.Count - 1; i >= 0; i--)
				{
					UnityEngine.Object.Destroy(this.buttons[i].gameObject);
				}
				UnityEngine.Object.Destroy(this.rowPanelTransform.gameObject);
			}

			// Token: 0x0400219B RID: 8603
			private List<MPButton> buttons = new List<MPButton>();

			// Token: 0x0400219C RID: 8604
			private LoadoutPanelController owner;

			// Token: 0x0400219D RID: 8605
			private UserProfile userProfile;

			// Token: 0x0400219E RID: 8606
			private RectTransform rowPanelTransform;

			// Token: 0x0400219F RID: 8607
			private RectTransform buttonContainerTransform;

			// Token: 0x040021A0 RID: 8608
			private RectTransform choiceHighlightRect;

			// Token: 0x040021A1 RID: 8609
			private Color primaryColor;

			// Token: 0x040021A2 RID: 8610
			private Color complementaryColor;

			// Token: 0x040021A3 RID: 8611
			private Func<Loadout, int> findCurrentChoice;
		}
	}
}
