using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleMonster
{
	// Token: 0x020008E4 RID: 2276
	public class HeadbuttState : BaseState
	{
		// Token: 0x060032F1 RID: 13041 RVA: 0x000DCE34 File Offset: 0x000DB034
		public override void OnEnter()
		{
			base.OnEnter();
			this.rootMotionAccumulator = base.GetModelRootMotionAccumulator();
			this.modelAnimator = base.GetModelAnimator();
			this.duration = HeadbuttState.baseDuration / this.attackSpeedStat;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = HeadbuttState.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = HeadbuttState.hitEffectPrefab;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Headbutt");
			}
			Util.PlaySound(HeadbuttState.attackSoundString, base.gameObject);
			base.PlayCrossfade("Body", "Headbutt", "Headbutt.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x060032F2 RID: 13042 RVA: 0x000DCF58 File Offset: 0x000DB158
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.rootMotionAccumulator)
			{
				Vector3 vector = this.rootMotionAccumulator.ExtractRootMotion();
				if (vector != Vector3.zero && base.isAuthority && base.characterMotor)
				{
					base.characterMotor.rootMotion += vector;
				}
			}
			if (base.isAuthority)
			{
				this.attack.forceVector = (base.characterDirection ? (base.characterDirection.forward * HeadbuttState.forceMagnitude) : Vector3.zero);
				if (base.characterDirection && base.inputBank)
				{
					base.characterDirection.moveVector = base.inputBank.aimDirection;
				}
				if (this.modelAnimator && this.modelAnimator.GetFloat("Headbutt.hitBoxActive") > 0.5f)
				{
					this.attack.Fire(null);
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060032F3 RID: 13043 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400324F RID: 12879
		public static float baseDuration = 3.5f;

		// Token: 0x04003250 RID: 12880
		public static float damageCoefficient;

		// Token: 0x04003251 RID: 12881
		public static float forceMagnitude = 16f;

		// Token: 0x04003252 RID: 12882
		public static GameObject hitEffectPrefab;

		// Token: 0x04003253 RID: 12883
		public static string attackSoundString;

		// Token: 0x04003254 RID: 12884
		private OverlapAttack attack;

		// Token: 0x04003255 RID: 12885
		private Animator modelAnimator;

		// Token: 0x04003256 RID: 12886
		private RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04003257 RID: 12887
		private float duration;
	}
}
