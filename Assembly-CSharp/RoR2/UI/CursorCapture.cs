using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AC RID: 1452
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	public class CursorCapture : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06002285 RID: 8837 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnPointerClick(PointerEventData eventData)
		{
		}
	}
}
