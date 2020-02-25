using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200059F RID: 1439
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(LayoutElement))]
	public class ColumnLayoutGroupElement : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x06002236 RID: 8758 RVA: 0x000940AF File Offset: 0x000922AF
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.layoutElement = base.GetComponent<LayoutElement>();
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x000940D0 File Offset: 0x000922D0
		public void OnBeginDrag(PointerEventData eventData)
		{
			ColumnLayoutGroupElement.ClickLocation clickLocation = ColumnLayoutGroupElement.ClickLocation.None;
			Vector2 vector;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out vector))
			{
				Rect rect = this.rectTransform.rect;
				float width = rect.width;
				Vector2 vector2 = new Vector2(vector.x * rect.width, vector.y * rect.height);
				clickLocation = ColumnLayoutGroupElement.ClickLocation.Middle;
				if (vector2.x < this.handleWidth)
				{
					clickLocation = ColumnLayoutGroupElement.ClickLocation.LeftHandle;
				}
				if (vector2.x > width - this.handleWidth)
				{
					clickLocation = ColumnLayoutGroupElement.ClickLocation.RightHandle;
				}
			}
			this.lastClickLocation = clickLocation;
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x0009415C File Offset: 0x0009235C
		public void OnDrag(PointerEventData eventData)
		{
			Transform parent = this.rectTransform.parent;
			int siblingIndex = this.rectTransform.GetSiblingIndex();
			if (this.lastClickLocation == ColumnLayoutGroupElement.ClickLocation.LeftHandle && siblingIndex != 0)
			{
				Transform child = parent.GetChild(siblingIndex - 1);
				this.<OnDrag>g__AdjustWidth|8_0((child != null) ? child.GetComponent<LayoutElement>() : null, this.layoutElement, eventData.delta.x);
			}
			if (this.lastClickLocation == ColumnLayoutGroupElement.ClickLocation.RightHandle && siblingIndex != parent.childCount - 1)
			{
				LayoutElement lhs = this.layoutElement;
				Transform child2 = parent.GetChild(siblingIndex + 1);
				this.<OnDrag>g__AdjustWidth|8_0(lhs, (child2 != null) ? child2.GetComponent<LayoutElement>() : null, eventData.delta.x);
			}
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x0009420C File Offset: 0x0009240C
		[CompilerGenerated]
		private void <OnDrag>g__AdjustWidth|8_0(LayoutElement lhs, LayoutElement rhs, float change)
		{
			if (!lhs || !rhs)
			{
				return;
			}
			if (lhs.preferredWidth + change < lhs.minWidth)
			{
				change = lhs.minWidth - lhs.preferredWidth;
			}
			if (rhs.preferredWidth - change < rhs.minWidth)
			{
				change = rhs.preferredWidth - rhs.minWidth;
			}
			if (change == 0f)
			{
				return;
			}
			lhs.preferredWidth += change;
			rhs.preferredWidth -= change;
			if (this.rectTransformToLayoutInvalidate)
			{
				LayoutRebuilder.MarkLayoutForRebuild(this.rectTransformToLayoutInvalidate);
			}
		}

		// Token: 0x04001F9C RID: 8092
		private RectTransform rectTransform;

		// Token: 0x04001F9D RID: 8093
		private LayoutElement layoutElement;

		// Token: 0x04001F9E RID: 8094
		public RectTransform rectTransformToLayoutInvalidate;

		// Token: 0x04001F9F RID: 8095
		private float handleWidth = 4f;

		// Token: 0x04001FA0 RID: 8096
		private ColumnLayoutGroupElement.ClickLocation lastClickLocation;

		// Token: 0x020005A0 RID: 1440
		private enum ClickLocation
		{
			// Token: 0x04001FA2 RID: 8098
			None,
			// Token: 0x04001FA3 RID: 8099
			Middle,
			// Token: 0x04001FA4 RID: 8100
			RightHandle,
			// Token: 0x04001FA5 RID: 8101
			LeftHandle
		}
	}
}
