using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CE RID: 1486
	public class CursorIndicatorController : MonoBehaviour
	{
		// Token: 0x06002157 RID: 8535 RVA: 0x0009C870 File Offset: 0x0009AA70
		public void SetCursor(CursorIndicatorController.CursorSet cursorSet, CursorIndicatorController.CursorImage cursorImage, Color color)
		{
			GameObject gameObject = cursorSet.GetGameObject(cursorImage);
			bool flag = color != this.cachedIndicatorColor;
			if (gameObject != this.currentChildIndicator)
			{
				if (this.currentChildIndicator)
				{
					this.currentChildIndicator.SetActive(false);
				}
				this.currentChildIndicator = gameObject;
				if (this.currentChildIndicator)
				{
					this.currentChildIndicator.SetActive(true);
				}
				flag = true;
			}
			if (flag)
			{
				this.cachedIndicatorColor = color;
				this.ApplyIndicatorColor();
			}
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x0009C8EC File Offset: 0x0009AAEC
		private void ApplyIndicatorColor()
		{
			if (!this.currentChildIndicator)
			{
				return;
			}
			Image[] componentsInChildren = this.currentChildIndicator.GetComponentsInChildren<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].color = this.cachedIndicatorColor;
			}
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x0009C92F File Offset: 0x0009AB2F
		public void SetPosition(Vector2 position)
		{
			this.containerTransform.position = position;
		}

		// Token: 0x040023DF RID: 9183
		[NonSerialized]
		public CursorIndicatorController.CursorSet noneCursorSet;

		// Token: 0x040023E0 RID: 9184
		public CursorIndicatorController.CursorSet mouseCursorSet;

		// Token: 0x040023E1 RID: 9185
		public CursorIndicatorController.CursorSet gamepadCursorSet;

		// Token: 0x040023E2 RID: 9186
		private GameObject currentChildIndicator;

		// Token: 0x040023E3 RID: 9187
		public RectTransform containerTransform;

		// Token: 0x040023E4 RID: 9188
		private Color cachedIndicatorColor = Color.clear;

		// Token: 0x020005CF RID: 1487
		public enum CursorImage
		{
			// Token: 0x040023E6 RID: 9190
			None,
			// Token: 0x040023E7 RID: 9191
			Pointer,
			// Token: 0x040023E8 RID: 9192
			Hover
		}

		// Token: 0x020005D0 RID: 1488
		[Serializable]
		public struct CursorSet
		{
			// Token: 0x0600215B RID: 8539 RVA: 0x0009C955 File Offset: 0x0009AB55
			public GameObject GetGameObject(CursorIndicatorController.CursorImage cursorImage)
			{
				switch (cursorImage)
				{
				case CursorIndicatorController.CursorImage.None:
					return null;
				case CursorIndicatorController.CursorImage.Pointer:
					return this.pointerObject;
				case CursorIndicatorController.CursorImage.Hover:
					return this.hoverObject;
				default:
					return null;
				}
			}

			// Token: 0x040023E9 RID: 9193
			public GameObject pointerObject;

			// Token: 0x040023EA RID: 9194
			public GameObject hoverObject;
		}
	}
}
