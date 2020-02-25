using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.LemurianMonster
{
	// Token: 0x020007F1 RID: 2033
	public class Bite : BaseState
	{
		// Token: 0x06002E40 RID: 11840 RVA: 0x000C4D8C File Offset: 0x000C2F8C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Bite.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = Bite.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = Bite.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			Util.PlayScaledSound(Bite.attackString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Bite");
			}
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture", "Bite", "Bite.playbackRate", this.duration);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x06002E41 RID: 11841 RVA: 0x000C4EF0 File Offset: 0x000C30F0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
			{
				if (!this.hasBit)
				{
					EffectManager.SimpleMuzzleFlash(Bite.biteEffectPrefab, base.gameObject, "MuzzleMouth", true);
					this.hasBit = true;
				}
				this.attack.forceVector = base.transform.forward * Bite.forceMagnitude;
				this.attack.Fire(null);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002E42 RID: 11842 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002B63 RID: 11107
		public static float baseDuration = 3.5f;

		// Token: 0x04002B64 RID: 11108
		public static float damageCoefficient = 4f;

		// Token: 0x04002B65 RID: 11109
		public static float forceMagnitude = 16f;

		// Token: 0x04002B66 RID: 11110
		public static float radius = 3f;

		// Token: 0x04002B67 RID: 11111
		public static GameObject hitEffectPrefab;

		// Token: 0x04002B68 RID: 11112
		public static GameObject biteEffectPrefab;

		// Token: 0x04002B69 RID: 11113
		public static string attackString;

		// Token: 0x04002B6A RID: 11114
		private OverlapAttack attack;

		// Token: 0x04002B6B RID: 11115
		private Animator modelAnimator;

		// Token: 0x04002B6C RID: 11116
		private float duration;

		// Token: 0x04002B6D RID: 11117
		private bool hasBit;
	}
}
