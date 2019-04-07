using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005D8 RID: 1496
	[RequireComponent(typeof(RectTransform))]
	public class DragMove : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x06002186 RID: 8582 RVA: 0x0009DB69 File Offset: 0x0009BD69
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x0009DB7C File Offset: 0x0009BD7C
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x0009DB85 File Offset: 0x0009BD85
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x0009DBB4 File Offset: 0x0009BDB4
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
			this.targetTransform.localPosition += new Vector3(vector.x, vector.y, 0f);
		}

		// Token: 0x04002437 RID: 9271
		public RectTransform targetTransform;

		// Token: 0x04002438 RID: 9272
		private Vector2 grabPoint;

		// Token: 0x04002439 RID: 9273
		private RectTransform rectTransform;
	}
}
