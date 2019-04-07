using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000569 RID: 1385
	public class RandomBlinkController : MonoBehaviour
	{
		// Token: 0x06001EDF RID: 7903 RVA: 0x00091BF4 File Offset: 0x0008FDF4
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

		// Token: 0x04002194 RID: 8596
		public Animator animator;

		// Token: 0x04002195 RID: 8597
		public string[] blinkTriggers;

		// Token: 0x04002196 RID: 8598
		public float blinkChancePerUpdate;

		// Token: 0x04002197 RID: 8599
		private float stopwatch;

		// Token: 0x04002198 RID: 8600
		private const float updateFrequency = 4f;
	}
}
