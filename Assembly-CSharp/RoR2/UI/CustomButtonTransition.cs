using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D2 RID: 1490
	public class CustomButtonTransition : MPButton
	{
		// Token: 0x06002164 RID: 8548 RVA: 0x0009CB0E File Offset: 0x0009AD0E
		protected override void Awake()
		{
			base.Awake();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x0009CB24 File Offset: 0x0009AD24
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

		// Token: 0x06002166 RID: 8550 RVA: 0x0009CB8C File Offset: 0x0009AD8C
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
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
			this.originalColor = base.targetGraphic.color;
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x0009CC97 File Offset: 0x0009AE97
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x0009CCB0 File Offset: 0x0009AEB0
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

		// Token: 0x040023EE RID: 9198
		private TextMeshProUGUI textMeshProUGui;

		// Token: 0x040023EF RID: 9199
		private Vector3 originalPosition;

		// Token: 0x040023F0 RID: 9200
		private Vector3 newPosition;

		// Token: 0x040023F1 RID: 9201
		private float newButtonScale = 1f;

		// Token: 0x040023F2 RID: 9202
		private float stopwatch;

		// Token: 0x040023F3 RID: 9203
		private Color originalColor;

		// Token: 0x040023F4 RID: 9204
		public bool scaleButtonOnHover = true;

		// Token: 0x040023F5 RID: 9205
		public bool showImageOnHover;

		// Token: 0x040023F6 RID: 9206
		public Image imageOnHover;

		// Token: 0x040023F7 RID: 9207
		public Image imageOnInteractable;

		// Token: 0x040023F8 RID: 9208
		private bool hovering;

		// Token: 0x040023F9 RID: 9209
		private float buttonScaleVelocity;

		// Token: 0x040023FA RID: 9210
		private float imageOnHoverAlphaVelocity;

		// Token: 0x040023FB RID: 9211
		private float imageOnHoverScaleVelocity;
	}
}
