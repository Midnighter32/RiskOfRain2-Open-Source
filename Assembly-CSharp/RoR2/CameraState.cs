using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200016F RID: 367
	public struct CameraState
	{
		// Token: 0x060006D4 RID: 1748 RVA: 0x0001BD9C File Offset: 0x00019F9C
		public static CameraState Lerp(ref CameraState a, ref CameraState b, float t)
		{
			return new CameraState
			{
				position = Vector3.LerpUnclamped(a.position, b.position, t),
				rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t),
				fov = Mathf.LerpUnclamped(a.fov, b.fov, t)
			};
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0001BE00 File Offset: 0x0001A000
		public static CameraState SmoothDamp(CameraState current, CameraState target, ref Vector3 positionVelocity, ref float angleVelocity, ref float fovVelocity, float smoothTime)
		{
			return new CameraState
			{
				position = Vector3.SmoothDamp(current.position, target.position, ref positionVelocity, smoothTime),
				rotation = Util.SmoothDampQuaternion(current.rotation, target.rotation, ref angleVelocity, smoothTime),
				fov = Mathf.SmoothDamp(current.fov, target.fov, ref fovVelocity, smoothTime)
			};
		}

		// Token: 0x0400071E RID: 1822
		public Vector3 position;

		// Token: 0x0400071F RID: 1823
		public Quaternion rotation;

		// Token: 0x04000720 RID: 1824
		public float fov;
	}
}
