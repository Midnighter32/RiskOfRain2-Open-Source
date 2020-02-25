using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Merc
{
	// Token: 0x020007C8 RID: 1992
	public class WhirlwindBase : BaseState
	{
		// Token: 0x06002D77 RID: 11639 RVA: 0x000C0C58 File Offset: 0x000BEE58
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.overlapAttack = base.InitMeleeOverlap(this.baseDamageCoefficient, WhirlwindBase.hitEffectPrefab, base.GetModelTransform(), this.hitboxString);
			if (base.characterDirection && base.inputBank)
			{
				base.characterDirection.forward = base.inputBank.aimDirection;
			}
			base.SmallHop(base.characterMotor, this.smallHopVelocity);
			this.PlayAnim();
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void PlayAnim()
		{
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x000C0CF8 File Offset: 0x000BEEF8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (base.isAuthority)
			{
				if (this.animator.GetFloat("Sword.active") > (float)this.swingCount)
				{
					this.swingCount++;
					this.overlapAttack.ResetIgnoredHealthComponents();
					Util.PlayScaledSound(WhirlwindBase.attackSoundString, base.gameObject, WhirlwindBase.slashPitch);
					EffectManager.SimpleMuzzleFlash(WhirlwindBase.swingEffectPrefab, base.gameObject, this.slashChildName, true);
					if (base.healthComponent)
					{
						base.healthComponent.TakeDamageForce(this.selfForceMagnitude * base.characterDirection.forward, true, false);
					}
				}
				if (base.FireMeleeOverlap(this.overlapAttack, this.animator, "Sword.active", 0f, true))
				{
					Util.PlaySound(WhirlwindBase.hitSoundString, base.gameObject);
					if (!this.isInHitPause)
					{
						this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Whirlwind.playbackRate");
						this.hitPauseTimer = WhirlwindBase.hitPauseDuration / this.attackSpeedStat;
						this.isInHitPause = true;
					}
				}
				if (this.hitPauseTimer <= 0f && this.isInHitPause)
				{
					base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
					this.isInHitPause = false;
				}
				if (!this.isInHitPause)
				{
					if (base.characterMotor && base.characterDirection)
					{
						Vector3 velocity = base.characterDirection.forward * this.moveSpeedStat * Mathf.Lerp(this.moveSpeedBonusCoefficient, 1f, base.age / this.duration);
						velocity.y = base.characterMotor.velocity.y;
						base.characterMotor.velocity = velocity;
					}
				}
				else
				{
					base.characterMotor.velocity = Vector3.zero;
					this.hitPauseTimer -= Time.fixedDeltaTime;
					this.animator.SetFloat("Whirlwind.playbackRate", 0f);
				}
				if (base.fixedAge >= this.duration)
				{
					while (this.swingCount < 2)
					{
						this.swingCount++;
						this.overlapAttack.Fire(null);
					}
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x04002A06 RID: 10758
		public static GameObject swingEffectPrefab;

		// Token: 0x04002A07 RID: 10759
		public static GameObject hitEffectPrefab;

		// Token: 0x04002A08 RID: 10760
		public static string attackSoundString;

		// Token: 0x04002A09 RID: 10761
		public static string hitSoundString;

		// Token: 0x04002A0A RID: 10762
		public static float slashPitch;

		// Token: 0x04002A0B RID: 10763
		public static float hitPauseDuration;

		// Token: 0x04002A0C RID: 10764
		[SerializeField]
		public float baseDuration;

		// Token: 0x04002A0D RID: 10765
		[SerializeField]
		public float baseDamageCoefficient;

		// Token: 0x04002A0E RID: 10766
		[SerializeField]
		public string slashChildName;

		// Token: 0x04002A0F RID: 10767
		[SerializeField]
		public float selfForceMagnitude;

		// Token: 0x04002A10 RID: 10768
		[SerializeField]
		public float moveSpeedBonusCoefficient;

		// Token: 0x04002A11 RID: 10769
		[SerializeField]
		public float smallHopVelocity;

		// Token: 0x04002A12 RID: 10770
		[SerializeField]
		public string hitboxString;

		// Token: 0x04002A13 RID: 10771
		protected Animator animator;

		// Token: 0x04002A14 RID: 10772
		protected float duration;

		// Token: 0x04002A15 RID: 10773
		protected float hitInterval;

		// Token: 0x04002A16 RID: 10774
		protected int swingCount;

		// Token: 0x04002A17 RID: 10775
		protected float hitPauseTimer;

		// Token: 0x04002A18 RID: 10776
		protected bool isInHitPause;

		// Token: 0x04002A19 RID: 10777
		protected OverlapAttack overlapAttack;

		// Token: 0x04002A1A RID: 10778
		protected BaseState.HitStopCachedState hitStopCachedState;
	}
}
