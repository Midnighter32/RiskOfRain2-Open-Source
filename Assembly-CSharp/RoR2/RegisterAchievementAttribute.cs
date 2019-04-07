using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020001F8 RID: 504
	[MeansImplicitUse]
	public class RegisterAchievementAttribute : Attribute
	{
		// Token: 0x060009EA RID: 2538 RVA: 0x00031699 File Offset: 0x0002F899
		public RegisterAchievementAttribute([NotNull] string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, Type serverTrackerType = null)
		{
			this.identifier = identifier;
			this.unlockableRewardIdentifier = unlockableRewardIdentifier;
			this.prerequisiteAchievementIdentifier = prerequisiteAchievementIdentifier;
			this.serverTrackerType = serverTrackerType;
		}

		// Token: 0x04000D20 RID: 3360
		public readonly string identifier;

		// Token: 0x04000D21 RID: 3361
		public readonly string unlockableRewardIdentifier;

		// Token: 0x04000D22 RID: 3362
		public readonly string prerequisiteAchievementIdentifier;

		// Token: 0x04000D23 RID: 3363
		public readonly Type serverTrackerType;
	}
}
