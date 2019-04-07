using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000633 RID: 1587
	[RequireComponent(typeof(MPEventSystemLocator))]
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollToSelection : MonoBehaviour
	{
		// Token: 0x1700031A RID: 794
		// (get) Token: 0x0600239C RID: 9116 RVA: 0x000A7862 File Offset: 0x000A5A62
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x000A786F File Offset: 0x000A5A6F
		private void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x000A788C File Offset: 0x000A5A8C
		private void Update()
		{
			GameObject gameObject = this.eventSystem ? this.eventSystem.currentSelectedGameObject : null;
			if (this.lastSelectedObject != gameObject)
			{
				if (gameObject && gameObject.transform.IsChildOf(base.transform))
				{
					this.ScrollToRect((RectTransform)gameObject.transform);
				}
				this.lastSelectedObject = gameObject;
			}
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x000A78F8 File Offset: 0x000A5AF8
		private void ScrollToRect(RectTransform targetRectTransform)
		{
			targetRectTransform.GetWorldCorners(this.targetWorldCorners);
			((RectTransform)base.transform).GetWorldCorners(this.viewPortWorldCorners);
			if (this.scrollRect.vertical && this.scrollRect.verticalScrollbar)
			{
				float y = this.targetWorldCorners[1].y;
				float y2 = this.targetWorldCorners[0].y;
				float y3 = this.viewPortWorldCorners[1].y;
				float y4 = this.viewPortWorldCorners[0].y;
				float num = y - y3;
				float num2 = y2 - y4;
				float num3 = y3 - y4;
				if (num > 0f)
				{
					this.scrollRect.verticalScrollbar.value += num / num3;
				}
				if (num2 < 0f)
				{
					this.scrollRect.verticalScrollbar.value += num2 / num3;
				}
			}
			if (this.scrollRect.horizontal && this.scrollRect.horizontalScrollbar)
			{
				float y5 = this.targetWorldCorners[2].y;
				float y6 = this.targetWorldCorners[0].y;
				float y7 = this.viewPortWorldCorners[2].y;
				float y8 = this.viewPortWorldCorners[0].y;
				float num4 = y5 - y7;
				float num5 = y6 - y8;
				float num6 = y7 - y8;
				if (num4 > 0f)
				{
					this.scrollRect.horizontalScrollbar.value += num4 / num6;
				}
				if (num5 < 0f)
				{
					this.scrollRect.horizontalScrollbar.value += num5 / num6;
				}
			}
		}

		// Token: 0x04002693 RID: 9875
		private ScrollRect scrollRect;

		// Token: 0x04002694 RID: 9876
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002695 RID: 9877
		private Vector3[] targetWorldCorners = new Vector3[4];

		// Token: 0x04002696 RID: 9878
		private Vector3[] viewPortWorldCorners = new Vector3[4];

		// Token: 0x04002697 RID: 9879
		private GameObject lastSelectedObject;
	}
}
