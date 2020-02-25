using System;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000837 RID: 2103
	public class FireSeekingArrow : BaseState
	{
		// Token: 0x06002F97 RID: 12183 RVA: 0x000CBC18 File Offset: 0x000C9E18
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

		// Token: 0x06002F98 RID: 12184 RVA: 0x000CBD2D File Offset: 0x000C9F2D
		public override void OnExit()
		{
			base.OnExit();
			this.FireOrbArrow();
		}

		// Token: 0x06002F99 RID: 12185 RVA: 0x000CBD3C File Offset: 0x000C9F3C
		private void FireOrbArrow()
		{
			if (this.hasFiredArrow || !NetworkServer.active)
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
				EffectManager.SimpleMuzzleFlash(FireSeekingArrow.muzzleflashEffectPrefab, base.gameObject, FireSeekingArrow.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x06002F9A RID: 12186 RVA: 0x000CBE1C File Offset: 0x000CA01C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.animator.GetFloat("FireSeekingShot.fire") > 0f)
			{
				this.FireOrbArrow();
			}
			if (this.stopwatch > this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F9B RID: 12187 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06002F9C RID: 12188 RVA: 0x000CBE80 File Offset: 0x000CA080
		public override void OnSerialize(NetworkWriter writer)
		{
			writer.Write(HurtBoxReference.FromHurtBox(this.initialOrbTarget));
		}

		// Token: 0x06002F9D RID: 12189 RVA: 0x000CBE94 File Offset: 0x000CA094
		public override void OnDeserialize(NetworkReader reader)
		{
			this.initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
		}

		// Token: 0x04002D4B RID: 11595
		public static float orbDamageCoefficient;

		// Token: 0x04002D4C RID: 11596
		public static float orbProcCoefficient;

		// Token: 0x04002D4D RID: 11597
		public static string muzzleString;

		// Token: 0x04002D4E RID: 11598
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002D4F RID: 11599
		public static string attackSoundString;

		// Token: 0x04002D50 RID: 11600
		public static float baseDuration;

		// Token: 0x04002D51 RID: 11601
		private float duration;

		// Token: 0x04002D52 RID: 11602
		private float stopwatch;

		// Token: 0x04002D53 RID: 11603
		private ChildLocator childLocator;

		// Token: 0x04002D54 RID: 11604
		private HuntressTracker huntressTracker;

		// Token: 0x04002D55 RID: 11605
		private Animator animator;

		// Token: 0x04002D56 RID: 11606
		private bool hasFiredArrow;

		// Token: 0x04002D57 RID: 11607
		private HurtBox initialOrbTarget;
	}
}
