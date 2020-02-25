using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot
{
	// Token: 0x02000744 RID: 1860
	public class BurrowDash : BaseCharacterMain
	{
		// Token: 0x06002B21 RID: 11041 RVA: 0x000B5998 File Offset: 0x000B3B98
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration;
			if (base.isAuthority)
			{
				if (base.inputBank)
				{
					this.idealDirection = base.inputBank.aimDirection;
					this.idealDirection.y = 0f;
				}
				this.UpdateDirection();
			}
			if (base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = true;
			}
			if (this.startEffectPrefab && base.characterBody)
			{
				EffectManager.SpawnEffect(this.startEffectPrefab, new EffectData
				{
					origin = base.characterBody.corePosition
				}, false);
			}
			if (base.characterDirection)
			{
				base.characterDirection.forward = this.idealDirection;
			}
			Util.PlaySound(BurrowDash.startSoundString, base.gameObject);
			base.PlayCrossfade("Body", "BurrowEnter", 0.1f);
			HitBoxGroup hitBoxGroup = null;
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "BurrowCharge");
				this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			}
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = BurrowDash.chargeDamageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = BurrowDash.impactEffectPrefab;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.damage = this.damageStat * BurrowDash.chargeDamageCoefficient;
			this.attack.damageType = DamageType.Freeze2s;
			this.attack.procCoefficient = 1f;
			base.gameObject.layer = LayerIndex.debris.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
			HurtBoxGroup component = this.modelTransform.GetComponent<HurtBoxGroup>();
			int hurtBoxesDeactivatorCounter = component.hurtBoxesDeactivatorCounter;
			component.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter + 1;
			base.characterBody.hideCrosshair = true;
			base.characterBody.isSprinting = true;
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild("BurrowCenter");
				if (transform && BurrowDash.burrowLoopEffectPrefab)
				{
					this.burrowLoopEffectInstance = UnityEngine.Object.Instantiate<GameObject>(BurrowDash.burrowLoopEffectPrefab, transform.position, transform.rotation);
					this.burrowLoopEffectInstance.transform.parent = transform;
				}
			}
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x000B5C50 File Offset: 0x000B3E50
		public override void OnExit()
		{
			if (base.characterBody && !this.outer.destroying && this.endEffectPrefab)
			{
				EffectManager.SpawnEffect(this.endEffectPrefab, new EffectData
				{
					origin = base.characterBody.corePosition
				}, false);
			}
			Util.PlaySound(BurrowDash.endSoundString, base.gameObject);
			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
			HurtBoxGroup component = this.modelTransform.GetComponent<HurtBoxGroup>();
			int hurtBoxesDeactivatorCounter = component.hurtBoxesDeactivatorCounter;
			component.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter - 1;
			base.characterBody.hideCrosshair = false;
			if (this.burrowLoopEffectInstance)
			{
				EntityState.Destroy(this.burrowLoopEffectInstance);
			}
			Animator modelAnimator = base.GetModelAnimator();
			int layerIndex = modelAnimator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				modelAnimator.SetLayerWeight(layerIndex, 2f);
				modelAnimator.PlayInFixedTime("LightImpact", layerIndex, 0f);
			}
			base.OnExit();
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x000B5D54 File Offset: 0x000B3F54
		private void UpdateDirection()
		{
			if (base.inputBank)
			{
				Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
				if (vector != Vector2.zero)
				{
					vector.Normalize();
					this.idealDirection = new Vector3(vector.x, 0f, vector.y).normalized;
				}
			}
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void UpdateAnimationParameters()
		{
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x000B5DB7 File Offset: 0x000B3FB7
		private Vector3 GetIdealVelocity()
		{
			return base.characterDirection.forward * base.characterBody.moveSpeed * BurrowDash.speedMultiplier.Evaluate(base.fixedAge / this.duration);
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x000B5DF0 File Offset: 0x000B3FF0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
			if (base.fixedAge >= this.duration - BurrowDash.timeBeforeExitToPlayExitAnimation && !this.beginPlayingExitAnimation)
			{
				this.beginPlayingExitAnimation = true;
				base.PlayCrossfade("Body", "BurrowExit", 0.1f);
			}
			if (base.isAuthority)
			{
				this.UpdateDirection();
				if (!this.inHitPause)
				{
					if (base.characterDirection)
					{
						base.characterDirection.moveVector = this.idealDirection;
						if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
						{
							base.characterMotor.rootMotion += this.GetIdealVelocity() * Time.fixedDeltaTime;
						}
					}
					if (this.attack.Fire(this.victimsStruck))
					{
						Util.PlaySound(BurrowDash.impactSoundString, base.gameObject);
						this.inHitPause = true;
						this.hitPauseTimer = BurrowDash.hitPauseDuration;
						if (BurrowDash.healPercent > 0f)
						{
							base.healthComponent.HealFraction(BurrowDash.healPercent, default(ProcChainMask));
							Util.PlaySound("Play_item_use_fruit", base.gameObject);
							EffectData effectData = new EffectData();
							effectData.origin = base.transform.position;
							effectData.SetNetworkedObjectReference(base.gameObject);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FruitHealEffect"), effectData, true);
						}
						if (BurrowDash.resetDurationOnImpact)
						{
							base.fixedAge = 0f;
							return;
						}
						base.fixedAge -= BurrowDash.hitPauseDuration;
						return;
					}
				}
				else
				{
					base.characterMotor.velocity = Vector3.zero;
					this.hitPauseTimer -= Time.fixedDeltaTime;
					if (this.hitPauseTimer < 0f)
					{
						this.inHitPause = false;
					}
				}
			}
		}

		// Token: 0x06002B27 RID: 11047 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040026FB RID: 9979
		[SerializeField]
		public float baseDuration;

		// Token: 0x040026FC RID: 9980
		[SerializeField]
		public static AnimationCurve speedMultiplier;

		// Token: 0x040026FD RID: 9981
		public static float chargeDamageCoefficient;

		// Token: 0x040026FE RID: 9982
		public static GameObject impactEffectPrefab;

		// Token: 0x040026FF RID: 9983
		public static GameObject burrowLoopEffectPrefab;

		// Token: 0x04002700 RID: 9984
		public static float hitPauseDuration;

		// Token: 0x04002701 RID: 9985
		public static float timeBeforeExitToPlayExitAnimation;

		// Token: 0x04002702 RID: 9986
		public static string impactSoundString;

		// Token: 0x04002703 RID: 9987
		public static string startSoundString;

		// Token: 0x04002704 RID: 9988
		public static string endSoundString;

		// Token: 0x04002705 RID: 9989
		public static float healPercent;

		// Token: 0x04002706 RID: 9990
		public static bool resetDurationOnImpact;

		// Token: 0x04002707 RID: 9991
		[SerializeField]
		public GameObject startEffectPrefab;

		// Token: 0x04002708 RID: 9992
		[SerializeField]
		public GameObject endEffectPrefab;

		// Token: 0x04002709 RID: 9993
		private float duration;

		// Token: 0x0400270A RID: 9994
		private float hitPauseTimer;

		// Token: 0x0400270B RID: 9995
		private Vector3 idealDirection;

		// Token: 0x0400270C RID: 9996
		private OverlapAttack attack;

		// Token: 0x0400270D RID: 9997
		private ChildLocator childLocator;

		// Token: 0x0400270E RID: 9998
		private bool inHitPause;

		// Token: 0x0400270F RID: 9999
		private bool beginPlayingExitAnimation;

		// Token: 0x04002710 RID: 10000
		private Transform modelTransform;

		// Token: 0x04002711 RID: 10001
		private GameObject burrowLoopEffectInstance;

		// Token: 0x04002712 RID: 10002
		private List<HealthComponent> victimsStruck = new List<HealthComponent>();
	}
}
