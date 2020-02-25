using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianMonster
{
	// Token: 0x020007F5 RID: 2037
	public class LeapAttackState : BaseState
	{
		// Token: 0x06002E54 RID: 11860 RVA: 0x000C5244 File Offset: 0x000C3444
		public override void OnEnter()
		{
			base.OnEnter();
			this.rootMotionAccumulator = base.GetModelRootMotionAccumulator();
			this.modelAnimator = base.GetModelAnimator();
			this.duration = LeapAttackState.baseDuration / this.attackSpeedStat;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = LeapAttackState.damage;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "LeapAttack");
			}
			base.PlayCrossfade("Body", "LeapAttack", "LeapAttack.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x06002E55 RID: 11861 RVA: 0x000C5340 File Offset: 0x000C3540
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
				this.attack.forceVector = (base.characterDirection ? (base.characterDirection.forward * LeapAttackState.forceMagnitude) : Vector3.zero);
				if (this.modelAnimator && this.modelAnimator.GetFloat("LeapAttack.hitBoxActive") > 0.5f)
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

		// Token: 0x06002E56 RID: 11862 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002B7C RID: 11132
		public static float baseDuration = 3.5f;

		// Token: 0x04002B7D RID: 11133
		public static float damage = 10f;

		// Token: 0x04002B7E RID: 11134
		public static float forceMagnitude = 16f;

		// Token: 0x04002B7F RID: 11135
		private OverlapAttack attack;

		// Token: 0x04002B80 RID: 11136
		private Animator modelAnimator;

		// Token: 0x04002B81 RID: 11137
		private RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04002B82 RID: 11138
		private float duration;
	}
}
