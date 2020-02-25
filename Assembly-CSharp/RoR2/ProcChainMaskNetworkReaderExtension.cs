using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E9 RID: 1001
	internal static class ProcChainMaskNetworkReaderExtension
	{
		// Token: 0x06001850 RID: 6224 RVA: 0x00069180 File Offset: 0x00067380
		public static ProcChainMask ReadProcChainMask(this NetworkReader reader)
		{
			return new ProcChainMask
			{
				mask = reader.ReadUInt16()
			};
		}
	}
}
