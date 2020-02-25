using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x0200064A RID: 1610
	public class ViewableTag : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x060025E9 RID: 9705 RVA: 0x000A4FCB File Offset: 0x000A31CB
		// (set) Token: 0x060025EA RID: 9706 RVA: 0x000A4FD3 File Offset: 0x000A31D3
		public string viewableName
		{
			get
			{
				return this._viewableName;
			}
			set
			{
				if (this._viewableName == value)
				{
					return;
				}
				this._viewableName = value;
				this.Refresh();
			}
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x000A4FF4 File Offset: 0x000A31F4
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
				if (ViewableTag.viewablesWarnUndefined.value)
				{
					Debug.LogWarningFormat("Viewable {0} is not defined.", new object[]
					{
						this.viewableName
					});
				}
				return false;
			}
			return node.shouldShowUnviewed(userProfile);
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000A506B File Offset: 0x000A326B
		private void OnEnable()
		{
			ViewableTag.instancesList.Add(this);
			RoR2Application.onNextUpdate += this.CallRefreshIfStillValid;
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x000A5089 File Offset: 0x000A3289
		private void CallRefreshIfStillValid()
		{
			if (!this)
			{
				return;
			}
			this.Refresh();
		}

		// Token: 0x060025EE RID: 9710 RVA: 0x000A509C File Offset: 0x000A329C
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

		// Token: 0x060025EF RID: 9711 RVA: 0x000A512D File Offset: 0x000A332D
		private void OnDisable()
		{
			ViewableTag.instancesList.Remove(this);
			this.Refresh();
			if (this.markAsViewedOnDisable)
			{
				this.TriggerView();
			}
		}

		// Token: 0x060025F0 RID: 9712 RVA: 0x000A514F File Offset: 0x000A334F
		private void TriggerView()
		{
			ViewableTrigger.TriggerView(this.viewableName);
		}

		// Token: 0x060025F1 RID: 9713 RVA: 0x000A515C File Offset: 0x000A335C
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

		// Token: 0x060025F2 RID: 9714 RVA: 0x000A5191 File Offset: 0x000A3391
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.markAsViewedOnHover)
			{
				this.TriggerView();
			}
		}

		// Token: 0x040023A8 RID: 9128
		private static readonly List<ViewableTag> instancesList = new List<ViewableTag>();

		// Token: 0x040023A9 RID: 9129
		[Tooltip("The path of the viewable that determines whether or not the \"NEW\" tag is activated.")]
		[FormerlySerializedAs("viewableName")]
		[SerializeField]
		private string _viewableName;

		// Token: 0x040023AA RID: 9130
		[Tooltip("Marks the named viewable as viewed when this component is disabled.")]
		public bool markAsViewedOnDisable;

		// Token: 0x040023AB RID: 9131
		public bool markAsViewedOnHover;

		// Token: 0x040023AC RID: 9132
		public ViewableTag.ViewableVisualStyle viewableVisualStyle;

		// Token: 0x040023AD RID: 9133
		public static readonly BoolConVar viewablesWarnUndefined = new BoolConVar("viewables_warn_undefined", ConVarFlags.None, "0", "Issues a warning in the console if a viewable is not defined.");

		// Token: 0x040023AE RID: 9134
		private static GameObject tagPrefab;

		// Token: 0x040023AF RID: 9135
		private GameObject tagInstance;

		// Token: 0x040023B0 RID: 9136
		private static bool pendingRefreshAll = false;

		// Token: 0x0200064B RID: 1611
		public enum ViewableVisualStyle
		{
			// Token: 0x040023B2 RID: 9138
			Button,
			// Token: 0x040023B3 RID: 9139
			Icon
		}
	}
}
