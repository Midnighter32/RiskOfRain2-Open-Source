using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x02000880 RID: 2176
	public class Detonate : BaseMineState
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x060030F6 RID: 12534 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x060030F7 RID: 12535 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldRevertToWaitForStickOnSurfaceLost
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060030F8 RID: 12536 RVA: 0x000D274C File Offset: 0x000D094C
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				this.Explode();
			}
		}

		// Token: 0x060030F9 RID: 12537 RVA: 0x000D2764 File Offset: 0x000D0964
		private void Explode()
		{
			ProjectileDamage component = base.GetComponent<ProjectileDamage>();
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			EntityStateMachine armingStateMachine = base.armingStateMachine;
			BaseMineArmingState baseMineArmingState;
			if ((baseMineArmingState = (((armingStateMachine != null) ? armingStateMachine.state : null) as BaseMineArmingState)) != null)
			{
				num = baseMineArmingState.damageScale;
				num2 = baseMineArmingState.forceScale;
				num3 = baseMineArmingState.blastRadiusScale;
			}
			float num4 = Detonate.blastRadius * num3;
			new BlastAttack
			{
				procChainMask = base.projectileController.procChainMask,
				procCoefficient = base.projectileController.procCoefficient,
				attacker = base.projectileController.owner,
				inflictor = base.gameObject,
				teamIndex = base.projectileController.teamFilter.teamIndex,
				procCoefficient = base.projectileController.procCoefficient,
				baseDamage = component.damage * num,
				baseForce = component.force * num2,
				falloffModel = BlastAttack.FalloffModel.None,
				crit = component.crit,
				radius = num4,
				position = base.transform.position,
				damageColorIndex = component.damageColorIndex
			}.Fire();
			if (Detonate.explosionEffectPrefab)
			{
				EffectManager.SpawnEffect(Detonate.explosionEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					rotation = base.transform.rotation,
					scale = num4
				}, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x04002F25 RID: 12069
		public static float blastRadius;

		// Token: 0x04002F26 RID: 12070
		public static GameObject explosionEffectPrefab;
	}
}
