using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Croco
{
	// Token: 0x020008A9 RID: 2217
	public class BaseLeap : BaseCharacterMain
	{
		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x060031AE RID: 12718 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected virtual DamageType blastDamageType
		{
			get
			{
				return DamageType.Generic;
			}
		}

		// Token: 0x060031AF RID: 12719 RVA: 0x000D6088 File Offset: 0x000D4288
		public override void OnEnter()
		{
			base.OnEnter();
			this.previousAirControl = base.characterMotor.airControl;
			base.characterMotor.airControl = BaseLeap.airControl;
			Vector3 direction = base.GetAimRay().direction;
			if (base.isAuthority)
			{
				base.characterBody.isSprinting = true;
				direction.y = Mathf.Max(direction.y, BaseLeap.minimumY);
				Vector3 a = direction.normalized * BaseLeap.aimVelocity * this.moveSpeedStat;
				Vector3 b = Vector3.up * BaseLeap.upwardVelocity;
				Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * BaseLeap.forwardVelocity;
				base.characterMotor.Motor.ForceUnground();
				base.characterMotor.velocity = a + b + b2;
				this.isCritAuthority = base.RollCrit();
			}
			base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
			base.GetModelTransform().GetComponent<AimAnimator>().enabled = true;
			base.PlayCrossfade("Gesture, Override", "Leap", 0.1f);
			base.PlayCrossfade("Gesture, AdditiveHigh", "Leap", 0.1f);
			base.PlayCrossfade("Gesture, Override", "Leap", 0.1f);
			Util.PlaySound(BaseLeap.leapSoundString, base.gameObject);
			base.characterDirection.moveVector = direction;
			this.leftFistEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.fistEffectPrefab, base.FindModelChild("MuzzleHandL"));
			this.rightFistEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.fistEffectPrefab, base.FindModelChild("MuzzleHandR"));
			if (base.isAuthority)
			{
				base.characterMotor.onMovementHit += this.OnMovementHit;
			}
			Util.PlaySound(BaseLeap.soundLoopStartEvent, base.gameObject);
		}

		// Token: 0x060031B0 RID: 12720 RVA: 0x000D626E File Offset: 0x000D446E
		private void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
		{
			this.detonateNextFrame = true;
		}

		// Token: 0x060031B1 RID: 12721 RVA: 0x000D6278 File Offset: 0x000D4478
		protected override void UpdateAnimationParameters()
		{
			base.UpdateAnimationParameters();
			float value = Mathf.Clamp01(Util.Remap(base.estimatedVelocity.y, BaseLeap.minYVelocityForAnim, BaseLeap.maxYVelocityForAnim, 0f, 1f)) * 0.97f;
			base.modelAnimator.SetFloat("LeapCycle", value, 0.1f, Time.deltaTime);
		}

		// Token: 0x060031B2 RID: 12722 RVA: 0x000D62D8 File Offset: 0x000D44D8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.characterMotor)
			{
				base.characterMotor.moveDirection = base.inputBank.moveVector;
				if (base.fixedAge >= BaseLeap.minimumDuration && (this.detonateNextFrame || (base.characterMotor.Motor.GroundingStatus.IsStableOnGround && !base.characterMotor.Motor.LastGroundingStatus.IsStableOnGround)))
				{
					this.DoImpactAuthority();
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x060031B3 RID: 12723 RVA: 0x000D636A File Offset: 0x000D456A
		protected virtual void DoImpactAuthority()
		{
			if (BaseLeap.landingSound)
			{
				EffectManager.SimpleSoundEffect(BaseLeap.landingSound.index, base.characterBody.footPosition, true);
			}
		}

		// Token: 0x060031B4 RID: 12724 RVA: 0x000D6394 File Offset: 0x000D4594
		protected BlastAttack.Result DetonateAuthority()
		{
			Vector3 footPosition = base.characterBody.footPosition;
			EffectManager.SpawnEffect(this.blastEffectPrefab, new EffectData
			{
				origin = footPosition,
				scale = BaseLeap.blastRadius
			}, true);
			return new BlastAttack
			{
				attacker = base.gameObject,
				baseDamage = this.damageStat * this.blastDamageCoefficient,
				baseForce = this.blastForce,
				bonusForce = this.blastBonusForce,
				crit = this.isCritAuthority,
				damageType = this.blastDamageType,
				falloffModel = BlastAttack.FalloffModel.None,
				procCoefficient = BaseLeap.blastProcCoefficient,
				radius = BaseLeap.blastRadius,
				position = footPosition,
				canHurtAttacker = false,
				impactEffect = EffectCatalog.FindEffectIndexFromPrefab(this.blastImpactEffectPrefab),
				teamIndex = base.teamComponent.teamIndex
			}.Fire();
		}

		// Token: 0x060031B5 RID: 12725 RVA: 0x000D6478 File Offset: 0x000D4678
		protected void DropAcidPoolAuthority()
		{
			Vector3 footPosition = base.characterBody.footPosition;
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = BaseLeap.projectilePrefab,
				crit = this.isCritAuthority,
				force = 0f,
				damage = this.damageStat,
				owner = base.gameObject,
				rotation = Quaternion.identity,
				position = footPosition
			};
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x060031B6 RID: 12726 RVA: 0x000D64FC File Offset: 0x000D46FC
		public override void OnExit()
		{
			Util.PlaySound(BaseLeap.soundLoopStopEvent, base.gameObject);
			if (base.isAuthority)
			{
				base.characterMotor.onMovementHit -= this.OnMovementHit;
			}
			base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
			base.characterMotor.airControl = this.previousAirControl;
			base.characterBody.isSprinting = false;
			int layerIndex = base.modelAnimator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				base.modelAnimator.SetLayerWeight(layerIndex, 2f);
				base.PlayAnimation("Impact", "LightImpact");
			}
			base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
			base.PlayCrossfade("Gesture, AdditiveHigh", "BufferEmpty", 0.1f);
			EntityState.Destroy(this.leftFistEffectInstance);
			EntityState.Destroy(this.rightFistEffectInstance);
			base.OnExit();
		}

		// Token: 0x060031B7 RID: 12727 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003032 RID: 12338
		public static float minimumDuration;

		// Token: 0x04003033 RID: 12339
		public static float blastRadius;

		// Token: 0x04003034 RID: 12340
		public static float blastProcCoefficient;

		// Token: 0x04003035 RID: 12341
		[SerializeField]
		public float blastDamageCoefficient;

		// Token: 0x04003036 RID: 12342
		[SerializeField]
		public float blastForce;

		// Token: 0x04003037 RID: 12343
		public static string leapSoundString;

		// Token: 0x04003038 RID: 12344
		public static GameObject projectilePrefab;

		// Token: 0x04003039 RID: 12345
		[SerializeField]
		public Vector3 blastBonusForce;

		// Token: 0x0400303A RID: 12346
		[SerializeField]
		public GameObject blastImpactEffectPrefab;

		// Token: 0x0400303B RID: 12347
		[SerializeField]
		public GameObject blastEffectPrefab;

		// Token: 0x0400303C RID: 12348
		public static float airControl;

		// Token: 0x0400303D RID: 12349
		public static float aimVelocity;

		// Token: 0x0400303E RID: 12350
		public static float upwardVelocity;

		// Token: 0x0400303F RID: 12351
		public static float forwardVelocity;

		// Token: 0x04003040 RID: 12352
		public static float minimumY;

		// Token: 0x04003041 RID: 12353
		public static float minYVelocityForAnim;

		// Token: 0x04003042 RID: 12354
		public static float maxYVelocityForAnim;

		// Token: 0x04003043 RID: 12355
		public static float knockbackForce;

		// Token: 0x04003044 RID: 12356
		[SerializeField]
		public GameObject fistEffectPrefab;

		// Token: 0x04003045 RID: 12357
		public static string soundLoopStartEvent;

		// Token: 0x04003046 RID: 12358
		public static string soundLoopStopEvent;

		// Token: 0x04003047 RID: 12359
		public static NetworkSoundEventDef landingSound;

		// Token: 0x04003048 RID: 12360
		private float previousAirControl;

		// Token: 0x04003049 RID: 12361
		private GameObject leftFistEffectInstance;

		// Token: 0x0400304A RID: 12362
		private GameObject rightFistEffectInstance;

		// Token: 0x0400304B RID: 12363
		protected bool isCritAuthority;

		// Token: 0x0400304C RID: 12364
		private bool detonateNextFrame;
	}
}
