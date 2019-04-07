using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005AA RID: 1450
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class AchievementListPanelController : MonoBehaviour
	{
		// Token: 0x0600207F RID: 8319 RVA: 0x0009926A File Offset: 0x0009746A
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002080 RID: 8320 RVA: 0x00099278 File Offset: 0x00097478
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x06002081 RID: 8321 RVA: 0x00099280 File Offset: 0x00097480
		private UserProfile GetUserProfile()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem)
			{
				LocalUser localUser = LocalUserManager.FindLocalUser(eventSystem.player);
				if (localUser != null)
				{
					return localUser.userProfile;
				}
			}
			return null;
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x000992B8 File Offset: 0x000974B8
		static AchievementListPanelController()
		{
			AchievementListPanelController.BuildAchievementListOrder();
			AchievementManager.onAchievementsRegistered += AchievementListPanelController.BuildAchievementListOrder;
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x000992DC File Offset: 0x000974DC
		private static void BuildAchievementListOrder()
		{
			AchievementListPanelController.sortedAchievementIdentifiers.Clear();
			HashSet<string> encounteredIdentifiers = new HashSet<string>();
			ReadOnlyCollection<string> readOnlyAchievementIdentifiers = AchievementManager.readOnlyAchievementIdentifiers;
			for (int i = 0; i < readOnlyAchievementIdentifiers.Count; i++)
			{
				string achievementIdentifier = readOnlyAchievementIdentifiers[i];
				if (string.IsNullOrEmpty(AchievementManager.GetAchievementDef(achievementIdentifier).prerequisiteAchievementIdentifier))
				{
					AchievementListPanelController.AddAchievementToOrderedList(achievementIdentifier, encounteredIdentifiers);
				}
			}
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x00099334 File Offset: 0x00097534
		private static void AddAchievementToOrderedList(string achievementIdentifier, HashSet<string> encounteredIdentifiers)
		{
			if (encounteredIdentifiers.Contains(achievementIdentifier))
			{
				return;
			}
			encounteredIdentifiers.Add(achievementIdentifier);
			AchievementListPanelController.sortedAchievementIdentifiers.Add(achievementIdentifier);
			string[] childAchievementIdentifiers = AchievementManager.GetAchievementDef(achievementIdentifier).childAchievementIdentifiers;
			for (int i = 0; i < childAchievementIdentifiers.Length; i++)
			{
				AchievementListPanelController.AddAchievementToOrderedList(childAchievementIdentifiers[i], encounteredIdentifiers);
			}
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x00099384 File Offset: 0x00097584
		private void SetCardCount(int desiredCardCount)
		{
			while (this.cardsList.Count < desiredCardCount)
			{
				AchievementCardController component = UnityEngine.Object.Instantiate<GameObject>(this.achievementCardPrefab, this.container).GetComponent<AchievementCardController>();
				this.cardsList.Add(component);
			}
			while (this.cardsList.Count > desiredCardCount)
			{
				UnityEngine.Object.Destroy(this.cardsList[this.cardsList.Count - 1].gameObject);
				this.cardsList.RemoveAt(this.cardsList.Count - 1);
			}
		}

		// Token: 0x06002086 RID: 8326 RVA: 0x00099410 File Offset: 0x00097610
		private void Rebuild()
		{
			UserProfile userProfile = this.GetUserProfile();
			this.SetCardCount(AchievementListPanelController.sortedAchievementIdentifiers.Count);
			for (int i = 0; i < AchievementListPanelController.sortedAchievementIdentifiers.Count; i++)
			{
				this.cardsList[i].SetAchievement(AchievementListPanelController.sortedAchievementIdentifiers[i], userProfile);
			}
		}

		// Token: 0x04002313 RID: 8979
		public GameObject achievementCardPrefab;

		// Token: 0x04002314 RID: 8980
		public RectTransform container;

		// Token: 0x04002315 RID: 8981
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002316 RID: 8982
		private readonly List<AchievementCardController> cardsList = new List<AchievementCardController>();

		// Token: 0x04002317 RID: 8983
		private static readonly List<string> sortedAchievementIdentifiers = new List<string>();
	}
}
