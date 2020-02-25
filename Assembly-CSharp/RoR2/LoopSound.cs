using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000277 RID: 631
	public class LoopSound : MonoBehaviour
	{
		// Token: 0x06000E08 RID: 3592 RVA: 0x0003EBA8 File Offset: 0x0003CDA8
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			if (this.stopwatch > this.repeatInterval)
			{
				this.stopwatch -= this.repeatInterval;
				if (this.soundOwner)
				{
					Util.PlaySound(this.akSoundString, this.soundOwner.gameObject);
				}
			}
		}

		// Token: 0x04000DFC RID: 3580
		public string akSoundString;

		// Token: 0x04000DFD RID: 3581
		public float repeatInterval;

		// Token: 0x04000DFE RID: 3582
		public Transform soundOwner;

		// Token: 0x04000DFF RID: 3583
		private float stopwatch;
	}
}
