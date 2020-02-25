using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000621 RID: 1569
	[RequireComponent(typeof(MPEventSystemLocator))]
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollToSelection : MonoBehaviour
	{
		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06002519 RID: 9497 RVA: 0x000A1BF6 File Offset: 0x0009FDF6
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x0600251A RID: 9498 RVA: 0x000A1C03 File Offset: 0x0009FE03
		private void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600251B RID: 9499 RVA: 0x000A1C20 File Offset: 0x0009FE20
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

		// Token: 0x0600251C RID: 9500 RVA: 0x000A1C8C File Offset: 0x0009FE8C
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

		// Token: 0x040022DA RID: 8922
		private ScrollRect scrollRect;

		// Token: 0x040022DB RID: 8923
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040022DC RID: 8924
		private Vector3[] targetWorldCorners = new Vector3[4];

		// Token: 0x040022DD RID: 8925
		private Vector3[] viewPortWorldCorners = new Vector3[4];

		// Token: 0x040022DE RID: 8926
		private GameObject lastSelectedObject;
	}
}
