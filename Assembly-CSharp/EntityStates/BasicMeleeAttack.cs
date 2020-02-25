using System;
using RoR2;
using RoR2.Audio;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020006FE RID: 1790
	public class BasicMeleeAttack : BaseState
	{
		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06002995 RID: 10645 RVA: 0x000AF298 File Offset: 0x000AD498
		protected bool authorityInHitPause
		{
			get
			{
				return this.hitPauseTimer > 0f;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06002996 RID: 10646 RVA: 0x000AF2A7 File Offset: 0x000AD4A7
		private bool meleeAttackHasBegun
		{
			get
			{
				return this.meleeAttackStartTime.hasPassed;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06002997 RID: 10647 RVA: 0x000AF2B4 File Offset: 0x000AD4B4
		protected bool authorityHasFiredAtAll
		{
			get
			{
				return this.meleeAttackTicks > 0;
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06002998 RID: 10648 RVA: 0x000AF2BF File Offset: 0x000AD4BF
		// (set) Token: 0x06002999 RID: 10649 RVA: 0x000AF2C7 File Offset: 0x000AD4C7
		private protected bool isCritAuthority { protected get; private set; }

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x0600299A RID: 10650 RVA: 0x0000B933 File Offset: 0x00009B33
		protected virtual bool allowExitFire
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x000AF2D0 File Offset: 0x000AD4D0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.CalcDuration();
			if (this.duration <= Time.fixedDeltaTime * 2f)
			{
				this.forceFire = true;
			}
			base.StartAimMode(2f, false);
			Util.PlaySound(this.beginStateSoundString, base.gameObject);
			this.animator = base.GetModelAnimator();
			if (base.isAuthority)
			{
				this.isCritAuthority = base.RollCrit();
				this.hitBoxGroup = base.FindHitBoxGroup(this.hitBoxGroupName);
				if (this.hitBoxGroup)
				{
					OverlapAttack overlapAttack = new OverlapAttack();
					overlapAttack.attacker = base.gameObject;
					overlapAttack.damage = this.damageCoefficient * this.damageStat;
					overlapAttack.damageColorIndex = DamageColorIndex.Default;
					overlapAttack.damageType = DamageType.Generic;
					overlapAttack.forceVector = this.forceVector;
					overlapAttack.hitBoxGroup = this.hitBoxGroup;
					overlapAttack.hitEffectPrefab = this.hitEffectPrefab;
					NetworkSoundEventDef networkSoundEventDef = this.impactSound;
					overlapAttack.impactSound = ((networkSoundEventDef != null) ? networkSoundEventDef.index : NetworkSoundEventIndex.Invalid);
					overlapAttack.inflictor = base.gameObject;
					overlapAttack.isCrit = this.isCritAuthority;
					overlapAttack.procChainMask = default(ProcChainMask);
					overlapAttack.pushAwayForce = this.pushAwayForce;
					overlapAttack.procCoefficient = this.procCoefficient;
					overlapAttack.teamIndex = base.GetTeam();
					this.overlapAttack = overlapAttack;
				}
			}
			this.PlayAnimation();
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x000AF42E File Offset: 0x000AD62E
		protected virtual float CalcDuration()
		{
			return this.baseDuration / this.attackSpeedStat;
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x000AF440 File Offset: 0x000AD640
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (string.IsNullOrEmpty(this.mecanimHitboxActiveParameter))
			{
				this.BeginMeleeAttack();
			}
			else if (this.animator.GetFloat(this.mecanimHitboxActiveParameter) > 0.5f)
			{
				this.BeginMeleeAttack();
			}
			if (base.isAuthority)
			{
				this.AuthorityFixedUpdate();
				if (this.hitPauseTimer > 0f)
				{
					this.hitPauseTimer -= Time.fixedDeltaTime;
					if (this.hitPauseTimer <= 0f)
					{
						this.AuthorityExitHitPause();
					}
				}
			}
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x000AF4C8 File Offset: 0x000AD6C8
		protected void AuthorityTriggerHitPause()
		{
			this.storedHitPauseVelocity += base.characterMotor.velocity;
			base.characterMotor.velocity = Vector3.zero;
			if (this.animator)
			{
				this.animator.speed = 0f;
			}
			if (this.swingEffectInstance)
			{
				ScaleParticleSystemDuration component = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = 20f;
				}
			}
			this.hitPauseTimer = this.hitPauseDuration;
		}

		// Token: 0x060029A0 RID: 10656 RVA: 0x000AF558 File Offset: 0x000AD758
		protected virtual void BeginMeleeAttack()
		{
			if (this.meleeAttackStartTime != Run.FixedTimeStamp.positiveInfinity)
			{
				return;
			}
			this.meleeAttackStartTime = Run.FixedTimeStamp.now;
			Util.PlaySound(this.beginSwingSoundString, base.gameObject);
			if (this.swingEffectPrefab)
			{
				Transform transform = base.FindModelChild(this.swingEffectMuzzleString);
				if (transform)
				{
					this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, transform);
					ScaleParticleSystemDuration component = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
					if (component)
					{
						component.newDuration = component.initialDuration;
					}
				}
			}
		}

		// Token: 0x060029A1 RID: 10657 RVA: 0x000AF5EC File Offset: 0x000AD7EC
		protected virtual void AuthorityExitHitPause()
		{
			this.hitPauseTimer = 0f;
			this.storedHitPauseVelocity.y = Mathf.Max(this.storedHitPauseVelocity.y, this.shorthopVelocityFromHit);
			base.characterMotor.velocity = this.storedHitPauseVelocity;
			this.storedHitPauseVelocity = Vector3.zero;
			if (this.animator)
			{
				this.animator.speed = 1f;
			}
			if (this.swingEffectInstance)
			{
				ScaleParticleSystemDuration component = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = component.initialDuration;
				}
			}
		}

		// Token: 0x060029A2 RID: 10658 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void PlayAnimation()
		{
		}

		// Token: 0x060029A3 RID: 10659 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnMeleeHitAuthority()
		{
		}

		// Token: 0x060029A4 RID: 10660 RVA: 0x000AF68B File Offset: 0x000AD88B
		private void FireAttack()
		{
			this.AuthorityModifyOverlapAttack(this.overlapAttack);
			this.authorityHitThisFixedUpdate = this.overlapAttack.Fire(null);
			this.meleeAttackTicks++;
		}

		// Token: 0x060029A5 RID: 10661 RVA: 0x000AF6BC File Offset: 0x000AD8BC
		protected virtual void AuthorityFixedUpdate()
		{
			if (this.authorityInHitPause)
			{
				base.characterMotor.velocity = Vector3.zero;
				base.fixedAge -= Time.fixedDeltaTime;
			}
			else if (this.forceForwardVelocity)
			{
				Vector3 vector = base.characterDirection.forward * this.forwardVelocityCurve.Evaluate(base.fixedAge / this.duration);
				Vector3 velocity = base.characterMotor.velocity;
				base.characterMotor.AddDisplacement(new Vector3(vector.x, 0f, vector.z));
			}
			this.authorityHitThisFixedUpdate = false;
			if (this.overlapAttack != null && (string.IsNullOrEmpty(this.mecanimHitboxActiveParameter) || this.animator.GetFloat(this.mecanimHitboxActiveParameter) > 0.5f || this.forceFire))
			{
				this.FireAttack();
			}
			if (this.authorityHitThisFixedUpdate)
			{
				this.AuthorityTriggerHitPause();
				this.OnMeleeHitAuthority();
			}
			if (this.duration <= base.fixedAge)
			{
				if (this.meleeAttackTicks > 0)
				{
					this.AuthorityOnFinish();
					return;
				}
				this.BeginMeleeAttack();
				this.forceFire = true;
			}
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x000AF7D8 File Offset: 0x000AD9D8
		public override void OnExit()
		{
			if (!this.outer.destroying && base.isAuthority && this.meleeAttackTicks == 0 && this.allowExitFire)
			{
				this.FireAttack();
			}
			if (this.swingEffectInstance)
			{
				EntityState.Destroy(this.swingEffectInstance);
			}
			if (this.animator)
			{
				this.animator.speed = 1f;
			}
			base.OnExit();
		}

		// Token: 0x060029A7 RID: 10663 RVA: 0x000AF84B File Offset: 0x000ADA4B
		protected virtual void AuthorityOnFinish()
		{
			this.outer.SetNextStateToMain();
		}

		// Token: 0x04002590 RID: 9616
		[SerializeField]
		public float baseDuration;

		// Token: 0x04002591 RID: 9617
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x04002592 RID: 9618
		[SerializeField]
		public string hitBoxGroupName;

		// Token: 0x04002593 RID: 9619
		[SerializeField]
		public GameObject hitEffectPrefab;

		// Token: 0x04002594 RID: 9620
		[SerializeField]
		public float procCoefficient;

		// Token: 0x04002595 RID: 9621
		[SerializeField]
		public float pushAwayForce;

		// Token: 0x04002596 RID: 9622
		[SerializeField]
		public Vector3 forceVector;

		// Token: 0x04002597 RID: 9623
		[SerializeField]
		public float hitPauseDuration;

		// Token: 0x04002598 RID: 9624
		[SerializeField]
		public GameObject swingEffectPrefab;

		// Token: 0x04002599 RID: 9625
		[SerializeField]
		public string swingEffectMuzzleString;

		// Token: 0x0400259A RID: 9626
		[SerializeField]
		public string mecanimHitboxActiveParameter;

		// Token: 0x0400259B RID: 9627
		[SerializeField]
		public float shorthopVelocityFromHit;

		// Token: 0x0400259C RID: 9628
		[SerializeField]
		public string beginStateSoundString;

		// Token: 0x0400259D RID: 9629
		[SerializeField]
		public string beginSwingSoundString;

		// Token: 0x0400259E RID: 9630
		[SerializeField]
		public NetworkSoundEventDef impactSound;

		// Token: 0x0400259F RID: 9631
		[SerializeField]
		public bool forceForwardVelocity;

		// Token: 0x040025A0 RID: 9632
		[SerializeField]
		public AnimationCurve forwardVelocityCurve;

		// Token: 0x040025A1 RID: 9633
		protected float duration;

		// Token: 0x040025A2 RID: 9634
		protected HitBoxGroup hitBoxGroup;

		// Token: 0x040025A3 RID: 9635
		protected Animator animator;

		// Token: 0x040025A4 RID: 9636
		private OverlapAttack overlapAttack;

		// Token: 0x040025A5 RID: 9637
		protected bool authorityHitThisFixedUpdate;

		// Token: 0x040025A6 RID: 9638
		protected float hitPauseTimer;

		// Token: 0x040025A7 RID: 9639
		protected Vector3 storedHitPauseVelocity;

		// Token: 0x040025A8 RID: 9640
		private Run.FixedTimeStamp meleeAttackStartTime = Run.FixedTimeStamp.positiveInfinity;

		// Token: 0x040025A9 RID: 9641
		private GameObject swingEffectInstance;

		// Token: 0x040025AA RID: 9642
		private int meleeAttackTicks;

		// Token: 0x040025AC RID: 9644
		private bool forceFire;
	}
}
