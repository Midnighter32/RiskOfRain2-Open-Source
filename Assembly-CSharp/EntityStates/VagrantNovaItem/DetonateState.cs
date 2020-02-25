using System;
using EntityStates.VagrantMonster;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.VagrantNovaItem
{
	// Token: 0x02000743 RID: 1859
	public class DetonateState : BaseVagrantNovaItemState
	{
		// Token: 0x06002B1E RID: 11038 RVA: 0x000B5810 File Offset: 0x000B3A10
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = DetonateState.baseDuration;
			if (NetworkServer.active && base.attachedBody)
			{
				new BlastAttack
				{
					attacker = base.attachedBody.gameObject,
					baseDamage = base.attachedBody.damage * DetonateState.blastDamageCoefficient,
					baseForce = DetonateState.blastForce,
					bonusForce = Vector3.zero,
					canHurtAttacker = false,
					crit = base.attachedBody.RollCrit(),
					damageColorIndex = DamageColorIndex.Item,
					damageType = DamageType.Generic,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = base.gameObject,
					position = base.attachedBody.corePosition,
					procChainMask = default(ProcChainMask),
					procCoefficient = DetonateState.blastProcCoefficient,
					radius = DetonateState.blastRadius,
					losType = BlastAttack.LoSType.NearestHit,
					teamIndex = base.attachedBody.teamComponent.teamIndex
				}.Fire();
				EffectData effectData = new EffectData();
				effectData.origin = base.attachedBody.corePosition;
				effectData.SetHurtBoxReference(base.attachedBody.mainHurtBox);
				EffectManager.SpawnEffect(FireMegaNova.novaEffectPrefab, effectData, true);
			}
			base.SetChargeSparkEmissionRateMultiplier(0f);
			Util.PlaySound(DetonateState.explosionSound, base.gameObject);
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000B596A File Offset: 0x000B3B6A
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(new RechargeState());
			}
		}

		// Token: 0x040026F4 RID: 9972
		public static float blastRadius;

		// Token: 0x040026F5 RID: 9973
		public static float blastDamageCoefficient;

		// Token: 0x040026F6 RID: 9974
		public static float blastProcCoefficient;

		// Token: 0x040026F7 RID: 9975
		public static float blastForce;

		// Token: 0x040026F8 RID: 9976
		public static float baseDuration;

		// Token: 0x040026F9 RID: 9977
		public static string explosionSound;

		// Token: 0x040026FA RID: 9978
		private float duration;
	}
}
