using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005B7 RID: 1463
	[RequireComponent(typeof(RectTransform))]
	public class DragMove : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		// Token: 0x060022B6 RID: 8886 RVA: 0x00096ADD File Offset: 0x00094CDD
		private void OnAwake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x00096AF0 File Offset: 0x00094CF0
		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateDrag(eventData);
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x00096AF9 File Offset: 0x00094CF9
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.targetTransform)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.targetTransform, eventData.position, eventData.pressEventCamera, out this.grabPoint);
			}
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x00096B28 File Offset: 0x00094D28
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

		// Token: 0x04002046 RID: 8262
		public RectTransform targetTransform;

		// Token: 0x04002047 RID: 8263
		private Vector2 grabPoint;

		// Token: 0x04002048 RID: 8264
		private RectTransform rectTransform;
	}
}
