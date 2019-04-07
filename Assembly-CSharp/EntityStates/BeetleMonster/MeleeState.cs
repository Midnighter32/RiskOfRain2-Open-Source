using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleMonster
{
	// Token: 0x020001CC RID: 460
	public class MeleeState : EntityState
	{
		// Token: 0x060008FC RID: 2300 RVA: 0x0002D494 File Offset: 0x0002B694
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = 10f;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Melee1");
			}
			base.PlayCrossfade("Body", "Melee1", "Melee1.playbackRate", MeleeState.duration, 0.1f);
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x0002D570 File Offset: 0x0002B770
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.hasAuthority)
			{
				this.attack.forceVector = (base.characterDirection ? (base.characterDirection.forward * MeleeState.forceMagnitude) : Vector3.zero);
				if (this.modelAnimator && this.modelAnimator.GetFloat("Melee1.hitBoxActive") > 0.5f)
				{
					this.attack.Fire(null);
				}
			}
			if (base.fixedAge >= MeleeState.duration && base.hasAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000C33 RID: 3123
		public static float duration = 3.5f;

		// Token: 0x04000C34 RID: 3124
		public static float damage = 10f;

		// Token: 0x04000C35 RID: 3125
		public static float forceMagnitude = 10f;

		// Token: 0x04000C36 RID: 3126
		private OverlapAttack attack;

		// Token: 0x04000C37 RID: 3127
		private Animator modelAnimator;
	}
}
