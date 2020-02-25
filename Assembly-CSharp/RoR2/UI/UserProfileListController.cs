using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000647 RID: 1607
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class UserProfileListController : MonoBehaviour
	{
		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x060025D4 RID: 9684 RVA: 0x000A4C45 File Offset: 0x000A2E45
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x000A4C52 File Offset: 0x000A2E52
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x000A4C60 File Offset: 0x000A2E60
		private void OnEnable()
		{
			this.RebuildElements();
			UserProfile.onAvailableUserProfilesChanged += this.RebuildElements;
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x000A4C79 File Offset: 0x000A2E79
		private void OnDisable()
		{
			UserProfile.onAvailableUserProfilesChanged -= this.RebuildElements;
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x000A4C8C File Offset: 0x000A2E8C
		private void RebuildElements()
		{
			foreach (object obj in this.contentRect)
			{
				UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
			this.elementsList.Clear();
			List<string> availableProfileNames = UserProfile.GetAvailableProfileNames();
			for (int i = 0; i < availableProfileNames.Count; i++)
			{
				if (this.allowDefault || !(availableProfileNames[i] == "default"))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab, this.contentRect);
					UserProfileListElementController component = gameObject.GetComponent<UserProfileListElementController>();
					component.listController = this;
					component.userProfile = UserProfile.GetProfile(availableProfileNames[i]);
					this.elementsList.Add(component);
					gameObject.SetActive(true);
				}
			}
			if (this.elementsList.Count > 0)
			{
				if (this.currentSelectionIndex >= this.elementsList.Count)
				{
					this.currentSelectionIndex = this.elementsList.Count - 1;
				}
				this.eventSystemLocator.eventSystem.SetSelectedGameObject(this.elementsList[this.currentSelectionIndex].gameObject);
			}
			if (this.onListRebuilt != null)
			{
				this.onListRebuilt();
			}
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x000A4DD8 File Offset: 0x000A2FD8
		public ReadOnlyCollection<UserProfileListElementController> GetReadOnlyElementsList()
		{
			return new ReadOnlyCollection<UserProfileListElementController>(this.elementsList);
		}

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x060025DA RID: 9690 RVA: 0x000A4DE8 File Offset: 0x000A2FE8
		// (remove) Token: 0x060025DB RID: 9691 RVA: 0x000A4E20 File Offset: 0x000A3020
		public event UserProfileListController.ProfileSelectedDelegate onProfileSelected;

		// Token: 0x060025DC RID: 9692 RVA: 0x000A4E55 File Offset: 0x000A3055
		public void SendProfileSelection(UserProfile userProfile)
		{
			UserProfileListController.ProfileSelectedDelegate profileSelectedDelegate = this.onProfileSelected;
			if (profileSelectedDelegate == null)
			{
				return;
			}
			profileSelectedDelegate(userProfile);
		}

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x060025DD RID: 9693 RVA: 0x000A4E68 File Offset: 0x000A3068
		// (remove) Token: 0x060025DE RID: 9694 RVA: 0x000A4EA0 File Offset: 0x000A30A0
		public event Action onListRebuilt;

		// Token: 0x0400239B RID: 9115
		public GameObject elementPrefab;

		// Token: 0x0400239C RID: 9116
		public RectTransform contentRect;

		// Token: 0x0400239D RID: 9117
		[Tooltip("Whether or not \"default\" profile appears as a selectable option.")]
		public bool allowDefault = true;

		// Token: 0x0400239E RID: 9118
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400239F RID: 9119
		private readonly List<UserProfileListElementController> elementsList = new List<UserProfileListElementController>();

		// Token: 0x040023A0 RID: 9120
		private int currentSelectionIndex;

		// Token: 0x02000648 RID: 1608
		// (Invoke) Token: 0x060025E1 RID: 9697
		public delegate void ProfileSelectedDelegate(UserProfile userProfile);
	}
}
