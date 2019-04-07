using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200041F RID: 1055
	public class WeatherParticles : MonoBehaviour
	{
		// Token: 0x06001770 RID: 6000 RVA: 0x0006F231 File Offset: 0x0006D431
		static WeatherParticles()
		{
			SceneCamera.onSceneCameraPreRender += WeatherParticles.OnSceneCameraPreRender;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0006F250 File Offset: 0x0006D450
		private void UpdateForCamera(CameraRigController cameraRigController, bool lockPosition, bool lockRotation)
		{
			Transform transform = cameraRigController.transform;
			base.transform.SetPositionAndRotation(lockPosition ? transform.position : base.transform.position, lockRotation ? transform.rotation : base.transform.rotation);
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x0006F29C File Offset: 0x0006D49C
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

		// Token: 0x06001773 RID: 6003 RVA: 0x0006F2EF File Offset: 0x0006D4EF
		private void OnEnable()
		{
			WeatherParticles.instancesList.Add(this);
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0006F2FC File Offset: 0x0006D4FC
		private void OnDisable()
		{
			WeatherParticles.instancesList.Remove(this);
		}

		// Token: 0x04001A9F RID: 6815
		public bool lockPosition = true;

		// Token: 0x04001AA0 RID: 6816
		public bool lockRotation = true;

		// Token: 0x04001AA1 RID: 6817
		private static List<WeatherParticles> instancesList = new List<WeatherParticles>();
	}
}
