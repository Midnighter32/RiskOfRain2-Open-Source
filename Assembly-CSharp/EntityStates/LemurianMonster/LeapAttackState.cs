using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianMonster
{
	// Token: 0x02000126 RID: 294
	public class LeapAttackState : BaseState
	{
		// Token: 0x060005AE RID: 1454 RVA: 0x00019F54 File Offset: 0x00018154
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

		// Token: 0x060005AF RID: 1455 RVA: 0x0001A050 File Offset: 0x00018250
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

		// Token: 0x060005B0 RID: 1456 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400067B RID: 1659
		public static float baseDuration = 3.5f;

		// Token: 0x0400067C RID: 1660
		public static float damage = 10f;

		// Token: 0x0400067D RID: 1661
		public static float forceMagnitude = 16f;

		// Token: 0x0400067E RID: 1662
		private OverlapAttack attack;

		// Token: 0x0400067F RID: 1663
		private Animator modelAnimator;

		// Token: 0x04000680 RID: 1664
		private RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04000681 RID: 1665
		private float duration;
	}
}
