using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000302 RID: 770
	public class GlobalEventManager : MonoBehaviour
	{
		// Token: 0x06000FC3 RID: 4035 RVA: 0x0004D7D2 File Offset: 0x0004B9D2
		private void OnEnable()
		{
			if (GlobalEventManager.instance)
			{
				Debug.LogError("Only one GlobalEventManager can exist at a time.");
				return;
			}
			GlobalEventManager.instance = this;
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x0004D7F1 File Offset: 0x0004B9F1
		private void OnDisable()
		{
			if (GlobalEventManager.instance == this)
			{
				GlobalEventManager.instance = null;
			}
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x00004507 File Offset: 0x00002707
		public void OnLocalPlayerBodySpawn(CharacterBody body)
		{
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x00004507 File Offset: 0x00002707
		public void OnCharacterBodySpawn(CharacterBody body)
		{
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x00004507 File Offset: 0x00002707
		public void OnCharacterBodyStart(CharacterBody body)
		{
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x0004D808 File Offset: 0x0004BA08
		public void OnHitEnemy(DamageInfo damageInfo, GameObject victim)
		{
			if (damageInfo.procCoefficient == 0f)
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
						if (itemCount2 > 0 && Util.CheckRoll((1f - 1f / (damageInfo.procCoefficient * 0.05f * (float)itemCount2 + 1f)) * 100f, master))
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
						if ((component.HasBuff(BuffIndex.AffixRed) ? 1 : 0) > 0 || (damageInfo.damageType & DamageType.IgniteOnHit) != DamageType.Generic)
						{
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
						}
						if ((component.HasBuff(BuffIndex.AffixWhite) ? 1 : 0) > 0 && characterBody)
						{
							characterBody.AddTimedBuff(BuffIndex.Slow80, 1.5f * damageInfo.procCoefficient);
						}
						int itemCount4 = master.inventory.GetItemCount(ItemIndex.SlowOnHit);
						if (itemCount4 > 0 && characterBody)
						{
							characterBody.AddTimedBuff(BuffIndex.Slow60, 1f * (float)itemCount4);
						}
						int itemCount5 = inventory.GetItemCount(ItemIndex.GoldOnHit);
						if (itemCount5 > 0 && Util.CheckRoll(30f * damageInfo.procCoefficient, master))
						{
							master.GiveMoney((uint)((float)itemCount5 * 2f * Run.instance.difficultyCoefficient));
							EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageInfo.position, Vector3.up, true);
						}
						if (!damageInfo.procChainMask.HasProc(ProcType.Missile))
						{
							this.ProcMissile(inventory.GetItemCount(ItemIndex.Missile), component, master, teamIndex, damageInfo.procChainMask, victim, damageInfo);
						}
						int itemCount6 = inventory.GetItemCount(ItemIndex.ChainLightning);
						if (itemCount6 > 0 && !damageInfo.procChainMask.HasProc(ProcType.ChainLightning) && Util.CheckRoll(25f * damageInfo.procCoefficient, master))
						{
							float damageCoefficient = 0.8f;
							float damageValue = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient);
							LightningOrb lightningOrb = new LightningOrb();
							lightningOrb.origin = damageInfo.position;
							lightningOrb.damageValue = damageValue;
							lightningOrb.isCrit = damageInfo.crit;
							lightningOrb.bouncesRemaining = 2 * itemCount6;
							lightningOrb.teamIndex = teamIndex;
							lightningOrb.attacker = damageInfo.attacker;
							lightningOrb.bouncedObjects = new List<HealthComponent>
							{
								victim.GetComponent<HealthComponent>()
							};
							lightningOrb.procChainMask = damageInfo.procChainMask;
							lightningOrb.procChainMask.AddProc(ProcType.ChainLightning);
							lightningOrb.procCoefficient = 0.2f;
							lightningOrb.lightningType = LightningOrb.LightningType.Ukulele;
							lightningOrb.damageColorIndex = DamageColorIndex.Item;
							lightningOrb.range += (float)(2 * itemCount6);
							HurtBox hurtBox = lightningOrb.PickNextTarget(damageInfo.position);
							if (hurtBox)
							{
								lightningOrb.target = hurtBox;
								OrbManager.instance.AddOrb(lightningOrb);
							}
						}
						int itemCount7 = inventory.GetItemCount(ItemIndex.BounceNearby);
						float num = (1f - 100f / (100f + 20f * (float)itemCount7)) * 100f;
						if (itemCount7 > 0 && !damageInfo.procChainMask.HasProc(ProcType.BounceNearby) && Util.CheckRoll(num * damageInfo.procCoefficient, master))
						{
							List<HealthComponent> bouncedObjects = new List<HealthComponent>
							{
								victim.GetComponent<HealthComponent>()
							};
							float damageCoefficient2 = 1f;
							float damageValue2 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient2);
							for (int i = 0; i < 5 + itemCount7 * 5; i++)
							{
								BounceOrb bounceOrb = new BounceOrb();
								bounceOrb.origin = damageInfo.position;
								bounceOrb.damageValue = damageValue2;
								bounceOrb.isCrit = damageInfo.crit;
								bounceOrb.teamIndex = teamIndex;
								bounceOrb.attacker = damageInfo.attacker;
								bounceOrb.procChainMask = damageInfo.procChainMask;
								bounceOrb.procChainMask.AddProc(ProcType.BounceNearby);
								bounceOrb.procCoefficient = 0.33f;
								bounceOrb.damageColorIndex = DamageColorIndex.Item;
								bounceOrb.bouncedObjects = bouncedObjects;
								HurtBox hurtBox2 = bounceOrb.PickNextTarget(victim.transform.position, 30f);
								if (hurtBox2)
								{
									bounceOrb.target = hurtBox2;
									OrbManager.instance.AddOrb(bounceOrb);
								}
							}
						}
						int itemCount8 = inventory.GetItemCount(ItemIndex.StickyBomb);
						if (itemCount8 > 0 && Util.CheckRoll((2.5f + 2.5f * (float)itemCount8) * damageInfo.procCoefficient, master) && characterBody)
						{
							Vector3 position = damageInfo.position;
							Vector3 forward = characterBody.corePosition - position;
							Quaternion rotation = (forward.magnitude != 0f) ? Util.QuaternionSafeLookRotation(forward) : UnityEngine.Random.rotationUniform;
							float damageCoefficient3 = 1.25f + 1.25f * (float)itemCount8;
							float damage = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient3);
							ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/StickyBomb"), position, rotation, damageInfo.attacker, damage, 100f, damageInfo.crit, DamageColorIndex.Item, null, forward.magnitude * 60f);
						}
						int itemCount9 = inventory.GetItemCount(ItemIndex.IceRing);
						int itemCount10 = inventory.GetItemCount(ItemIndex.FireRing);
						if ((itemCount9 | itemCount10) > 0)
						{
							Vector3 position2 = damageInfo.position;
							if (Util.CheckRoll(8f * damageInfo.procCoefficient, master))
							{
								ProcChainMask procChainMask3 = damageInfo.procChainMask;
								procChainMask3.AddProc(ProcType.Rings);
								if (itemCount9 > 0)
								{
									float damageCoefficient4 = 1.25f + 1.25f * (float)itemCount9;
									float damage2 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient4);
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
										procChainMask = procChainMask3,
										procCoefficient = 1f
									};
									EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IceRingExplosion"), position2, Vector3.up, true);
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
									float damageCoefficient5 = 2.5f + 2.5f * (float)itemCount10;
									float damage3 = Util.OnHitProcDamage(damageInfo.damage, component.damage, damageCoefficient5) / lifetime * resetInterval;
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
										procChainMask = procChainMask3,
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

		// Token: 0x06000FC9 RID: 4041 RVA: 0x0004E0B0 File Offset: 0x0004C2B0
		private void ProcMissile(int stack, CharacterBody attackerBody, CharacterMaster attackerMaster, TeamIndex attackerTeamIndex, ProcChainMask procChainMask, GameObject victim, DamageInfo damageInfo)
		{
			if (stack > 0)
			{
				GameObject gameObject = attackerBody.gameObject;
				InputBankTest component = gameObject.GetComponent<InputBankTest>();
				Vector3 position = component ? component.aimOrigin : base.transform.position;
				Vector3 vector = component ? component.aimDirection : base.transform.forward;
				Vector3 up = Vector3.up;
				if (Util.CheckRoll(10f * damageInfo.procCoefficient, attackerMaster))
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.missilePrefab, position, Util.QuaternionSafeLookRotation(up + UnityEngine.Random.insideUnitSphere * 0f));
					ProjectileController component2 = gameObject2.GetComponent<ProjectileController>();
					component2.Networkowner = gameObject.gameObject;
					component2.procChainMask = procChainMask;
					component2.procChainMask.AddProc(ProcType.Missile);
					gameObject2.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
					gameObject2.GetComponent<MissileController>().target = victim.transform;
					float damageCoefficient = 3f * (float)stack;
					float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);
					ProjectileDamage component3 = gameObject2.GetComponent<ProjectileDamage>();
					component3.damage = damage;
					component3.crit = damageInfo.crit;
					component3.force = 200f;
					component3.damageColorIndex = DamageColorIndex.Item;
					NetworkServer.Spawn(gameObject2);
				}
			}
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0004E1E4 File Offset: 0x0004C3E4
		public void OnCharacterHitGround(CharacterBody characterBody, Vector3 impactVelocity)
		{
			float num = Mathf.Abs(impactVelocity.y);
			Inventory inventory = characterBody.inventory;
			CharacterMaster master = characterBody.master;
			if (num >= characterBody.jumpPower - 1f)
			{
				Vector3 footPosition = characterBody.footPosition;
				float radius = characterBody.radius;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(footPosition + Vector3.up * 1.5f, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask | LayerIndex.water.mask, QueryTriggerInteraction.Collide))
				{
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(raycastHit.collider, raycastHit.point);
					if (objectSurfaceDef)
					{
						EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
						{
							origin = footPosition,
							scale = radius,
							color = objectSurfaceDef.approximateColor
						}, true);
						if (objectSurfaceDef.footstepEffectPrefab)
						{
							EffectManager.instance.SpawnEffect(objectSurfaceDef.footstepEffectPrefab, new EffectData
							{
								origin = raycastHit.point,
								scale = radius * 3f
							}, false);
						}
						SfxLocator component = characterBody.GetComponent<SfxLocator>();
						if (component)
						{
							if (objectSurfaceDef.materialSwitchString != null && objectSurfaceDef.materialSwitchString.Length > 0)
							{
								AkSoundEngine.SetSwitch("material", objectSurfaceDef.materialSwitchString, characterBody.gameObject);
							}
							else
							{
								AkSoundEngine.SetSwitch("material", "dirt", characterBody.gameObject);
							}
							Util.PlaySound(component.landingSound, characterBody.gameObject);
						}
					}
				}
			}
			if ((inventory ? inventory.GetItemCount(ItemIndex.FallBoots) : 0) <= 0 && (characterBody.bodyFlags & CharacterBody.BodyFlags.IgnoreFallDamage) == CharacterBody.BodyFlags.None)
			{
				float num2 = Mathf.Max(num - (characterBody.jumpPower + 20f), 0f);
				if (num2 > 0f)
				{
					float num3 = num2 / 60f;
					HealthComponent component2 = characterBody.GetComponent<HealthComponent>();
					if (component2)
					{
						component2.TakeDamage(new DamageInfo
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
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0004E458 File Offset: 0x0004C658
		private void OnPlayerCharacterDeath(DamageInfo damageInfo, GameObject victim, NetworkUser victimNetworkUser)
		{
			CharacterBody component = victim.GetComponent<CharacterBody>();
			int num = UnityEngine.Random.Range(0, 37);
			string baseToken = "PLAYER_DEATH_QUOTE_" + num;
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(component);
			if (networkUser)
			{
				Chat.SendBroadcastChat(new Chat.PlayerDeathChatMessage
				{
					subjectNetworkUser = networkUser,
					baseToken = baseToken,
					paramTokens = new string[]
					{
						networkUser.userName
					}
				});
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000FCC RID: 4044 RVA: 0x0004E4C4 File Offset: 0x0004C6C4
		// (remove) Token: 0x06000FCD RID: 4045 RVA: 0x0004E4F8 File Offset: 0x0004C6F8
		public static event Action<DamageReport> onCharacterDeathGlobal;

		// Token: 0x06000FCE RID: 4046 RVA: 0x0004E52C File Offset: 0x0004C72C
		public void OnCharacterDeath(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			GameObject gameObject = damageReport.victim.gameObject;
			DamageInfo damageInfo = damageReport.damageInfo;
			TeamComponent component = gameObject.GetComponent<TeamComponent>();
			TeamIndex teamIndex = TeamIndex.Neutral;
			CharacterBody component2 = gameObject.GetComponent<CharacterBody>();
			EquipmentIndex equipmentIndex = component2.equipmentSlot ? component2.equipmentSlot.equipmentIndex : EquipmentIndex.None;
			if (component)
			{
				teamIndex = component.teamIndex;
				if (teamIndex == TeamIndex.Monster && Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Bomb))
				{
					Debug.Log("team and artifact OK");
					ModelLocator component3 = gameObject.GetComponent<ModelLocator>();
					if (component3)
					{
						Debug.Log("victimModelLocator OK");
						Transform modelTransform = component3.modelTransform;
						if (modelTransform)
						{
							Debug.Log("victimModelTransform OK");
							HurtBoxGroup component4 = modelTransform.GetComponent<HurtBoxGroup>();
							if (component4)
							{
								Debug.Log("victimHurtBoxGroup OK");
								float damage = 0f;
								if (component2)
								{
									damage = component2.damage;
								}
								HurtBoxGroup.VolumeDistribution volumeDistribution = component4.GetVolumeDistribution();
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
				}
			}
			if (component2)
			{
				CharacterMaster master = component2.master;
				if (master)
				{
					PlayerCharacterMasterController component5 = master.GetComponent<PlayerCharacterMasterController>();
					if (component5)
					{
						GameObject networkUserObject = component5.networkUserObject;
						if (networkUserObject)
						{
							NetworkUser component6 = networkUserObject.GetComponent<NetworkUser>();
							if (component6)
							{
								this.OnPlayerCharacterDeath(damageInfo, gameObject, component6);
							}
						}
					}
					if (component2.HasBuff(BuffIndex.AffixWhite))
					{
						Vector3 corePosition = Util.GetCorePosition(gameObject);
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
						float num2 = 12f + component2.radius;
						gameObject2.transform.localScale = new Vector3(num2, num2, num2);
						DelayBlast component7 = gameObject2.GetComponent<DelayBlast>();
						component7.position = corePosition;
						component7.baseDamage = component2.damage * 3.5f;
						component7.baseForce = 2300f;
						component7.attacker = component2.gameObject;
						component7.radius = num2;
						component7.crit = Util.CheckRoll(component2.crit, master);
						component7.maxTimer = 2f;
						component7.falloffModel = BlastAttack.FalloffModel.SweetSpot;
						component7.explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
						component7.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/AffixWhiteDelayEffect");
						gameObject2.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component7.attacker);
					}
				}
			}
			if (damageInfo.attacker)
			{
				CharacterBody component8 = damageInfo.attacker.GetComponent<CharacterBody>();
				if (component8)
				{
					CharacterMaster master2 = component8.master;
					if (master2)
					{
						Inventory inventory = master2.inventory;
						TeamComponent component9 = component8.GetComponent<TeamComponent>();
						TeamIndex teamIndex2 = component9 ? component9.teamIndex : TeamIndex.Neutral;
						int itemCount = inventory.GetItemCount(ItemIndex.IgniteOnKill);
						if (itemCount > 0)
						{
							ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
							float num3 = 8f + 4f * (float)itemCount;
							float radius = component2.radius;
							float num4 = num3 + radius;
							float num5 = num4 * num4;
							Vector3 corePosition2 = Util.GetCorePosition(gameObject);
							EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX"), new EffectData
							{
								origin = corePosition2,
								scale = num4,
								rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
							}, true);
							for (int j = 0; j < teamMembers.Count; j++)
							{
								if ((teamMembers[j].transform.position - corePosition2).sqrMagnitude <= num5)
								{
									DotController.InflictDot(teamMembers[j].gameObject, damageInfo.attacker, DotController.DotIndex.Burn, 1.5f + 1.5f * (float)itemCount, 1f);
								}
							}
						}
						int itemCount2 = inventory.GetItemCount(ItemIndex.ExplodeOnDeath);
						if (itemCount2 > 0)
						{
							Vector3 corePosition3 = Util.GetCorePosition(gameObject);
							float damageCoefficient = 3.5f * (1f + (float)(itemCount2 - 1) * 0.8f);
							float baseDamage = Util.OnKillProcDamage(component8.damage, damageCoefficient);
							GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.explodeOnDeathPrefab, corePosition3, Quaternion.identity);
							DelayBlast component10 = gameObject3.GetComponent<DelayBlast>();
							component10.position = corePosition3;
							component10.baseDamage = baseDamage;
							component10.baseForce = 2000f;
							component10.bonusForce = Vector3.up * 1000f;
							component10.radius = 12f + 2.4f * ((float)itemCount2 - 1f);
							component10.attacker = damageInfo.attacker;
							component10.inflictor = null;
							component10.crit = Util.CheckRoll(component8.crit, master2);
							component10.maxTimer = 0.5f;
							component10.damageColorIndex = DamageColorIndex.Item;
							component10.falloffModel = BlastAttack.FalloffModel.SweetSpot;
							gameObject3.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component10.attacker);
							NetworkServer.Spawn(gameObject3);
						}
						int itemCount3 = inventory.GetItemCount(ItemIndex.Dagger);
						if (itemCount3 > 0)
						{
							for (int k = 0; k < itemCount3 * 3; k++)
							{
								GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.daggerPrefab, gameObject.transform.position + Vector3.up * 1.8f + UnityEngine.Random.insideUnitSphere * 0.5f, Util.QuaternionSafeLookRotation(Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f));
								gameObject4.GetComponent<ProjectileController>().Networkowner = component8.gameObject;
								gameObject4.GetComponent<TeamFilter>().teamIndex = teamIndex2;
								gameObject4.GetComponent<DaggerController>().delayTimer += (float)k * 0.05f;
								float damageCoefficient2 = 1.5f;
								float damage2 = Util.OnKillProcDamage(component8.damage, damageCoefficient2);
								ProjectileDamage component11 = gameObject4.GetComponent<ProjectileDamage>();
								component11.damage = damage2;
								component11.crit = Util.CheckRoll(component8.crit, master2);
								component11.force = 200f;
								component11.damageColorIndex = DamageColorIndex.Item;
								NetworkServer.Spawn(gameObject4);
							}
						}
						int itemCount4 = inventory.GetItemCount(ItemIndex.Tooth);
						if (itemCount4 > 0)
						{
							float num6 = Mathf.Pow((float)itemCount4, 0.25f);
							GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), gameObject.transform.position, UnityEngine.Random.rotation);
							gameObject5.GetComponent<TeamFilter>().teamIndex = teamIndex2;
							gameObject5.GetComponentInChildren<HealthPickup>().flatHealing = 4f * (float)itemCount4;
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
								infusionOrb.target = Util.FindBodyMainHurtBox(component8);
								infusionOrb.maxHpValue = 1;
								OrbManager.instance.AddOrb(infusionOrb);
							}
						}
						if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) == DamageType.ResetCooldownsOnKill)
						{
							SkillLocator component12 = component8.GetComponent<SkillLocator>();
							if (component12)
							{
								component12.ResetSkills();
							}
						}
						if (inventory)
						{
							int itemCount6 = inventory.GetItemCount(ItemIndex.Talisman);
							if (itemCount6 > 0 && component8.GetComponent<EquipmentSlot>())
							{
								inventory.DeductActiveEquipmentCooldown(2f + (float)itemCount6 * 2f);
							}
						}
						int itemCount7 = inventory.GetItemCount(ItemIndex.TempestOnKill);
						if (itemCount7 > 0 && Util.CheckRoll(25f, master2))
						{
							GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/TempestWard"), component2.footPosition, Quaternion.identity);
							gameObject6.GetComponent<TeamFilter>().teamIndex = component9.teamIndex;
							gameObject6.GetComponent<BuffWard>().expireDuration = 2f + 6f * (float)itemCount7;
							NetworkServer.Spawn(gameObject6);
						}
						int itemCount8 = inventory.GetItemCount(ItemIndex.Bandolier);
						if (itemCount8 > 0 && Util.CheckRoll((1f - 1f / Mathf.Pow((float)(itemCount8 + 1), 0.33f)) * 100f, master2))
						{
							GameObject gameObject7 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack"), gameObject.transform.position, UnityEngine.Random.rotation);
							gameObject7.GetComponent<TeamFilter>().teamIndex = teamIndex2;
							NetworkServer.Spawn(gameObject7);
						}
						if (component2 && component2.isElite)
						{
							int itemCount9 = inventory.GetItemCount(ItemIndex.HeadHunter);
							int itemCount10 = inventory.GetItemCount(ItemIndex.KillEliteFrenzy);
							if (itemCount9 > 0)
							{
								float duration = 3f + 5f * (float)itemCount9;
								for (int l = 0; l < BuffCatalog.eliteBuffIndices.Length; l++)
								{
									BuffIndex buffType = BuffCatalog.eliteBuffIndices[l];
									if (component2.HasBuff(buffType))
									{
										component8.AddTimedBuff(buffType, duration);
									}
								}
							}
							if (itemCount10 > 0)
							{
								component8.AddTimedBuff(BuffIndex.NoCooldowns, 1f + (float)itemCount10 * 2f);
							}
						}
						int itemCount11 = inventory.GetItemCount(ItemIndex.GhostOnKill);
						if (itemCount11 > 0 && component2 && Util.CheckRoll(10f, master2))
						{
							Util.TryToCreateGhost(component2, component8, itemCount11 * 30);
						}
						DeathRewards component13 = component2.GetComponent<DeathRewards>();
						if (Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Sacrifice) && component13)
						{
							float num8 = component13.expReward;
							if (Util.CheckRoll(3f * Mathf.Log(num8 + 1f, 3f), master2))
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
						if (Util.CheckRoll(0.025f, master2) && component2 && component2.isElite)
						{
							PickupDropletController.CreatePickupDroplet(new PickupIndex(equipmentIndex), component2.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + base.transform.forward * 2f);
						}
					}
				}
			}
			int num9 = (equipmentIndex == EquipmentIndex.AffixGreen) ? 1 : 0;
			if (num9 > 0)
			{
				float num10 = 0.25f * component2.maxHealth;
				float num11 = 900f + component2.radius * component2.radius;
				Vector3 position = gameObject.transform.position;
				ReadOnlyCollection<TeamComponent> teamMembers2 = TeamComponent.GetTeamMembers(teamIndex);
				for (int m = 0; m < teamMembers2.Count; m++)
				{
					Vector3 position2 = teamMembers2[m].transform.position;
					if (Vector3.SqrMagnitude(position - position2) < num11)
					{
						HealOrb healOrb = new HealOrb();
						healOrb.origin = position;
						healOrb.target = Util.FindBodyMainHurtBox(teamMembers2[m].gameObject);
						healOrb.healValue = num10 * (float)num9;
						healOrb.scaleOrb = false;
						OrbManager.instance.AddOrb(healOrb);
					}
				}
			}
			Action<DamageReport> action = GlobalEventManager.onCharacterDeathGlobal;
			if (action == null)
			{
				return;
			}
			action(damageReport);
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0004F064 File Offset: 0x0004D264
		public void OnHitAll(DamageInfo damageInfo, GameObject hitObject)
		{
			if (damageInfo.procCoefficient == 0f)
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
									EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
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
								float damageCoefficient2 = 1f;
								float damage = Util.OnHitProcDamage(damageInfo.damage, component.baseDamage, damageCoefficient2);
								float force = 0f;
								Vector3 position = damageInfo.position;
								ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/LightningStake"), position, Quaternion.identity, damageInfo.attacker, damage, force, damageInfo.crit, DamageColorIndex.Item, null, -1f);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0004F278 File Offset: 0x0004D478
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
					body.AddTimedBuff(BuffIndex.AttackSpeedOnCrit, 2f * procCoefficient);
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

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0004F3E4 File Offset: 0x0004D5E4
		public static void OnTeamLevelUp(TeamIndex teamIndex)
		{
			GameObject teamLevelUpEffect = TeamManager.GetTeamLevelUpEffect(teamIndex);
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				TeamComponent teamComponent = teamMembers[i];
				if (teamComponent)
				{
					CharacterBody component = teamComponent.GetComponent<CharacterBody>();
					if (component)
					{
						if (NetworkServer.active)
						{
							HealthComponent component2 = component.GetComponent<HealthComponent>();
							if (component2 && component)
							{
								HealthComponent healthComponent = component2;
								healthComponent.Networkhealth = healthComponent.health + component.levelMaxHealth * (component2.health / component2.fullHealth);
							}
						}
						Transform transform = component.mainHurtBox ? component.mainHurtBox.transform : component.transform;
						EffectData effectData = new EffectData
						{
							origin = transform.position
						};
						EffectManager.instance.SpawnEffect(teamLevelUpEffect, effectData, false);
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

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0004F610 File Offset: 0x0004D810
		public void OnInteractionBegin(Interactor interactor, IInteractable interactable, GameObject interactableObject)
		{
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				Inventory inventory = component.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(ItemIndex.Firework);
					if (itemCount > 0 && !((MonoBehaviour)interactable).GetComponent<GenericPickupController>())
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

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06000FD3 RID: 4051 RVA: 0x0004F714 File Offset: 0x0004D914
		// (remove) Token: 0x06000FD4 RID: 4052 RVA: 0x0004F748 File Offset: 0x0004D948
		public static event Action<DamageDealtMessage> onClientDamageNotified;

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0004F77B File Offset: 0x0004D97B
		public static void ClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			Action<DamageDealtMessage> action = GlobalEventManager.onClientDamageNotified;
			if (action == null)
			{
				return;
			}
			action(damageDealtMessage);
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06000FD6 RID: 4054 RVA: 0x0004F790 File Offset: 0x0004D990
		// (remove) Token: 0x06000FD7 RID: 4055 RVA: 0x0004F7C4 File Offset: 0x0004D9C4
		public static event Action<DamageReport> onServerDamageDealt;

		// Token: 0x06000FD8 RID: 4056 RVA: 0x0004F7F7 File Offset: 0x0004D9F7
		public static void ServerDamageDealt(DamageReport damageReport)
		{
			Action<DamageReport> action = GlobalEventManager.onServerDamageDealt;
			if (action == null)
			{
				return;
			}
			action(damageReport);
		}

		// Token: 0x040013CE RID: 5070
		public static GlobalEventManager instance;

		// Token: 0x040013CF RID: 5071
		public GameObject missilePrefab;

		// Token: 0x040013D0 RID: 5072
		public GameObject explodeOnDeathPrefab;

		// Token: 0x040013D1 RID: 5073
		public GameObject daggerPrefab;

		// Token: 0x040013D2 RID: 5074
		public GameObject healthOrbPrefab;

		// Token: 0x040013D3 RID: 5075
		public GameObject AACannonPrefab;

		// Token: 0x040013D4 RID: 5076
		public GameObject AACannonMuzzleEffect;

		// Token: 0x040013D5 RID: 5077
		public GameObject chainLightingPrefab;

		// Token: 0x040013D6 RID: 5078
		public GameObject plasmaCorePrefab;

		// Token: 0x040013D7 RID: 5079
		public const float bootTriggerSpeed = 20f;

		// Token: 0x040013D8 RID: 5080
		private const int deathQuoteCount = 37;
	}
}
