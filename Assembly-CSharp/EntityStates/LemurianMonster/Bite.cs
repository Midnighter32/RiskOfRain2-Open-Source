using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.LemurianMonster
{
	// Token: 0x02000122 RID: 290
	public class Bite : BaseState
	{
		// Token: 0x0600059A RID: 1434 RVA: 0x00019A90 File Offset: 0x00017C90
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

		// Token: 0x0600059B RID: 1435 RVA: 0x00019BF4 File Offset: 0x00017DF4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
			{
				if (!this.hasBit)
				{
					EffectManager.instance.SimpleMuzzleFlash(Bite.biteEffectPrefab, base.gameObject, "MuzzleMouth", true);
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

		// Token: 0x0600059C RID: 1436 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000662 RID: 1634
		public static float baseDuration = 3.5f;

		// Token: 0x04000663 RID: 1635
		public static float damageCoefficient = 4f;

		// Token: 0x04000664 RID: 1636
		public static float forceMagnitude = 16f;

		// Token: 0x04000665 RID: 1637
		public static float radius = 3f;

		// Token: 0x04000666 RID: 1638
		public static GameObject hitEffectPrefab;

		// Token: 0x04000667 RID: 1639
		public static GameObject biteEffectPrefab;

		// Token: 0x04000668 RID: 1640
		public static string attackString;

		// Token: 0x04000669 RID: 1641
		private OverlapAttack attack;

		// Token: 0x0400066A RID: 1642
		private Animator modelAnimator;

		// Token: 0x0400066B RID: 1643
		private float duration;

		// Token: 0x0400066C RID: 1644
		private bool hasBit;
	}
}
