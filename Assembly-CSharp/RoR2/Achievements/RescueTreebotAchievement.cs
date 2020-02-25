using System;
using EntityStates.Treebot.UnlockInteractable;

namespace RoR2.Achievements
{
	// Token: 0x020006C7 RID: 1735
	[RegisterAchievement("RescueTreebot", "Characters.Treebot", null, typeof(RescueTreebotAchievement.RescueTreebotServerAchievement))]
	public class RescueTreebotAchievement : BaseAchievement
	{
		// Token: 0x06002854 RID: 10324 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002855 RID: 10325 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006C8 RID: 1736
		public class RescueTreebotServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002857 RID: 10327 RVA: 0x000AC07E File Offset: 0x000AA27E
			public override void OnInstall()
			{
				base.OnInstall();
				Unlock.onActivated += this.OnActivated;
			}

			// Token: 0x06002858 RID: 10328 RVA: 0x000AC097 File Offset: 0x000AA297
			public override void OnUninstall()
			{
				Unlock.onActivated -= this.OnActivated;
				base.OnInstall();
			}

			// Token: 0x06002859 RID: 10329 RVA: 0x000AC0B0 File Offset: 0x000AA2B0
			private void OnActivated(Interactor interactor)
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
