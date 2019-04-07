using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E3 RID: 227
	public class DroneProjectileHoverStun : DroneProjectileHover
	{
		// Token: 0x0600046F RID: 1135 RVA: 0x000129B4 File Offset: 0x00010BB4
		protected override void Pulse()
		{
			BlastAttack blastAttack = new BlastAttack
			{
				baseDamage = 0f,
				baseForce = 0f,
				attacker = (this.projectileController ? this.projectileController.owner : null),
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
				teamIndex = (this.projectileController ? this.projectileController.teamFilter.teamIndex : TeamIndex.None),
				radius = DroneProjectileHover.pulseRadius
			};
			blastAttack.Fire();
			EffectData effectData = new EffectData();
			effectData.origin = blastAttack.position;
			effectData.scale = blastAttack.radius;
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionVFX"), effectData, true);
		}
	}
}
