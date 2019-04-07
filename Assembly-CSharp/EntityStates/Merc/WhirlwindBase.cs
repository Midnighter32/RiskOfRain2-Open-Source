using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Merc
{
	// Token: 0x0200010A RID: 266
	public class WhirlwindBase : BaseState
	{
		// Token: 0x06000522 RID: 1314 RVA: 0x00016930 File Offset: 0x00014B30
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

		// Token: 0x06000523 RID: 1315 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void PlayAnim()
		{
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x000169D0 File Offset: 0x00014BD0
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
					EffectManager.instance.SimpleMuzzleFlash(WhirlwindBase.swingEffectPrefab, base.gameObject, this.slashChildName, true);
					if (base.healthComponent)
					{
						base.healthComponent.TakeDamageForce(this.selfForceMagnitude * base.characterDirection.forward, true);
					}
				}
				if (base.FireMeleeOverlap(this.overlapAttack, this.animator, "Sword.active", 0f))
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
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x0400054C RID: 1356
		public static GameObject swingEffectPrefab;

		// Token: 0x0400054D RID: 1357
		public static GameObject hitEffectPrefab;

		// Token: 0x0400054E RID: 1358
		public static string attackSoundString;

		// Token: 0x0400054F RID: 1359
		public static string hitSoundString;

		// Token: 0x04000550 RID: 1360
		public static float slashPitch;

		// Token: 0x04000551 RID: 1361
		public static float hitPauseDuration;

		// Token: 0x04000552 RID: 1362
		[SerializeField]
		public float baseDuration;

		// Token: 0x04000553 RID: 1363
		[SerializeField]
		public float baseDamageCoefficient;

		// Token: 0x04000554 RID: 1364
		[SerializeField]
		public string slashChildName;

		// Token: 0x04000555 RID: 1365
		[SerializeField]
		public float selfForceMagnitude;

		// Token: 0x04000556 RID: 1366
		[SerializeField]
		public float moveSpeedBonusCoefficient;

		// Token: 0x04000557 RID: 1367
		[SerializeField]
		public float smallHopVelocity;

		// Token: 0x04000558 RID: 1368
		[SerializeField]
		public string hitboxString;

		// Token: 0x04000559 RID: 1369
		protected Animator animator;

		// Token: 0x0400055A RID: 1370
		protected float duration;

		// Token: 0x0400055B RID: 1371
		protected float hitInterval;

		// Token: 0x0400055C RID: 1372
		protected int swingCount;

		// Token: 0x0400055D RID: 1373
		protected float hitPauseTimer;

		// Token: 0x0400055E RID: 1374
		protected bool isInHitPause;

		// Token: 0x0400055F RID: 1375
		protected OverlapAttack overlapAttack;

		// Token: 0x04000560 RID: 1376
		protected BaseState.HitStopCachedState hitStopCachedState;
	}
}
