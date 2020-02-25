using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B1 RID: 1713
	[RegisterAchievement("KillElementalLemurians", "Items.ElementalRings", null, typeof(KillElementalLemuriansAchievement.KillElementalLemuriansServerAchievement))]
	public class KillElementalLemuriansAchievement : BaseAchievement
	{
		// Token: 0x060027FA RID: 10234 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x020006B2 RID: 1714
		private class KillElementalLemuriansServerAchievement : BaseServerAchievement
		{
		}
	}
}
