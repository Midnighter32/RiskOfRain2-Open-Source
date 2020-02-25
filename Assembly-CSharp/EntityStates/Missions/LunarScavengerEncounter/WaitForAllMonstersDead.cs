using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.Missions.LunarScavengerEncounter
{
	// Token: 0x020007B6 RID: 1974
	public class WaitForAllMonstersDead : BaseState
	{
		// Token: 0x06002D1E RID: 11550 RVA: 0x000BE890 File Offset: 0x000BCA90
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06002D1F RID: 11551 RVA: 0x000BE8A5 File Offset: 0x000BCAA5
		private void FixedUpdateServer()
		{
			if (TeamComponent.GetTeamMembers(TeamIndex.Monster).Count == 0)
			{
				this.outer.SetNextState(new FadeOut());
			}
		}
	}
}
