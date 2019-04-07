using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000143 RID: 323
	public class Backstab : BaseState
	{
		// Token: 0x06000632 RID: 1586 RVA: 0x0001CE70 File Offset: 0x0001B070
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

		// Token: 0x06000633 RID: 1587 RVA: 0x0001CFBC File Offset: 0x0001B1BC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
			{
				if (!this.hasBit)
				{
					EffectManager.instance.SimpleMuzzleFlash(Backstab.biteEffectPrefab, base.gameObject, "MuzzleMouth", true);
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

		// Token: 0x06000634 RID: 1588 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000754 RID: 1876
		public static float baseDuration = 3.5f;

		// Token: 0x04000755 RID: 1877
		public static float damageCoefficient = 4f;

		// Token: 0x04000756 RID: 1878
		public static float forceMagnitude = 16f;

		// Token: 0x04000757 RID: 1879
		public static float radius = 3f;

		// Token: 0x04000758 RID: 1880
		public static GameObject hitEffectPrefab;

		// Token: 0x04000759 RID: 1881
		public static GameObject biteEffectPrefab;

		// Token: 0x0400075A RID: 1882
		private OverlapAttack attack;

		// Token: 0x0400075B RID: 1883
		private Animator modelAnimator;

		// Token: 0x0400075C RID: 1884
		private float duration;

		// Token: 0x0400075D RID: 1885
		private bool hasBit;
	}
}
