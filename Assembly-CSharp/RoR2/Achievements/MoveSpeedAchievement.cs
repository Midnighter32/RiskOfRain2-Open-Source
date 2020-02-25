using System;

namespace RoR2.Achievements
{
	// Token: 0x020006C0 RID: 1728
	[RegisterAchievement("MoveSpeed", "Items.JumpBoost", null, null)]
	public class MoveSpeedAchievement : BaseAchievement
	{
		// Token: 0x06002835 RID: 10293 RVA: 0x000ABD3F File Offset: 0x000A9F3F
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckMoveSpeed;
		}

		// Token: 0x06002836 RID: 10294 RVA: 0x000ABD58 File Offset: 0x000A9F58
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.CheckMoveSpeed;
			base.OnUninstall();
		}

		// Token: 0x06002837 RID: 10295 RVA: 0x000ABD74 File Offset: 0x000A9F74
		public void CheckMoveSpeed()
		{
			if (base.localUser != null && base.localUser.cachedBody && base.localUser.cachedBody.moveSpeed / base.localUser.cachedBody.baseMoveSpeed >= 4f)
			{
				base.Grant();
			}
		}

		// Token: 0x0400250B RID: 9483
		private const float requirement = 4f;
	}
}
