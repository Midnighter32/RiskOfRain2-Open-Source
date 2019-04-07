using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleMonster
{
	// Token: 0x020001C9 RID: 457
	public class HeadbuttState : BaseState
	{
		// Token: 0x060008F1 RID: 2289 RVA: 0x0002D1C4 File Offset: 0x0002B3C4
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

		// Token: 0x060008F2 RID: 2290 RVA: 0x0002D2E8 File Offset: 0x0002B4E8
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

		// Token: 0x060008F3 RID: 2291 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000C27 RID: 3111
		public static float baseDuration = 3.5f;

		// Token: 0x04000C28 RID: 3112
		public static float damageCoefficient;

		// Token: 0x04000C29 RID: 3113
		public static float forceMagnitude = 16f;

		// Token: 0x04000C2A RID: 3114
		public static GameObject hitEffectPrefab;

		// Token: 0x04000C2B RID: 3115
		public static string attackSoundString;

		// Token: 0x04000C2C RID: 3116
		private OverlapAttack attack;

		// Token: 0x04000C2D RID: 3117
		private Animator modelAnimator;

		// Token: 0x04000C2E RID: 3118
		private RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04000C2F RID: 3119
		private float duration;
	}
}
