using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001C6 RID: 454
	public class ConstrainToScreen : MonoBehaviour
	{
		// Token: 0x060009C1 RID: 2497 RVA: 0x0002A9CB File Offset: 0x00028BCB
		static ConstrainToScreen()
		{
			SceneCamera.onSceneCameraPreCull += ConstrainToScreen.OnSceneCameraPreCull;
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x0002A9F4 File Offset: 0x00028BF4
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

		// Token: 0x060009C3 RID: 2499 RVA: 0x0002AA87 File Offset: 0x00028C87
		private void OnEnable()
		{
			ConstrainToScreen.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x0002AA99 File Offset: 0x00028C99
		private void OnDisable()
		{
			ConstrainToScreen.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x040009F1 RID: 2545
		private static float boundaryUVSize = 0.05f;

		// Token: 0x040009F2 RID: 2546
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
