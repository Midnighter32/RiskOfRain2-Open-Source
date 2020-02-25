using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Rewired;
using RoR2.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000598 RID: 1432
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CharacterSelectController : MonoBehaviour
	{
		// Token: 0x17000397 RID: 919
		// (get) Token: 0x0600220B RID: 8715 RVA: 0x000930D5 File Offset: 0x000912D5
		private NetworkUser networkUser
		{
			get
			{
				LocalUser localUser = this.localUser;
				if (localUser == null)
				{
					return null;
				}
				return localUser.currentNetworkUser;
			}
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x000930E8 File Offset: 0x000912E8
		private void SetEventSystem(MPEventSystem newEventSystem)
		{
			if (newEventSystem == this.eventSystem)
			{
				return;
			}
			this.eventSystem = newEventSystem;
			this.localUser = LocalUserManager.FindLocalUser(newEventSystem.player);
			this.RebuildLocal();
		}

		// Token: 0x0600220D RID: 8717 RVA: 0x00093117 File Offset: 0x00091317
		public void SelectSurvivor(SurvivorIndex survivor)
		{
			this.selectedSurvivorIndex = survivor;
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x00093120 File Offset: 0x00091320
		private static UnlockableDef[] GenerateLoadoutAssociatedUnlockableDefs()
		{
			CharacterSelectController.<>c__DisplayClass23_0 CS$<>8__locals1;
			CS$<>8__locals1.encounteredUnlockables = new HashSet<UnlockableDef>();
			foreach (SkillFamily skillFamily in SkillCatalog.allSkillFamilies)
			{
				for (int i = 0; i < skillFamily.variants.Length; i++)
				{
					CharacterSelectController.<GenerateLoadoutAssociatedUnlockableDefs>g__TryAddUnlockable|23_0(skillFamily.variants[i].unlockableName, ref CS$<>8__locals1);
				}
			}
			foreach (CharacterBody characterBody in BodyCatalog.allBodyPrefabBodyBodyComponents)
			{
				SkinDef[] bodySkins = BodyCatalog.GetBodySkins(characterBody.bodyIndex);
				for (int j = 0; j < bodySkins.Length; j++)
				{
					CharacterSelectController.<GenerateLoadoutAssociatedUnlockableDefs>g__TryAddUnlockable|23_0(bodySkins[j].unlockableName, ref CS$<>8__locals1);
				}
			}
			return CS$<>8__locals1.encounteredUnlockables.ToArray<UnlockableDef>();
		}

		// Token: 0x0600220F RID: 8719 RVA: 0x00093210 File Offset: 0x00091410
		private static bool UserHasAnyLoadoutUnlockables(LocalUser localUser)
		{
			if (CharacterSelectController.loadoutAssociatedUnlockableDefs == null)
			{
				CharacterSelectController.loadoutAssociatedUnlockableDefs = CharacterSelectController.GenerateLoadoutAssociatedUnlockableDefs();
			}
			UserProfile userProfile = localUser.userProfile;
			foreach (UnlockableDef unlockableDef in CharacterSelectController.loadoutAssociatedUnlockableDefs)
			{
				if (userProfile.HasUnlockable(unlockableDef))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002210 RID: 8720 RVA: 0x0009325C File Offset: 0x0009145C
		private void RebuildLocal()
		{
			Loadout loadout = new Loadout();
			LocalUser localUser = this.localUser;
			if (localUser != null)
			{
				UserProfile userProfile = localUser.userProfile;
				if (userProfile != null)
				{
					userProfile.CopyLoadout(loadout);
				}
			}
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(this.selectedSurvivorIndex);
			int bodyIndexFromSurvivorIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex(this.selectedSurvivorIndex);
			Color color = Color.white;
			string text = string.Empty;
			string text2 = string.Empty;
			string viewableName = string.Empty;
			if (survivorDef != null)
			{
				color = survivorDef.primaryColor;
				text = Language.GetString(survivorDef.displayNameToken);
				text2 = Language.GetString(survivorDef.descriptionToken);
			}
			List<CharacterSelectController.StripDisplayData> list = new List<CharacterSelectController.StripDisplayData>();
			if (bodyIndexFromSurvivorIndex != -1)
			{
				GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(bodyIndexFromSurvivorIndex);
				SkillLocator component = bodyPrefab.GetComponent<SkillLocator>();
				if (component.passiveSkill.enabled)
				{
					list.Add(new CharacterSelectController.StripDisplayData
					{
						enabled = true,
						primaryColor = color,
						icon = component.passiveSkill.icon,
						titleString = Language.GetString(component.passiveSkill.skillNameToken),
						descriptionString = Language.GetString(component.passiveSkill.skillDescriptionToken)
					});
				}
				GenericSkill[] components = bodyPrefab.GetComponents<GenericSkill>();
				for (int i = 0; i < components.Length; i++)
				{
					uint skillVariant = loadout.bodyLoadoutManager.GetSkillVariant(bodyIndexFromSurvivorIndex, i);
					SkillDef skillDef = components[i].skillFamily.variants[(int)skillVariant].skillDef;
					list.Add(new CharacterSelectController.StripDisplayData
					{
						enabled = true,
						primaryColor = color,
						icon = skillDef.icon,
						titleString = Language.GetString(skillDef.skillNameToken),
						descriptionString = Language.GetString(skillDef.skillDescriptionToken)
					});
				}
				viewableName = "/Loadout/Bodies/" + BodyCatalog.GetBodyName(bodyIndexFromSurvivorIndex) + "/";
			}
			this.skillStripAllocator.AllocateElements(list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				this.RebuildStrip(this.skillStripAllocator.elements[j], list[j]);
			}
			this.survivorName.SetText(text);
			this.survivorDescription.SetText(text2);
			Image[] array = this.primaryColorImages;
			for (int k = 0; k < array.Length; k++)
			{
				array[k].color = color;
			}
			TextMeshProUGUI[] array2 = this.primaryColorTexts;
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k].color = color;
			}
			this.loadoutViewableTag.viewableName = viewableName;
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x000934E8 File Offset: 0x000916E8
		private void RebuildStrip(RectTransform skillStrip, CharacterSelectController.StripDisplayData stripDisplayData)
		{
			GameObject gameObject = skillStrip.gameObject;
			Image component = skillStrip.Find("Icon").GetComponent<Image>();
			HGTextMeshProUGUI component2 = skillStrip.Find("SkillDescriptionPanel/SkillName").GetComponent<HGTextMeshProUGUI>();
			HGTextMeshProUGUI component3 = skillStrip.Find("SkillDescriptionPanel/SkillDescription").GetComponent<HGTextMeshProUGUI>();
			if (stripDisplayData.enabled)
			{
				gameObject.SetActive(true);
				component.sprite = stripDisplayData.icon;
				component2.SetText(stripDisplayData.titleString);
				component2.color = stripDisplayData.primaryColor;
				component3.SetText(stripDisplayData.descriptionString);
				return;
			}
			gameObject.SetActive(false);
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x00093578 File Offset: 0x00091778
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.skillStripAllocator = new UIElementAllocator<RectTransform>(this.skillStripContainer, this.skillStripPrefab);
			this.SetEventSystem(this.eventSystemLocator.eventSystem);
			bool active = true;
			this.loadoutHeaderButton.SetActive(active);
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x000935C7 File Offset: 0x000917C7
		private void Start()
		{
			this.selectedSurvivorIndex = this.GetSelectedSurvivorIndexFromBodyPreference();
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x000935D5 File Offset: 0x000917D5
		private void OnEnable()
		{
			UserProfile.onLoadoutChangedGlobal += this.OnLoadoutChangedGlobal;
			NetworkUser.onLoadoutChangedGlobal += this.OnNetworkUserLoadoutChanged;
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x000935F9 File Offset: 0x000917F9
		private void OnLoadoutChangedGlobal(UserProfile userProfile)
		{
			LocalUser localUser = this.localUser;
			if (userProfile == ((localUser != null) ? localUser.userProfile : null))
			{
				this.RebuildLocal();
			}
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x00093616 File Offset: 0x00091816
		private void OnDisable()
		{
			NetworkUser.onLoadoutChangedGlobal -= this.OnNetworkUserLoadoutChanged;
			UserProfile.onLoadoutChangedGlobal -= this.OnLoadoutChangedGlobal;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x0009363C File Offset: 0x0009183C
		private SurvivorIndex GetSelectedSurvivorIndexFromBodyPreference()
		{
			if (this.networkUser)
			{
				SurvivorIndex survivorIndexFromBodyIndex = SurvivorCatalog.GetSurvivorIndexFromBodyIndex(this.networkUser.bodyIndexPreference);
				Debug.Log(survivorIndexFromBodyIndex);
				if (survivorIndexFromBodyIndex != SurvivorIndex.None)
				{
					return survivorIndexFromBodyIndex;
				}
			}
			return SurvivorIndex.Commando;
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x0009367C File Offset: 0x0009187C
		private List<NetworkUser> GetSortedNetworkUsersList()
		{
			List<NetworkUser> list = new List<NetworkUser>(NetworkUser.readOnlyInstancesList.Count);
			list.AddRange(NetworkUser.readOnlyLocalPlayersList);
			for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
			{
				NetworkUser item = NetworkUser.readOnlyInstancesList[i];
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x000936D8 File Offset: 0x000918D8
		private void Update()
		{
			this.SetEventSystem(this.eventSystemLocator.eventSystem);
			if (this.previousSurvivorIndex != this.selectedSurvivorIndex)
			{
				this.RebuildLocal();
				this.previousSurvivorIndex = this.selectedSurvivorIndex;
			}
			if (this.characterDisplayPads.Length != 0)
			{
				List<NetworkUser> sortedNetworkUsersList = this.GetSortedNetworkUsersList();
				for (int i = 0; i < this.characterDisplayPads.Length; i++)
				{
					ref CharacterSelectController.CharacterPad ptr = ref this.characterDisplayPads[i];
					NetworkUser networkUser = null;
					if (i < sortedNetworkUsersList.Count)
					{
						networkUser = sortedNetworkUsersList[i];
					}
					if (networkUser)
					{
						GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(networkUser.bodyIndexPreference);
						SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);
						if (survivorDef != null)
						{
							SurvivorDef survivorDef2 = SurvivorCatalog.GetSurvivorDef(ptr.displaySurvivorIndex);
							bool flag = true;
							if (survivorDef2 != null && survivorDef2.bodyPrefab == bodyPrefab)
							{
								flag = false;
							}
							if (flag)
							{
								GameObject displayPrefab = survivorDef.displayPrefab;
								this.ClearPadDisplay(ptr);
								if (displayPrefab)
								{
									ptr.displayInstance = UnityEngine.Object.Instantiate<GameObject>(displayPrefab, ptr.padTransform.position, ptr.padTransform.rotation, ptr.padTransform);
								}
								ptr.displaySurvivorIndex = survivorDef.survivorIndex;
								this.OnNetworkUserLoadoutChanged(networkUser);
							}
						}
						else
						{
							this.ClearPadDisplay(ptr);
						}
					}
					else
					{
						this.ClearPadDisplay(ptr);
					}
					if (!ptr.padTransform)
					{
						return;
					}
					if (this.characterDisplayPads[i].padTransform)
					{
						this.characterDisplayPads[i].padTransform.gameObject.SetActive(this.characterDisplayPads[i].displayInstance != null);
					}
				}
			}
			if (!RoR2Application.isInSinglePlayer)
			{
				bool flag2 = this.IsClientReady();
				this.readyButton.gameObject.SetActive(!flag2);
				this.unreadyButton.gameObject.SetActive(flag2);
			}
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x000938BA File Offset: 0x00091ABA
		private void ClearPadDisplay(CharacterSelectController.CharacterPad characterPad)
		{
			if (characterPad.displayInstance)
			{
				UnityEngine.Object.Destroy(characterPad.displayInstance);
			}
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x000938D4 File Offset: 0x00091AD4
		private static bool InputPlayerIsAssigned(Player inputPlayer)
		{
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				if (readOnlyInstancesList[i].inputPlayer == inputPlayer)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x0009390C File Offset: 0x00091B0C
		public bool IsClientReady()
		{
			int num = 0;
			if (!PreGameController.instance)
			{
				return false;
			}
			VoteController component = PreGameController.instance.GetComponent<VoteController>();
			if (!component)
			{
				return false;
			}
			int i = 0;
			int voteCount = component.GetVoteCount();
			while (i < voteCount)
			{
				VoteController.UserVote vote = component.GetVote(i);
				if (vote.networkUserObject && vote.receivedVote)
				{
					NetworkUser component2 = vote.networkUserObject.GetComponent<NetworkUser>();
					if (component2 && component2.isLocalPlayer)
					{
						num++;
					}
				}
				i++;
			}
			return num == NetworkUser.readOnlyLocalPlayersList.Count;
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x000939A4 File Offset: 0x00091BA4
		public void ClientSetReady()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
			{
				if (networkUser)
				{
					networkUser.CallCmdSubmitVote(PreGameController.instance.gameObject, 0);
				}
				else
				{
					Debug.Log("Null network user in readonly local player list!!");
				}
			}
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x00093A10 File Offset: 0x00091C10
		public void ClientSetUnready()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
			{
				networkUser.CallCmdSubmitVote(PreGameController.instance.gameObject, -1);
			}
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x00093A64 File Offset: 0x00091C64
		private void OnNetworkUserLoadoutChanged(NetworkUser networkUser)
		{
			int num = this.GetSortedNetworkUsersList().IndexOf(networkUser);
			if (num != -1)
			{
				CharacterSelectController.CharacterPad safe = HGArrayUtilities.GetSafe<CharacterSelectController.CharacterPad>(this.characterDisplayPads, num);
				if (safe.displayInstance)
				{
					Loadout loadout = new Loadout();
					networkUser.networkLoadout.CopyLoadout(loadout);
					int bodyIndexFromSurvivorIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex(safe.displaySurvivorIndex);
					int skinIndex = (int)loadout.bodyLoadoutManager.GetSkinIndex(bodyIndexFromSurvivorIndex);
					SkinDef safe2 = HGArrayUtilities.GetSafe<SkinDef>(BodyCatalog.GetBodySkins(bodyIndexFromSurvivorIndex), skinIndex);
					CharacterModel componentInChildren = safe.displayInstance.GetComponentInChildren<CharacterModel>();
					if (componentInChildren && safe2 != null)
					{
						safe2.Apply(componentInChildren.gameObject);
					}
				}
			}
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x00093B10 File Offset: 0x00091D10
		[CompilerGenerated]
		internal static void <GenerateLoadoutAssociatedUnlockableDefs>g__TryAddUnlockable|23_0(string unlockableName, ref CharacterSelectController.<>c__DisplayClass23_0 A_1)
		{
			if (unlockableName == null)
			{
				return;
			}
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef != null)
			{
				A_1.encounteredUnlockables.Add(unlockableDef);
			}
		}

		// Token: 0x04001F64 RID: 8036
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001F65 RID: 8037
		private MPEventSystem eventSystem;

		// Token: 0x04001F66 RID: 8038
		private LocalUser localUser;

		// Token: 0x04001F67 RID: 8039
		public SurvivorIndex selectedSurvivorIndex;

		// Token: 0x04001F68 RID: 8040
		private SurvivorIndex previousSurvivorIndex = SurvivorIndex.None;

		// Token: 0x04001F69 RID: 8041
		public TextMeshProUGUI survivorName;

		// Token: 0x04001F6A RID: 8042
		public GameObject skillStripPrefab;

		// Token: 0x04001F6B RID: 8043
		public RectTransform skillStripContainer;

		// Token: 0x04001F6C RID: 8044
		private UIElementAllocator<RectTransform> skillStripAllocator;

		// Token: 0x04001F6D RID: 8045
		public TextMeshProUGUI survivorDescription;

		// Token: 0x04001F6E RID: 8046
		public CharacterSelectController.CharacterPad[] characterDisplayPads;

		// Token: 0x04001F6F RID: 8047
		public Image[] primaryColorImages;

		// Token: 0x04001F70 RID: 8048
		public TextMeshProUGUI[] primaryColorTexts;

		// Token: 0x04001F71 RID: 8049
		public MPButton readyButton;

		// Token: 0x04001F72 RID: 8050
		public MPButton unreadyButton;

		// Token: 0x04001F73 RID: 8051
		[Tooltip("The header button for the loadout tab. Will be disabled if the user has no unlocked loadout options.")]
		public GameObject loadoutHeaderButton;

		// Token: 0x04001F74 RID: 8052
		public ViewableTag loadoutViewableTag;

		// Token: 0x04001F75 RID: 8053
		private static UnlockableDef[] loadoutAssociatedUnlockableDefs;

		// Token: 0x02000599 RID: 1433
		[Serializable]
		public struct SkillStrip
		{
			// Token: 0x04001F76 RID: 8054
			public GameObject stripRoot;

			// Token: 0x04001F77 RID: 8055
			public Image skillIcon;

			// Token: 0x04001F78 RID: 8056
			public TextMeshProUGUI skillName;

			// Token: 0x04001F79 RID: 8057
			public TextMeshProUGUI skillDescription;
		}

		// Token: 0x0200059A RID: 1434
		[Serializable]
		public struct CharacterPad
		{
			// Token: 0x04001F7A RID: 8058
			public Transform padTransform;

			// Token: 0x04001F7B RID: 8059
			public GameObject displayInstance;

			// Token: 0x04001F7C RID: 8060
			public SurvivorIndex displaySurvivorIndex;
		}

		// Token: 0x0200059B RID: 1435
		private struct StripDisplayData
		{
			// Token: 0x04001F7D RID: 8061
			public bool enabled;

			// Token: 0x04001F7E RID: 8062
			public Color primaryColor;

			// Token: 0x04001F7F RID: 8063
			public Sprite icon;

			// Token: 0x04001F80 RID: 8064
			public string titleString;

			// Token: 0x04001F81 RID: 8065
			public string descriptionString;
		}
	}
}
