using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E8 RID: 1000
	internal static class ProcChainMaskNetworkWriterExtension
	{
		// Token: 0x0600184F RID: 6223 RVA: 0x00069172 File Offset: 0x00067372
		public static void Write(this NetworkWriter writer, ProcChainMask procChainMask)
		{
			writer.Write(procChainMask.mask);
		}
	}
}
