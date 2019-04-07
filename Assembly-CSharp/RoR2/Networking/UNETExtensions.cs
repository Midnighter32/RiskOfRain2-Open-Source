using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000594 RID: 1428
	public static class UNETExtensions
	{
		// Token: 0x0600202E RID: 8238 RVA: 0x00097168 File Offset: 0x00095368
		public static void ForceInitialize(this NetworkConnection conn, HostTopology hostTopology)
		{
			int num = 0;
			conn.Initialize("localhost", num, num, hostTopology);
		}

		// Token: 0x0400224B RID: 8779
		private static int nextConnectionId = -1;
	}
}
