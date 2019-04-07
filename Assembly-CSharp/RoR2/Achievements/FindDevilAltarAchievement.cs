using System;
using EntityStates.Destructible;

namespace RoR2.Achievements
{
	// Token: 0x0200069D RID: 1693
	[RegisterAchievement("FindDevilAltar", "Items.NovaOnHeal", null, null)]
	public class FindDevilAltarAchievement : BaseAchievement
	{
		// Token: 0x060025B8 RID: 9656 RVA: 0x000AFA38 File Offset: 0x000ADC38
		public override void OnInstall()
		{
			base.OnInstall();
			AltarSkeletonDeath.onDeath += this.OnDeath;
		}

		// Token: 0x060025B9 RID: 9657 RVA: 0x000AFA51 File Offset: 0x000ADC51
		public override void OnUninstall()
		{
			base.OnUninstall();
			AltarSkeletonDeath.onDeath -= this.OnDeath;
		}

		// Token: 0x060025BA RID: 9658 RVA: 0x000AFA6A File Offset: 0x000ADC6A
		private void OnDeath()
		{
			base.Grant();
		}
	}
}
