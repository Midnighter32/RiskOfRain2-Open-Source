using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BE RID: 1470
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CharacterSelectController : MonoBehaviour
	{
		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060020E9 RID: 8425 RVA: 0x0009A911 File Offset: 0x00098B11
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

		// Token: 0x060020EA RID: 8426 RVA: 0x0009A924 File Offset: 0x00098B24
		private void SetEventSystem(MPEventSystem newEventSystem)
		{
			if (newEventSystem == this.eventSystem)
			{
				return;
			}
			this.eventSystem = newEventSystem;
			this.localUser = LocalUserManager.FindLocalUser(newEventSystem.player);
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x0009A94D File Offset: 0x00098B4D
		public void SelectSurvivor(SurvivorIndex survivor)
		{
			this.selectedSurvivorIndex = survivor;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x0009A958 File Offset: 0x00098B58
		private void RebuildLocal()
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(this.selectedSurvivorIndex);
			this.survivorName.text = survivorDef.displayNameToken;
			if (survivorDef.descriptionToken != null)
			{
				this.survivorDescription.text = Language.GetString(survivorDef.descriptionToken);
			}
			SkillLocator component = survivorDef.bodyPrefab.GetComponent<SkillLocator>();
			if (component)
			{
				this.RebuildStrip(this.primarySkillStrip, component.primary);
				this.RebuildStrip(this.secondarySkillStrip, component.secondary);
				this.RebuildStrip(this.utilitySkillStrip, component.utility);
				this.RebuildStrip(this.specialSkillStrip, component.special);
				this.RebuildStrip(this.passiveSkillStrip, component.passiveSkill);
			}
			Image[] array = this.primaryColorImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = survivorDef.primaryColor;
			}
			TextMeshProUGUI[] array2 = this.primaryColorTexts;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].color = survivorDef.primaryColor;
			}
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x0009AA58 File Offset: 0x00098C58
		private void RebuildStrip(CharacterSelectController.SkillStrip skillStrip, GenericSkill skill)
		{
			if (skill)
			{
				skillStrip.stripRoot.SetActive(true);
				skillStrip.skillIcon.sprite = skill.icon;
				skillStrip.skillDescription.text = Language.GetString(skill.skillDescriptionToken);
				skillStrip.skillName.text = Language.GetString(skill.skillNameToken);
				return;
			}
			skillStrip.stripRoot.SetActive(false);
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x0009AAC4 File Offset: 0x00098CC4
		private void RebuildStrip(CharacterSelectController.SkillStrip skillStrip, SkillLocator.PassiveSkill skill)
		{
			if (skill.enabled)
			{
				skillStrip.stripRoot.SetActive(true);
				skillStrip.skillIcon.sprite = skill.icon;
				skillStrip.skillDescription.text = Language.GetString(skill.skillDescriptionToken);
				skillStrip.skillName.text = Language.GetString(skill.skillNameToken);
				return;
			}
			skillStrip.stripRoot.SetActive(false);
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x0009AB2F File Offset: 0x00098D2F
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.SetEventSystem(this.eventSystemLocator.eventSystem);
			this.selectedSurvivorIndex = this.GetSelectedSurvivorIndexFromBodyPreference();
			this.RebuildLocal();
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x0009AB60 File Offset: 0x00098D60
		private SurvivorIndex GetSelectedSurvivorIndexFromBodyPreference()
		{
			if (this.networkUser)
			{
				SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(BodyCatalog.GetBodyPrefab(this.networkUser.bodyIndexPreference));
				if (survivorDef != null && survivorDef.survivorIndex != SurvivorIndex.None)
				{
					return survivorDef.survivorIndex;
				}
			}
			return SurvivorIndex.Commando;
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x0009ABA4 File Offset: 0x00098DA4
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
				for (int i = 0; i < this.characterDisplayPads.Length; i++)
				{
					CharacterSelectController.CharacterPad characterPad = this.characterDisplayPads[i];
					NetworkUser networkUser = null;
					List<NetworkUser> list = new List<NetworkUser>(NetworkUser.readOnlyInstancesList.Count);
					list.AddRange(NetworkUser.readOnlyLocalPlayersList);
					for (int j = 0; j < NetworkUser.readOnlyInstancesList.Count; j++)
					{
						NetworkUser item = NetworkUser.readOnlyInstancesList[j];
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
					if (i < list.Count)
					{
						networkUser = list[i];
					}
					if (networkUser)
					{
						GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(networkUser.bodyIndexPreference);
						SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);
						if (survivorDef != null)
						{
							SurvivorDef survivorDef2 = SurvivorCatalog.GetSurvivorDef(characterPad.displaySurvivorIndex);
							bool flag = true;
							if (survivorDef2 != null && survivorDef2.bodyPrefab == bodyPrefab)
							{
								flag = false;
							}
							if (flag)
							{
								GameObject displayPrefab = survivorDef.displayPrefab;
								this.ClearPadDisplay(characterPad);
								if (displayPrefab)
								{
									characterPad.displayInstance = UnityEngine.Object.Instantiate<GameObject>(displayPrefab, characterPad.padTransform.position, characterPad.padTransform.rotation, characterPad.padTransform);
								}
								characterPad.displaySurvivorIndex = survivorDef.survivorIndex;
							}
						}
						else
						{
							this.ClearPadDisplay(characterPad);
						}
					}
					else
					{
						this.ClearPadDisplay(characterPad);
					}
					if (!characterPad.padTransform)
					{
						return;
					}
					this.characterDisplayPads[i] = characterPad;
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

		// Token: 0x060020F2 RID: 8434 RVA: 0x0009ADC9 File Offset: 0x00098FC9
		private void ClearPadDisplay(CharacterSelectController.CharacterPad characterPad)
		{
			if (characterPad.displayInstance)
			{
				UnityEngine.Object.Destroy(characterPad.displayInstance);
			}
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x0009ADE4 File Offset: 0x00098FE4
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

		// Token: 0x060020F4 RID: 8436 RVA: 0x0009AE1C File Offset: 0x0009901C
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

		// Token: 0x060020F5 RID: 8437 RVA: 0x0009AEB4 File Offset: 0x000990B4
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

		// Token: 0x060020F6 RID: 8438 RVA: 0x0009AF20 File Offset: 0x00099120
		public void ClientSetUnready()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
			{
				networkUser.CallCmdSubmitVote(PreGameController.instance.gameObject, -1);
			}
		}

		// Token: 0x0400236E RID: 9070
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400236F RID: 9071
		private MPEventSystem eventSystem;

		// Token: 0x04002370 RID: 9072
		private LocalUser localUser;

		// Token: 0x04002371 RID: 9073
		public SurvivorIndex selectedSurvivorIndex;

		// Token: 0x04002372 RID: 9074
		private SurvivorIndex previousSurvivorIndex = SurvivorIndex.None;

		// Token: 0x04002373 RID: 9075
		public TextMeshProUGUI survivorName;

		// Token: 0x04002374 RID: 9076
		public CharacterSelectController.SkillStrip primarySkillStrip;

		// Token: 0x04002375 RID: 9077
		public CharacterSelectController.SkillStrip secondarySkillStrip;

		// Token: 0x04002376 RID: 9078
		public CharacterSelectController.SkillStrip utilitySkillStrip;

		// Token: 0x04002377 RID: 9079
		public CharacterSelectController.SkillStrip specialSkillStrip;

		// Token: 0x04002378 RID: 9080
		public CharacterSelectController.SkillStrip passiveSkillStrip;

		// Token: 0x04002379 RID: 9081
		public TextMeshProUGUI survivorDescription;

		// Token: 0x0400237A RID: 9082
		public CharacterSelectController.CharacterPad[] characterDisplayPads;

		// Token: 0x0400237B RID: 9083
		public Image[] primaryColorImages;

		// Token: 0x0400237C RID: 9084
		public TextMeshProUGUI[] primaryColorTexts;

		// Token: 0x0400237D RID: 9085
		public MPButton readyButton;

		// Token: 0x0400237E RID: 9086
		public MPButton unreadyButton;

		// Token: 0x020005BF RID: 1471
		[Serializable]
		public struct SkillStrip
		{
			// Token: 0x0400237F RID: 9087
			public GameObject stripRoot;

			// Token: 0x04002380 RID: 9088
			public Image skillIcon;

			// Token: 0x04002381 RID: 9089
			public TextMeshProUGUI skillName;

			// Token: 0x04002382 RID: 9090
			public TextMeshProUGUI skillDescription;
		}

		// Token: 0x020005C0 RID: 1472
		[Serializable]
		public struct CharacterPad
		{
			// Token: 0x04002383 RID: 9091
			public Transform padTransform;

			// Token: 0x04002384 RID: 9092
			public GameObject displayInstance;

			// Token: 0x04002385 RID: 9093
			public SurvivorIndex displaySurvivorIndex;
		}
	}
}
