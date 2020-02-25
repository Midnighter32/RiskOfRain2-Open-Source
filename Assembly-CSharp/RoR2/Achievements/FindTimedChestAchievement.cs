using System;
using EntityStates.TimedChest;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006A6 RID: 1702
	[RegisterAchievement("FindTimedChest", "Items.BFG", null, typeof(FindTimedChestAchievement.FindTimedChestServerAchievement))]
	public class FindTimedChestAchievement : BaseAchievement
	{
		// Token: 0x060027C7 RID: 10183 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060027C8 RID: 10184 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A7 RID: 1703
		private class FindTimedChestServerAchievement : BaseServerAchievement
		{
			// Token: 0x060027CA RID: 10186 RVA: 0x000AB2BE File Offset: 0x000A94BE
			public override void OnInstall()
			{
				base.OnInstall();
				Debug.Log("subscribed");
				Opening.onOpened += this.OnOpened;
			}

			// Token: 0x060027CB RID: 10187 RVA: 0x000AB2E1 File Offset: 0x000A94E1
			public override void OnUninstall()
			{
				base.OnInstall();
				Opening.onOpened -= this.OnOpened;
			}

			// Token: 0x060027CC RID: 10188 RVA: 0x000AB2FA File Offset: 0x000A94FA
			private void OnOpened()
			{
				Debug.Log("grant");
				base.Grant();
			}
		}
	}
}
