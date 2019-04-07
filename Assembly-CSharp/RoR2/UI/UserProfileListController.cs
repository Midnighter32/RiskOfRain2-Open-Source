using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000652 RID: 1618
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class UserProfileListController : MonoBehaviour
	{
		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06002435 RID: 9269 RVA: 0x000AA17D File Offset: 0x000A837D
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x000AA18A File Offset: 0x000A838A
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x000AA198 File Offset: 0x000A8398
		private void OnEnable()
		{
			this.RebuildElements();
			UserProfile.onAvailableUserProfilesChanged += this.RebuildElements;
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x000AA1B1 File Offset: 0x000A83B1
		private void OnDisable()
		{
			UserProfile.onAvailableUserProfilesChanged -= this.RebuildElements;
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x000AA1C4 File Offset: 0x000A83C4
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

		// Token: 0x0600243A RID: 9274 RVA: 0x000AA310 File Offset: 0x000A8510
		public ReadOnlyCollection<UserProfileListElementController> GetReadOnlyElementsList()
		{
			return new ReadOnlyCollection<UserProfileListElementController>(this.elementsList);
		}

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x0600243B RID: 9275 RVA: 0x000AA320 File Offset: 0x000A8520
		// (remove) Token: 0x0600243C RID: 9276 RVA: 0x000AA358 File Offset: 0x000A8558
		public event UserProfileListController.ProfileSelectedDelegate onProfileSelected;

		// Token: 0x0600243D RID: 9277 RVA: 0x000AA38D File Offset: 0x000A858D
		public void SendProfileSelection(UserProfile userProfile)
		{
			UserProfileListController.ProfileSelectedDelegate profileSelectedDelegate = this.onProfileSelected;
			if (profileSelectedDelegate == null)
			{
				return;
			}
			profileSelectedDelegate(userProfile);
		}

		// Token: 0x1400005C RID: 92
		// (add) Token: 0x0600243E RID: 9278 RVA: 0x000AA3A0 File Offset: 0x000A85A0
		// (remove) Token: 0x0600243F RID: 9279 RVA: 0x000AA3D8 File Offset: 0x000A85D8
		public event Action onListRebuilt;

		// Token: 0x04002736 RID: 10038
		public GameObject elementPrefab;

		// Token: 0x04002737 RID: 10039
		public RectTransform contentRect;

		// Token: 0x04002738 RID: 10040
		[Tooltip("Whether or not \"default\" profile appears as a selectable option.")]
		public bool allowDefault = true;

		// Token: 0x04002739 RID: 10041
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400273A RID: 10042
		private readonly List<UserProfileListElementController> elementsList = new List<UserProfileListElementController>();

		// Token: 0x0400273B RID: 10043
		private int currentSelectionIndex;

		// Token: 0x02000653 RID: 1619
		// (Invoke) Token: 0x06002442 RID: 9282
		public delegate void ProfileSelectedDelegate(UserProfile userProfile);
	}
}
