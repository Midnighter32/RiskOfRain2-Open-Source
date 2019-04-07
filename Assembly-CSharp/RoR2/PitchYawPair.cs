using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000469 RID: 1129
	[Serializable]
	public struct PitchYawPair
	{
		// Token: 0x06001947 RID: 6471 RVA: 0x00079228 File Offset: 0x00077428
		public PitchYawPair(float pitch, float yaw)
		{
			this.pitch = pitch;
			this.yaw = yaw;
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x00079238 File Offset: 0x00077438
		public static PitchYawPair Lerp(PitchYawPair a, PitchYawPair b, float t)
		{
			float num = Mathf.LerpAngle(a.pitch, b.pitch, t);
			float num2 = Mathf.LerpAngle(a.yaw, b.yaw, t);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x00079270 File Offset: 0x00077470
		public static PitchYawPair SmoothDamp(PitchYawPair current, PitchYawPair target, ref PitchYawPair velocity, float smoothTime, float maxSpeed)
		{
			float num = Mathf.SmoothDampAngle(current.pitch, target.pitch, ref velocity.pitch, smoothTime, maxSpeed);
			float num2 = Mathf.SmoothDampAngle(current.yaw, target.yaw, ref velocity.yaw, smoothTime, maxSpeed);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x04001CBB RID: 7355
		public static readonly PitchYawPair zero = new PitchYawPair(0f, 0f);

		// Token: 0x04001CBC RID: 7356
		public float pitch;

		// Token: 0x04001CBD RID: 7357
		public float yaw;
	}
}
