using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Merc
{
	// Token: 0x020007C7 RID: 1991
	public class Uppercut : BaseState
	{
		// Token: 0x06002D72 RID: 11634 RVA: 0x000C0914 File Offset: 0x000BEB14
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.duration = Uppercut.baseDuration / this.attackSpeedStat;
			this.overlapAttack = base.InitMeleeOverlap(Uppercut.baseDamageCoefficient, Uppercut.hitEffectPrefab, base.GetModelTransform(), Uppercut.hitboxString);
			this.overlapAttack.forceVector = Vector3.up * Uppercut.upwardForceStrength;
			if (base.characterDirection && base.inputBank)
			{
				base.characterDirection.forward = base.inputBank.aimDirection;
			}
			Util.PlaySound(Uppercut.enterSoundString, base.gameObject);
			this.PlayAnim();
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x000C09C7 File Offset: 0x000BEBC7
		protected virtual void PlayAnim()
		{
			base.PlayCrossfade("FullBody, Override", "Uppercut", "Uppercut.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x000C09E9 File Offset: 0x000BEBE9
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("FullBody, Override", "UppercutExit");
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x000C0A04 File Offset: 0x000BEC04
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (base.isAuthority)
			{
				if (this.animator.GetFloat("Sword.active") > 0.2f && !this.hasSwung)
				{
					this.hasSwung = true;
					base.characterMotor.Motor.ForceUnground();
					Util.PlayScaledSound(Uppercut.attackSoundString, base.gameObject, Uppercut.slashPitch);
					EffectManager.SimpleMuzzleFlash(Uppercut.swingEffectPrefab, base.gameObject, Uppercut.slashChildName, true);
				}
				if (base.FireMeleeOverlap(this.overlapAttack, this.animator, "Sword.active", 0f, false))
				{
					Util.PlaySound(Uppercut.hitSoundString, base.gameObject);
					if (!this.isInHitPause)
					{
						this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Uppercut.playbackRate");
						this.hitPauseTimer = Uppercut.hitPauseDuration / this.attackSpeedStat;
						this.isInHitPause = true;
					}
				}
				if (this.hitPauseTimer <= 0f && this.isInHitPause)
				{
					base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
					base.characterMotor.Motor.ForceUnground();
					this.isInHitPause = false;
				}
				if (!this.isInHitPause)
				{
					if (base.characterMotor && base.characterDirection)
					{
						Vector3 velocity = base.characterDirection.forward * this.moveSpeedStat * Mathf.Lerp(Uppercut.moveSpeedBonusCoefficient, 0f, base.age / this.duration);
						velocity.y = Uppercut.yVelocityCurve.Evaluate(base.fixedAge / this.duration);
						base.characterMotor.velocity = velocity;
					}
				}
				else
				{
					base.fixedAge -= Time.fixedDeltaTime;
					base.characterMotor.velocity = Vector3.zero;
					this.hitPauseTimer -= Time.fixedDeltaTime;
					this.animator.SetFloat("Uppercut.playbackRate", 0f);
				}
				if (base.fixedAge >= this.duration)
				{
					if (this.hasSwung)
					{
						this.hasSwung = true;
						this.overlapAttack.Fire(null);
					}
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x040029F0 RID: 10736
		public static GameObject swingEffectPrefab;

		// Token: 0x040029F1 RID: 10737
		public static GameObject hitEffectPrefab;

		// Token: 0x040029F2 RID: 10738
		public static string enterSoundString;

		// Token: 0x040029F3 RID: 10739
		public static string attackSoundString;

		// Token: 0x040029F4 RID: 10740
		public static string hitSoundString;

		// Token: 0x040029F5 RID: 10741
		public static float slashPitch;

		// Token: 0x040029F6 RID: 10742
		public static float hitPauseDuration;

		// Token: 0x040029F7 RID: 10743
		public static float upwardForceStrength;

		// Token: 0x040029F8 RID: 10744
		public static float baseDuration;

		// Token: 0x040029F9 RID: 10745
		public static float baseDamageCoefficient;

		// Token: 0x040029FA RID: 10746
		public static string slashChildName;

		// Token: 0x040029FB RID: 10747
		public static float moveSpeedBonusCoefficient;

		// Token: 0x040029FC RID: 10748
		public static string hitboxString;

		// Token: 0x040029FD RID: 10749
		public static AnimationCurve yVelocityCurve;

		// Token: 0x040029FE RID: 10750
		protected Animator animator;

		// Token: 0x040029FF RID: 10751
		protected float duration;

		// Token: 0x04002A00 RID: 10752
		protected float hitInterval;

		// Token: 0x04002A01 RID: 10753
		protected bool hasSwung;

		// Token: 0x04002A02 RID: 10754
		protected float hitPauseTimer;

		// Token: 0x04002A03 RID: 10755
		protected bool isInHitPause;

		// Token: 0x04002A04 RID: 10756
		protected OverlapAttack overlapAttack;

		// Token: 0x04002A05 RID: 10757
		protected BaseState.HitStopCachedState hitStopCachedState;
	}
}
