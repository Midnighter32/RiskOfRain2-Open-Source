using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000233 RID: 563
	public class HoverEngineDisplay : MonoBehaviour
	{
		// Token: 0x06000C8D RID: 3213 RVA: 0x000388D4 File Offset: 0x00036AD4
		private void FixedUpdate()
		{
			ref Vector3 localEulerAngles = base.transform.localEulerAngles;
			float t = Mathf.Clamp01(this.hoverEngine.forceStrength / this.hoverEngine.hoverForce * this.forceScale);
			float target = Mathf.LerpAngle(this.minPitch, this.maxPitch, t);
			float x = Mathf.SmoothDampAngle(localEulerAngles.x, target, ref this.smoothVelocity, this.smoothTime);
			base.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		}

		// Token: 0x04000C8A RID: 3210
		public HoverEngine hoverEngine;

		// Token: 0x04000C8B RID: 3211
		[Tooltip("The local pitch at zero engine strength")]
		public float minPitch = -20f;

		// Token: 0x04000C8C RID: 3212
		[Tooltip("The local pitch at max engine strength")]
		public float maxPitch = 60f;

		// Token: 0x04000C8D RID: 3213
		public float smoothTime = 0.2f;

		// Token: 0x04000C8E RID: 3214
		public float forceScale = 1f;

		// Token: 0x04000C8F RID: 3215
		private float smoothVelocity;
	}
}
