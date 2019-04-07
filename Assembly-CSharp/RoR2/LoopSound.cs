using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034C RID: 844
	public class LoopSound : MonoBehaviour
	{
		// Token: 0x06001175 RID: 4469 RVA: 0x00056BFC File Offset: 0x00054DFC
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			if (this.stopwatch > this.repeatInterval)
			{
				this.stopwatch -= this.repeatInterval;
				Util.PlaySound(this.akSoundString, this.soundOwner.gameObject);
			}
		}

		// Token: 0x0400157E RID: 5502
		public string akSoundString;

		// Token: 0x0400157F RID: 5503
		public float repeatInterval;

		// Token: 0x04001580 RID: 5504
		public Transform soundOwner;

		// Token: 0x04001581 RID: 5505
		private float stopwatch;
	}
}
