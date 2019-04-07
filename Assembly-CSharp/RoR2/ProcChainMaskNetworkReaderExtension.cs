using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200046D RID: 1133
	internal static class ProcChainMaskNetworkReaderExtension
	{
		// Token: 0x06001953 RID: 6483 RVA: 0x00079358 File Offset: 0x00077558
		public static ProcChainMask ReadProcChainMask(this NetworkReader reader)
		{
			return new ProcChainMask
			{
				mask = reader.ReadUInt16()
			};
		}
	}
}
