using System;
using System.Linq;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006B8 RID: 1720
	[RegisterAchievement("StayAlive1", "Items.ExtraLife", null, null)]
	public class StayAlive1Achievement : BaseAchievement
	{
		// Token: 0x06002626 RID: 9766 RVA: 0x000B04FA File Offset: 0x000AE6FA
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.Check;
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x000B0513 File Offset: 0x000AE713
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000B052C File Offset: 0x000AE72C
		private void Check()
		{
			NetworkUser networkUser = NetworkUser.readOnlyLocalPlayersList.FirstOrDefault((NetworkUser v) => v.localUser == this.localUser);
			if (networkUser)
			{
				GameObject masterObject = networkUser.masterObject;
				if (masterObject)
				{
					CharacterMaster component = masterObject.GetComponent<CharacterMaster>();
					if (component && component.currentLifeStopwatch >= 1800f)
					{
						base.Grant();
					}
				}
			}
		}

		// Token: 0x0400287D RID: 10365
		private const float requirement = 1800f;
	}
}
