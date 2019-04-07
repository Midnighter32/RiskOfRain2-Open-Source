using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B3 RID: 1715
	[RegisterAchievement("MoveSpeed", "Items.JumpBoost", null, null)]
	public class MoveSpeedAchievement : BaseAchievement
	{
		// Token: 0x06002611 RID: 9745 RVA: 0x000B0253 File Offset: 0x000AE453
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckMoveSpeed;
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x000B026C File Offset: 0x000AE46C
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.CheckMoveSpeed;
			base.OnUninstall();
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x000B0288 File Offset: 0x000AE488
		public void CheckMoveSpeed()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.moveSpeed / this.localUser.cachedBody.baseMoveSpeed >= 4f)
			{
				base.Grant();
			}
		}

		// Token: 0x04002878 RID: 10360
		private const float requirement = 4f;
	}
}
