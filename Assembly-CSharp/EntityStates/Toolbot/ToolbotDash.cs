using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x02000765 RID: 1893
	public class ToolbotDash : BaseCharacterMain
	{
		// Token: 0x06002BB2 RID: 11186 RVA: 0x000B88AC File Offset: 0x000B6AAC
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
			Util.PlaySound(ToolbotDash.startSoundString, base.gameObject);
			base.PlayCrossfade("Body", "BoxModeEnter", 0.1f);
			base.PlayCrossfade("Stance, Override", "PutAwayGun", 0.1f);
			base.modelAnimator.SetFloat("aimWeight", 0f);
			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.ArmorBoost);
			}
			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge");
			}
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = ToolbotDash.chargeDamageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = ToolbotDash.impactEffectPrefab;
			this.attack.forceVector = Vector3.up * ToolbotDash.upwardForceMagnitude;
			this.attack.pushAwayForce = ToolbotDash.awayForceMagnitude;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
		}

		// Token: 0x06002BB3 RID: 11187 RVA: 0x000B8AD0 File Offset: 0x000B6CD0
		public override void OnExit()
		{
			if (base.characterBody)
			{
				if (!this.outer.destroying && this.endEffectPrefab)
				{
					EffectManager.SpawnEffect(this.endEffectPrefab, new EffectData
					{
						origin = base.characterBody.corePosition
					}, false);
				}
				base.PlayAnimation("Body", "BoxModeExit");
				base.PlayCrossfade("Stance, Override", "Empty", 0.1f);
				base.characterBody.isSprinting = false;
				if (NetworkServer.active)
				{
					base.characterBody.RemoveBuff(BuffIndex.ArmorBoost);
				}
			}
			if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
			{
				base.characterMotor.velocity += this.GetIdealVelocity();
			}
			if (base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = false;
			}
			Util.PlaySound(ToolbotDash.endSoundString, base.gameObject);
			base.modelAnimator.SetFloat("aimWeight", 1f);
			base.OnExit();
		}

		// Token: 0x06002BB4 RID: 11188 RVA: 0x000B8BE8 File Offset: 0x000B6DE8
		private float GetDamageBoostFromSpeed()
		{
			return Mathf.Max(1f, base.characterBody.moveSpeed / base.characterBody.baseMoveSpeed);
		}

		// Token: 0x06002BB5 RID: 11189 RVA: 0x000B8C0C File Offset: 0x000B6E0C
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

		// Token: 0x06002BB6 RID: 11190 RVA: 0x000B8C6F File Offset: 0x000B6E6F
		private Vector3 GetIdealVelocity()
		{
			return base.characterDirection.forward * base.characterBody.moveSpeed * this.speedMultiplier;
		}

		// Token: 0x06002BB7 RID: 11191 RVA: 0x000B8C98 File Offset: 0x000B6E98
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
			if (base.isAuthority)
			{
				if (base.characterBody)
				{
					base.characterBody.isSprinting = true;
				}
				if (base.skillLocator.special && base.inputBank.skill4.down)
				{
					base.skillLocator.special.ExecuteIfReady();
				}
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
					this.attack.damage = this.damageStat * (ToolbotDash.chargeDamageCoefficient * this.GetDamageBoostFromSpeed());
					if (this.attack.Fire(this.victimsStruck))
					{
						Util.PlaySound(ToolbotDash.impactSoundString, base.gameObject);
						this.inHitPause = true;
						this.hitPauseTimer = ToolbotDash.hitPauseDuration;
						base.AddRecoil(-0.5f * ToolbotDash.recoilAmplitude, -0.5f * ToolbotDash.recoilAmplitude, -0.5f * ToolbotDash.recoilAmplitude, 0.5f * ToolbotDash.recoilAmplitude);
						base.PlayAnimation("Gesture, Additive", "BoxModeImpact", "BoxModeImpact.playbackRate", ToolbotDash.hitPauseDuration);
						for (int i = 0; i < this.victimsStruck.Count; i++)
						{
							float num = 0f;
							HealthComponent healthComponent = this.victimsStruck[i];
							CharacterMotor component = healthComponent.GetComponent<CharacterMotor>();
							if (component)
							{
								num = component.mass;
							}
							else
							{
								Rigidbody component2 = healthComponent.GetComponent<Rigidbody>();
								if (component2)
								{
									num = component2.mass;
								}
							}
							if (num >= ToolbotDash.massThresholdForKnockback)
							{
								healthComponent.TakeDamage(new DamageInfo
								{
									attacker = base.gameObject,
									damage = this.damageStat * ToolbotDash.knockbackDamageCoefficient * this.GetDamageBoostFromSpeed(),
									crit = this.attack.isCrit,
									procCoefficient = 1f,
									damageColorIndex = DamageColorIndex.Item,
									damageType = DamageType.Stun1s,
									position = base.characterBody.corePosition
								});
								base.AddRecoil(-0.5f * ToolbotDash.recoilAmplitude * 3f, -0.5f * ToolbotDash.recoilAmplitude * 3f, -0.5f * ToolbotDash.recoilAmplitude * 8f, 0.5f * ToolbotDash.recoilAmplitude * 3f);
								base.healthComponent.TakeDamageForce(this.idealDirection * -ToolbotDash.knockbackForce, true, false);
								EffectManager.SimpleImpactEffect(ToolbotDash.knockbackEffectPrefab, base.characterBody.corePosition, base.characterDirection.forward, true);
								this.outer.SetNextStateToMain();
								return;
							}
						}
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

		// Token: 0x06002BB8 RID: 11192 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x040027D4 RID: 10196
		[SerializeField]
		public float baseDuration;

		// Token: 0x040027D5 RID: 10197
		[SerializeField]
		public float speedMultiplier;

		// Token: 0x040027D6 RID: 10198
		public static float chargeDamageCoefficient;

		// Token: 0x040027D7 RID: 10199
		public static float awayForceMagnitude;

		// Token: 0x040027D8 RID: 10200
		public static float upwardForceMagnitude;

		// Token: 0x040027D9 RID: 10201
		public static GameObject impactEffectPrefab;

		// Token: 0x040027DA RID: 10202
		public static float hitPauseDuration;

		// Token: 0x040027DB RID: 10203
		public static string impactSoundString;

		// Token: 0x040027DC RID: 10204
		public static float recoilAmplitude;

		// Token: 0x040027DD RID: 10205
		public static string startSoundString;

		// Token: 0x040027DE RID: 10206
		public static string endSoundString;

		// Token: 0x040027DF RID: 10207
		public static GameObject knockbackEffectPrefab;

		// Token: 0x040027E0 RID: 10208
		public static float knockbackDamageCoefficient;

		// Token: 0x040027E1 RID: 10209
		public static float massThresholdForKnockback;

		// Token: 0x040027E2 RID: 10210
		public static float knockbackForce;

		// Token: 0x040027E3 RID: 10211
		[SerializeField]
		public GameObject startEffectPrefab;

		// Token: 0x040027E4 RID: 10212
		[SerializeField]
		public GameObject endEffectPrefab;

		// Token: 0x040027E5 RID: 10213
		private float duration;

		// Token: 0x040027E6 RID: 10214
		private float hitPauseTimer;

		// Token: 0x040027E7 RID: 10215
		private Vector3 idealDirection;

		// Token: 0x040027E8 RID: 10216
		private OverlapAttack attack;

		// Token: 0x040027E9 RID: 10217
		private bool inHitPause;

		// Token: 0x040027EA RID: 10218
		private List<HealthComponent> victimsStruck = new List<HealthComponent>();
	}
}
