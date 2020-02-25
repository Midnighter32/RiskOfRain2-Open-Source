using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069C RID: 1692
	[RegisterAchievement("CompleteUnknownEnding", "Characters.Mercenary", null, typeof(CompleteUnknownEndingAchievement.CompleteUnknownEndingServerAchievement))]
	public class CompleteUnknownEndingAchievement : BaseAchievement
	{
		// Token: 0x06002799 RID: 10137 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069D RID: 1693
		private class CompleteUnknownEndingServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600279C RID: 10140 RVA: 0x000AAF15 File Offset: 0x000A9115
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x0600279D RID: 10141 RVA: 0x000AAF2E File Offset: 0x000A912E
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x0600279E RID: 10142 RVA: 0x000AAF47 File Offset: 0x000A9147
			private void OnServerGameOver(Run run, GameResultType result)
			{
				if (result == GameResultType.Unknown)
				{
					base.Grant();
				}
			}
		}
	}
}
