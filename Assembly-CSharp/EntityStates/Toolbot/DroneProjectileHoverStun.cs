using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x0200076A RID: 1898
	public class DroneProjectileHoverStun : DroneProjectileHover
	{
		// Token: 0x06002BC9 RID: 11209 RVA: 0x000B92A8 File Offset: 0x000B74A8
		protected override void Pulse()
		{
			BlastAttack blastAttack = new BlastAttack
			{
				baseDamage = 0f,
				baseForce = 0f,
				attacker = (base.projectileController ? base.projectileController.owner : null),
				inflictor = base.gameObject,
				bonusForce = Vector3.zero,
				canHurtAttacker = false,
				crit = false,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Stun1s,
				falloffModel = BlastAttack.FalloffModel.None,
				procChainMask = default(ProcChainMask),
				position = base.transform.position,
				procCoefficient = 0f,
				teamIndex = (base.projectileController ? base.projectileController.teamFilter.teamIndex : TeamIndex.None),
				radius = this.pulseRadius
			};
			blastAttack.Fire();
			EffectData effectData = new EffectData();
			effectData.origin = blastAttack.position;
			effectData.scale = blastAttack.radius;
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionVFX"), effectData, true);
		}
	}
}
