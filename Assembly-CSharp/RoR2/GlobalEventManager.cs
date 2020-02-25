using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000218 RID: 536
	public class GlobalEventManager : MonoBehaviour
	{
		// Token: 0x06000BBF RID: 3007 RVA: 0x000332AE File Offset: 0x000314AE
		private void OnEnable()
		{
			if (GlobalEventManager.instance)
			{
				Debug.LogError("Only one GlobalEventManager can exist at a time.");
				return;
			}
			GlobalEventManager.instance = this;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x000332CD File Offset: 0x000314CD
		private void OnDisable()
		{
			if (GlobalEventManager.instance == this)
			{
				GlobalEventManager.instance = null;
			}
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnLocalPlayerBodySpawn(CharacterBody body)
		{
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnCharacterBodySpawn(CharacterBody body)
		{
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnCharacterBodyStart(CharacterBody body)
		{
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x000332E4 File Offset: 0x000314E4
		public void OnHitEnemy(DamageInfo damageInfo, GameObject victim)
		{
			if (damageInfo.procCoefficient == 0f || damageInfo.rejected)
			{
				return;
			}
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageInfo.attacker && damageInfo.procCoefficient > 0f)
			{
				CharacterBody component = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody characterBody = victim ? victim.GetComponent<CharacterBody>() : null;
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						Inventory inventory = master.inventory;
						TeamComponent component2 = component.GetComponent<TeamComponent>();
						TeamIndex teamIndex = component2 ? component2.teamIndex : TeamIndex.Neutral;
						Vector3 aimOrigin = component.aimOrigin;
						if (damageInfo.crit)
						{
							GlobalEventManager.instance.OnCrit(component, master, damageInfo.procCoefficient, damageInfo.procChainMask);
						}
						if (!damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
						{
							int itemCount = inventory.GetItemCount(ItemIndex.Seed);
							if (itemCount > 0)
							{
								HealthComponent component3 = component.GetComponent<HealthComponent>();
								if (component3)
								{
									ProcChainMask procChainMask = damageInfo.procChainMask;
									procChainMask.AddProc(ProcType.HealOnHit);
									component3.Heal((float)itemCount * damageInfo.procCoefficient, procChainMask, true);
								}
							}
						}
						int itemCount2 = inventory.GetItemCount(ItemIndex.StunChanceOnHit);
						if (itemCount2 > 0 && Util.CheckRoll(Util.ConvertAmplificationPercentageIntoReductionPercentage(5f * (float)itemCount2), master))
						{
							SetStateOnHurt component4 = victim.GetComponent<SetStateOnHurt>();
							if (component4)
							{
								component4.SetStun(2f);
							}
						}
						if (!damageInfo.procChainMask.HasProc(ProcType.BleedOnHit))
						{
							int itemCount3 = inventory.GetItemCount(ItemIndex.BleedOnHit);
							bool flag = (damageInfo.damageType & DamageType.BleedOnHit) > DamageType.Generic;
							if ((itemCount3 > 0 || flag) && (flag || Util.CheckRoll(15f * (float)itemCount3 * damageInfo.procCoefficient, master)))
							{
								ProcChainMask procChainMask2 = damageInfo.procChainMask;
								procChainMask2.AddProc(ProcType.BleedOnHit);
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Bleed, 3f * damageInfo.procCoefficient, 1f);
							}
						}
						bool flag2 = (damageInfo.damageType & DamageType.PoisonOnHit) > DamageType.Generic;
						if (flag2 && flag2)
						{
							ProcChainMask procChainMask3 = damageInfo.procChainMask;
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Poison, 10f * damageInfo.procCoefficient, 1f);
						}
						bool flag3 = (damageInfo.damageType & DamageType.WeakOnHit) > DamageType.Generic;
						if (flag3 && flag3)
						{
							characterBody.AddTimedBuff(BuffIndex.Weak, 6f * damageInfo.procCoefficient);
						}
						bool flag4 = (damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic;
						bool flag5 = (damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic || component.HasBuff(BuffIndex.AffixRed);
						if (flag4 || flag5)
						{
							DotController.InflictDot(victim, damageInfo.attacker, flag5 ? DotController.DotIndex.PercentBurn : DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
						}
						int num = component.HasBuff(BuffIndex.AffixWhite) ? 1 : 0;
						num += (component.HasBuff(BuffIndex.AffixHaunted) ? 2 : 0);
						if (num > 0 && characterBody)
						{
							characterBody.AddTimedBuff(BuffIndex.Slow80, 1.5f * damageInfo.procCoefficient * (float)num);
						}
						int itemCount4 = master.inventory.GetItemCount(ItemIndex.SlowOnHit);
						if (itemCount4 > 0 && characterBody)
						{
							characterBody.AddTimedBuff(BuffIndex.Slow60, 2f * (float)itemCount4);
						}
						if ((component.HasBuff(BuffIndex.AffixPoison) ? 1 : 0) > 0 && characterBody)
						{
							characterBody.AddTimedBuff(BuffIndex.HealingDisabled, 8f * damageInfo.procCoefficient);
						}
						int itemCount5 = inventory.GetItemCount(ItemIndex.GoldOnHit);
						if (itemCount5 > 0 && Util.CheckRoll(30f * damageInfo.procCoefficient, master))
						{
							master.GiveMoney((uint)((float)itemCount5 * 2f * Run.instance.difficultyCoefficient));
							EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageInfo.position, Vector3.up, true);
						}
						if (!damageInfo.procChainMask.HasProc(ProcType.Missile))
						{
							this.ProcMissile(inventory.GetItemCount(ItemIndex.Missile), component, master, teamIndex, damageInfo.procChainMask, victim, damageInfo);
						}
						if (component.HasBuff(BuffIndex.LoaderPylonPowered) && !damageInfo.procChainMask.HasProc(ProcType.LoaderLightning))
						{
							float damageCoefficient = 0.3f;
							float damageValue = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient);
							LightningOrb lightningOrb = new LightningOrb();
							lightningOrb.origin = damageInfo.position;
							lightningOrb.damageValue = damageValue;
							lightningOrb.isCrit = damageInfo.crit;
							lightningOrb.bouncesRemaining = 3;
							lightningOrb.teamIndex = teamIndex;
							lightningOrb.attacker = damageInfo.attacker;
							lightningOrb.bouncedObjects = new List<HealthComponent>
							{
								victim.GetComponent<HealthComponent>()
							};
							lightningOrb.procChainMask = damageInfo.procChainMask;
							lightningOrb.procChainMask.AddProc(ProcType.LoaderLightning);
							lightningOrb.procCoefficient = 0f;
							lightningOrb.lightningType = LightningOrb.LightningType.Loader;
							lightningOrb.damageColorIndex = DamageColorIndex.Item;
							lightningOrb.range = 20f;
							HurtBox hurtBox = lightningOrb.PickNextTarget(damageInfo.position);
							if (hurtBox)
							{
								lightningOrb.target = hurtBox;
								OrbManager.instance.AddOrb(lightningOrb);
							}
						}
						int itemCount6 = inventory.GetItemCount(ItemIndex.ChainLightning);
						float num2 = 25f;
						if (itemCount6 > 0 && !damageInfo.procChainMask.HasProc(ProcType.ChainLightning) && Util.CheckRoll(num2 * damageInfo.procCoefficient, master))
						{
							float damageCoefficient2 = 0.8f;
							float damageValue2 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient2);
							LightningOrb lightningOrb2 = new LightningOrb();
							lightningOrb2.origin = damageInfo.position;
							lightningOrb2.damageValue = damageValue2;
							lightningOrb2.isCrit = damageInfo.crit;
							lightningOrb2.bouncesRemaining = 2 * itemCount6;
							lightningOrb2.teamIndex = teamIndex;
							lightningOrb2.attacker = damageInfo.attacker;
							lightningOrb2.bouncedObjects = new List<HealthComponent>
							{
								victim.GetComponent<HealthComponent>()
							};
							lightningOrb2.procChainMask = damageInfo.procChainMask;
							lightningOrb2.procChainMask.AddProc(ProcType.ChainLightning);
							lightningOrb2.procCoefficient = 0.2f;
							lightningOrb2.lightningType = LightningOrb.LightningType.Ukulele;
							lightningOrb2.damageColorIndex = DamageColorIndex.Item;
							lightningOrb2.range += (float)(2 * itemCount6);
							HurtBox hurtBox2 = lightningOrb2.PickNextTarget(damageInfo.position);
							if (hurtBox2)
							{
								lightningOrb2.target = hurtBox2;
								OrbManager.instance.AddOrb(lightningOrb2);
							}
						}
						int itemCount7 = inventory.GetItemCount(ItemIndex.BounceNearby);
						float num3 = (1f - 100f / (100f + 20f * (float)itemCount7)) * 100f;
						if (itemCount7 > 0 && !damageInfo.procChainMask.HasProc(ProcType.BounceNearby) && Util.CheckRoll(num3 * damageInfo.procCoefficient, master))
						{
							List<HealthComponent> bouncedObjects = new List<HealthComponent>
							{
								victim.GetComponent<HealthComponent>()
							};
							float damageCoefficient3 = 1f;
							float damageValue3 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient3);
							for (int i = 0; i < 5 + itemCount7 * 5; i++)
							{
								BounceOrb bounceOrb = new BounceOrb();
								bounceOrb.origin = damageInfo.position;
								bounceOrb.damageValue = damageValue3;
								bounceOrb.isCrit = damageInfo.crit;
								bounceOrb.teamIndex = teamIndex;
								bounceOrb.attacker = damageInfo.attacker;
								bounceOrb.procChainMask = damageInfo.procChainMask;
								bounceOrb.procChainMask.AddProc(ProcType.BounceNearby);
								bounceOrb.procCoefficient = 0.33f;
								bounceOrb.damageColorIndex = DamageColorIndex.Item;
								bounceOrb.bouncedObjects = bouncedObjects;
								HurtBox hurtBox3 = bounceOrb.PickNextTarget(victim.transform.position, 30f);
								if (hurtBox3)
								{
									bounceOrb.target = hurtBox3;
									OrbManager.instance.AddOrb(bounceOrb);
								}
							}
						}
						int itemCount8 = inventory.GetItemCount(ItemIndex.StickyBomb);
						if (itemCount8 > 0 && Util.CheckRoll(5f * (float)itemCount8 * damageInfo.procCoefficient, master) && characterBody)
						{
							Vector3 position = damageInfo.position;
							Vector3 forward = characterBody.corePosition - position;
							Quaternion rotation = (forward.magnitude != 0f) ? Util.QuaternionSafeLookRotation(forward) : UnityEngine.Random.rotationUniform;
							float damageCoefficient4 = 1.8f;
							float damage = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient4);
							ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/StickyBomb"), position, rotation, damageInfo.attacker, damage, 100f, damageInfo.crit, DamageColorIndex.Item, null, forward.magnitude * 60f);
						}
						int itemCount9 = inventory.GetItemCount(ItemIndex.IceRing);
						int itemCount10 = inventory.GetItemCount(ItemIndex.FireRing);
						if ((itemCount9 | itemCount10) > 0)
						{
							Vector3 position2 = damageInfo.position;
							if (Util.CheckRoll(8f * damageInfo.procCoefficient, master))
							{
								ProcChainMask procChainMask4 = damageInfo.procChainMask;
								procChainMask4.AddProc(ProcType.Rings);
								if (itemCount9 > 0)
								{
									float damageCoefficient5 = 1.25f + 1.25f * (float)itemCount9;
									float damage2 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient5);
									DamageInfo damageInfo2 = new DamageInfo
									{
										damage = damage2,
										damageColorIndex = DamageColorIndex.Item,
										damageType = DamageType.Generic,
										attacker = damageInfo.attacker,
										crit = damageInfo.crit,
										force = Vector3.zero,
										inflictor = null,
										position = position2,
										procChainMask = procChainMask4,
										procCoefficient = 1f
									};
									EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IceRingExplosion"), position2, Vector3.up, true);
									characterBody.AddTimedBuff(BuffIndex.Slow80, 3f);
									HealthComponent component5 = victim.GetComponent<HealthComponent>();
									if (component5 != null)
									{
										component5.TakeDamage(damageInfo2);
									}
								}
								if (itemCount10 > 0)
								{
									GameObject gameObject = Resources.Load<GameObject>("Prefabs/Projectiles/FireTornado");
									float resetInterval = gameObject.GetComponent<ProjectileOverlapAttack>().resetInterval;
									float lifetime = gameObject.GetComponent<ProjectileSimple>().lifetime;
									float damageCoefficient6 = 2.5f + 2.5f * (float)itemCount10;
									float damage3 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient6) / lifetime * resetInterval;
									float speedOverride = 0f;
									Quaternion rotation2 = Quaternion.identity;
									Vector3 vector = position2 - aimOrigin;
									vector.y = 0f;
									if (vector != Vector3.zero)
									{
										speedOverride = -1f;
										rotation2 = Util.QuaternionSafeLookRotation(vector, Vector3.up);
									}
									ProjectileManager.instance.FireProjectile(new FireProjectileInfo
									{
										damage = damage3,
										crit = damageInfo.crit,
										damageColorIndex = DamageColorIndex.Item,
										position = position2,
										procChainMask = procChainMask4,
										force = 0f,
										owner = damageInfo.attacker,
										projectilePrefab = gameObject,
										rotation = rotation2,
										speedOverride = speedOverride,
										target = null
									});
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00033D38 File Offset: 0x00031F38
		private void ProcMissile(int stack, CharacterBody attackerBody, CharacterMaster attackerMaster, TeamIndex attackerTeamIndex, ProcChainMask procChainMask, GameObject victim, DamageInfo damageInfo)
		{
			if (stack > 0)
			{
				GameObject gameObject = attackerBody.gameObject;
				InputBankTest component = gameObject.GetComponent<InputBankTest>();
				Vector3 position = component ? component.aimOrigin : gameObject.transform.position;
				Vector3 vector = component ? component.aimDirection : gameObject.transform.forward;
				Vector3 up = Vector3.up;
				if (Util.CheckRoll(10f * damageInfo.procCoefficient, attackerMaster))
				{
					float damageCoefficient = 3f * (float)stack;
					float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);
					ProcChainMask procChainMask2 = procChainMask;
					procChainMask2.AddProc(ProcType.Missile);
					FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
					{
						projectilePrefab = this.missilePrefab,
						position = position,
						rotation = Util.QuaternionSafeLookRotation(up),
						procChainMask = procChainMask2,
						target = victim,
						owner = gameObject,
						damage = damage,
						crit = damageInfo.crit,
						force = 200f,
						damageColorIndex = DamageColorIndex.Item
					};
					ProjectileManager.instance.FireProjectile(fireProjectileInfo);
				}
			}
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00033E5C File Offset: 0x0003205C
		public void OnCharacterHitGround(CharacterBody characterBody, Vector3 impactVelocity)
		{
			float num = Mathf.Abs(impactVelocity.y);
			Inventory inventory = characterBody.inventory;
			CharacterMaster master = characterBody.master;
			CharacterMotor characterMotor = characterBody.characterMotor;
			bool flag = false;
			if ((inventory ? inventory.GetItemCount(ItemIndex.FallBoots) : 0) <= 0 && (characterBody.bodyFlags & CharacterBody.BodyFlags.IgnoreFallDamage) == CharacterBody.BodyFlags.None)
			{
				float num2 = Mathf.Max(num - (characterBody.jumpPower + 20f), 0f);
				if (num2 > 0f)
				{
					flag = true;
					float num3 = num2 / 60f;
					HealthComponent component = characterBody.GetComponent<HealthComponent>();
					if (component)
					{
						component.TakeDamage(new DamageInfo
						{
							attacker = null,
							inflictor = null,
							crit = false,
							damage = num3 * characterBody.maxHealth,
							damageType = DamageType.NonLethal,
							force = Vector3.zero,
							position = characterBody.footPosition,
							procCoefficient = 0f
						});
					}
				}
			}
			if (characterMotor && Run.FixedTimeStamp.now - characterMotor.lastGroundedTime > 0.2f)
			{
				Vector3 footPosition = characterBody.footPosition;
				float radius = characterBody.radius;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(footPosition + Vector3.up * 1.5f, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask | LayerIndex.water.mask, QueryTriggerInteraction.Collide))
				{
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(raycastHit.collider, raycastHit.point);
					if (objectSurfaceDef)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
						{
							origin = footPosition,
							scale = radius,
							color = objectSurfaceDef.approximateColor
						}, true);
						if (objectSurfaceDef.footstepEffectPrefab)
						{
							EffectManager.SpawnEffect(objectSurfaceDef.footstepEffectPrefab, new EffectData
							{
								origin = raycastHit.point,
								scale = radius * 3f
							}, false);
						}
						SfxLocator component2 = characterBody.GetComponent<SfxLocator>();
						if (component2)
						{
							if (objectSurfaceDef.materialSwitchString != null && objectSurfaceDef.materialSwitchString.Length > 0)
							{
								AkSoundEngine.SetSwitch("material", objectSurfaceDef.materialSwitchString, characterBody.gameObject);
							}
							else
							{
								AkSoundEngine.SetSwitch("material", "dirt", characterBody.gameObject);
							}
							Util.PlaySound(component2.landingSound, characterBody.gameObject);
							if (flag)
							{
								Util.PlaySound(component2.fallDamageSound, characterBody.gameObject);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00034100 File Offset: 0x00032300
		private void OnPlayerCharacterDeath(DamageReport damageReport, NetworkUser victimNetworkUser)
		{
			if (!victimNetworkUser)
			{
				return;
			}
			CharacterBody victimBody = damageReport.victimBody;
			string baseToken;
			if ((damageReport.damageInfo.damageType & DamageType.VoidDeath) != DamageType.Generic)
			{
				baseToken = "PLAYER_DEATH_QUOTE_VOIDDEATH";
			}
			else if (victimBody && victimBody.inventory && victimBody.inventory.GetItemCount(ItemIndex.LunarDagger) > 0)
			{
				baseToken = "PLAYER_DEATH_QUOTE_BRITTLEDEATH";
			}
			else
			{
				baseToken = GlobalEventManager.standardDeathQuoteTokens[UnityEngine.Random.Range(0, GlobalEventManager.standardDeathQuoteTokens.Length)];
			}
			Chat.SendBroadcastChat(new Chat.PlayerDeathChatMessage
			{
				subjectAsNetworkUser = victimNetworkUser,
				baseToken = baseToken
			});
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06000BC8 RID: 3016 RVA: 0x00034194 File Offset: 0x00032394
		// (remove) Token: 0x06000BC9 RID: 3017 RVA: 0x000341C8 File Offset: 0x000323C8
		public static event Action<DamageReport> onCharacterDeathGlobal;

		// Token: 0x06000BCA RID: 3018 RVA: 0x000341FC File Offset: 0x000323FC
		public void OnCharacterDeath(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			DamageInfo damageInfo = damageReport.damageInfo;
			GameObject gameObject = damageReport.victim.gameObject;
			CharacterBody victimBody = damageReport.victimBody;
			TeamComponent teamComponent = victimBody.teamComponent;
			CharacterMaster victimMaster = damageReport.victimMaster;
			TeamIndex teamIndex = damageReport.victimTeamIndex;
			Transform transform = gameObject.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Vector3 vector = position;
			InputBankTest inputBankTest = null;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			if (victimBody)
			{
				inputBankTest = victimBody.inputBank;
				vector = victimBody.corePosition;
				if (victimBody.equipmentSlot)
				{
					equipmentIndex = victimBody.equipmentSlot.equipmentIndex;
				}
			}
			Ray ray = inputBankTest ? inputBankTest.GetAimRay() : new Ray(position, rotation * Vector3.forward);
			CharacterBody attackerBody = damageReport.attackerBody;
			CharacterMaster attackerMaster = damageReport.attackerMaster;
			Inventory inventory = attackerMaster ? attackerMaster.inventory : null;
			TeamIndex attackerTeamIndex = damageReport.attackerTeamIndex;
			if (teamComponent)
			{
				teamIndex = teamComponent.teamIndex;
				if (teamIndex == TeamIndex.Monster && Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Bomb))
				{
					Debug.Log("team and artifact OK");
					HurtBoxGroup hurtBoxGroup = victimBody.hurtBoxGroup;
					if (hurtBoxGroup)
					{
						Debug.Log("victimHurtBoxGroup OK");
						float damage = 0f;
						if (victimBody)
						{
							damage = victimBody.damage;
						}
						HurtBoxGroup.VolumeDistribution volumeDistribution = hurtBoxGroup.GetVolumeDistribution();
						int num = Mathf.CeilToInt(volumeDistribution.totalVolume / 10f);
						Debug.LogFormat("bombCount={0}", new object[]
						{
							num
						});
						for (int i = 0; i < num; i++)
						{
							ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/Funball"), volumeDistribution.randomVolumePoint, Quaternion.identity, gameObject, damage, 700f, false, DamageColorIndex.Default, null, -1f);
						}
					}
				}
			}
			if (victimBody && victimMaster)
			{
				PlayerCharacterMasterController playerCharacterMasterController = victimMaster.playerCharacterMasterController;
				if (playerCharacterMasterController)
				{
					NetworkUser networkUser = playerCharacterMasterController.networkUser;
					if (networkUser)
					{
						this.OnPlayerCharacterDeath(damageReport, networkUser);
					}
				}
				if (victimBody.HasBuff(BuffIndex.AffixWhite))
				{
					Vector3 corePosition = Util.GetCorePosition(gameObject);
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
					float num2 = 12f + victimBody.radius;
					gameObject2.transform.localScale = new Vector3(num2, num2, num2);
					DelayBlast component = gameObject2.GetComponent<DelayBlast>();
					component.position = corePosition;
					component.baseDamage = victimBody.damage * 1.5f;
					component.baseForce = 2300f;
					component.attacker = victimBody.gameObject;
					component.radius = num2;
					component.crit = Util.CheckRoll(victimBody.crit, victimMaster);
					component.procCoefficient = 0.75f;
					component.maxTimer = 2f;
					component.falloffModel = BlastAttack.FalloffModel.None;
					component.explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
					component.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/AffixWhiteDelayEffect");
					component.damageType = DamageType.Freeze2s;
					gameObject2.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component.attacker);
				}
				if (victimBody.HasBuff(BuffIndex.AffixPoison))
				{
					Vector3 position2 = vector;
					Quaternion rotation2 = Quaternion.LookRotation(ray.direction);
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/CharacterMasters/UrchinTurretMaster"), position2, rotation2);
					CharacterMaster component2 = gameObject3.GetComponent<CharacterMaster>();
					component2.teamIndex = teamIndex;
					NetworkServer.Spawn(gameObject3);
					component2.SpawnBodyHere();
				}
			}
			if (attackerBody && attackerMaster)
			{
				int itemCount = inventory.GetItemCount(ItemIndex.IgniteOnKill);
				if (itemCount > 0)
				{
					ReadOnlyCollection<CharacterBody> readOnlyInstancesList = CharacterBody.readOnlyInstancesList;
					float num3 = 8f + 4f * (float)itemCount;
					float radius = victimBody.radius;
					float num4 = num3 + radius;
					float num5 = num4 * num4;
					Vector3 corePosition2 = Util.GetCorePosition(gameObject);
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX"), new EffectData
					{
						origin = corePosition2,
						scale = num4,
						rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
					}, true);
					for (int j = 0; j < readOnlyInstancesList.Count; j++)
					{
						CharacterBody characterBody = readOnlyInstancesList[j];
						if (characterBody.teamComponent.teamIndex != attackerTeamIndex && (readOnlyInstancesList[j].transform.position - corePosition2).sqrMagnitude <= num5)
						{
							DotController.InflictDot(characterBody.gameObject, damageInfo.attacker, DotController.DotIndex.Burn, 1.5f + 1.5f * (float)itemCount, 1f);
						}
					}
				}
				int itemCount2 = inventory.GetItemCount(ItemIndex.ExplodeOnDeath);
				if (itemCount2 > 0)
				{
					Vector3 corePosition3 = Util.GetCorePosition(gameObject);
					float damageCoefficient = 3.5f * (1f + (float)(itemCount2 - 1) * 0.8f);
					float baseDamage = Util.OnKillProcDamage(attackerBody.damage, damageCoefficient);
					GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.explodeOnDeathPrefab, corePosition3, Quaternion.identity);
					DelayBlast component3 = gameObject4.GetComponent<DelayBlast>();
					component3.position = corePosition3;
					component3.baseDamage = baseDamage;
					component3.baseForce = 2000f;
					component3.bonusForce = Vector3.up * 1000f;
					component3.radius = 12f + 2.4f * ((float)itemCount2 - 1f);
					component3.attacker = damageInfo.attacker;
					component3.inflictor = null;
					component3.crit = Util.CheckRoll(attackerBody.crit, attackerMaster);
					component3.maxTimer = 0.5f;
					component3.damageColorIndex = DamageColorIndex.Item;
					component3.falloffModel = BlastAttack.FalloffModel.SweetSpot;
					gameObject4.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
					NetworkServer.Spawn(gameObject4);
				}
				int itemCount3 = inventory.GetItemCount(ItemIndex.Dagger);
				if (itemCount3 > 0)
				{
					float damageCoefficient2 = 1.5f * (float)itemCount3;
					Vector3 a = gameObject.transform.position + Vector3.up * 1.8f;
					for (int k = 0; k < 3; k++)
					{
						ProjectileManager.instance.FireProjectile(this.daggerPrefab, a + UnityEngine.Random.insideUnitSphere * 0.5f, Util.QuaternionSafeLookRotation(Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f), attackerBody.gameObject, Util.OnKillProcDamage(attackerBody.damage, damageCoefficient2), 200f, Util.CheckRoll(attackerBody.crit, attackerMaster), DamageColorIndex.Item, null, -1f);
					}
				}
				int itemCount4 = inventory.GetItemCount(ItemIndex.Tooth);
				if (itemCount4 > 0)
				{
					float num6 = Mathf.Pow((float)itemCount4, 0.25f);
					GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), gameObject.transform.position, UnityEngine.Random.rotation);
					gameObject5.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
					gameObject5.GetComponentInChildren<HealthPickup>().flatHealing = 8f * (float)itemCount4;
					gameObject5.transform.localScale = new Vector3(num6, num6, num6);
					NetworkServer.Spawn(gameObject5);
				}
				int itemCount5 = inventory.GetItemCount(ItemIndex.Infusion);
				if (itemCount5 > 0)
				{
					int num7 = itemCount5 * 100;
					if ((ulong)inventory.infusionBonus < (ulong)((long)num7))
					{
						InfusionOrb infusionOrb = new InfusionOrb();
						infusionOrb.origin = gameObject.transform.position;
						infusionOrb.target = Util.FindBodyMainHurtBox(attackerBody);
						infusionOrb.maxHpValue = 1;
						OrbManager.instance.AddOrb(infusionOrb);
					}
				}
				if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) == DamageType.ResetCooldownsOnKill)
				{
					SkillLocator skillLocator = attackerBody.skillLocator;
					if (skillLocator)
					{
						skillLocator.ResetSkills();
					}
				}
				if (inventory)
				{
					int itemCount6 = inventory.GetItemCount(ItemIndex.Talisman);
					if (itemCount6 > 0 && attackerBody.equipmentSlot)
					{
						inventory.DeductActiveEquipmentCooldown(2f + (float)itemCount6 * 2f);
					}
				}
				int itemCount7 = inventory.GetItemCount(ItemIndex.TempestOnKill);
				if (itemCount7 > 0 && Util.CheckRoll(25f, attackerMaster))
				{
					GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/TempestWard"), victimBody.footPosition, Quaternion.identity);
					gameObject6.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
					gameObject6.GetComponent<BuffWard>().expireDuration = 2f + 6f * (float)itemCount7;
					NetworkServer.Spawn(gameObject6);
				}
				int itemCount8 = inventory.GetItemCount(ItemIndex.Bandolier);
				if (itemCount8 > 0 && Util.CheckRoll((1f - 1f / Mathf.Pow((float)(itemCount8 + 1), 0.33f)) * 100f, attackerMaster))
				{
					GameObject gameObject7 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack"), gameObject.transform.position, UnityEngine.Random.rotation);
					gameObject7.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
					NetworkServer.Spawn(gameObject7);
				}
				if (victimBody && damageReport.victimIsElite)
				{
					int itemCount9 = inventory.GetItemCount(ItemIndex.HeadHunter);
					int itemCount10 = inventory.GetItemCount(ItemIndex.KillEliteFrenzy);
					if (itemCount9 > 0)
					{
						float duration = 3f + 5f * (float)itemCount9;
						for (int l = 0; l < BuffCatalog.eliteBuffIndices.Length; l++)
						{
							BuffIndex buffType = BuffCatalog.eliteBuffIndices[l];
							if (victimBody.HasBuff(buffType))
							{
								attackerBody.AddTimedBuff(buffType, duration);
							}
						}
					}
					if (itemCount10 > 0)
					{
						attackerBody.AddTimedBuff(BuffIndex.NoCooldowns, (float)itemCount10 * 4f);
					}
				}
				int itemCount11 = inventory.GetItemCount(ItemIndex.GhostOnKill);
				if (itemCount11 > 0 && victimBody && Util.CheckRoll(7f, attackerMaster))
				{
					Util.TryToCreateGhost(victimBody, attackerBody, itemCount11 * 30);
				}
				if (Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Sacrifice))
				{
					DeathRewards component4 = victimBody.GetComponent<DeathRewards>();
					if (component4)
					{
						float num8 = component4.expReward;
						if (Util.CheckRoll(3f * Mathf.Log(num8 + 1f, 3f), attackerMaster))
						{
							List<PickupIndex> list = Run.instance.smallChestDropTierSelector.Evaluate(UnityEngine.Random.value);
							PickupIndex pickupIndex = PickupIndex.none;
							if (list.Count > 0)
							{
								pickupIndex = list[UnityEngine.Random.Range(0, list.Count - 1)];
							}
							PickupDropletController.CreatePickupDroplet(pickupIndex, gameObject.transform.position, Vector3.up * 20f);
						}
					}
				}
				if (Util.CheckRoll(0.025f, attackerMaster) && victimBody && victimBody.isElite)
				{
					PickupDropletController.CreatePickupDroplet(new PickupIndex(equipmentIndex), victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + ray.direction * 2f);
				}
				int itemCount12 = inventory.GetItemCount(ItemIndex.BarrierOnKill);
				if (itemCount12 > 0)
				{
					attackerBody.healthComponent.AddBarrier(15f * (float)itemCount12);
				}
				int itemCount13 = inventory.GetItemCount(ItemIndex.BonusGoldPackOnKill);
				if (itemCount13 > 0 && Util.CheckRoll(4f * (float)itemCount13, attackerMaster))
				{
					GameObject gameObject8 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BonusMoneyPack"), gameObject.transform.position, UnityEngine.Random.rotation);
					gameObject8.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
					NetworkServer.Spawn(gameObject8);
				}
				int itemCount14 = inventory.GetItemCount(ItemIndex.RegenOnKill);
				if (itemCount14 > 0)
				{
					attackerBody.AddTimedBuff(BuffIndex.MeatRegenBoost, 3f * (float)itemCount14);
				}
			}
			Action<DamageReport> action = GlobalEventManager.onCharacterDeathGlobal;
			if (action == null)
			{
				return;
			}
			action(damageReport);
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00034CCC File Offset: 0x00032ECC
		public void OnHitAll(DamageInfo damageInfo, GameObject hitObject)
		{
			if (damageInfo.procCoefficient == 0f || damageInfo.rejected)
			{
				return;
			}
			bool active = NetworkServer.active;
			if (damageInfo.attacker)
			{
				CharacterBody component = damageInfo.attacker.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						Inventory inventory = master.inventory;
						if (master.inventory)
						{
							if (!damageInfo.procChainMask.HasProc(ProcType.Behemoth))
							{
								int itemCount = inventory.GetItemCount(ItemIndex.Behemoth);
								if (itemCount > 0 && damageInfo.procCoefficient != 0f)
								{
									float num = (1.5f + 2.5f * (float)itemCount) * damageInfo.procCoefficient;
									float damageCoefficient = 0.6f;
									float baseDamage = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient);
									EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
									{
										origin = damageInfo.position,
										scale = num,
										rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
									}, true);
									BlastAttack blastAttack = new BlastAttack();
									blastAttack.position = damageInfo.position;
									blastAttack.baseDamage = baseDamage;
									blastAttack.baseForce = 0f;
									blastAttack.radius = num;
									blastAttack.attacker = damageInfo.attacker;
									blastAttack.inflictor = null;
									blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
									blastAttack.crit = damageInfo.crit;
									blastAttack.procChainMask = damageInfo.procChainMask;
									blastAttack.procCoefficient = 0f;
									blastAttack.damageColorIndex = DamageColorIndex.Item;
									blastAttack.falloffModel = BlastAttack.FalloffModel.None;
									blastAttack.damageType = damageInfo.damageType;
									blastAttack.Fire();
								}
							}
							if ((component.HasBuff(BuffIndex.AffixBlue) ? 1 : 0) > 0)
							{
								float damageCoefficient2 = 0.5f;
								float damage = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient2);
								float force = 0f;
								Vector3 position = damageInfo.position;
								ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/LightningStake"), position, Quaternion.identity, damageInfo.attacker, damage, force, damageInfo.crit, DamageColorIndex.Item, null, -1f);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x00034EE4 File Offset: 0x000330E4
		public void OnCrit(CharacterBody body, CharacterMaster master, float procCoefficient, ProcChainMask procChainMask)
		{
			if (body && procCoefficient > 0f && body && master && master.inventory)
			{
				Inventory inventory = master.inventory;
				if (!procChainMask.HasProc(ProcType.HealOnCrit))
				{
					procChainMask.AddProc(ProcType.HealOnCrit);
					int itemCount = inventory.GetItemCount(ItemIndex.HealOnCrit);
					if (itemCount > 0 && body.healthComponent)
					{
						Util.PlaySound("Play_item_proc_crit_heal", body.gameObject);
						if (NetworkServer.active)
						{
							body.healthComponent.Heal((4f + (float)itemCount * 4f) * procCoefficient, procChainMask, true);
						}
					}
				}
				if (inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit) > 0)
				{
					body.AddTimedBuff(BuffIndex.AttackSpeedOnCrit, 3f * procCoefficient);
				}
				int itemCount2 = inventory.GetItemCount(ItemIndex.CooldownOnCrit);
				if (itemCount2 > 0)
				{
					Util.PlaySound("Play_item_proc_crit_cooldown", body.gameObject);
					SkillLocator component = body.GetComponent<SkillLocator>();
					if (component)
					{
						float dt = (float)itemCount2 * procCoefficient;
						if (component.primary)
						{
							component.primary.RunRecharge(dt);
						}
						if (component.secondary)
						{
							component.secondary.RunRecharge(dt);
						}
						if (component.utility)
						{
							component.utility.RunRecharge(dt);
						}
						if (component.special)
						{
							component.special.RunRecharge(dt);
						}
					}
				}
			}
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x0003504E File Offset: 0x0003324E
		public IEnumerator CreateLevelUpEffect(float delay, GameObject levelUpEffect, EffectData effectData)
		{
			yield return new WaitForSeconds(delay);
			EffectManager.SpawnEffect(levelUpEffect, effectData, false);
			yield break;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x0003506C File Offset: 0x0003326C
		public static void OnTeamLevelUp(TeamIndex teamIndex)
		{
			GameObject teamLevelUpEffect = TeamManager.GetTeamLevelUpEffect(teamIndex);
			string teamLevelUpSoundString = TeamManager.GetTeamLevelUpSoundString(teamIndex);
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				TeamComponent teamComponent = teamMembers[i];
				if (teamComponent)
				{
					CharacterBody component = teamComponent.GetComponent<CharacterBody>();
					if (component)
					{
						Transform transform = component.mainHurtBox ? component.mainHurtBox.transform : component.transform;
						EffectData effectData = new EffectData
						{
							origin = transform.position
						};
						if (component.mainHurtBox)
						{
							effectData.SetHurtBoxReference(component.gameObject);
							effectData.scale = component.radius;
						}
						GlobalEventManager.instance.StartCoroutine(GlobalEventManager.instance.CreateLevelUpEffect(UnityEngine.Random.Range(0f, 0.5f), teamLevelUpEffect, effectData));
					}
					if (NetworkServer.active)
					{
						CharacterMaster master = component.master;
						if (master)
						{
							int itemCount = master.inventory.GetItemCount(ItemIndex.WardOnLevel);
							if (itemCount > 0)
							{
								GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), component.transform.position, Quaternion.identity);
								gameObject.GetComponent<TeamFilter>().teamIndex = teamComponent.teamIndex;
								gameObject.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount;
								NetworkServer.Spawn(gameObject);
							}
						}
					}
				}
			}
			if (teamMembers.Count > 0)
			{
				Util.PlaySound(teamLevelUpSoundString, RoR2Application.instance.gameObject);
			}
			if (NetworkServer.active)
			{
				foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
				{
					if (characterMaster && characterMaster.teamIndex != teamIndex)
					{
						int itemCount2 = characterMaster.inventory.GetItemCount(ItemIndex.CrippleWardOnLevel);
						if (itemCount2 > 0)
						{
							CharacterBody body = characterMaster.GetBody();
							if (body)
							{
								GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/CrippleWard"), body.transform.position, Quaternion.identity);
								gameObject2.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount2;
								NetworkServer.Spawn(gameObject2);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x000352BC File Offset: 0x000334BC
		public void OnInteractionBegin(Interactor interactor, IInteractable interactable, GameObject interactableObject)
		{
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				Inventory inventory = component.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(ItemIndex.Firework);
					if (itemCount > 0 && GlobalEventManager.<OnInteractionBegin>g__InteractableIsPermittedForFireworks|29_0((MonoBehaviour)interactable))
					{
						ModelLocator component2 = interactableObject.GetComponent<ModelLocator>();
						Transform transform;
						if (component2 == null)
						{
							transform = null;
						}
						else
						{
							Transform modelTransform = component2.modelTransform;
							if (modelTransform == null)
							{
								transform = null;
							}
							else
							{
								ChildLocator component3 = modelTransform.GetComponent<ChildLocator>();
								transform = ((component3 != null) ? component3.FindChild("FireworkOrigin") : null);
							}
						}
						Transform transform2 = transform;
						Vector3 position = transform2 ? transform2.position : (interactableObject.transform.position + Vector3.up * 2f);
						int remaining = 4 + itemCount * 4;
						FireworkLauncher component4 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireworkLauncher"), position, Quaternion.identity).GetComponent<FireworkLauncher>();
						component4.owner = interactor.gameObject;
						component4.crit = Util.CheckRoll(component.crit, component.master);
						component4.remaining = remaining;
					}
				}
			}
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06000BD0 RID: 3024 RVA: 0x000353BC File Offset: 0x000335BC
		// (remove) Token: 0x06000BD1 RID: 3025 RVA: 0x000353F0 File Offset: 0x000335F0
		public static event Action<DamageDealtMessage> onClientDamageNotified;

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00035423 File Offset: 0x00033623
		public static void ClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			Action<DamageDealtMessage> action = GlobalEventManager.onClientDamageNotified;
			if (action == null)
			{
				return;
			}
			action(damageDealtMessage);
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06000BD3 RID: 3027 RVA: 0x00035438 File Offset: 0x00033638
		// (remove) Token: 0x06000BD4 RID: 3028 RVA: 0x0003546C File Offset: 0x0003366C
		public static event Action<DamageReport> onServerDamageDealt;

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0003549F File Offset: 0x0003369F
		public static void ServerDamageDealt(DamageReport damageReport)
		{
			Action<DamageReport> action = GlobalEventManager.onServerDamageDealt;
			if (action == null)
			{
				return;
			}
			action(damageReport);
		}

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06000BD6 RID: 3030 RVA: 0x000354B4 File Offset: 0x000336B4
		// (remove) Token: 0x06000BD7 RID: 3031 RVA: 0x000354E8 File Offset: 0x000336E8
		public static event Action<DamageReport, float> onServerCharacterExecuted;

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0003551B File Offset: 0x0003371B
		public static void ServerCharacterExecuted(DamageReport damageReport, float executionHealthLost)
		{
			Action<DamageReport, float> action = GlobalEventManager.onServerCharacterExecuted;
			if (action == null)
			{
				return;
			}
			action(damageReport, executionHealthLost);
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x00035557 File Offset: 0x00033757
		[CompilerGenerated]
		internal static bool <OnInteractionBegin>g__InteractableIsPermittedForFireworks|29_0(MonoBehaviour interactableAsMonoBehaviour)
		{
			return !interactableAsMonoBehaviour.GetComponent<GenericPickupController>() && !interactableAsMonoBehaviour.GetComponent<VehicleSeat>();
		}

		// Token: 0x04000BDD RID: 3037
		public static GlobalEventManager instance;

		// Token: 0x04000BDE RID: 3038
		[Obsolete("Transform of the global event manager should not be used! You probably meant something else instead.")]
		private new object transform;

		// Token: 0x04000BDF RID: 3039
		public GameObject missilePrefab;

		// Token: 0x04000BE0 RID: 3040
		public GameObject explodeOnDeathPrefab;

		// Token: 0x04000BE1 RID: 3041
		public GameObject daggerPrefab;

		// Token: 0x04000BE2 RID: 3042
		public GameObject healthOrbPrefab;

		// Token: 0x04000BE3 RID: 3043
		public GameObject AACannonPrefab;

		// Token: 0x04000BE4 RID: 3044
		public GameObject AACannonMuzzleEffect;

		// Token: 0x04000BE5 RID: 3045
		public GameObject chainLightingPrefab;

		// Token: 0x04000BE6 RID: 3046
		public GameObject plasmaCorePrefab;

		// Token: 0x04000BE7 RID: 3047
		public const float bootTriggerSpeed = 20f;

		// Token: 0x04000BE8 RID: 3048
		private static readonly string[] standardDeathQuoteTokens = (from i in Enumerable.Range(0, 37)
		select "PLAYER_DEATH_QUOTE_" + i).ToArray<string>();
	}
}
