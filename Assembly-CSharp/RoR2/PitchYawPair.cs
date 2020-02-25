using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E4 RID: 996
	[Serializable]
	public struct PitchYawPair
	{
		// Token: 0x06001840 RID: 6208 RVA: 0x00068FBF File Offset: 0x000671BF
		public PitchYawPair(float pitch, float yaw)
		{
			this.pitch = pitch;
			this.yaw = yaw;
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x00068FD0 File Offset: 0x000671D0
		public static PitchYawPair Lerp(PitchYawPair a, PitchYawPair b, float t)
		{
			float num = Mathf.LerpAngle(a.pitch, b.pitch, t);
			float num2 = Mathf.LerpAngle(a.yaw, b.yaw, t);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x00069008 File Offset: 0x00067208
		public static PitchYawPair SmoothDamp(PitchYawPair current, PitchYawPair target, ref PitchYawPair velocity, float smoothTime, float maxSpeed)
		{
			float num = Mathf.SmoothDampAngle(current.pitch, target.pitch, ref velocity.pitch, smoothTime, maxSpeed);
			float num2 = Mathf.SmoothDampAngle(current.yaw, target.yaw, ref velocity.yaw, smoothTime, maxSpeed);
			return new PitchYawPair(num, num2);
		}

		// Token: 0x040016CF RID: 5839
		public static readonly PitchYawPair zero = new PitchYawPair(0f, 0f);

		// Token: 0x040016D0 RID: 5840
		public float pitch;

		// Token: 0x040016D1 RID: 5841
		public float yaw;
	}
}
