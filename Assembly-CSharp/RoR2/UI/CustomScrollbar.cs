using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D3 RID: 1491
	public class CustomScrollbar : MPScrollbar
	{
		// Token: 0x0600216A RID: 8554 RVA: 0x0009CE81 File Offset: 0x0009B081
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x0009CE89 File Offset: 0x0009B089
		protected override void Start()
		{
			base.Start();
			this.newPosition = this.originalPosition;
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x0009CEA0 File Offset: 0x0009B0A0
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			switch (state)
			{
			case Selectable.SelectionState.Normal:
				this.hovering = false;
				break;
			case Selectable.SelectionState.Highlighted:
				Util.PlaySound("Play_UI_menuHover", RoR2Application.instance.gameObject);
				this.hovering = true;
				break;
			case Selectable.SelectionState.Pressed:
				this.hovering = true;
				break;
			case Selectable.SelectionState.Disabled:
				this.hovering = false;
				break;
			}
			this.originalColor = base.targetGraphic.color;
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x0009CC97 File Offset: 0x0009AE97
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x0009CF18 File Offset: 0x0009B118
		private void LateUpdate()
		{
			this.stopwatch += Time.deltaTime;
			if (Application.isPlaying)
			{
				if (this.showImageOnHover)
				{
					float target = this.hovering ? 1f : 0f;
					Color color = this.imageOnHover.color;
					float a = Mathf.SmoothDamp(color.a, target, ref this.imageOnHoverAlphaVelocity, 0.03f, 100f, Time.unscaledDeltaTime);
					Color color2 = new Color(color.r, color.g, color.g, a);
					this.imageOnHover.color = color2;
				}
				if (this.imageOnInteractable)
				{
					this.imageOnInteractable.enabled = base.interactable;
				}
			}
		}

		// Token: 0x040023FC RID: 9212
		private Vector3 originalPosition;

		// Token: 0x040023FD RID: 9213
		private Vector3 newPosition;

		// Token: 0x040023FE RID: 9214
		private float newButtonScale = 1f;

		// Token: 0x040023FF RID: 9215
		private float stopwatch;

		// Token: 0x04002400 RID: 9216
		private Color originalColor;

		// Token: 0x04002401 RID: 9217
		public bool scaleButtonOnHover = true;

		// Token: 0x04002402 RID: 9218
		public bool showImageOnHover;

		// Token: 0x04002403 RID: 9219
		public Image imageOnHover;

		// Token: 0x04002404 RID: 9220
		public Image imageOnInteractable;

		// Token: 0x04002405 RID: 9221
		private bool hovering;

		// Token: 0x04002406 RID: 9222
		private float imageOnHoverAlphaVelocity;
	}
}
