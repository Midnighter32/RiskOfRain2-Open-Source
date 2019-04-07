using System;
using EntityStates.LockedMage;

namespace RoR2.Achievements
{
	// Token: 0x020006A1 RID: 1697
	[RegisterAchievement("FreeMage", "Characters.Mage", null, typeof(FreeMageAchievement.FreeMageServerAchievement))]
	public class FreeMageAchievement : BaseAchievement
	{
		// Token: 0x060025CB RID: 9675 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A2 RID: 1698
		private class FreeMageServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025CE RID: 9678 RVA: 0x000AFB9F File Offset: 0x000ADD9F
			public override void OnInstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened += this.OnOpened;
			}

			// Token: 0x060025CF RID: 9679 RVA: 0x000AFBB8 File Offset: 0x000ADDB8
			public override void OnUninstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened -= this.OnOpened;
			}

			// Token: 0x060025D0 RID: 9680 RVA: 0x000AFBD4 File Offset: 0x000ADDD4
			private void OnOpened(Interactor interactor)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == interactor)
				{
					base.Grant();
				}
			}
		}
	}
}
