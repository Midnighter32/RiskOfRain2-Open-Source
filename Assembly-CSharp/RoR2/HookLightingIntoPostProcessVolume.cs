using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x0200039B RID: 923
	[RequireComponent(typeof(PostProcessVolume))]
	public class HookLightingIntoPostProcessVolume : MonoBehaviour
	{
		// Token: 0x0600166D RID: 5741 RVA: 0x00060799 File Offset: 0x0005E999
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

		// Token: 0x0600166E RID: 5742 RVA: 0x000607D2 File Offset: 0x0005E9D2
		private void OnDisable()
		{
			SceneCamera.onSceneCameraPreRender -= this.OnPreRenderSceneCam;
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x000607E8 File Offset: 0x0005E9E8
		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			float interpFactor = this.GetInterpFactor(sceneCam.camera.transform.position);
			RenderSettings.ambientLight = Color.Lerp(this.defaultAmbientColor, this.overrideAmbientColor, interpFactor);
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x00060824 File Offset: 0x0005EA24
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

		// Token: 0x04001513 RID: 5395
		public PostProcessVolume volume;

		// Token: 0x04001514 RID: 5396
		private Collider[] volumeColliders;

		// Token: 0x04001515 RID: 5397
		[ColorUsage(true, true)]
		public Color overrideAmbientColor;

		// Token: 0x04001516 RID: 5398
		private Color defaultAmbientColor;

		// Token: 0x04001517 RID: 5399
		private bool hasCachedAmbientColor;
	}
}
