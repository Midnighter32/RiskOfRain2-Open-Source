using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001EA RID: 490
	public class DitherModel : MonoBehaviour
	{
		// Token: 0x06000A38 RID: 2616 RVA: 0x0002C973 File Offset: 0x0002AB73
		static DitherModel()
		{
			SceneCamera.onSceneCameraPreRender += DitherModel.OnSceneCameraPreRender;
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0002C990 File Offset: 0x0002AB90
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				DitherModel.RefreshObstructorsForCamera(sceneCamera.cameraRigController);
			}
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0002C9AC File Offset: 0x0002ABAC
		private static void RefreshObstructorsForCamera(CameraRigController cameraRigController)
		{
			Vector3 position = cameraRigController.transform.position;
			for (int i = 0; i < DitherModel.instancesList.Count; i++)
			{
				DitherModel ditherModel = DitherModel.instancesList[i];
				if (ditherModel.bounds)
				{
					Vector3 a = ditherModel.bounds.ClosestPointOnBounds(position);
					ditherModel.fade = Mathf.Clamp01(Util.Remap(Vector3.Distance(a, position), cameraRigController.fadeStartDistance, cameraRigController.fadeEndDistance, 0f, 1f));
					ditherModel.UpdateDither();
				}
				else
				{
					Debug.LogFormat("{0} has missing collider for dither model", new object[]
					{
						ditherModel.gameObject
					});
				}
			}
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x0002CA54 File Offset: 0x0002AC54
		private void UpdateDither()
		{
			for (int i = this.renderers.Length - 1; i >= 0; i--)
			{
				Renderer renderer = this.renderers[i];
				renderer.GetPropertyBlock(this.propertyStorage);
				this.propertyStorage.SetFloat("_Fade", this.fade);
				renderer.SetPropertyBlock(this.propertyStorage);
			}
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x0002CAAB File Offset: 0x0002ACAB
		private void Awake()
		{
			this.propertyStorage = new MaterialPropertyBlock();
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x0002CAB8 File Offset: 0x0002ACB8
		private void OnEnable()
		{
			DitherModel.instancesList.Add(this);
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x0002CAC5 File Offset: 0x0002ACC5
		private void OnDisable()
		{
			DitherModel.instancesList.Remove(this);
		}

		// Token: 0x04000A95 RID: 2709
		[HideInInspector]
		public float fade;

		// Token: 0x04000A96 RID: 2710
		public Collider bounds;

		// Token: 0x04000A97 RID: 2711
		public Renderer[] renderers;

		// Token: 0x04000A98 RID: 2712
		private MaterialPropertyBlock propertyStorage;

		// Token: 0x04000A99 RID: 2713
		private static List<DitherModel> instancesList = new List<DitherModel>();
	}
}
