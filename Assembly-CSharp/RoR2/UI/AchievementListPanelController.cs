using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200057D RID: 1405
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class AchievementListPanelController : MonoBehaviour
	{
		// Token: 0x0600216F RID: 8559 RVA: 0x00090E22 File Offset: 0x0008F022
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002170 RID: 8560 RVA: 0x00090E30 File Offset: 0x0008F030
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x00090E38 File Offset: 0x0008F038
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

		// Token: 0x06002172 RID: 8562 RVA: 0x00090E70 File Offset: 0x0008F070
		static AchievementListPanelController()
		{
			AchievementListPanelController.BuildAchievementListOrder();
			AchievementManager.onAchievementsRegistered += AchievementListPanelController.BuildAchievementListOrder;
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x00090E94 File Offset: 0x0008F094
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

		// Token: 0x06002174 RID: 8564 RVA: 0x00090EEC File Offset: 0x0008F0EC
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

		// Token: 0x06002175 RID: 8565 RVA: 0x00090F3C File Offset: 0x0008F13C
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

		// Token: 0x06002176 RID: 8566 RVA: 0x00090FC8 File Offset: 0x0008F1C8
		private void Rebuild()
		{
			UserProfile userProfile = this.GetUserProfile();
			this.SetCardCount(AchievementListPanelController.sortedAchievementIdentifiers.Count);
			for (int i = 0; i < AchievementListPanelController.sortedAchievementIdentifiers.Count; i++)
			{
				this.cardsList[i].SetAchievement(AchievementListPanelController.sortedAchievementIdentifiers[i], userProfile);
			}
		}

		// Token: 0x04001EE6 RID: 7910
		public GameObject achievementCardPrefab;

		// Token: 0x04001EE7 RID: 7911
		public RectTransform container;

		// Token: 0x04001EE8 RID: 7912
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001EE9 RID: 7913
		private readonly List<AchievementCardController> cardsList = new List<AchievementCardController>();

		// Token: 0x04001EEA RID: 7914
		private static readonly List<string> sortedAchievementIdentifiers = new List<string>();
	}
}
