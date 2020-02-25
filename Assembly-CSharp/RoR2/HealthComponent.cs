using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EntityStates;
using RoR2.Networking;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000226 RID: 550
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterBody))]
	public class HealthComponent : NetworkBehaviour
	{
		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000C25 RID: 3109 RVA: 0x00036075 File Offset: 0x00034275
		// (set) Token: 0x06000C26 RID: 3110 RVA: 0x0003607D File Offset: 0x0003427D
		public DamageType killingDamageType
		{
			get
			{
				return (DamageType)this._killingDamageType;
			}
			private set
			{
				this.Network_killingDamageType = (uint)value;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000C27 RID: 3111 RVA: 0x00036086 File Offset: 0x00034286
		public bool alive
		{
			get
			{
				return this.health > 0f;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000C28 RID: 3112 RVA: 0x00036095 File Offset: 0x00034295
		public float fullHealth
		{
			get
			{
				return this.body.maxHealth;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000C29 RID: 3113 RVA: 0x000360A2 File Offset: 0x000342A2
		public float fullShield
		{
			get
			{
				return this.body.maxShield;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000C2A RID: 3114 RVA: 0x000360AF File Offset: 0x000342AF
		public float fullBarrier
		{
			get
			{
				return this.body.maxBarrier;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000C2B RID: 3115 RVA: 0x000360BC File Offset: 0x000342BC
		public float combinedHealth
		{
			get
			{
				return this.health + this.shield + this.barrier;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000C2C RID: 3116 RVA: 0x000360D2 File Offset: 0x000342D2
		public float fullCombinedHealth
		{
			get
			{
				return this.fullHealth + this.fullShield;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000C2D RID: 3117 RVA: 0x000360E1 File Offset: 0x000342E1
		public float combinedHealthFraction
		{
			get
			{
				return this.combinedHealth / this.fullCombinedHealth;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000C2E RID: 3118 RVA: 0x000360F0 File Offset: 0x000342F0
		// (set) Token: 0x06000C2F RID: 3119 RVA: 0x000360F8 File Offset: 0x000342F8
		public Run.FixedTimeStamp lastHitTime { get; private set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x00036101 File Offset: 0x00034301
		// (set) Token: 0x06000C31 RID: 3121 RVA: 0x00036109 File Offset: 0x00034309
		public Run.FixedTimeStamp lastHealTime { get; private set; }

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000C32 RID: 3122 RVA: 0x00036112 File Offset: 0x00034312
		// (set) Token: 0x06000C33 RID: 3123 RVA: 0x0003611A File Offset: 0x0003431A
		public GameObject lastHitAttacker { get; private set; }

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000C34 RID: 3124 RVA: 0x00036124 File Offset: 0x00034324
		public float timeSinceLastHit
		{
			get
			{
				return this.lastHitTime.timeSince;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000C35 RID: 3125 RVA: 0x00036140 File Offset: 0x00034340
		public float timeSinceLastHeal
		{
			get
			{
				return this.lastHealTime.timeSince;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000C36 RID: 3126 RVA: 0x0003615B File Offset: 0x0003435B
		// (set) Token: 0x06000C37 RID: 3127 RVA: 0x00036163 File Offset: 0x00034363
		public bool godMode { get; set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000C38 RID: 3128 RVA: 0x0003616C File Offset: 0x0003436C
		// (set) Token: 0x06000C39 RID: 3129 RVA: 0x00036174 File Offset: 0x00034374
		public float potionReserve { get; private set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000C3A RID: 3130 RVA: 0x0003617D File Offset: 0x0003437D
		// (set) Token: 0x06000C3B RID: 3131 RVA: 0x00036185 File Offset: 0x00034385
		public bool isInFrozenState { get; set; }

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06000C3C RID: 3132 RVA: 0x00036190 File Offset: 0x00034390
		// (remove) Token: 0x06000C3D RID: 3133 RVA: 0x000361C4 File Offset: 0x000343C4
		public static event Action<HealthComponent, float> onCharacterHealServer;

		// Token: 0x06000C3E RID: 3134 RVA: 0x000361F7 File Offset: 0x000343F7
		public void OnValidate()
		{
			if (base.gameObject.GetComponents<HealthComponent>().Length > 1)
			{
				Debug.LogErrorFormat(base.gameObject, "{0} has multiple health components!!", new object[]
				{
					base.gameObject
				});
			}
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x00036228 File Offset: 0x00034428
		public float Heal(float amount, ProcChainMask procChainMask, bool nonRegen = true)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Single RoR2.HealthComponent::Heal(System.Single, RoR2.ProcChainMask, System.Boolean)' called on client");
				return 0f;
			}
			if (!this.alive || amount <= 0f || this.body.HasBuff(BuffIndex.HealingDisabled))
			{
				return 0f;
			}
			float num = this.health;
			CharacterMaster characterMaster = null;
			Inventory inventory = null;
			bool flag = false;
			if (this.body)
			{
				characterMaster = this.body.master;
				if (characterMaster)
				{
					inventory = characterMaster.inventory;
					if (inventory && inventory.currentEquipmentIndex == EquipmentIndex.LunarPotion && !procChainMask.HasProc(ProcType.LunarPotionActivation))
					{
						this.potionReserve += amount;
						return amount;
					}
					if (nonRegen && !procChainMask.HasProc(ProcType.CritHeal) && Util.CheckRoll(this.body.critHeal, characterMaster))
					{
						procChainMask.AddProc(ProcType.CritHeal);
						flag = true;
					}
				}
			}
			if (flag)
			{
				amount *= 2f;
			}
			if (this.increaseHealingCount > 0)
			{
				amount *= 1f + (float)this.increaseHealingCount;
			}
			if (nonRegen && this.repeatHealComponent && !procChainMask.HasProc(ProcType.RepeatHeal))
			{
				this.repeatHealComponent.healthFractionToRestorePerSecond = 0.1f / (float)this.repeatHealCount;
				this.repeatHealComponent.AddReserve(amount * (float)(1 + this.repeatHealCount), this.fullHealth);
				return 0f;
			}
			float num2 = amount;
			if (this.health < this.fullHealth)
			{
				float num3 = Mathf.Max(Mathf.Min(amount, this.fullHealth - this.health), 0f);
				num2 = amount - num3;
				this.Networkhealth = this.health + num3;
			}
			if (num2 > 0f && nonRegen && this.barrierOnOverHealCount > 0)
			{
				float value = num2 * ((float)this.barrierOnOverHealCount * 0.5f);
				this.AddBarrier(value);
			}
			if (nonRegen)
			{
				this.lastHealTime = Run.FixedTimeStamp.now;
				HealthComponent.SendHeal(base.gameObject, amount, flag);
				if (inventory && !procChainMask.HasProc(ProcType.HealNova))
				{
					int itemCount = inventory.GetItemCount(ItemIndex.NovaOnHeal);
					if (itemCount > 0)
					{
						this.devilOrbHealPool = Mathf.Min(this.devilOrbHealPool + amount * (float)itemCount, this.fullCombinedHealth);
					}
				}
			}
			if (flag)
			{
				GlobalEventManager.instance.OnCrit(this.body, characterMaster, amount / this.fullHealth * 10f, procChainMask);
			}
			if (nonRegen)
			{
				Action<HealthComponent, float> action = HealthComponent.onCharacterHealServer;
				if (action != null)
				{
					action(this, amount);
				}
			}
			return this.health - num;
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x00036488 File Offset: 0x00034688
		public void UsePotion()
		{
			ProcChainMask procChainMask = default(ProcChainMask);
			procChainMask.AddProc(ProcType.LunarPotionActivation);
			this.Heal(this.potionReserve, procChainMask, true);
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x000364B5 File Offset: 0x000346B5
		public float HealFraction(float fraction, ProcChainMask procChainMask)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Single RoR2.HealthComponent::HealFraction(System.Single, RoR2.ProcChainMask)' called on client");
				return 0f;
			}
			return this.Heal(fraction * this.fullHealth, procChainMask, true);
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x000364DE File Offset: 0x000346DE
		[Server]
		public void RechargeShieldFull()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::RechargeShieldFull()' called on client");
				return;
			}
			if (this.shield < this.fullShield)
			{
				this.Networkshield = this.fullShield;
			}
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x00036510 File Offset: 0x00034710
		[Server]
		public void RechargeShield(float value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::RechargeShield(System.Single)' called on client");
				return;
			}
			if (this.shield < this.fullShield)
			{
				this.Networkshield = this.shield + value;
				if (this.shield > this.fullShield)
				{
					this.Networkshield = this.fullShield;
				}
			}
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x00036568 File Offset: 0x00034768
		[Server]
		public void AddBarrier(float value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::AddBarrier(System.Single)' called on client");
				return;
			}
			if (!this.alive)
			{
				return;
			}
			if (this.barrier < this.fullBarrier)
			{
				this.Networkbarrier = Mathf.Min(this.barrier + value, this.fullBarrier);
			}
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x000365BA File Offset: 0x000347BA
		[Command]
		private void CmdAddBarrier(float value)
		{
			this.AddBarrier(value);
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x000365C3 File Offset: 0x000347C3
		public void AddBarrierAuthority(float value)
		{
			if (NetworkServer.active)
			{
				this.AddBarrier(value);
				return;
			}
			this.CallCmdAddBarrier(value);
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x000365DC File Offset: 0x000347DC
		[Server]
		public void TakeDamageForce(DamageInfo damageInfo, bool alwaysApply = false, bool disableAirControlUntilCollision = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamageForce(RoR2.DamageInfo,System.Boolean,System.Boolean)' called on client");
				return;
			}
			if (this.body.HasBuff(BuffIndex.EngiShield) && this.shield > 0f)
			{
				return;
			}
			CharacterMotor component = base.GetComponent<CharacterMotor>();
			if (component)
			{
				component.ApplyForce(damageInfo.force, alwaysApply, disableAirControlUntilCollision);
			}
			Rigidbody component2 = base.GetComponent<Rigidbody>();
			if (component2)
			{
				component2.AddForce(damageInfo.force, ForceMode.Impulse);
			}
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x00036654 File Offset: 0x00034854
		[Server]
		public void TakeDamageForce(Vector3 force, bool alwaysApply = false, bool disableAirControlUntilCollision = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamageForce(UnityEngine.Vector3,System.Boolean,System.Boolean)' called on client");
				return;
			}
			if (this.body.HasBuff(BuffIndex.EngiShield) && this.shield > 0f)
			{
				return;
			}
			CharacterMotor component = base.GetComponent<CharacterMotor>();
			if (component)
			{
				component.ApplyForce(force, alwaysApply, disableAirControlUntilCollision);
			}
			Rigidbody component2 = base.GetComponent<Rigidbody>();
			if (component2)
			{
				component2.AddForce(force, ForceMode.Impulse);
			}
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x000366C4 File Offset: 0x000348C4
		[Server]
		public void TakeDamage(DamageInfo damageInfo)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamage(RoR2.DamageInfo)' called on client");
				return;
			}
			if (!this.alive || this.godMode)
			{
				return;
			}
			CharacterBody characterBody = null;
			if (damageInfo.attacker)
			{
				characterBody = damageInfo.attacker.GetComponent<CharacterBody>();
			}
			bool flag = (damageInfo.damageType & DamageType.BypassArmor) > DamageType.Generic;
			CharacterMaster master = this.body.master;
			if (master && master.inventory)
			{
				int itemCount = master.inventory.GetItemCount(ItemIndex.Bear);
				if (itemCount > 0 && Util.CheckRoll(Util.ConvertAmplificationPercentageIntoReductionPercentage(15f * (float)itemCount), 0f, null))
				{
					EffectData effectData = new EffectData
					{
						origin = damageInfo.position,
						rotation = Util.QuaternionSafeLookRotation((damageInfo.force != Vector3.zero) ? damageInfo.force : UnityEngine.Random.onUnitSphere)
					};
					EffectManager.SpawnEffect(HealthComponent.AssetReferences.bearEffectPrefab, effectData, true);
					damageInfo.rejected = true;
				}
			}
			if (this.body.HasBuff(BuffIndex.HiddenInvincibility) && !flag)
			{
				damageInfo.rejected = true;
			}
			if (this.body.HasBuff(BuffIndex.Immune) && (!characterBody || !characterBody.HasBuff(BuffIndex.GoldEmpowered)))
			{
				EffectManager.SpawnEffect(HealthComponent.AssetReferences.damageRejectedPrefab, new EffectData
				{
					origin = damageInfo.position
				}, true);
				damageInfo.rejected = true;
			}
			IOnIncomingDamageServerReceiver[] array = this.onIncomingDamageReceivers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnIncomingDamageServer(damageInfo);
			}
			if (damageInfo.rejected)
			{
				return;
			}
			float num = damageInfo.damage;
			if (num > 0f)
			{
				if (characterBody)
				{
					CharacterMaster master2 = characterBody.master;
					if (master2 && master2.inventory)
					{
						if (this.combinedHealth >= this.fullCombinedHealth * 0.9f)
						{
							int itemCount2 = master2.inventory.GetItemCount(ItemIndex.Crowbar);
							if (itemCount2 > 0)
							{
								num *= 1f + 0.5f * (float)itemCount2;
								EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.crowbarImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
							}
						}
						int itemCount3 = master2.inventory.GetItemCount(ItemIndex.NearbyDamageBonus);
						if (itemCount3 > 0)
						{
							Vector3 normal = characterBody.corePosition - damageInfo.position;
							if (normal.sqrMagnitude <= 169f)
							{
								num *= 1f + (float)itemCount3 * 0.15f;
								EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.diamondDamageBonusImpactEffectPrefab, damageInfo.position, normal, true);
							}
						}
						if (damageInfo.procCoefficient > 0f)
						{
							int itemCount4 = master2.inventory.GetItemCount(ItemIndex.ArmorReductionOnHit);
							if (itemCount4 > 0 && !this.body.HasBuff(BuffIndex.Pulverized))
							{
								this.body.AddTimedBuff(BuffIndex.PulverizeBuildup, 2f * damageInfo.procCoefficient);
								if (this.body.GetBuffCount(BuffIndex.PulverizeBuildup) >= 5)
								{
									this.body.ClearTimedBuffs(BuffIndex.PulverizeBuildup);
									this.body.AddTimedBuff(BuffIndex.Pulverized, 8f * (float)itemCount4);
									EffectManager.SpawnEffect(HealthComponent.AssetReferences.pulverizedEffectPrefab, new EffectData
									{
										origin = this.body.corePosition,
										scale = this.body.radius
									}, true);
								}
							}
						}
						if (this.body.isBoss)
						{
							int itemCount5 = master2.inventory.GetItemCount(ItemIndex.BossDamageBonus);
							if (itemCount5 > 0)
							{
								num *= 1f + 0.2f * (float)itemCount5;
								damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
								EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.bossDamageBonusImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
							}
						}
					}
				}
				if (damageInfo.crit)
				{
					num *= 2f;
				}
				if ((damageInfo.damageType & DamageType.WeakPointHit) != DamageType.Generic)
				{
					num *= 1.5f;
					damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
				}
				if (!flag)
				{
					float num2 = this.body.armor;
					bool flag2 = (damageInfo.damageType & DamageType.AOE) > DamageType.Generic;
					if ((this.body.bodyFlags & CharacterBody.BodyFlags.ResistantToAOE) > CharacterBody.BodyFlags.None && flag2)
					{
						num2 += 300f;
					}
					float num3 = (num2 >= 0f) ? (1f - num2 / (num2 + 100f)) : (2f - 100f / (100f - num2));
					num = Mathf.Max(1f, num * num3);
				}
				if ((damageInfo.damageType & DamageType.BarrierBlocked) != DamageType.Generic)
				{
					damageInfo.force *= 0.5f;
					IBarrier component = base.GetComponent<IBarrier>();
					if (component != null)
					{
						component.BlockedDamage(damageInfo, num);
					}
					damageInfo.procCoefficient = 0f;
					num = 0f;
				}
				if (this.hasOneshotProtection)
				{
					num = Mathf.Min(num, (this.fullCombinedHealth + this.barrier) * 0.9f);
				}
			}
			if ((damageInfo.damageType & DamageType.SlowOnHit) != DamageType.Generic)
			{
				this.body.AddTimedBuff(BuffIndex.Slow50, 2f);
			}
			if ((damageInfo.damageType & DamageType.ClayGoo) != DamageType.Generic && (this.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToGoo) == CharacterBody.BodyFlags.None)
			{
				this.body.AddTimedBuff(BuffIndex.ClayGoo, 2f);
			}
			if ((damageInfo.damageType & DamageType.Nullify) != DamageType.Generic)
			{
				this.body.AddTimedBuff(BuffIndex.NullifyStack, 8f);
			}
			if (master && master.inventory)
			{
				int itemCount6 = master.inventory.GetItemCount(ItemIndex.GoldOnHit);
				if (itemCount6 > 0)
				{
					uint num4 = (uint)(num / this.fullCombinedHealth * master.money * (float)itemCount6);
					float damage = damageInfo.damage;
					uint num5 = num4;
					master.money = (uint)Mathf.Max(0f, master.money - num5);
					EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.coinImpactEffectPrefab, damageInfo.position, Vector3.up, true);
				}
			}
			float num6 = num;
			if (num6 > 0f && this.barrier > 0f)
			{
				if (num6 <= this.barrier)
				{
					this.Networkbarrier = this.barrier - num6;
					num6 = 0f;
				}
				else
				{
					num6 -= this.barrier;
					this.Networkbarrier = 0f;
				}
			}
			if (num6 > 0f && this.shield > 0f)
			{
				if (num6 <= this.shield)
				{
					this.Networkshield = this.shield - num6;
					num6 = 0f;
				}
				else
				{
					num6 -= this.shield;
					this.Networkshield = 0f;
					float scale = 1f;
					if (this.body)
					{
						scale = this.body.radius;
					}
					EffectManager.SpawnEffect(HealthComponent.AssetReferences.shieldBreakEffectPrefab, new EffectData
					{
						origin = base.transform.position,
						scale = scale
					}, true);
				}
			}
			bool flag3 = false;
			float executionHealthLost = 0f;
			GameObject gameObject = null;
			if (num6 > 0f)
			{
				float num7 = this.health - num6;
				if (num7 < 1f && (damageInfo.damageType & DamageType.NonLethal) != DamageType.Generic && this.health >= 1f)
				{
					num7 = 1f;
				}
				float num8 = float.NegativeInfinity;
				if (this.isInFrozenState && (this.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToExecutes) == CharacterBody.BodyFlags.None && num8 < 0.3f)
				{
					num8 = 0.3f;
					gameObject = FrozenState.executeEffectPrefab;
				}
				if (this.body.isElite && characterBody)
				{
					float executeEliteHealthFraction = characterBody.executeEliteHealthFraction;
					if (num8 < executeEliteHealthFraction)
					{
						num8 = executeEliteHealthFraction;
						gameObject = HealthComponent.AssetReferences.executeEffectPrefab;
					}
				}
				this.Networkhealth = num7;
				if (num8 > 0f && this.combinedHealthFraction <= num8)
				{
					flag3 = true;
					executionHealthLost = Mathf.Max(this.combinedHealth, 0f);
					if (this.health > 0f)
					{
						this.Networkhealth = 0f;
					}
					if (this.shield > 0f)
					{
						this.Networkshield = 0f;
					}
					if (this.barrier > 0f)
					{
						this.Networkbarrier = 0f;
					}
				}
			}
			this.TakeDamageForce(damageInfo, false, false);
			DamageReport damageReport = new DamageReport(damageInfo, this, num);
			IOnTakeDamageServerReceiver[] array2 = this.onTakeDamageReceivers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].OnTakeDamageServer(damageReport);
			}
			if (num > 0f)
			{
				HealthComponent.SendDamageDealt(damageReport);
			}
			this.UpdateLastHitTime(damageReport.damageDealt, damageInfo.position, (damageInfo.damageType & DamageType.Silent) > DamageType.Generic, damageInfo.attacker);
			if (damageInfo.attacker)
			{
				List<IOnDamageDealtServerReceiver> gameObjectComponents = GetComponentsCache<IOnDamageDealtServerReceiver>.GetGameObjectComponents(damageInfo.attacker);
				foreach (IOnDamageDealtServerReceiver onDamageDealtServerReceiver in gameObjectComponents)
				{
					onDamageDealtServerReceiver.OnDamageDealtServer(damageReport);
				}
				GetComponentsCache<IOnDamageDealtServerReceiver>.ReturnBuffer(gameObjectComponents);
			}
			if (damageInfo.inflictor)
			{
				List<IOnDamageInflictedServerReceiver> gameObjectComponents2 = GetComponentsCache<IOnDamageInflictedServerReceiver>.GetGameObjectComponents(damageInfo.inflictor);
				foreach (IOnDamageInflictedServerReceiver onDamageInflictedServerReceiver in gameObjectComponents2)
				{
					onDamageInflictedServerReceiver.OnDamageInflictedServer(damageReport);
				}
				GetComponentsCache<IOnDamageInflictedServerReceiver>.ReturnBuffer(gameObjectComponents2);
			}
			GlobalEventManager.ServerDamageDealt(damageReport);
			if (!this.alive)
			{
				this.killingDamageType = damageInfo.damageType;
				if (flag3)
				{
					GlobalEventManager.ServerCharacterExecuted(damageReport, executionHealthLost);
					if (gameObject != null)
					{
						EffectManager.SpawnEffect(gameObject, new EffectData
						{
							origin = this.body.corePosition,
							scale = (this.body ? this.body.radius : 1f)
						}, true);
					}
				}
				IOnKilledServerReceiver[] components = base.GetComponents<IOnKilledServerReceiver>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnKilledServer(damageReport);
				}
				if (damageInfo.attacker)
				{
					IOnKilledOtherServerReceiver[] components2 = damageInfo.attacker.GetComponents<IOnKilledOtherServerReceiver>();
					for (int i = 0; i < components2.Length; i++)
					{
						components2[i].OnKilledOtherServer(damageReport);
					}
				}
				if (Util.CheckRoll(this.globalDeathEventChanceCoefficient * 100f, 0f, null))
				{
					GlobalEventManager.instance.OnCharacterDeath(damageReport);
					return;
				}
			}
			else if (master && master.inventory)
			{
				int itemCount7 = master.inventory.GetItemCount(ItemIndex.Phasing);
				if (itemCount7 > 0 && Util.CheckRoll(damageReport.damageDealt / this.fullCombinedHealth * 100f, master))
				{
					this.body.AddTimedBuff(BuffIndex.Cloak, 1.5f + (float)itemCount7 * 1.5f);
					this.body.AddTimedBuff(BuffIndex.CloakSpeed, 1.5f + (float)itemCount7 * 1.5f);
					EffectManager.SpawnEffect(HealthComponent.AssetReferences.stealthKitEffectPrefab, new EffectData
					{
						origin = base.transform.position,
						rotation = Quaternion.identity
					}, true);
				}
				int itemCount8 = master.inventory.GetItemCount(ItemIndex.Thorns);
				int a = 5 + 2 * (itemCount8 - 1);
				if (itemCount8 > 0)
				{
					float radius = 25f + 10f * (float)(itemCount8 - 1);
					bool isCrit = this.body.RollCrit();
					float damageValue = 1.6f * this.body.damage;
					TeamIndex teamIndex = this.body.teamComponent.teamIndex;
					HurtBox[] hurtBoxes = new SphereSearch
					{
						origin = damageReport.damageInfo.position,
						radius = radius,
						mask = LayerIndex.entityPrecise.mask,
						queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
					}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.AllExcept(teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
					for (int j = 0; j < Mathf.Min(a, hurtBoxes.Length); j++)
					{
						LightningOrb lightningOrb = new LightningOrb();
						lightningOrb.attacker = base.gameObject;
						lightningOrb.bouncedObjects = null;
						lightningOrb.bouncesRemaining = 0;
						lightningOrb.damageCoefficientPerBounce = 1f;
						lightningOrb.damageColorIndex = DamageColorIndex.Item;
						lightningOrb.damageValue = damageValue;
						lightningOrb.isCrit = isCrit;
						lightningOrb.lightningType = LightningOrb.LightningType.RazorWire;
						lightningOrb.origin = damageReport.damageInfo.position;
						lightningOrb.procChainMask = default(ProcChainMask);
						lightningOrb.procCoefficient = 0.5f;
						lightningOrb.range = 0f;
						lightningOrb.teamIndex = teamIndex;
						lightningOrb.target = hurtBoxes[j];
						OrbManager.instance.AddOrb(lightningOrb);
					}
				}
			}
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x000372E4 File Offset: 0x000354E4
		[Server]
		public void Suicide(GameObject killerOverride = null, GameObject inflictorOverride = null, DamageType damageType = DamageType.Generic)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::Suicide(UnityEngine.GameObject,UnityEngine.GameObject,RoR2.DamageType)' called on client");
				return;
			}
			if (this.alive)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = this.combinedHealth;
				damageInfo.position = base.transform.position;
				damageInfo.damageType = damageType;
				damageInfo.procCoefficient = 1f;
				if (killerOverride)
				{
					damageInfo.attacker = killerOverride;
				}
				if (inflictorOverride)
				{
					damageInfo.inflictor = inflictorOverride;
				}
				this.Networkhealth = 0f;
				DamageReport damageReport = new DamageReport(damageInfo, this, damageInfo.damage);
				this.killingDamageType = damageInfo.damageType;
				IOnKilledServerReceiver[] components = base.GetComponents<IOnKilledServerReceiver>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnKilledServer(damageReport);
				}
				GlobalEventManager.instance.OnCharacterDeath(damageReport);
			}
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x000373BC File Offset: 0x000355BC
		public void UpdateLastHitTime(float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
		{
			if (NetworkServer.active && this.body && this.body.inventory && this.body.inventory.GetItemCount(ItemIndex.Medkit) > 0)
			{
				this.body.AddTimedBuff(BuffIndex.MedkitHeal, 1.1f);
			}
			if (damageIsSilent)
			{
				return;
			}
			this.lastHitTime = Run.FixedTimeStamp.now;
			this.lastHitAttacker = attacker;
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					Animator component = modelTransform.GetComponent<Animator>();
					if (component)
					{
						string layerName = "Flinch";
						int layerIndex = component.GetLayerIndex(layerName);
						if (layerIndex >= 0)
						{
							component.SetLayerWeight(layerIndex, 1f + Mathf.Clamp01(damageValue / this.fullCombinedHealth * 10f) * 3f);
							component.Play("FlinchStart", layerIndex);
						}
					}
				}
			}
			IPainAnimationHandler painAnimationHandler = this.painAnimationHandler;
			if (painAnimationHandler == null)
			{
				return;
			}
			painAnimationHandler.HandlePain(damageValue, damagePosition);
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000C4C RID: 3148 RVA: 0x000374B3 File Offset: 0x000356B3
		private bool hasOneshotProtection
		{
			get
			{
				return this.body && this.body.isPlayerControlled;
			}
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x000374CF File Offset: 0x000356CF
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			HealthComponent.AssetReferences.Resolve();
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x000374D8 File Offset: 0x000356D8
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.painAnimationHandler = base.GetComponent<IPainAnimationHandler>();
			this.onIncomingDamageReceivers = base.GetComponents<IOnIncomingDamageServerReceiver>();
			this.onTakeDamageReceivers = base.GetComponents<IOnTakeDamageServerReceiver>();
			this.lastHitTime = Run.FixedTimeStamp.negativeInfinity;
			this.lastHealTime = Run.FixedTimeStamp.negativeInfinity;
			this.body.onInventoryChanged += this.OnInventoryChanged;
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x0003754E File Offset: 0x0003574E
		private void OnDestroy()
		{
			this.body.onInventoryChanged -= this.OnInventoryChanged;
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x00037567 File Offset: 0x00035767
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.ServerFixedUpdate();
			}
			if (!this.alive && this.wasAlive)
			{
				this.wasAlive = false;
				CharacterDeathBehavior component = base.GetComponent<CharacterDeathBehavior>();
				if (component == null)
				{
					return;
				}
				component.OnDeath();
			}
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x000375A0 File Offset: 0x000357A0
		private void ServerFixedUpdate()
		{
			if (this.alive)
			{
				this.regenAccumulator += this.body.regen * Time.fixedDeltaTime;
				if (this.barrier > 0f)
				{
					this.Networkbarrier = Mathf.Max(this.barrier - this.body.barrierDecayRate * Time.fixedDeltaTime, 0f);
				}
				if (this.regenAccumulator > 1f)
				{
					float num = Mathf.Floor(this.regenAccumulator);
					this.regenAccumulator -= num;
					this.Heal(num, default(ProcChainMask), false);
				}
				if (this.regenAccumulator < -1f)
				{
					float num2 = Mathf.Ceil(this.regenAccumulator);
					this.regenAccumulator -= num2;
					this.Networkhealth = this.health + num2;
					if (this.health <= 0f)
					{
						this.Suicide(null, null, DamageType.Generic);
					}
				}
				float num3 = this.shield;
				bool flag = num3 >= this.body.maxShield;
				if (this.body.outOfDanger && !flag)
				{
					num3 += this.body.maxShield * 0.5f * Time.fixedDeltaTime;
					if (num3 > this.body.maxShield)
					{
						num3 = this.body.maxShield;
					}
				}
				if (num3 >= this.body.maxShield && !flag)
				{
					Util.PlaySound("Play_item_proc_personal_shield_end", base.gameObject);
				}
				if (!num3.Equals(this.shield))
				{
					this.Networkshield = num3;
				}
				if (this.devilOrbHealPool > 0f)
				{
					this.devilOrbTimer -= Time.fixedDeltaTime;
					if (this.devilOrbTimer <= 0f)
					{
						this.devilOrbTimer += 0.1f;
						float scale = 1f;
						float num4 = this.fullCombinedHealth / 10f;
						float num5 = 2.5f;
						this.devilOrbHealPool -= num4;
						DevilOrb devilOrb = new DevilOrb();
						devilOrb.origin = this.body.aimOriginTransform.position;
						devilOrb.damageValue = num4 * num5;
						devilOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
						devilOrb.attacker = base.gameObject;
						devilOrb.damageColorIndex = DamageColorIndex.Poison;
						devilOrb.scale = scale;
						devilOrb.procChainMask.AddProc(ProcType.HealNova);
						devilOrb.effectType = DevilOrb.EffectType.Skull;
						HurtBox hurtBox = devilOrb.PickNextTarget(devilOrb.origin, 40f);
						if (hurtBox)
						{
							devilOrb.target = hurtBox;
							devilOrb.isCrit = Util.CheckRoll(this.body.crit, this.body.master);
							OrbManager.instance.AddOrb(devilOrb);
						}
					}
				}
			}
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x00037860 File Offset: 0x00035A60
		private static void SendDamageDealt(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			NetworkServer.SendToAll(60, new DamageDealtMessage
			{
				victim = damageReport.victim.gameObject,
				damage = damageReport.damageDealt,
				attacker = damageInfo.attacker,
				position = damageInfo.position,
				crit = damageInfo.crit,
				damageType = damageInfo.damageType,
				damageColorIndex = damageInfo.damageColorIndex
			});
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x000378DC File Offset: 0x00035ADC
		[NetworkMessageHandler(msgType = 60, client = true)]
		private static void HandleDamageDealt(NetworkMessage netMsg)
		{
			DamageDealtMessage damageDealtMessage = netMsg.ReadMessage<DamageDealtMessage>();
			if (damageDealtMessage.victim)
			{
				HealthComponent component = damageDealtMessage.victim.GetComponent<HealthComponent>();
				if (component && !NetworkServer.active)
				{
					component.UpdateLastHitTime(damageDealtMessage.damage, damageDealtMessage.position, damageDealtMessage.isSilent, damageDealtMessage.attacker);
				}
			}
			if (RoR2Application.enableDamageNumbers.value && DamageNumberManager.instance)
			{
				TeamComponent teamComponent = null;
				if (damageDealtMessage.attacker)
				{
					teamComponent = damageDealtMessage.attacker.GetComponent<TeamComponent>();
				}
				DamageNumberManager.instance.SpawnDamageNumber(damageDealtMessage.damage, damageDealtMessage.position, damageDealtMessage.crit, teamComponent ? teamComponent.teamIndex : TeamIndex.None, damageDealtMessage.damageColorIndex);
			}
			GlobalEventManager.ClientDamageNotified(damageDealtMessage);
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x000379A4 File Offset: 0x00035BA4
		private static void SendHeal(GameObject target, float amount, bool isCrit)
		{
			NetworkServer.SendToAll(61, new HealthComponent.HealMessage
			{
				target = target,
				amount = (isCrit ? (-amount) : amount)
			});
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x000379D8 File Offset: 0x00035BD8
		[NetworkMessageHandler(msgType = 61, client = true)]
		private static void HandleHeal(NetworkMessage netMsg)
		{
			HealthComponent.HealMessage healMessage = netMsg.ReadMessage<HealthComponent.HealMessage>();
			if (RoR2Application.enableDamageNumbers.value && healMessage.target && DamageNumberManager.instance)
			{
				DamageNumberManager.instance.SpawnDamageNumber(healMessage.amount, Util.GetCorePosition(healMessage.target), healMessage.amount < 0f, TeamIndex.None, DamageColorIndex.Heal);
			}
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x00037A3C File Offset: 0x00035C3C
		private void OnInventoryChanged()
		{
			if (NetworkServer.active)
			{
				this.repeatHealCount = this.body.inventory.GetItemCount(ItemIndex.RepeatHeal);
				this.increaseHealingCount = this.body.inventory.GetItemCount(ItemIndex.IncreaseHealing);
				this.barrierOnOverHealCount = this.body.inventory.GetItemCount(ItemIndex.BarrierOnOverHeal);
				bool flag = this.repeatHealCount != 0;
				if (flag != this.repeatHealComponent)
				{
					if (flag)
					{
						this.repeatHealComponent = base.gameObject.AddComponent<HealthComponent.RepeatHealComponent>();
						this.repeatHealComponent.healthComponent = this;
						return;
					}
					UnityEngine.Object.Destroy(this.repeatHealComponent);
					this.repeatHealComponent = null;
				}
			}
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x00037AE8 File Offset: 0x00035CE8
		public HealthComponent.HealthBarValues GetHealthBarValues()
		{
			float num = 1f - 1f / this.body.cursePenalty;
			float num2 = (1f - num) / this.fullCombinedHealth;
			return new HealthComponent.HealthBarValues
			{
				hasInfusion = (this.body.inventory && (float)this.body.inventory.GetItemCount(ItemIndex.Infusion) > 0f),
				isElite = this.body.isElite,
				isBoss = this.body.isBoss,
				cullFraction = ((this.isInFrozenState && (this.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToExecutes) == CharacterBody.BodyFlags.None) ? Mathf.Clamp01(0.3f * this.fullCombinedHealth * num2) : 0f),
				healthFraction = Mathf.Clamp01(this.health * num2),
				shieldFraction = Mathf.Clamp01(this.shield * num2),
				barrierFraction = Mathf.Clamp01(this.barrier * num2),
				curseFraction = num,
				healthDisplayValue = (int)this.combinedHealth,
				maxHealthDisplayValue = (int)this.fullHealth
			};
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x00037C40 File Offset: 0x00035E40
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x00037C53 File Offset: 0x00035E53
		public float Networkhealth
		{
			get
			{
				return this.health;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.health, 1U);
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x00037C68 File Offset: 0x00035E68
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x00037C7B File Offset: 0x00035E7B
		public float Networkshield
		{
			get
			{
				return this.shield;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.shield, 2U);
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00037C90 File Offset: 0x00035E90
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x00037CA3 File Offset: 0x00035EA3
		public float Networkbarrier
		{
			get
			{
				return this.barrier;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.barrier, 4U);
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x00037CB8 File Offset: 0x00035EB8
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x00037CCB File Offset: 0x00035ECB
		public uint Network_killingDamageType
		{
			get
			{
				return this._killingDamageType;
			}
			[param: In]
			set
			{
				base.SetSyncVar<uint>(value, ref this._killingDamageType, 8U);
			}
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x00037CDF File Offset: 0x00035EDF
		protected static void InvokeCmdCmdAddBarrier(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdAddBarrier called on client.");
				return;
			}
			((HealthComponent)obj).CmdAddBarrier(reader.ReadSingle());
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00037D0C File Offset: 0x00035F0C
		public void CallCmdAddBarrier(float value)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdAddBarrier called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdAddBarrier(value);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)HealthComponent.kCmdCmdAddBarrier);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(value);
			base.SendCommandInternal(networkWriter, 0, "CmdAddBarrier");
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00037D96 File Offset: 0x00035F96
		static HealthComponent()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(HealthComponent), HealthComponent.kCmdCmdAddBarrier, new NetworkBehaviour.CmdDelegate(HealthComponent.InvokeCmdCmdAddBarrier));
			NetworkCRC.RegisterBehaviour("HealthComponent", 0);
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x00037DD4 File Offset: 0x00035FD4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.health);
				writer.Write(this.shield);
				writer.Write(this.barrier);
				writer.WritePackedUInt32(this._killingDamageType);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.health);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.shield);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.barrier);
			}
			if ((base.syncVarDirtyBits & 8U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32(this._killingDamageType);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x00037EFC File Offset: 0x000360FC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.health = reader.ReadSingle();
				this.shield = reader.ReadSingle();
				this.barrier = reader.ReadSingle();
				this._killingDamageType = reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.health = reader.ReadSingle();
			}
			if ((num & 2) != 0)
			{
				this.shield = reader.ReadSingle();
			}
			if ((num & 4) != 0)
			{
				this.barrier = reader.ReadSingle();
			}
			if ((num & 8) != 0)
			{
				this._killingDamageType = reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04000C1F RID: 3103
		[Tooltip("How much health this object has.")]
		[HideInInspector]
		[SyncVar]
		public float health = 100f;

		// Token: 0x04000C20 RID: 3104
		[Tooltip("How much shield this object has.")]
		[HideInInspector]
		[SyncVar]
		public float shield;

		// Token: 0x04000C21 RID: 3105
		[SyncVar]
		[Tooltip("How much barrier this object has.")]
		[HideInInspector]
		public float barrier;

		// Token: 0x04000C22 RID: 3106
		[SyncVar]
		private uint _killingDamageType;

		// Token: 0x04000C23 RID: 3107
		public CharacterBody body;

		// Token: 0x04000C24 RID: 3108
		private ModelLocator modelLocator;

		// Token: 0x04000C25 RID: 3109
		private IPainAnimationHandler painAnimationHandler;

		// Token: 0x04000C26 RID: 3110
		private IOnIncomingDamageServerReceiver[] onIncomingDamageReceivers;

		// Token: 0x04000C27 RID: 3111
		private IOnTakeDamageServerReceiver[] onTakeDamageReceivers;

		// Token: 0x04000C2C RID: 3116
		public bool dontShowHealthbar;

		// Token: 0x04000C2D RID: 3117
		public float globalDeathEventChanceCoefficient = 1f;

		// Token: 0x04000C2E RID: 3118
		private float devilOrbHealPool;

		// Token: 0x04000C2F RID: 3119
		private float devilOrbTimer;

		// Token: 0x04000C30 RID: 3120
		private const float devilOrbMaxTimer = 0.1f;

		// Token: 0x04000C31 RID: 3121
		private float regenAccumulator;

		// Token: 0x04000C32 RID: 3122
		private bool wasAlive = true;

		// Token: 0x04000C33 RID: 3123
		public const float medkitActivationDelay = 1.1f;

		// Token: 0x04000C36 RID: 3126
		public const float frozenExecuteThreshold = 0.3f;

		// Token: 0x04000C38 RID: 3128
		private int increaseHealingCount;

		// Token: 0x04000C39 RID: 3129
		private int barrierOnOverHealCount;

		// Token: 0x04000C3A RID: 3130
		private int repeatHealCount;

		// Token: 0x04000C3B RID: 3131
		private HealthComponent.RepeatHealComponent repeatHealComponent;

		// Token: 0x04000C3C RID: 3132
		private static int kCmdCmdAddBarrier = -1976809257;

		// Token: 0x02000227 RID: 551
		private static class AssetReferences
		{
			// Token: 0x06000C67 RID: 3175 RVA: 0x00037FAC File Offset: 0x000361AC
			public static void Resolve()
			{
				HealthComponent.AssetReferences.bearEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/BearProc");
				HealthComponent.AssetReferences.executeEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactExecute");
				HealthComponent.AssetReferences.stealthKitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ProcStealthkit");
				HealthComponent.AssetReferences.shieldBreakEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ShieldBreakEffect");
				HealthComponent.AssetReferences.coinImpactEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact");
				HealthComponent.AssetReferences.damageRejectedPrefab = Resources.Load<GameObject>("Prefabs/Effects/DamageRejected");
				HealthComponent.AssetReferences.bossDamageBonusImpactEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactBossDamageBonus");
				HealthComponent.AssetReferences.pulverizedEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/PulverizedEffect");
				HealthComponent.AssetReferences.diamondDamageBonusImpactEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/DiamondDamageBonusEffect");
				HealthComponent.AssetReferences.crowbarImpactEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactCrowbar");
			}

			// Token: 0x04000C3D RID: 3133
			public static GameObject bearEffectPrefab;

			// Token: 0x04000C3E RID: 3134
			public static GameObject executeEffectPrefab;

			// Token: 0x04000C3F RID: 3135
			public static GameObject stealthKitEffectPrefab;

			// Token: 0x04000C40 RID: 3136
			public static GameObject shieldBreakEffectPrefab;

			// Token: 0x04000C41 RID: 3137
			public static GameObject coinImpactEffectPrefab;

			// Token: 0x04000C42 RID: 3138
			public static GameObject damageRejectedPrefab;

			// Token: 0x04000C43 RID: 3139
			public static GameObject bossDamageBonusImpactEffectPrefab;

			// Token: 0x04000C44 RID: 3140
			public static GameObject pulverizedEffectPrefab;

			// Token: 0x04000C45 RID: 3141
			public static GameObject diamondDamageBonusImpactEffectPrefab;

			// Token: 0x04000C46 RID: 3142
			public static GameObject crowbarImpactEffectPrefab;
		}

		// Token: 0x02000228 RID: 552
		private class HealMessage : MessageBase
		{
			// Token: 0x06000C69 RID: 3177 RVA: 0x0003804F File Offset: 0x0003624F
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.target);
				writer.Write(this.amount);
			}

			// Token: 0x06000C6A RID: 3178 RVA: 0x00038069 File Offset: 0x00036269
			public override void Deserialize(NetworkReader reader)
			{
				this.target = reader.ReadGameObject();
				this.amount = reader.ReadSingle();
			}

			// Token: 0x04000C47 RID: 3143
			public GameObject target;

			// Token: 0x04000C48 RID: 3144
			public float amount;
		}

		// Token: 0x02000229 RID: 553
		public struct HealthBarValues
		{
			// Token: 0x04000C49 RID: 3145
			public bool hasInfusion;

			// Token: 0x04000C4A RID: 3146
			public bool isElite;

			// Token: 0x04000C4B RID: 3147
			public bool isBoss;

			// Token: 0x04000C4C RID: 3148
			public float cullFraction;

			// Token: 0x04000C4D RID: 3149
			public float healthFraction;

			// Token: 0x04000C4E RID: 3150
			public float shieldFraction;

			// Token: 0x04000C4F RID: 3151
			public float barrierFraction;

			// Token: 0x04000C50 RID: 3152
			public float curseFraction;

			// Token: 0x04000C51 RID: 3153
			public int healthDisplayValue;

			// Token: 0x04000C52 RID: 3154
			public int maxHealthDisplayValue;
		}

		// Token: 0x0200022A RID: 554
		private class RepeatHealComponent : MonoBehaviour
		{
			// Token: 0x06000C6B RID: 3179 RVA: 0x00038084 File Offset: 0x00036284
			private void FixedUpdate()
			{
				this.timer -= Time.fixedDeltaTime;
				if (this.timer <= 0f)
				{
					this.timer = 0.2f;
					if (this.reserve > 0f)
					{
						float num = Mathf.Min(this.healthComponent.fullHealth * this.healthFractionToRestorePerSecond * 0.2f, this.reserve);
						this.reserve -= num;
						ProcChainMask procChainMask = default(ProcChainMask);
						procChainMask.AddProc(ProcType.RepeatHeal);
						this.healthComponent.Heal(num, procChainMask, true);
					}
				}
			}

			// Token: 0x06000C6C RID: 3180 RVA: 0x0003811A File Offset: 0x0003631A
			public void AddReserve(float amount, float max)
			{
				this.reserve = Mathf.Min(this.reserve + amount, max);
			}

			// Token: 0x04000C53 RID: 3155
			private float reserve;

			// Token: 0x04000C54 RID: 3156
			private float timer;

			// Token: 0x04000C55 RID: 3157
			private const float interval = 0.2f;

			// Token: 0x04000C56 RID: 3158
			public float healthFractionToRestorePerSecond = 0.1f;

			// Token: 0x04000C57 RID: 3159
			public HealthComponent healthComponent;
		}
	}
}
