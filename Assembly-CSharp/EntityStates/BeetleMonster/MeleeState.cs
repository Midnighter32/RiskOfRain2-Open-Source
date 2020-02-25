using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleMonster
{
	// Token: 0x020008E7 RID: 2279
	public class MeleeState : EntityState
	{
		// Token: 0x060032FC RID: 13052 RVA: 0x000DD104 File Offset: 0x000DB304
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

		// Token: 0x060032FD RID: 13053 RVA: 0x000DD1E0 File Offset: 0x000DB3E0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.attack.forceVector = (base.characterDirection ? (base.characterDirection.forward * MeleeState.forceMagnitude) : Vector3.zero);
				if (this.modelAnimator && this.modelAnimator.GetFloat("Melee1.hitBoxActive") > 0.5f)
				{
					this.attack.Fire(null);
				}
			}
			if (base.fixedAge >= MeleeState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060032FE RID: 13054 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400325B RID: 12891
		public static float duration = 3.5f;

		// Token: 0x0400325C RID: 12892
		public static float damage = 10f;

		// Token: 0x0400325D RID: 12893
		public static float forceMagnitude = 10f;

		// Token: 0x0400325E RID: 12894
		private OverlapAttack attack;

		// Token: 0x0400325F RID: 12895
		private Animator modelAnimator;
	}
}
