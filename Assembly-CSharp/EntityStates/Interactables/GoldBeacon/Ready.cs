using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Interactables.GoldBeacon
{
	// Token: 0x0200013C RID: 316
	public class Ready : GoldBeaconBaseState
	{
		// Token: 0x0600060A RID: 1546 RVA: 0x0001BD12 File Offset: 0x00019F12
		public override void OnEnter()
		{
			base.OnEnter();
			base.SetReady(true);
			GoldshoresMissionController.instance.beaconsActive++;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x0001BD34 File Offset: 0x00019F34
		public override void OnExit()
		{
			base.OnExit();
			GoldshoresMissionController.instance.beaconsActive--;
			EffectManager.instance.SpawnEffect(Ready.activationEffectPrefab, new EffectData
			{
				origin = base.transform.position,
				scale = 10f
			}, true);
		}

		// Token: 0x04000707 RID: 1799
		public static GameObject activationEffectPrefab;
	}
}
