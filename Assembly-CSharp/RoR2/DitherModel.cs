using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D8 RID: 728
	public class DitherModel : MonoBehaviour
	{
		// Token: 0x06000E8C RID: 3724 RVA: 0x00047A3F File Offset: 0x00045C3F
		static DitherModel()
		{
			SceneCamera.onSceneCameraPreRender += DitherModel.OnSceneCameraPreRender;
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00047A5C File Offset: 0x00045C5C
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				DitherModel.RefreshObstructorsForCamera(sceneCamera.cameraRigController);
			}
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x00047A78 File Offset: 0x00045C78
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

		// Token: 0x06000E8F RID: 3727 RVA: 0x00047B20 File Offset: 0x00045D20
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

		// Token: 0x06000E90 RID: 3728 RVA: 0x00047B77 File Offset: 0x00045D77
		private void Awake()
		{
			this.propertyStorage = new MaterialPropertyBlock();
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x00047B84 File Offset: 0x00045D84
		private void OnEnable()
		{
			DitherModel.instancesList.Add(this);
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x00047B91 File Offset: 0x00045D91
		private void OnDisable()
		{
			DitherModel.instancesList.Remove(this);
		}

		// Token: 0x040012A0 RID: 4768
		[HideInInspector]
		public float fade;

		// Token: 0x040012A1 RID: 4769
		public Collider bounds;

		// Token: 0x040012A2 RID: 4770
		public Renderer[] renderers;

		// Token: 0x040012A3 RID: 4771
		private MaterialPropertyBlock propertyStorage;

		// Token: 0x040012A4 RID: 4772
		private static List<DitherModel> instancesList = new List<DitherModel>();
	}
}
