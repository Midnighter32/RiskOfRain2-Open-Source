using System;
using EntityStates.TimedChest;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x0200069E RID: 1694
	[RegisterAchievement("FindTimedChest", "Items.BFG", null, typeof(FindTimedChestAchievement.FindTimedChestServerAchievement))]
	public class FindTimedChestAchievement : BaseAchievement
	{
		// Token: 0x060025BC RID: 9660 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025BD RID: 9661 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069F RID: 1695
		private class FindTimedChestServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025BF RID: 9663 RVA: 0x000AFA72 File Offset: 0x000ADC72
			public override void OnInstall()
			{
				base.OnInstall();
				Debug.Log("subscribed");
				Opening.onOpened += this.OnOpened;
			}

			// Token: 0x060025C0 RID: 9664 RVA: 0x000AFA95 File Offset: 0x000ADC95
			public override void OnUninstall()
			{
				base.OnInstall();
				Opening.onOpened -= this.OnOpened;
			}

			// Token: 0x060025C1 RID: 9665 RVA: 0x000AFAAE File Offset: 0x000ADCAE
			private void OnOpened()
			{
				Debug.Log("grant");
				base.Grant();
			}
		}
	}
}
