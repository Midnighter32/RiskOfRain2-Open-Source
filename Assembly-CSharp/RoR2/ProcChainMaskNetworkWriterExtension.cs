using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200046C RID: 1132
	internal static class ProcChainMaskNetworkWriterExtension
	{
		// Token: 0x06001952 RID: 6482 RVA: 0x00079349 File Offset: 0x00077549
		public static void Write(this NetworkWriter writer, ProcChainMask procChainMask)
		{
			writer.Write(procChainMask.mask);
		}
	}
}
