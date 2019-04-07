using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.Missions.Goldshores
{
	// Token: 0x02000101 RID: 257
	public class ActivateBeacons : EntityState
	{
		// Token: 0x060004F8 RID: 1272 RVA: 0x00014FC4 File Offset: 0x000131C4
		public override void OnEnter()
		{
			base.OnEnter();
			GoldshoresMissionController.instance.SpawnBeacons();
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x00014FD6 File Offset: 0x000131D6
		public override void OnExit()
		{
			base.OnExit();
			GoldshoresMissionController.instance.BeginTransitionIntoBossfight();
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00014FE8 File Offset: 0x000131E8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && GoldshoresMissionController.instance.beaconsActive >= GoldshoresMissionController.instance.beaconsRequiredToSpawnBoss)
			{
				this.outer.SetNextState(new GoldshoresBossfight());
			}
		}
	}
}
