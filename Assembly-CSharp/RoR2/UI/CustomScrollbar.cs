using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B2 RID: 1458
	public class CustomScrollbar : MPScrollbar
	{
		// Token: 0x0600229A RID: 8858 RVA: 0x00095DF8 File Offset: 0x00093FF8
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x00095E00 File Offset: 0x00094000
		protected override void Start()
		{
			base.Start();
			this.newPosition = this.originalPosition;
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x00095E14 File Offset: 0x00094014
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

		// Token: 0x0600229D RID: 8861 RVA: 0x00095C06 File Offset: 0x00093E06
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x00095E8C File Offset: 0x0009408C
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

		// Token: 0x0400200B RID: 8203
		private Vector3 originalPosition;

		// Token: 0x0400200C RID: 8204
		private Vector3 newPosition;

		// Token: 0x0400200D RID: 8205
		private float newButtonScale = 1f;

		// Token: 0x0400200E RID: 8206
		private float stopwatch;

		// Token: 0x0400200F RID: 8207
		private Color originalColor;

		// Token: 0x04002010 RID: 8208
		public bool scaleButtonOnHover = true;

		// Token: 0x04002011 RID: 8209
		public bool showImageOnHover;

		// Token: 0x04002012 RID: 8210
		public Image imageOnHover;

		// Token: 0x04002013 RID: 8211
		public Image imageOnInteractable;

		// Token: 0x04002014 RID: 8212
		private bool hovering;

		// Token: 0x04002015 RID: 8213
		private float imageOnHoverAlphaVelocity;
	}
}
