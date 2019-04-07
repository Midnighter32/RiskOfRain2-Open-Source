using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000277 RID: 631
	public struct CameraState
	{
		// Token: 0x06000BDE RID: 3038 RVA: 0x00039F04 File Offset: 0x00038104
		public static CameraState Lerp(ref CameraState a, ref CameraState b, float t)
		{
			return new CameraState
			{
				position = Vector3.LerpUnclamped(a.position, b.position, t),
				rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t),
				fov = Mathf.LerpUnclamped(a.fov, b.fov, t)
			};
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x00039F68 File Offset: 0x00038168
		public static CameraState SmoothDamp(CameraState current, CameraState target, ref Vector3 positionVelocity, ref float angleVelocity, ref float fovVelocity, float smoothTime)
		{
			return new CameraState
			{
				position = Vector3.SmoothDamp(current.position, target.position, ref positionVelocity, smoothTime),
				rotation = Util.SmoothDampQuaternion(current.rotation, target.rotation, ref angleVelocity, smoothTime),
				fov = Mathf.SmoothDamp(current.fov, target.fov, ref fovVelocity, smoothTime)
			};
		}

		// Token: 0x04000FCC RID: 4044
		public Vector3 position;

		// Token: 0x04000FCD RID: 4045
		public Quaternion rotation;

		// Token: 0x04000FCE RID: 4046
		public float fov;
	}
}
