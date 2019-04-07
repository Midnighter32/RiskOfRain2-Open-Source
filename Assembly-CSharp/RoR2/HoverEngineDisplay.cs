using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000313 RID: 787
	public class HoverEngineDisplay : MonoBehaviour
	{
		// Token: 0x0600104B RID: 4171 RVA: 0x00051B18 File Offset: 0x0004FD18
		private void FixedUpdate()
		{
			ref Vector3 localEulerAngles = base.transform.localEulerAngles;
			float t = Mathf.Clamp01(this.hoverEngine.forceStrength / this.hoverEngine.hoverForce * this.forceScale);
			float target = Mathf.LerpAngle(this.minPitch, this.maxPitch, t);
			float x = Mathf.SmoothDampAngle(localEulerAngles.x, target, ref this.smoothVelocity, this.smoothTime);
			base.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		}

		// Token: 0x0400144C RID: 5196
		public HoverEngine hoverEngine;

		// Token: 0x0400144D RID: 5197
		[Tooltip("The local pitch at zero engine strength")]
		public float minPitch = -20f;

		// Token: 0x0400144E RID: 5198
		[Tooltip("The local pitch at max engine strength")]
		public float maxPitch = 60f;

		// Token: 0x0400144F RID: 5199
		public float smoothTime = 0.2f;

		// Token: 0x04001450 RID: 5200
		public float forceScale = 1f;

		// Token: 0x04001451 RID: 5201
		private float smoothVelocity;
	}
}
