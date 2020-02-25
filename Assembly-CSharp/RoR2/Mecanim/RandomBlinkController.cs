using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000533 RID: 1331
	public class RandomBlinkController : MonoBehaviour
	{
		// Token: 0x06001F6D RID: 8045 RVA: 0x00088768 File Offset: 0x00086968
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= 0.25f)
			{
				this.stopwatch = 0f;
				for (int i = 0; i < this.blinkTriggers.Length; i++)
				{
					if (Util.CheckRoll(this.blinkChancePerUpdate, 0f, null))
					{
						this.animator.SetTrigger(this.blinkTriggers[i]);
					}
				}
			}
		}

		// Token: 0x04001D1E RID: 7454
		public Animator animator;

		// Token: 0x04001D1F RID: 7455
		public string[] blinkTriggers;

		// Token: 0x04001D20 RID: 7456
		public float blinkChancePerUpdate;

		// Token: 0x04001D21 RID: 7457
		private float stopwatch;

		// Token: 0x04001D22 RID: 7458
		private const float updateFrequency = 4f;
	}
}
