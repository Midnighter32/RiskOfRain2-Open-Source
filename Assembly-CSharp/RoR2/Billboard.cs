using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000266 RID: 614
	public class Billboard : MonoBehaviour
	{
		// Token: 0x06000B8A RID: 2954 RVA: 0x00038795 File Offset: 0x00036995
		static Billboard()
		{
			SceneCamera.onSceneCameraPreCull += Billboard.OnSceneCameraPreCull;
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x000387B4 File Offset: 0x000369B4
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Quaternion rotation = sceneCamera.transform.rotation;
			for (int i = 0; i < Billboard.instanceTransformsList.Count; i++)
			{
				Billboard.instanceTransformsList[i].rotation = rotation;
			}
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x000387F3 File Offset: 0x000369F3
		private void OnEnable()
		{
			Billboard.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x00038805 File Offset: 0x00036A05
		private void OnDisable()
		{
			Billboard.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x04000F75 RID: 3957
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
