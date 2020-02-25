using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x02000872 RID: 2162
	public class Detonate : BaseSpiderMineState
	{
		// Token: 0x060030BD RID: 12477 RVA: 0x000D1D84 File Offset: 0x000CFF84
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				ProjectileDamage component = base.GetComponent<ProjectileDamage>();
				Vector3 position = base.transform.position;
				new BlastAttack
				{
					position = position,
					attacker = base.projectileController.owner,
					baseDamage = component.damage,
					baseForce = component.force,
					bonusForce = Vector3.zero,
					canHurtAttacker = false,
					crit = component.crit,
					damageColorIndex = component.damageColorIndex,
					damageType = component.damageType,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = base.gameObject,
					procChainMask = base.projectileController.procChainMask,
					radius = Detonate.blastRadius,
					teamIndex = base.projectileController.teamFilter.teamIndex,
					procCoefficient = base.projectileController.procCoefficient
				}.Fire();
				EffectManager.SpawnEffect(Detonate.blastEffectPrefab, new EffectData
				{
					origin = position,
					scale = Detonate.blastRadius
				}, true);
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x060030BE RID: 12478 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002F05 RID: 12037
		public static float blastRadius;

		// Token: 0x04002F06 RID: 12038
		public static GameObject blastEffectPrefab;
	}
}
