using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x0200052F RID: 1327
	public class ScreenDamage : MonoBehaviour
	{
		// Token: 0x06001F5F RID: 8031 RVA: 0x00088330 File Offset: 0x00086530
		private void Awake()
		{
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
			this.mat = UnityEngine.Object.Instantiate<Material>(this.mat);
		}

		// Token: 0x06001F60 RID: 8032 RVA: 0x00088350 File Offset: 0x00086550
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

		// Token: 0x04001D06 RID: 7430
		private CameraRigController cameraRigController;

		// Token: 0x04001D07 RID: 7431
		public Material mat;

		// Token: 0x04001D08 RID: 7432
		public float DistortionScale = 1f;

		// Token: 0x04001D09 RID: 7433
		public float DistortionPower = 1f;

		// Token: 0x04001D0A RID: 7434
		public float DesaturationScale = 1f;

		// Token: 0x04001D0B RID: 7435
		public float DesaturationPower = 1f;

		// Token: 0x04001D0C RID: 7436
		public float TintScale = 1f;

		// Token: 0x04001D0D RID: 7437
		public float TintPower = 1f;

		// Token: 0x04001D0E RID: 7438
		private float healthPercentage = 1f;

		// Token: 0x04001D0F RID: 7439
		private const float hitTintDecayTime = 0.6f;

		// Token: 0x04001D10 RID: 7440
		private const float hitTintScale = 1.6f;

		// Token: 0x04001D11 RID: 7441
		private const float deathWeight = 2f;
	}
}
