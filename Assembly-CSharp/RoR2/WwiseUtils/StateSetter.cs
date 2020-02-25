using System;

namespace RoR2.WwiseUtils
{
	// Token: 0x02000496 RID: 1174
	public struct StateSetter
	{
		// Token: 0x06001C73 RID: 7283 RVA: 0x00079A21 File Offset: 0x00077C21
		public StateSetter(string name)
		{
			this.name = name;
			this.id = AkSoundEngine.GetIDFromString(name);
			this.expectedEngineValueId = 0U;
			this.valueId = this.expectedEngineValueId;
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x00079A49 File Offset: 0x00077C49
		public void FlushIfChanged()
		{
			if (!this.expectedEngineValueId.Equals(this.valueId))
			{
				this.expectedEngineValueId = this.valueId;
				AkSoundEngine.SetState(this.id, this.valueId);
			}
		}

		// Token: 0x04001972 RID: 6514
		private readonly string name;

		// Token: 0x04001973 RID: 6515
		private readonly uint id;

		// Token: 0x04001974 RID: 6516
		private uint expectedEngineValueId;

		// Token: 0x04001975 RID: 6517
		public uint valueId;
	}
}
