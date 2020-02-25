using System;
using System.Linq;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006C9 RID: 1737
	[RegisterAchievement("StayAlive1", "Items.ExtraLife", null, null)]
	public class StayAlive1Achievement : BaseAchievement
	{
		// Token: 0x0600285B RID: 10331 RVA: 0x000AC0EA File Offset: 0x000AA2EA
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.Check;
		}

		// Token: 0x0600285C RID: 10332 RVA: 0x000AC103 File Offset: 0x000AA303
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600285D RID: 10333 RVA: 0x000AC11C File Offset: 0x000AA31C
		private void Check()
		{
			NetworkUser networkUser = NetworkUser.readOnlyLocalPlayersList.FirstOrDefault((NetworkUser v) => v.localUser == base.localUser);
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

		// Token: 0x04002512 RID: 9490
		private const float requirement = 1800f;
	}
}
