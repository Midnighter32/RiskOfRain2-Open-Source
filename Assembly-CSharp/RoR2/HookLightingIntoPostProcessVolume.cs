using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000438 RID: 1080
	[RequireComponent(typeof(PostProcessVolume))]
	public class HookLightingIntoPostProcessVolume : MonoBehaviour
	{
		// Token: 0x0600180B RID: 6155 RVA: 0x00072CE0 File Offset: 0x00070EE0
		private void OnEnable()
		{
			this.volumeColliders = base.GetComponents<Collider>();
			if (!this.hasCachedAmbientColor)
			{
				this.defaultAmbientColor = RenderSettings.ambientLight;
				this.hasCachedAmbientColor = true;
			}
			SceneCamera.onSceneCameraPreRender += this.OnPreRenderSceneCam;
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x00072D19 File Offset: 0x00070F19
		private void OnDisable()
		{
			SceneCamera.onSceneCameraPreRender -= this.OnPreRenderSceneCam;
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x00072D2C File Offset: 0x00070F2C
		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			float interpFactor = this.GetInterpFactor(sceneCam.camera.transform.position);
			RenderSettings.ambientLight = Color.Lerp(this.defaultAmbientColor, this.overrideAmbientColor, interpFactor);
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x00072D68 File Offset: 0x00070F68
		private float GetInterpFactor(Vector3 triggerPos)
		{
			if (!this.volume.enabled || this.volume.weight <= 0f)
			{
				return 0f;
			}
			if (this.volume.isGlobal)
			{
				return 1f;
			}
			float num = 0f;
			foreach (Collider collider in this.volumeColliders)
			{
				float num2 = float.PositiveInfinity;
				if (collider.enabled)
				{
					float sqrMagnitude = ((collider.ClosestPoint(triggerPos) - triggerPos) / 2f).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
					}
					float num3 = this.volume.blendDistance * this.volume.blendDistance;
					if (num2 <= num3 && num3 > 0f)
					{
						num = Mathf.Max(num, 1f - num2 / num3);
					}
				}
			}
			return num;
		}

		// Token: 0x04001B66 RID: 7014
		public PostProcessVolume volume;

		// Token: 0x04001B67 RID: 7015
		private Collider[] volumeColliders;

		// Token: 0x04001B68 RID: 7016
		[ColorUsage(true, true)]
		public Color overrideAmbientColor;

		// Token: 0x04001B69 RID: 7017
		private Color defaultAmbientColor;

		// Token: 0x04001B6A RID: 7018
		private bool hasCachedAmbientColor;
	}
}
