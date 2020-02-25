using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpMonster
{
	// Token: 0x0200081F RID: 2079
	public class Backstab : BaseState
	{
		// Token: 0x06002F1F RID: 12063 RVA: 0x000C91FC File Offset: 0x000C73FC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Backstab.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = Backstab.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = Backstab.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Backstab");
			}
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture", "Backstab", "Backstab.playbackRate", this.duration);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x000C9348 File Offset: 0x000C7548
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
			{
				if (!this.hasBit)
				{
					EffectManager.SimpleMuzzleFlash(Backstab.biteEffectPrefab, base.gameObject, "MuzzleMouth", true);
					this.hasBit = true;
				}
				this.attack.forceVector = base.transform.forward * Backstab.forceMagnitude;
				this.attack.Fire(null);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002C94 RID: 11412
		public static float baseDuration = 3.5f;

		// Token: 0x04002C95 RID: 11413
		public static float damageCoefficient = 4f;

		// Token: 0x04002C96 RID: 11414
		public static float forceMagnitude = 16f;

		// Token: 0x04002C97 RID: 11415
		public static float radius = 3f;

		// Token: 0x04002C98 RID: 11416
		public static GameObject hitEffectPrefab;

		// Token: 0x04002C99 RID: 11417
		public static GameObject biteEffectPrefab;

		// Token: 0x04002C9A RID: 11418
		private OverlapAttack attack;

		// Token: 0x04002C9B RID: 11419
		private Animator modelAnimator;

		// Token: 0x04002C9C RID: 11420
		private float duration;

		// Token: 0x04002C9D RID: 11421
		private bool hasBit;
	}
}
