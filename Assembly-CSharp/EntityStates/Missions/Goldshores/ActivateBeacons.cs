using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.Missions.Goldshores
{
	// Token: 0x020007B8 RID: 1976
	public class ActivateBeacons : EntityState
	{
		// Token: 0x06002D28 RID: 11560 RVA: 0x000BEB15 File Offset: 0x000BCD15
		public override void OnEnter()
		{
			base.OnEnter();
			if (GoldshoresMissionController.instance)
			{
				GoldshoresMissionController.instance.SpawnBeacons();
			}
		}

		// Token: 0x06002D29 RID: 11561 RVA: 0x000BEB33 File Offset: 0x000BCD33
		public override void OnExit()
		{
			base.OnExit();
			if (!this.outer.destroying && GoldshoresMissionController.instance)
			{
				GoldshoresMissionController.instance.BeginTransitionIntoBossfight();
			}
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x000BEB60 File Offset: 0x000BCD60
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && GoldshoresMissionController.instance && GoldshoresMissionController.instance.beaconsActive >= GoldshoresMissionController.instance.beaconsRequiredToSpawnBoss)
			{
				this.outer.SetNextState(new GoldshoresBossfight());
			}
		}
	}
}
