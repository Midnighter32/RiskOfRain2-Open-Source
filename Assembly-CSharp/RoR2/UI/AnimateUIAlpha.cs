using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000588 RID: 1416
	public class AnimateUIAlpha : MonoBehaviour
	{
		// Token: 0x060021B1 RID: 8625 RVA: 0x00091C04 File Offset: 0x0008FE04
		private void Start()
		{
			if (this.image)
			{
				this.originalColor = this.image.color;
			}
			if (this.rawImage)
			{
				this.originalColor = this.rawImage.color;
			}
			else if (this.spriteRenderer)
			{
				this.originalColor = this.spriteRenderer.color;
			}
			this.UpdateAlphas(0f);
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x00091C78 File Offset: 0x0008FE78
		private void OnDisable()
		{
			this.time = 0f;
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x00091C85 File Offset: 0x0008FE85
		private void Update()
		{
			this.UpdateAlphas(Time.unscaledDeltaTime);
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x00091C94 File Offset: 0x0008FE94
		private void UpdateAlphas(float deltaTime)
		{
			this.time = Mathf.Min(this.timeMax, this.time + deltaTime);
			float num = this.alphaCurve.Evaluate(this.time / this.timeMax);
			Color color = new Color(this.originalColor.r, this.originalColor.g, this.originalColor.b, this.originalColor.a * num);
			if (this.image)
			{
				this.image.color = color;
			}
			if (this.rawImage)
			{
				this.rawImage.color = color;
			}
			else if (this.spriteRenderer)
			{
				this.spriteRenderer.color = color;
			}
			if (this.loopOnEnd && this.time >= this.timeMax)
			{
				this.time -= this.timeMax;
			}
			if (this.destroyOnEnd && this.time >= this.timeMax)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.disableGameObjectOnEnd && this.time >= this.timeMax)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04001F10 RID: 7952
		public AnimationCurve alphaCurve;

		// Token: 0x04001F11 RID: 7953
		public Image image;

		// Token: 0x04001F12 RID: 7954
		public RawImage rawImage;

		// Token: 0x04001F13 RID: 7955
		public SpriteRenderer spriteRenderer;

		// Token: 0x04001F14 RID: 7956
		public float timeMax = 5f;

		// Token: 0x04001F15 RID: 7957
		public bool destroyOnEnd;

		// Token: 0x04001F16 RID: 7958
		public bool loopOnEnd;

		// Token: 0x04001F17 RID: 7959
		public bool disableGameObjectOnEnd;

		// Token: 0x04001F18 RID: 7960
		[HideInInspector]
		public float time;

		// Token: 0x04001F19 RID: 7961
		private Color originalColor;
	}
}
