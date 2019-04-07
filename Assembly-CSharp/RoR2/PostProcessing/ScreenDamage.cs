using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x02000565 RID: 1381
	public class ScreenDamage : MonoBehaviour
	{
		// Token: 0x06001ED1 RID: 7889 RVA: 0x00091760 File Offset: 0x0008F960
		private void Awake()
		{
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
			this.mat = UnityEngine.Object.Instantiate<Material>(this.mat);
		}

		// Token: 0x06001ED2 RID: 7890 RVA: 0x00091780 File Offset: 0x0008F980
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = 0f;
			float num2 = 0f;
			if (this.cameraRigController)
			{
				if (this.cameraRigController.target)
				{
					num2 = 0.5f;
					HealthComponent component = this.cameraRigController.target.GetComponent<HealthComponent>();
					if (component)
					{
						this.healthPercentage = Mathf.Clamp(component.health / component.fullHealth, 0f, 1f);
						num = Mathf.Clamp01(1f - component.timeSinceLastHit / 0.6f) * 1.6f;
						if (component.health <= 0f)
						{
							num2 = 0f;
						}
					}
				}
				this.mat.SetFloat("_DistortionStrength", num2 * this.DistortionScale * Mathf.Pow(1f - this.healthPercentage, this.DistortionPower));
				this.mat.SetFloat("_DesaturationStrength", num2 * this.DesaturationScale * Mathf.Pow(1f - this.healthPercentage, this.DesaturationPower));
				this.mat.SetFloat("_TintStrength", num2 * this.TintScale * (Mathf.Pow(1f - this.healthPercentage, this.TintPower) + num));
			}
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x0400217C RID: 8572
		private CameraRigController cameraRigController;

		// Token: 0x0400217D RID: 8573
		public Material mat;

		// Token: 0x0400217E RID: 8574
		public float DistortionScale = 1f;

		// Token: 0x0400217F RID: 8575
		public float DistortionPower = 1f;

		// Token: 0x04002180 RID: 8576
		public float DesaturationScale = 1f;

		// Token: 0x04002181 RID: 8577
		public float DesaturationPower = 1f;

		// Token: 0x04002182 RID: 8578
		public float TintScale = 1f;

		// Token: 0x04002183 RID: 8579
		public float TintPower = 1f;

		// Token: 0x04002184 RID: 8580
		private float healthPercentage = 1f;

		// Token: 0x04002185 RID: 8581
		private const float hitTintDecayTime = 0.6f;

		// Token: 0x04002186 RID: 8582
		private const float hitTintScale = 1.6f;

		// Token: 0x04002187 RID: 8583
		private const float deathWeight = 2f;
	}
}
