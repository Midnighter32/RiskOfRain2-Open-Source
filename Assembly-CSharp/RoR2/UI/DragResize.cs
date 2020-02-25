using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005B8 RID: 1464
	[RequireComponent(typeof(RectTransform))]
	public class DragResize : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x060022BB RID: 8891 RVA: 0x00096B9F File Offset: 0x00094D9F
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x00096BB2 File Offset: 0x00094DB2
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x060022BD RID: 8893 RVA: 0x00096BBB File Offset: 0x00094DBB
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x00096BE8 File Offset: 0x00094DE8
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

		// Token: 0x04002049 RID: 8265
		public RectTransform targetTransform;

		// Token: 0x0400204A RID: 8266
		public Vector2 minSize;

		// Token: 0x0400204B RID: 8267
		private Vector2 grabPoint;

		// Token: 0x0400204C RID: 8268
		private RectTransform rectTransform;
	}
}
