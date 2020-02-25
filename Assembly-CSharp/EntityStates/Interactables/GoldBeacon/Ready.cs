using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Interactables.GoldBeacon
{
	// Token: 0x02000813 RID: 2067
	public class Ready : GoldBeaconBaseState
	{
		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06002EE8 RID: 12008 RVA: 0x000C7AA1 File Offset: 0x000C5CA1
		// (set) Token: 0x06002EE9 RID: 12009 RVA: 0x000C7AA8 File Offset: 0x000C5CA8
		public static int count { get; private set; }

		// Token: 0x06002EEA RID: 12010 RVA: 0x000C7AB0 File Offset: 0x000C5CB0
		public override void OnEnter()
		{
			base.OnEnter();
			base.SetReady(true);
			Ready.count++;
		}

		// Token: 0x06002EEB RID: 12011 RVA: 0x000C7ACC File Offset: 0x000C5CCC
		public override void OnExit()
		{
			Ready.count--;
			if (!this.outer.destroying)
			{
				EffectManager.SpawnEffect(Ready.activationEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = 10f
				}, false);
			}
			base.OnExit();
		}

		// Token: 0x04002C2E RID: 11310
		public static GameObject activationEffectPrefab;
	}
}
