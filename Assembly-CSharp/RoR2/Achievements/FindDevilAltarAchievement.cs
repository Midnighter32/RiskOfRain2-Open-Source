using System;
using EntityStates.Destructible;

namespace RoR2.Achievements
{
	// Token: 0x020006A5 RID: 1701
	[RegisterAchievement("FindDevilAltar", "Items.NovaOnHeal", null, null)]
	public class FindDevilAltarAchievement : BaseAchievement
	{
		// Token: 0x060027C3 RID: 10179 RVA: 0x000AB284 File Offset: 0x000A9484
		public override void OnInstall()
		{
			base.OnInstall();
			AltarSkeletonDeath.onDeath += this.OnDeath;
		}

		// Token: 0x060027C4 RID: 10180 RVA: 0x000AB29D File Offset: 0x000A949D
		public override void OnUninstall()
		{
			base.OnUninstall();
			AltarSkeletonDeath.onDeath -= this.OnDeath;
		}

		// Token: 0x060027C5 RID: 10181 RVA: 0x000AB2B6 File Offset: 0x000A94B6
		private void OnDeath()
		{
			base.Grant();
		}
	}
}
