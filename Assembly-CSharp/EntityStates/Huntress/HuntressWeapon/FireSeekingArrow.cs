using System;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000155 RID: 341
	internal class FireSeekingArrow : BaseState
	{
		// Token: 0x06000696 RID: 1686 RVA: 0x0001F55C File Offset: 0x0001D75C
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(FireSeekingArrow.attackSoundString, base.gameObject, this.attackSpeedStat);
			this.huntressTracker = base.GetComponent<HuntressTracker>();
			if (this.huntressTracker && base.isAuthority)
			{
				this.initialOrbTarget = this.huntressTracker.GetTrackingTarget();
			}
			this.duration = FireSeekingArrow.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", this.duration, this.duration * 0.2f / this.attackSpeedStat);
			base.PlayCrossfade("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", this.duration, this.duration * 0.2f / this.attackSpeedStat);
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				this.animator = modelTransform.GetComponent<Animator>();
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration + 1f);
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0001F671 File Offset: 0x0001D871
		public override void OnExit()
		{
			base.OnExit();
			if (!this.hasFiredArrow)
			{
				this.FireOrbArrow();
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0001F688 File Offset: 0x0001D888
		private void FireOrbArrow()
		{
			if (!NetworkServer.active || this.hasFiredArrow)
			{
				return;
			}
			this.hasFiredArrow = true;
			ArrowOrb arrowOrb = new ArrowOrb();
			arrowOrb.damageValue = base.characterBody.damage * FireSeekingArrow.orbDamageCoefficient;
			arrowOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
			arrowOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
			arrowOrb.attacker = base.gameObject;
			arrowOrb.procCoefficient = FireSeekingArrow.orbProcCoefficient;
			HurtBox hurtBox = this.initialOrbTarget;
			if (hurtBox)
			{
				Transform transform = this.childLocator.FindChild(FireSeekingArrow.muzzleString);
				EffectManager.instance.SimpleMuzzleFlash(FireSeekingArrow.muzzleflashEffectPrefab, base.gameObject, FireSeekingArrow.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0001F76C File Offset: 0x0001D96C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.animator.GetFloat("FireSeekingShot.fire") > 0f && !this.hasFiredArrow)
			{
				this.FireOrbArrow();
			}
			if (this.stopwatch > this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0001F7D8 File Offset: 0x0001D9D8
		public override void OnSerialize(NetworkWriter writer)
		{
			writer.Write(HurtBoxReference.FromHurtBox(this.initialOrbTarget));
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0001F7EC File Offset: 0x0001D9EC
		public override void OnDeserialize(NetworkReader reader)
		{
			this.initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
		}

		// Token: 0x04000805 RID: 2053
		public static float orbDamageCoefficient;

		// Token: 0x04000806 RID: 2054
		public static float orbProcCoefficient;

		// Token: 0x04000807 RID: 2055
		public static string muzzleString;

		// Token: 0x04000808 RID: 2056
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04000809 RID: 2057
		public static string attackSoundString;

		// Token: 0x0400080A RID: 2058
		public static float baseDuration;

		// Token: 0x0400080B RID: 2059
		private float duration;

		// Token: 0x0400080C RID: 2060
		private float stopwatch;

		// Token: 0x0400080D RID: 2061
		private ChildLocator childLocator;

		// Token: 0x0400080E RID: 2062
		private HuntressTracker huntressTracker;

		// Token: 0x0400080F RID: 2063
		private Animator animator;

		// Token: 0x04000810 RID: 2064
		private bool hasFiredArrow;

		// Token: 0x04000811 RID: 2065
		private HurtBox initialOrbTarget;
	}
}
