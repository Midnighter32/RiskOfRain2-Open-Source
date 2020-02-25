using System;
using EntityStates.Loader;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006B9 RID: 1721
	[RegisterAchievement("LoaderBigSlam", "Skills.Loader.ZapFist", "DefeatSuperRoboBallBoss", null)]
	public class LoaderBigSlamAchievement : BaseAchievement
	{
		// Token: 0x06002815 RID: 10261 RVA: 0x000AB9EB File Offset: 0x000A9BEB
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("LoaderBody");
		}

		// Token: 0x06002816 RID: 10262 RVA: 0x000AB9F7 File Offset: 0x000A9BF7
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			BaseSwingChargedFist.onHitAuthorityGlobal += this.SwingChargedFistOnOnHitAuthorityGlobal;
		}

		// Token: 0x06002817 RID: 10263 RVA: 0x000ABA10 File Offset: 0x000A9C10
		protected override void OnBodyRequirementBroken()
		{
			BaseSwingChargedFist.onHitAuthorityGlobal -= this.SwingChargedFistOnOnHitAuthorityGlobal;
			base.OnBodyRequirementBroken();
		}

		// Token: 0x06002818 RID: 10264 RVA: 0x000ABA2C File Offset: 0x000A9C2C
		private void SwingChargedFistOnOnHitAuthorityGlobal(BaseSwingChargedFist state)
		{
			if (state.outer.commonComponents.characterBody == base.localUser.cachedBody)
			{
				Debug.LogFormat("{0}/{1}", new object[]
				{
					state.punchSpeed,
					LoaderBigSlamAchievement.requirement
				});
				if (state.punchSpeed >= LoaderBigSlamAchievement.requirement)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x04002507 RID: 9479
		private static readonly float requirement = (float)(300.0 * HGUnitConversions.milesToMeters / (1.0 * HGUnitConversions.hoursToSeconds));
	}
}
