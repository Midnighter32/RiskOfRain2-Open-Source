using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002B7 RID: 695
	public class ConstrainToScreen : MonoBehaviour
	{
		// Token: 0x06000E22 RID: 3618 RVA: 0x00045A4A File Offset: 0x00043C4A
		static ConstrainToScreen()
		{
			SceneCamera.onSceneCameraPreCull += ConstrainToScreen.OnSceneCameraPreCull;
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x00045A74 File Offset: 0x00043C74
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Camera camera = sceneCamera.camera;
			for (int i = 0; i < ConstrainToScreen.instanceTransformsList.Count; i++)
			{
				Transform transform = ConstrainToScreen.instanceTransformsList[i];
				Vector3 vector = camera.WorldToViewportPoint(transform.position);
				vector.x = Mathf.Clamp(vector.x, ConstrainToScreen.boundaryUVSize, 1f - ConstrainToScreen.boundaryUVSize);
				vector.y = Mathf.Clamp(vector.y, ConstrainToScreen.boundaryUVSize, 1f - ConstrainToScreen.boundaryUVSize);
				transform.position = camera.ViewportToWorldPoint(vector);
			}
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x00045B07 File Offset: 0x00043D07
		private void OnEnable()
		{
			ConstrainToScreen.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x00045B19 File Offset: 0x00043D19
		private void OnDisable()
		{
			ConstrainToScreen.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x04001209 RID: 4617
		private static float boundaryUVSize = 0.05f;

		// Token: 0x0400120A RID: 4618
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
