using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x0200046E RID: 1134
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse]
	public class AssetCheckAttribute : Attribute
	{
		// Token: 0x06001954 RID: 6484 RVA: 0x0007937B File Offset: 0x0007757B
		public AssetCheckAttribute(Type assetType)
		{
			this.assetType = assetType;
		}

		// Token: 0x04001CCF RID: 7375
		public Type assetType;
	}
}
