using System;
using EntityStates.LockedMage;

namespace RoR2.Achievements
{
	// Token: 0x020006A9 RID: 1705
	[RegisterAchievement("FreeMage", "Characters.Mage", null, typeof(FreeMageAchievement.FreeMageServerAchievement))]
	public class FreeMageAchievement : BaseAchievement
	{
		// Token: 0x060027D6 RID: 10198 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060027D7 RID: 10199 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AA RID: 1706
		private class FreeMageServerAchievement : BaseServerAchievement
		{
			// Token: 0x060027D9 RID: 10201 RVA: 0x000AB3EB File Offset: 0x000A95EB
			public override void OnInstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened += this.OnOpened;
			}

			// Token: 0x060027DA RID: 10202 RVA: 0x000AB404 File Offset: 0x000A9604
			public override void OnUninstall()
			{
				base.OnInstall();
				UnlockingMage.onOpened -= this.OnOpened;
			}

			// Token: 0x060027DB RID: 10203 RVA: 0x000AB420 File Offset: 0x000A9620
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
