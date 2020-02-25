using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007E8 RID: 2024
	public class BaseSwingChargedFist : LoaderMeleeAttack
	{
		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06002E0A RID: 11786 RVA: 0x000C3F4B File Offset: 0x000C214B
		// (set) Token: 0x06002E0B RID: 11787 RVA: 0x000C3F53 File Offset: 0x000C2153
		public float punchSpeed { get; private set; }

		// Token: 0x06002E0C RID: 11788 RVA: 0x000C3F5C File Offset: 0x000C215C
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				base.characterMotor.Motor.ForceUnground();
				base.characterMotor.disableAirControlUntilCollision |= BaseSwingChargedFist.disableAirControlUntilCollision;
				this.punchVelocity = BaseSwingChargedFist.CalculateLungeVelocity(base.characterMotor.velocity, base.GetAimRay().direction, this.charge, this.minLungeSpeed, this.maxLungeSpeed);
				base.characterMotor.velocity = this.punchVelocity;
				base.characterDirection.forward = base.characterMotor.velocity.normalized;
				this.punchSpeed = base.characterMotor.velocity.magnitude;
				this.bonusDamage = this.punchSpeed * (BaseSwingChargedFist.velocityDamageCoefficient * this.damageStat);
			}
		}

		// Token: 0x06002E0D RID: 11789 RVA: 0x000C4032 File Offset: 0x000C2232
		protected override float CalcDuration()
		{
			return Mathf.Lerp(this.minDuration, this.maxDuration, this.charge);
		}

		// Token: 0x06002E0E RID: 11790 RVA: 0x000C404B File Offset: 0x000C224B
		protected override void PlayAnimation()
		{
			base.PlayAnimation();
			base.PlayAnimation("FullBody, Override", "ChargePunch", "ChargePunch.playbackRate", this.duration);
		}

		// Token: 0x06002E0F RID: 11791 RVA: 0x000C406E File Offset: 0x000C226E
		protected override void AuthorityFixedUpdate()
		{
			base.AuthorityFixedUpdate();
			if (!base.authorityInHitPause)
			{
				base.characterMotor.velocity = this.punchVelocity;
				base.characterDirection.forward = this.punchVelocity;
			}
		}

		// Token: 0x06002E10 RID: 11792 RVA: 0x000C40A0 File Offset: 0x000C22A0
		protected override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
			base.AuthorityModifyOverlapAttack(overlapAttack);
			overlapAttack.damage = this.damageCoefficient * this.damageStat + this.bonusDamage;
			overlapAttack.forceVector = base.characterMotor.velocity + base.GetAimRay().direction * Mathf.Lerp(this.minPunchForce, this.maxPunchForce, this.charge);
			if (base.fixedAge + Time.fixedDeltaTime >= this.duration)
			{
				HitBoxGroup hitBoxGroup = base.FindHitBoxGroup("PunchLollypop");
				if (hitBoxGroup)
				{
					this.hitBoxGroup = hitBoxGroup;
					overlapAttack.hitBoxGroup = hitBoxGroup;
				}
			}
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x000C4144 File Offset: 0x000C2344
		protected override void OnMeleeHitAuthority()
		{
			base.OnMeleeHitAuthority();
			Action<BaseSwingChargedFist> action = BaseSwingChargedFist.onHitAuthorityGlobal;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x06002E12 RID: 11794 RVA: 0x000C415C File Offset: 0x000C235C
		// (remove) Token: 0x06002E13 RID: 11795 RVA: 0x000C4190 File Offset: 0x000C2390
		public static event Action<BaseSwingChargedFist> onHitAuthorityGlobal;

		// Token: 0x06002E14 RID: 11796 RVA: 0x000C41C3 File Offset: 0x000C23C3
		public override void OnExit()
		{
			base.OnExit();
			base.characterMotor.velocity *= BaseSwingChargedFist.speedCoefficientOnExit;
		}

		// Token: 0x06002E15 RID: 11797 RVA: 0x000C41E6 File Offset: 0x000C23E6
		public static Vector3 CalculateLungeVelocity(Vector3 currentVelocity, Vector3 aimDirection, float charge, float minLungeSpeed, float maxLungeSpeed)
		{
			currentVelocity = ((Vector3.Dot(currentVelocity, aimDirection) < 0f) ? Vector3.zero : Vector3.Project(currentVelocity, aimDirection));
			return currentVelocity + aimDirection * Mathf.Lerp(minLungeSpeed, maxLungeSpeed, charge);
		}

		// Token: 0x06002E16 RID: 11798 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002B1A RID: 11034
		public float charge;

		// Token: 0x04002B1B RID: 11035
		[SerializeField]
		public float minLungeSpeed;

		// Token: 0x04002B1C RID: 11036
		[SerializeField]
		public float maxLungeSpeed;

		// Token: 0x04002B1D RID: 11037
		[SerializeField]
		public float minPunchForce;

		// Token: 0x04002B1E RID: 11038
		[SerializeField]
		public float maxPunchForce;

		// Token: 0x04002B1F RID: 11039
		[SerializeField]
		public float minDuration;

		// Token: 0x04002B20 RID: 11040
		[SerializeField]
		public float maxDuration;

		// Token: 0x04002B21 RID: 11041
		public static bool disableAirControlUntilCollision;

		// Token: 0x04002B22 RID: 11042
		public static float speedCoefficientOnExit;

		// Token: 0x04002B23 RID: 11043
		public static float velocityDamageCoefficient;

		// Token: 0x04002B24 RID: 11044
		protected Vector3 punchVelocity;

		// Token: 0x04002B25 RID: 11045
		private float bonusDamage;
	}
}
