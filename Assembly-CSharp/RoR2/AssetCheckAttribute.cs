using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020003EC RID: 1004
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse]
	public class AssetCheckAttribute : Attribute
	{
		// Token: 0x0600185E RID: 6238 RVA: 0x00069427 File Offset: 0x00067627
		public AssetCheckAttribute(Type assetType)
		{
			this.assetType = assetType;
		}

		// Token: 0x040016EB RID: 5867
		public Type assetType;
	}
}
