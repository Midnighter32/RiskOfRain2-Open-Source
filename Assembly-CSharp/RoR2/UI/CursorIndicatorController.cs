using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AD RID: 1453
	public class CursorIndicatorController : MonoBehaviour
	{
		// Token: 0x06002287 RID: 8839 RVA: 0x000957C8 File Offset: 0x000939C8
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

		// Token: 0x06002288 RID: 8840 RVA: 0x00095844 File Offset: 0x00093A44
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

		// Token: 0x06002289 RID: 8841 RVA: 0x00095887 File Offset: 0x00093A87
		public void SetPosition(Vector2 position)
		{
			this.containerTransform.position = position;
		}

		// Token: 0x04001FED RID: 8173
		[NonSerialized]
		public CursorIndicatorController.CursorSet noneCursorSet;

		// Token: 0x04001FEE RID: 8174
		public CursorIndicatorController.CursorSet mouseCursorSet;

		// Token: 0x04001FEF RID: 8175
		public CursorIndicatorController.CursorSet gamepadCursorSet;

		// Token: 0x04001FF0 RID: 8176
		private GameObject currentChildIndicator;

		// Token: 0x04001FF1 RID: 8177
		public RectTransform containerTransform;

		// Token: 0x04001FF2 RID: 8178
		private Color cachedIndicatorColor = Color.clear;

		// Token: 0x020005AE RID: 1454
		public enum CursorImage
		{
			// Token: 0x04001FF4 RID: 8180
			None,
			// Token: 0x04001FF5 RID: 8181
			Pointer,
			// Token: 0x04001FF6 RID: 8182
			Hover
		}

		// Token: 0x020005AF RID: 1455
		[Serializable]
		public struct CursorSet
		{
			// Token: 0x0600228B RID: 8843 RVA: 0x000958AD File Offset: 0x00093AAD
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

			// Token: 0x04001FF7 RID: 8183
			public GameObject pointerObject;

			// Token: 0x04001FF8 RID: 8184
			public GameObject hoverObject;
		}
	}
}
