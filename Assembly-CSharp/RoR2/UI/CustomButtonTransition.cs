using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B1 RID: 1457
	public class CustomButtonTransition : MPButton
	{
		// Token: 0x06002294 RID: 8852 RVA: 0x00095A6B File Offset: 0x00093C6B
		protected override void Awake()
		{
			base.Awake();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x00095A80 File Offset: 0x00093C80
		protected override void Start()
		{
			base.Start();
			base.onClick.AddListener(new UnityAction(this.OnClickCustom));
			if (this.textMeshProUGui)
			{
				this.originalPosition = new Vector3(this.textMeshProUGui.margin.x, 0f, 0f);
			}
			this.newPosition = this.originalPosition;
		}

		// Token: 0x06002296 RID: 8854 RVA: 0x00095AE8 File Offset: 0x00093CE8
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			if (this.previousState != state)
			{
				switch (state)
				{
				case Selectable.SelectionState.Normal:
					this.newPosition.x = this.originalPosition.x;
					this.newButtonScale = 1f;
					this.hovering = false;
					break;
				case Selectable.SelectionState.Highlighted:
					this.newPosition.x = this.originalPosition.x + 4f;
					Util.PlaySound("Play_UI_menuHover", RoR2Application.instance.gameObject);
					this.newButtonScale = 1.05f;
					this.hovering = true;
					break;
				case Selectable.SelectionState.Pressed:
					this.newPosition.x = this.originalPosition.x + 6f;
					this.newButtonScale = 0.95f;
					this.hovering = true;
					break;
				case Selectable.SelectionState.Disabled:
					this.newPosition.x = this.originalPosition.x;
					this.newButtonScale = 1f;
					this.hovering = false;
					break;
				}
				this.previousState = state;
			}
			this.originalColor = base.targetGraphic.color;
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x00095C06 File Offset: 0x00093E06
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x00095C20 File Offset: 0x00093E20
		private void LateUpdate()
		{
			this.stopwatch += Time.deltaTime;
			if (Application.isPlaying)
			{
				if (this.textMeshProUGui)
				{
					Vector3 position = this.textMeshProUGui.transform.position;
					this.textMeshProUGui.margin = this.newPosition;
				}
				if (base.image && this.scaleButtonOnHover)
				{
					float num = Mathf.SmoothDamp(base.image.transform.localScale.x, this.newButtonScale, ref this.buttonScaleVelocity, base.colors.fadeDuration * 0.2f, Time.unscaledDeltaTime);
					base.image.transform.localScale = new Vector3(num, num, num);
				}
				if (this.showImageOnHover)
				{
					float target = this.hovering ? 1f : 0f;
					float target2 = this.hovering ? 1f : 1.1f;
					Color color = this.imageOnHover.color;
					float x = this.imageOnHover.transform.localScale.x;
					float a = Mathf.SmoothDamp(color.a, target, ref this.imageOnHoverAlphaVelocity, 0.03f, 100f, Time.unscaledDeltaTime);
					float num2 = Mathf.SmoothDamp(x, target2, ref this.imageOnHoverScaleVelocity, 0.03f, 100f, Time.unscaledDeltaTime);
					Color color2 = new Color(color.r, color.g, color.g, a);
					new Vector3(num2, num2, num2);
					this.imageOnHover.color = color2;
				}
				if (this.imageOnInteractable)
				{
					this.imageOnInteractable.enabled = base.interactable;
				}
			}
		}

		// Token: 0x04001FFC RID: 8188
		private TextMeshProUGUI textMeshProUGui;

		// Token: 0x04001FFD RID: 8189
		private Vector3 originalPosition;

		// Token: 0x04001FFE RID: 8190
		private Vector3 newPosition;

		// Token: 0x04001FFF RID: 8191
		private float newButtonScale = 1f;

		// Token: 0x04002000 RID: 8192
		private float stopwatch;

		// Token: 0x04002001 RID: 8193
		private Color originalColor;

		// Token: 0x04002002 RID: 8194
		public bool scaleButtonOnHover = true;

		// Token: 0x04002003 RID: 8195
		public bool showImageOnHover;

		// Token: 0x04002004 RID: 8196
		public Image imageOnHover;

		// Token: 0x04002005 RID: 8197
		public Image imageOnInteractable;

		// Token: 0x04002006 RID: 8198
		private bool hovering;

		// Token: 0x04002007 RID: 8199
		private Selectable.SelectionState previousState = Selectable.SelectionState.Disabled;

		// Token: 0x04002008 RID: 8200
		private float buttonScaleVelocity;

		// Token: 0x04002009 RID: 8201
		private float imageOnHoverAlphaVelocity;

		// Token: 0x0400200A RID: 8202
		private float imageOnHoverScaleVelocity;
	}
}
