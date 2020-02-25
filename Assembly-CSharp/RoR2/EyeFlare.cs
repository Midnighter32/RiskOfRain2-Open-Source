using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FE RID: 510
	public class EyeFlare : MonoBehaviour
	{
		// Token: 0x06000AE1 RID: 2785 RVA: 0x0003003E File Offset: 0x0002E23E
		static EyeFlare()
		{
			SceneCamera.onSceneCameraPreCull += EyeFlare.OnSceneCameraPreCull;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0003005B File Offset: 0x0002E25B
		private void OnEnable()
		{
			EyeFlare.instancesList.Add(this);
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x00030068 File Offset: 0x0002E268
		private void OnDisable()
		{
			EyeFlare.instancesList.Remove(this);
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00030078 File Offset: 0x0002E278
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

		// Token: 0x06000AE5 RID: 2789 RVA: 0x0003011D File Offset: 0x0002E31D
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x04000B2C RID: 2860
		[Tooltip("The transform whose forward vector will be tested against the camera angle to determine scaling. This is usually the parent, and never this object since billboarding will affect the direction.")]
		public Transform directionSource;

		// Token: 0x04000B2D RID: 2861
		public float localScale = 1f;

		// Token: 0x04000B2E RID: 2862
		private static List<EyeFlare> instancesList = new List<EyeFlare>();

		// Token: 0x04000B2F RID: 2863
		private new Transform transform;
	}
}
