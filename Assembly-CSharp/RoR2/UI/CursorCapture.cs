using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CD RID: 1485
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	public class CursorCapture : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06002155 RID: 8533 RVA: 0x00004507 File Offset: 0x00002707
		public void OnPointerClick(PointerEventData eventData)
		{
		}
	}
}
