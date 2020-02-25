using System;

namespace RoR2
{
	// Token: 0x0200039E RID: 926
	internal struct HullDef
	{
		// Token: 0x06001672 RID: 5746 RVA: 0x00060904 File Offset: 0x0005EB04
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

		// Token: 0x06001673 RID: 5747 RVA: 0x000609A0 File Offset: 0x0005EBA0
		public static HullDef Find(HullClassification hullClassification)
		{
			return HullDef.hullDefs[(int)hullClassification];
		}

		// Token: 0x04001522 RID: 5410
		public float height;

		// Token: 0x04001523 RID: 5411
		public float radius;

		// Token: 0x04001524 RID: 5412
		private static HullDef[] hullDefs = new HullDef[3];
	}
}
