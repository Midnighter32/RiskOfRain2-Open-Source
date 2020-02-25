using System;

namespace RoR2.Audio
{
	// Token: 0x0200067C RID: 1660
	public struct AkEventIdArg
	{
		// Token: 0x060026F0 RID: 9968 RVA: 0x000A99E1 File Offset: 0x000A7BE1
		public static explicit operator AkEventIdArg(string eventName)
		{
			return new AkEventIdArg((eventName == null) ? 0U : AkSoundEngine.GetIDFromString(eventName));
		}

		// Token: 0x060026F1 RID: 9969 RVA: 0x000A99F4 File Offset: 0x000A7BF4
		public static implicit operator AkEventIdArg(uint akEventId)
		{
			return new AkEventIdArg(akEventId);
		}

		// Token: 0x060026F2 RID: 9970 RVA: 0x000A99FC File Offset: 0x000A7BFC
		public static implicit operator uint(AkEventIdArg akEventIdArg)
		{
			return akEventIdArg.id;
		}

		// Token: 0x060026F3 RID: 9971 RVA: 0x000A9A04 File Offset: 0x000A7C04
		private AkEventIdArg(uint id)
		{
			this.id = id;
		}

		// Token: 0x040024BD RID: 9405
		public readonly uint id;
	}
}
