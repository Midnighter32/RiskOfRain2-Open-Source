using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200015C RID: 348
	public class Billboard : MonoBehaviour
	{
		// Token: 0x06000668 RID: 1640 RVA: 0x0001A49D File Offset: 0x0001869D
		static Billboard()
		{
			SceneCamera.onSceneCameraPreCull += Billboard.OnSceneCameraPreCull;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0001A4BC File Offset: 0x000186BC
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Quaternion rotation = sceneCamera.transform.rotation;
			for (int i = 0; i < Billboard.instanceTransformsList.Count; i++)
			{
				Billboard.instanceTransformsList[i].rotation = rotation;
			}
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0001A4FB File Offset: 0x000186FB
		private void OnEnable()
		{
			Billboard.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0001A50D File Offset: 0x0001870D
		private void OnDisable()
		{
			Billboard.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x040006BD RID: 1725
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
