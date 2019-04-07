using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C4 RID: 964
	public class ScaleSpriteByCamDistance : MonoBehaviour
	{
		// Token: 0x060014F1 RID: 5361 RVA: 0x00064AF0 File Offset: 0x00062CF0
		static ScaleSpriteByCamDistance()
		{
			SceneCamera.onSceneCameraPreCull += ScaleSpriteByCamDistance.OnSceneCameraPreCull;
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00064B0D File Offset: 0x00062D0D
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00064B1B File Offset: 0x00062D1B
		private void OnEnable()
		{
			ScaleSpriteByCamDistance.instancesList.Add(this);
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x00064B28 File Offset: 0x00062D28
		private void OnDisable()
		{
			ScaleSpriteByCamDistance.instancesList.Remove(this);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x00064B38 File Offset: 0x00062D38
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

		// Token: 0x04001847 RID: 6215
		private static List<ScaleSpriteByCamDistance> instancesList = new List<ScaleSpriteByCamDistance>();

		// Token: 0x04001848 RID: 6216
		private new Transform transform;

		// Token: 0x04001849 RID: 6217
		[Tooltip("The amount by which to scale.")]
		public float scaleFactor = 1f;

		// Token: 0x0400184A RID: 6218
		public ScaleSpriteByCamDistance.ScalingMode scalingMode;

		// Token: 0x020003C5 RID: 965
		public enum ScalingMode
		{
			// Token: 0x0400184C RID: 6220
			Direct,
			// Token: 0x0400184D RID: 6221
			Square,
			// Token: 0x0400184E RID: 6222
			Sqrt,
			// Token: 0x0400184F RID: 6223
			Cube,
			// Token: 0x04001850 RID: 6224
			CubeRoot
		}
	}
}
