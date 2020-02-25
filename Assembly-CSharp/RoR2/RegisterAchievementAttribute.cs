using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020000B9 RID: 185
	[MeansImplicitUse]
	public class RegisterAchievementAttribute : Attribute
	{
		// Token: 0x060003AF RID: 943 RVA: 0x0000E359 File Offset: 0x0000C559
		public RegisterAchievementAttribute([NotNull] string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, Type serverTrackerType = null)
		{
			this.identifier = identifier;
			this.unlockableRewardIdentifier = unlockableRewardIdentifier;
			this.prerequisiteAchievementIdentifier = prerequisiteAchievementIdentifier;
			this.serverTrackerType = serverTrackerType;
		}

		// Token: 0x0400032F RID: 815
		public readonly string identifier;

		// Token: 0x04000330 RID: 816
		public readonly string unlockableRewardIdentifier;

		// Token: 0x04000331 RID: 817
		public readonly string prerequisiteAchievementIdentifier;

		// Token: 0x04000332 RID: 818
		public readonly Type serverTrackerType;
	}
}
