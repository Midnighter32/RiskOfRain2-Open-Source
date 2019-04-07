using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005D9 RID: 1497
	[RequireComponent(typeof(RectTransform))]
	public class DragResize : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x0600218B RID: 8587 RVA: 0x0009DC2B File Offset: 0x0009BE2B
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x0009DC3E File Offset: 0x0009BE3E
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x0009DC47 File Offset: 0x0009BE47
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x0009DC74 File Offset: 0x0009BE74
		private void UpdateDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (!this.targetTransform)
			{
				return;
			}
			Vector2 a;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out a);
			Vector2 vector = a - this.grabPoint;
			this.grabPoint = a;
			vector.y = -vector.y;
			this.targetTransform.sizeDelta = Vector2.Max(this.targetTransform.sizeDelta + vector, this.minSize);
		}

		// Token: 0x0400243A RID: 9274
		public RectTransform targetTransform;

		// Token: 0x0400243B RID: 9275
		public Vector2 minSize;

		// Token: 0x0400243C RID: 9276
		private Vector2 grabPoint;

		// Token: 0x0400243D RID: 9277
		private RectTransform rectTransform;
	}
}
