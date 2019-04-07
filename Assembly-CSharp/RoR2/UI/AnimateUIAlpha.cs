using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AF RID: 1455
	public class AnimateUIAlpha : MonoBehaviour
	{
		// Token: 0x0600209B RID: 8347 RVA: 0x000997AC File Offset: 0x000979AC
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

		// Token: 0x0600209C RID: 8348 RVA: 0x00099820 File Offset: 0x00097A20
		private void OnDisable()
		{
			this.time = 0f;
		}

		// Token: 0x0600209D RID: 8349 RVA: 0x0009982D File Offset: 0x00097A2D
		private void Update()
		{
			this.UpdateAlphas(Time.unscaledDeltaTime);
		}

		// Token: 0x0600209E RID: 8350 RVA: 0x0009983C File Offset: 0x00097A3C
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

		// Token: 0x04002324 RID: 8996
		public AnimationCurve alphaCurve;

		// Token: 0x04002325 RID: 8997
		public Image image;

		// Token: 0x04002326 RID: 8998
		public RawImage rawImage;

		// Token: 0x04002327 RID: 8999
		public SpriteRenderer spriteRenderer;

		// Token: 0x04002328 RID: 9000
		public float timeMax = 5f;

		// Token: 0x04002329 RID: 9001
		public bool destroyOnEnd;

		// Token: 0x0400232A RID: 9002
		public bool loopOnEnd;

		// Token: 0x0400232B RID: 9003
		public bool disableGameObjectOnEnd;

		// Token: 0x0400232C RID: 9004
		[HideInInspector]
		public float time;

		// Token: 0x0400232D RID: 9005
		private Color originalColor;
	}
}
