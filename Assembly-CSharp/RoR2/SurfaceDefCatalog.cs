using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044C RID: 1100
	public static class SurfaceDefCatalog
	{
		// Token: 0x06001ACD RID: 6861 RVA: 0x00071B17 File Offset: 0x0006FD17
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SurfaceDefCatalog.SetSurfaceDefs(Resources.LoadAll<SurfaceDef>("SurfaceDefs/"));
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x00071B28 File Offset: 0x0006FD28
		private static void SetSurfaceDefs(SurfaceDef[] newSurfaceDefs)
		{
			Array.Resize<SurfaceDef>(ref SurfaceDefCatalog.surfaceDefs, newSurfaceDefs.Length);
			Array.Copy(newSurfaceDefs, SurfaceDefCatalog.surfaceDefs, newSurfaceDefs.Length);
			for (int i = 0; i < SurfaceDefCatalog.surfaceDefs.Length; i++)
			{
				SurfaceDefCatalog.surfaceDefs[i].surfaceDefIndex = (SurfaceDefIndex)i;
			}
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x00071B6F File Offset: 0x0006FD6F
		public static SurfaceDef GetSurfaceDef(SurfaceDefIndex surfaceDefIndex)
		{
			return HGArrayUtilities.GetSafe<SurfaceDef>(SurfaceDefCatalog.surfaceDefs, (int)surfaceDefIndex);
		}

		// Token: 0x0400184B RID: 6219
		private static SurfaceDef[] surfaceDefs = Array.Empty<SurfaceDef>();
	}
}
