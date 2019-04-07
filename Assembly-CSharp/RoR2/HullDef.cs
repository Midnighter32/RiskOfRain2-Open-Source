using System;

namespace RoR2
{
	// Token: 0x0200043B RID: 1083
	internal struct HullDef
	{
		// Token: 0x06001810 RID: 6160 RVA: 0x00072E48 File Offset: 0x00071048
		static HullDef()
		{
			HullDef.hullDefs[0] = new HullDef
			{
				height = 2f,
				radius = 0.5f
			};
			HullDef.hullDefs[1] = new HullDef
			{
				height = 8f,
				radius = 1.8f
			};
			HullDef.hullDefs[2] = new HullDef
			{
				height = 20f,
				radius = 5f
			};
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x00072EE4 File Offset: 0x000710E4
		public static HullDef Find(HullClassification hullClassification)
		{
			return HullDef.hullDefs[(int)hullClassification];
		}

		// Token: 0x04001B75 RID: 7029
		public float height;

		// Token: 0x04001B76 RID: 7030
		public float radius;

		// Token: 0x04001B77 RID: 7031
		private static HullDef[] hullDefs = new HullDef[3];
	}
}
