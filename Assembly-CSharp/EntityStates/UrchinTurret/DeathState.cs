using System;
using RoR2;
using UnityEngine;

namespace EntityStates.UrchinTurret
{
	// Token: 0x02000903 RID: 2307
	public class DeathState : BaseState
	{
		// Token: 0x06003383 RID: 13187 RVA: 0x000DFB94 File Offset: 0x000DDD94
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(DeathState.deathString, base.gameObject);
			Transform transform = base.FindModelChild("Muzzle");
			if (base.isAuthority)
			{
				if (DeathState.initialExplosion)
				{
					EffectManager.SpawnEffect(DeathState.initialExplosion, new EffectData
					{
						origin = transform.position,
						scale = DeathState.effectScale
					}, true);
				}
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x06003384 RID: 13188 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04003304 RID: 13060
		public static GameObject initialExplosion;

		// Token: 0x04003305 RID: 13061
		public static float effectScale;

		// Token: 0x04003306 RID: 13062
		public static string deathString;
	}
}
