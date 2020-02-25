using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037B RID: 891
	public class WeatherParticles : MonoBehaviour
	{
		// Token: 0x060015A8 RID: 5544 RVA: 0x0005C52D File Offset: 0x0005A72D
		static WeatherParticles()
		{
			SceneCamera.onSceneCameraPreRender += WeatherParticles.OnSceneCameraPreRender;
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x0005C54C File Offset: 0x0005A74C
		private void UpdateForCamera(CameraRigController cameraRigController, bool lockPosition, bool lockRotation)
		{
			Transform transform = cameraRigController.transform;
			base.transform.SetPositionAndRotation(lockPosition ? transform.position : base.transform.position, lockRotation ? transform.rotation : base.transform.rotation);
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x0005C598 File Offset: 0x0005A798
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				for (int i = 0; i < WeatherParticles.instancesList.Count; i++)
				{
					WeatherParticles weatherParticles = WeatherParticles.instancesList[i];
					weatherParticles.UpdateForCamera(sceneCamera.cameraRigController, weatherParticles.lockPosition, weatherParticles.lockRotation);
				}
			}
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0005C5EB File Offset: 0x0005A7EB
		private void OnEnable()
		{
			WeatherParticles.instancesList.Add(this);
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x0005C5F8 File Offset: 0x0005A7F8
		private void OnDisable()
		{
			WeatherParticles.instancesList.Remove(this);
		}

		// Token: 0x04001435 RID: 5173
		public bool lockPosition = true;

		// Token: 0x04001436 RID: 5174
		public bool lockRotation = true;

		// Token: 0x04001437 RID: 5175
		private static List<WeatherParticles> instancesList = new List<WeatherParticles>();
	}
}
