using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000565 RID: 1381
	public static class UNETExtensions
	{
		// Token: 0x060020F8 RID: 8440 RVA: 0x0008E744 File Offset: 0x0008C944
		public static void ForceInitialize(this NetworkConnection conn, HostTopology hostTopology)
		{
			int num = 0;
			conn.Initialize("localhost", num, num, hostTopology);
		}

		// Token: 0x04001E02 RID: 7682
		private static int nextConnectionId = -1;
	}
}
