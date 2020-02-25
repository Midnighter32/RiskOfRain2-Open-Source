using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200030E RID: 782
	public class ScaleSpriteByCamDistance : MonoBehaviour
	{
		// Token: 0x0600125C RID: 4700 RVA: 0x0004F2B0 File Offset: 0x0004D4B0
		static ScaleSpriteByCamDistance()
		{
			SceneCamera.onSceneCameraPreCull += ScaleSpriteByCamDistance.OnSceneCameraPreCull;
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x0004F2CD File Offset: 0x0004D4CD
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x0004F2DB File Offset: 0x0004D4DB
		private void OnEnable()
		{
			ScaleSpriteByCamDistance.instancesList.Add(this);
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x0004F2E8 File Offset: 0x0004D4E8
		private void OnDisable()
		{
			ScaleSpriteByCamDistance.instancesList.Remove(this);
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x0004F2F8 File Offset: 0x0004D4F8
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Vector3 position = sceneCamera.transform.position;
			for (int i = 0; i < ScaleSpriteByCamDistance.instancesList.Count; i++)
			{
				ScaleSpriteByCamDistance scaleSpriteByCamDistance = ScaleSpriteByCamDistance.instancesList[i];
				Transform transform = scaleSpriteByCamDistance.transform;
				float num = 1f;
				float num2 = Vector3.Distance(position, transform.position);
				switch (scaleSpriteByCamDistance.scalingMode)
				{
				case ScaleSpriteByCamDistance.ScalingMode.Direct:
					num = num2;
					break;
				case ScaleSpriteByCamDistance.ScalingMode.Square:
					num = num2 * num2;
					break;
				case ScaleSpriteByCamDistance.ScalingMode.Sqrt:
					num = Mathf.Sqrt(num2);
					break;
				case ScaleSpriteByCamDistance.ScalingMode.Cube:
					num = num2 * num2 * num2;
					break;
				case ScaleSpriteByCamDistance.ScalingMode.CubeRoot:
					num = Mathf.Pow(num2, 0.33333334f);
					break;
				}
				num *= scaleSpriteByCamDistance.scaleFactor;
				transform.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x0400114F RID: 4431
		private static List<ScaleSpriteByCamDistance> instancesList = new List<ScaleSpriteByCamDistance>();

		// Token: 0x04001150 RID: 4432
		private new Transform transform;

		// Token: 0x04001151 RID: 4433
		[Tooltip("The amount by which to scale.")]
		public float scaleFactor = 1f;

		// Token: 0x04001152 RID: 4434
		public ScaleSpriteByCamDistance.ScalingMode scalingMode;

		// Token: 0x0200030F RID: 783
		public enum ScalingMode
		{
			// Token: 0x04001154 RID: 4436
			Direct,
			// Token: 0x04001155 RID: 4437
			Square,
			// Token: 0x04001156 RID: 4438
			Sqrt,
			// Token: 0x04001157 RID: 4439
			Cube,
			// Token: 0x04001158 RID: 4440
			CubeRoot
		}
	}
}
