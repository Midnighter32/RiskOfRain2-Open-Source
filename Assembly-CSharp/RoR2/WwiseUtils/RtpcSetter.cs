using System;
using UnityEngine;

namespace RoR2.WwiseUtils
{
	// Token: 0x02000494 RID: 1172
	public struct RtpcSetter
	{
		// Token: 0x06001C6F RID: 7279 RVA: 0x0007997A File Offset: 0x00077B7A
		public RtpcSetter(string name, GameObject gameObject = null)
		{
			this.name = name;
			this.id = AkSoundEngine.GetIDFromString(name);
			this.gameObject = gameObject;
			this.expectedEngineValue = float.NegativeInfinity;
			this.value = this.expectedEngineValue;
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000799AD File Offset: 0x00077BAD
		public void FlushIfChanged()
		{
			if (!this.expectedEngineValue.Equals(this.value))
			{
				this.expectedEngineValue = this.value;
				AkSoundEngine.SetRTPCValue(this.id, this.value, this.gameObject);
			}
		}

		// Token: 0x0400196A RID: 6506
		private readonly string name;

		// Token: 0x0400196B RID: 6507
		private readonly uint id;

		// Token: 0x0400196C RID: 6508
		private readonly GameObject gameObject;

		// Token: 0x0400196D RID: 6509
		private float expectedEngineValue;

		// Token: 0x0400196E RID: 6510
		public float value;
	}
}
