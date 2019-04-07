using System;

namespace RoR2.Achievements
{
	// Token: 0x02000696 RID: 1686
	[RegisterAchievement("CompleteUnknownEnding", "Characters.Mercenary", null, typeof(CompleteUnknownEndingAchievement.CompleteUnknownEndingServerAchievement))]
	public class CompleteUnknownEndingAchievement : BaseAchievement
	{
		// Token: 0x06002596 RID: 9622 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002597 RID: 9623 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000697 RID: 1687
		private class CompleteUnknownEndingServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002599 RID: 9625 RVA: 0x000AF73D File Offset: 0x000AD93D
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x0600259A RID: 9626 RVA: 0x000AF756 File Offset: 0x000AD956
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x0600259B RID: 9627 RVA: 0x000AF76F File Offset: 0x000AD96F
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
