using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A8 RID: 1704
	[RegisterAchievement("KillElementalLemurians", "Items.ElementalRings", null, typeof(KillElementalLemuriansAchievement.KillElementalLemuriansServerAchievement))]
	public class KillElementalLemuriansAchievement : BaseAchievement
	{
		// Token: 0x060025E8 RID: 9704 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x020006A9 RID: 1705
		private class KillElementalLemuriansServerAchievement : BaseServerAchievement
		{
		}
	}
}
