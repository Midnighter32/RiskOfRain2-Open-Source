using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F0 RID: 752
	public class EyeFlare : MonoBehaviour
	{
		// Token: 0x06000F33 RID: 3891 RVA: 0x0004B0CE File Offset: 0x000492CE
		static EyeFlare()
		{
			SceneCamera.onSceneCameraPreCull += EyeFlare.OnSceneCameraPreCull;
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0004B0EB File Offset: 0x000492EB
		private void OnEnable()
		{
			EyeFlare.instancesList.Add(this);
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0004B0F8 File Offset: 0x000492F8
		private void OnDisable()
		{
			EyeFlare.instancesList.Remove(this);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0004B108 File Offset: 0x00049308
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Transform transform = Camera.current.transform;
			Quaternion rotation = transform.rotation;
			Vector3 forward = transform.forward;
			for (int i = 0; i < EyeFlare.instancesList.Count; i++)
			{
				EyeFlare eyeFlare = EyeFlare.instancesList[i];
				float num = eyeFlare.localScale;
				if (eyeFlare.directionSource)
				{
					float num2 = Vector3.Dot(forward, eyeFlare.directionSource.forward) * -0.5f + 0.5f;
					num *= num2 * num2;
				}
				eyeFlare.transform.localScale = new Vector3(num, num, num);
				eyeFlare.transform.rotation = rotation;
			}
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0004B1AD File Offset: 0x000493AD
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0400134A RID: 4938
		[Tooltip("The transform whose forward vector will be tested against the camera angle to determine scaling. This is usually the parent, and never this object since billboarding will affect the direction.")]
		public Transform directionSource;

		// Token: 0x0400134B RID: 4939
		public float localScale = 1f;

		// Token: 0x0400134C RID: 4940
		private static List<EyeFlare> instancesList = new List<EyeFlare>();

		// Token: 0x0400134D RID: 4941
		private new Transform transform;
	}
}
