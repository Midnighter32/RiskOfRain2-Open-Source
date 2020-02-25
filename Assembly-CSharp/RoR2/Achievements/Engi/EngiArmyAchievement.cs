using System;
using UnityEngine.Networking;

namespace RoR2.Achievements.Engi
{
	// Token: 0x020006EB RID: 1771
	[RegisterAchievement("EngiArmy", "Skills.Engi.WalkerTurret", "Complete30StagesCareer", null)]
	public class EngiArmyAchievement : BaseAchievement
	{
		// Token: 0x0600292C RID: 10540 RVA: 0x000AD916 File Offset: 0x000ABB16
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("EngiBody");
		}

		// Token: 0x0600292D RID: 10541 RVA: 0x000AD922 File Offset: 0x000ABB22
		private void SubscribeToMinionChanges()
		{
			MinionOwnership.onMinionGroupChangedGlobal += this.OnMinionGroupChangedGlobal;
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x000AD935 File Offset: 0x000ABB35
		private void UnsubscribeFromMinionChanges()
		{
			MinionOwnership.onMinionGroupChangedGlobal -= this.OnMinionGroupChangedGlobal;
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x000AD948 File Offset: 0x000ABB48
		private void OnMinionGroupChangedGlobal(MinionOwnership minion)
		{
			int num = EngiArmyAchievement.requirement;
			MinionOwnership.MinionGroup group = minion.group;
			if (num <= ((group != null) ? group.memberCount : 0))
			{
				CharacterMaster master = base.localUser.cachedMasterController.master;
				if (!master)
				{
					return;
				}
				NetworkInstanceId netId = master.netId;
				if (minion.group.ownerId == netId)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x000AD9A8 File Offset: 0x000ABBA8
		public override void OnInstall()
		{
			base.OnInstall();
			this.monitorMinions = new ToggleAction(new Action(this.SubscribeToMinionChanges), new Action(this.UnsubscribeFromMinionChanges));
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x000AD9D3 File Offset: 0x000ABBD3
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			this.monitorMinions.SetActive(true);
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x000AD9E7 File Offset: 0x000ABBE7
		protected override void OnBodyRequirementBroken()
		{
			this.monitorMinions.SetActive(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x04002549 RID: 9545
		private static readonly int requirement = 12;

		// Token: 0x0400254A RID: 9546
		private ToggleAction monitorMinions;
	}
}
