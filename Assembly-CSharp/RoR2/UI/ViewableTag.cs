using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000655 RID: 1621
	public class ViewableTag : MonoBehaviour
	{
		// Token: 0x0600244A RID: 9290 RVA: 0x000AA504 File Offset: 0x000A8704
		private bool Check()
		{
			if (LocalUserManager.readOnlyLocalUsersList.Count == 0)
			{
				return false;
			}
			UserProfile userProfile = LocalUserManager.readOnlyLocalUsersList[0].userProfile;
			ViewablesCatalog.Node node = ViewablesCatalog.FindNode(this.viewableName ?? "");
			if (node == null)
			{
				Debug.LogErrorFormat("Viewable {0} is not defined.", new object[]
				{
					this.viewableName
				});
				return false;
			}
			return node.shouldShowUnviewed(userProfile);
		}

		// Token: 0x0600244B RID: 9291 RVA: 0x000AA56F File Offset: 0x000A876F
		private void OnEnable()
		{
			ViewableTag.instancesList.Add(this);
			RoR2Application.onNextUpdate += this.Refresh;
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x000AA590 File Offset: 0x000A8790
		public void Refresh()
		{
			bool flag = base.enabled && this.Check();
			if (this.tagInstance != flag)
			{
				if (this.tagInstance)
				{
					UnityEngine.Object.Destroy(this.tagInstance);
					this.tagInstance = null;
					return;
				}
				string childName = this.viewableVisualStyle.ToString();
				this.tagInstance = UnityEngine.Object.Instantiate<GameObject>(ViewableTag.tagPrefab, base.transform);
				this.tagInstance.GetComponent<ChildLocator>().FindChild(childName).gameObject.SetActive(true);
			}
		}

		// Token: 0x0600244D RID: 9293 RVA: 0x000AA621 File Offset: 0x000A8821
		private void OnDisable()
		{
			ViewableTag.instancesList.Remove(this);
			this.Refresh();
			if (this.markAsViewedOnDisable)
			{
				ViewableTrigger.TriggerView(this.viewableName);
			}
		}

		// Token: 0x0600244E RID: 9294 RVA: 0x000AA648 File Offset: 0x000A8848
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			ViewableTag.tagPrefab = Resources.Load<GameObject>("Prefabs/UI/NewViewableTag");
			UserProfile.onUserProfileViewedViewablesChanged += delegate(UserProfile userProfile)
			{
				if (!ViewableTag.pendingRefreshAll)
				{
					ViewableTag.pendingRefreshAll = true;
					RoR2Application.onNextUpdate += delegate()
					{
						foreach (ViewableTag viewableTag in ViewableTag.instancesList)
						{
							viewableTag.Refresh();
						}
						ViewableTag.pendingRefreshAll = false;
					};
				}
			};
		}

		// Token: 0x04002743 RID: 10051
		private static readonly List<ViewableTag> instancesList = new List<ViewableTag>();

		// Token: 0x04002744 RID: 10052
		[Tooltip("The path of the viewable that determines whether or not the \"NEW\" tag is activated.")]
		public string viewableName;

		// Token: 0x04002745 RID: 10053
		[Tooltip("Marks the named viewable as viewed when this component is disabled.")]
		public bool markAsViewedOnDisable;

		// Token: 0x04002746 RID: 10054
		public ViewableTag.ViewableVisualStyle viewableVisualStyle;

		// Token: 0x04002747 RID: 10055
		private static GameObject tagPrefab;

		// Token: 0x04002748 RID: 10056
		private GameObject tagInstance;

		// Token: 0x04002749 RID: 10057
		private static bool pendingRefreshAll = false;

		// Token: 0x02000656 RID: 1622
		public enum ViewableVisualStyle
		{
			// Token: 0x0400274B RID: 10059
			Button,
			// Token: 0x0400274C RID: 10060
			Icon
		}
	}
}
