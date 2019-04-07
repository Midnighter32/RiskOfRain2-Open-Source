using System;
using UnityEngine.Networking;

namespace RoR2.Extensions
{
	// Token: 0x0200050B RID: 1291
	public static class PickupIndexNetworkSerializationExtensions
	{
		// Token: 0x06001D2E RID: 7470 RVA: 0x00087E73 File Offset: 0x00086073
		public static void Write(this NetworkWriter writer, PickupIndex value)
		{
			PickupIndex.WriteToNetworkWriter(writer, value);
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x00087E7C File Offset: 0x0008607C
		public static PickupIndex ReadPickupIndex(this NetworkReader reader)
		{
			return PickupIndex.ReadFromNetworkReader(reader);
		}
	}
}
