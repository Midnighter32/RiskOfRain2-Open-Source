using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001F6 RID: 502
	[RequireComponent(typeof(CharacterBody))]
	public class EquipmentSlot : NetworkBehaviour
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000A83 RID: 2691 RVA: 0x0002DFD9 File Offset: 0x0002C1D9
		// (set) Token: 0x06000A84 RID: 2692 RVA: 0x0002DFE1 File Offset: 0x0002C1E1
		public byte activeEquipmentSlot { get; private set; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000A85 RID: 2693 RVA: 0x0002DFEA File Offset: 0x0002C1EA
		// (set) Token: 0x06000A86 RID: 2694 RVA: 0x0002DFF2 File Offset: 0x0002C1F2
		public EquipmentIndex equipmentIndex { get; private set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000A87 RID: 2695 RVA: 0x0002DFFB File Offset: 0x0002C1FB
		// (set) Token: 0x06000A88 RID: 2696 RVA: 0x0002E003 File Offset: 0x0002C203
		public int stock { get; private set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000A89 RID: 2697 RVA: 0x0002E00C File Offset: 0x0002C20C
		// (set) Token: 0x06000A8A RID: 2698 RVA: 0x0002E014 File Offset: 0x0002C214
		public int maxStock { get; private set; }

		// Token: 0x06000A8B RID: 2699 RVA: 0x0002E01D File Offset: 0x0002C21D
		public override void OnStartServer()
		{
			base.OnStartServer();
			this.UpdateAuthority();
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0002E02B File Offset: 0x0002C22B
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0002E039 File Offset: 0x0002C239
		public override void OnStopAuthority()
		{
			base.OnStopAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x0002E047 File Offset: 0x0002C247
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0002E05C File Offset: 0x0002C25C
		private void UpdateInventory()
		{
			this.inventory = this.characterBody.inventory;
			if (this.inventory)
			{
				this.activeEquipmentSlot = this.inventory.activeEquipmentSlot;
				this.equipmentIndex = this.inventory.GetEquipmentIndex();
				this.stock = (int)this.inventory.GetEquipment((uint)this.inventory.activeEquipmentSlot).charges;
				this.maxStock = this.inventory.GetActiveEquipmentMaxCharges();
				this._rechargeTime = this.inventory.GetEquipment((uint)this.inventory.activeEquipmentSlot).chargeFinishTime;
				return;
			}
			this.activeEquipmentSlot = 0;
			this.equipmentIndex = EquipmentIndex.None;
			this.stock = 0;
			this.maxStock = 0;
			this._rechargeTime = Run.FixedTimeStamp.positiveInfinity;
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000A90 RID: 2704 RVA: 0x0002E124 File Offset: 0x0002C324
		// (set) Token: 0x06000A91 RID: 2705 RVA: 0x0002E12C File Offset: 0x0002C32C
		public CharacterBody characterBody { get; private set; }

		// Token: 0x06000A92 RID: 2706 RVA: 0x0002E138 File Offset: 0x0002C338
		[Server]
		private void UpdateGoldGat()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.EquipmentSlot::UpdateGoldGat()' called on client");
				return;
			}
			bool flag = this.equipmentIndex == EquipmentIndex.GoldGat;
			if (flag != this.goldgatControllerObject)
			{
				if (flag)
				{
					this.goldgatControllerObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GoldGatController"));
					this.goldgatControllerObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
					return;
				}
				UnityEngine.Object.Destroy(this.goldgatControllerObject);
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000A93 RID: 2707 RVA: 0x0002E1AD File Offset: 0x0002C3AD
		public float cooldownTimer
		{
			get
			{
				return this._rechargeTime.timeUntil;
			}
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x0002E1BC File Offset: 0x0002C3BC
		public Transform FindActiveEquipmentDisplay()
		{
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				Transform modelTransform = component.modelTransform;
				if (modelTransform)
				{
					CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
					if (component2)
					{
						List<GameObject> equipmentDisplayObjects = component2.GetEquipmentDisplayObjects(this.equipmentIndex);
						if (equipmentDisplayObjects.Count > 0)
						{
							return equipmentDisplayObjects[0].transform;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x0002E21C File Offset: 0x0002C41C
		[ClientRpc]
		private void RpcOnClientEquipmentActivationRecieved()
		{
			Util.PlaySound(EquipmentSlot.equipmentActivateString, base.gameObject);
			EquipmentIndex equipmentIndex = this.equipmentIndex;
			if (equipmentIndex <= EquipmentIndex.CritOnUse)
			{
				if (equipmentIndex != EquipmentIndex.Blackhole)
				{
					if (equipmentIndex != EquipmentIndex.CritOnUse)
					{
						return;
					}
					Transform transform = this.FindActiveEquipmentDisplay();
					if (transform)
					{
						Animator componentInChildren = transform.GetComponentInChildren<Animator>();
						if (componentInChildren)
						{
							componentInChildren.SetBool("active", true);
							componentInChildren.SetFloat("activeDuration", 8f);
							componentInChildren.SetFloat("activeStopwatch", 0f);
							return;
						}
					}
				}
				else
				{
					Transform transform2 = this.FindActiveEquipmentDisplay();
					if (transform2)
					{
						GravCubeController component = transform2.GetComponent<GravCubeController>();
						if (component)
						{
							component.ActivateCube(9f);
							return;
						}
					}
				}
			}
			else if (equipmentIndex != EquipmentIndex.BFG)
			{
				if (equipmentIndex != EquipmentIndex.GainArmor)
				{
					return;
				}
				Util.PlaySound("Play_item_use_gainArmor", base.gameObject);
			}
			else
			{
				Transform transform3 = this.FindActiveEquipmentDisplay();
				if (transform3)
				{
					Animator componentInChildren2 = transform3.GetComponentInChildren<Animator>();
					if (componentInChildren2)
					{
						componentInChildren2.SetTrigger("Fire");
						return;
					}
				}
			}
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x0002E320 File Offset: 0x0002C520
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.targetIndicator = new Indicator(base.gameObject, null);
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x0002E36F File Offset: 0x0002C56F
		private void OnDestroy()
		{
			this.targetIndicator.active = false;
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x0002E380 File Offset: 0x0002C580
		private void FixedUpdate()
		{
			this.UpdateInventory();
			if (NetworkServer.active)
			{
				this.subcooldownTimer -= Time.fixedDeltaTime;
				if (this.missileTimer > 0f)
				{
					this.missileTimer = Mathf.Max(this.missileTimer - Time.fixedDeltaTime, 0f);
				}
				if (this.missileTimer == 0f && this.remainingMissiles > 0)
				{
					this.remainingMissiles--;
					this.missileTimer = 0.125f;
					this.FireMissile();
				}
				this.UpdateGoldGat();
				if (this.bfgChargeTimer > 0f)
				{
					this.bfgChargeTimer -= Time.fixedDeltaTime;
					if (this.bfgChargeTimer < 0f)
					{
						Vector3 position = base.transform.position;
						Ray aimRay = this.GetAimRay();
						Transform transform = this.FindActiveEquipmentDisplay();
						if (transform)
						{
							ChildLocator componentInChildren = transform.GetComponentInChildren<ChildLocator>();
							if (componentInChildren)
							{
								Transform transform2 = componentInChildren.FindChild("Muzzle");
								if (transform2)
								{
									aimRay.origin = transform2.position;
								}
							}
						}
						this.healthComponent.TakeDamageForce(aimRay.direction * -1500f, true, false);
						ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/BeamSphere"), aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.characterBody.damage * 2f, 0f, Util.CheckRoll(this.characterBody.crit, this.characterBody.master), DamageColorIndex.Item, null, -1f);
						this.bfgChargeTimer = 0f;
					}
				}
				if (this.equipmentIndex == EquipmentIndex.PassiveHealing != this.passiveHealingFollower)
				{
					if (this.equipmentIndex == EquipmentIndex.PassiveHealing)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealingFollower"), base.transform.position, Quaternion.identity);
						this.passiveHealingFollower = gameObject.GetComponent<HealingFollowerController>();
						this.passiveHealingFollower.NetworkownerBodyObject = base.gameObject;
						NetworkServer.Spawn(gameObject);
					}
					else
					{
						UnityEngine.Object.Destroy(this.passiveHealingFollower.gameObject);
						this.passiveHealingFollower = null;
					}
				}
			}
			if (!this.inputBank.activateEquipment.justPressed)
			{
				Inventory inventory = this.inventory;
				if (((inventory != null) ? inventory.GetItemCount(ItemIndex.AutoCastEquipment) : 0) <= 0)
				{
					return;
				}
			}
			if (this.hasEffectiveAuthority)
			{
				if (NetworkServer.active)
				{
					this.ExecuteIfReady();
					return;
				}
				this.CallCmdExecuteIfReady();
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x0002E5EB File Offset: 0x0002C7EB
		[Command]
		private void CmdExecuteIfReady()
		{
			this.ExecuteIfReady();
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x0002E5F4 File Offset: 0x0002C7F4
		[Server]
		public bool ExecuteIfReady()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.EquipmentSlot::ExecuteIfReady()' called on client");
				return false;
			}
			if (this.equipmentIndex != EquipmentIndex.None && this.stock > 0)
			{
				this.Execute();
				return true;
			}
			return false;
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x0002E628 File Offset: 0x0002C828
		[Server]
		private void Execute()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.EquipmentSlot::Execute()' called on client");
				return;
			}
			EquipmentIndex equipmentIndex = this.equipmentIndex;
			if (EquipmentCatalog.GetEquipmentDef(equipmentIndex) != null && this.subcooldownTimer <= 0f && this.PerformEquipmentAction(equipmentIndex))
			{
				this.inventory.DeductEquipmentCharges(this.activeEquipmentSlot, 1);
				this.UpdateInventory();
				this.CallRpcOnClientEquipmentActivationRecieved();
				Action<EquipmentSlot, EquipmentIndex> action = EquipmentSlot.onServerEquipmentActivated;
				if (action != null)
				{
					action(this, equipmentIndex);
				}
				if (this.characterBody && this.inventory)
				{
					int itemCount = this.inventory.GetItemCount(ItemIndex.EnergizedOnEquipmentUse);
					if (itemCount > 0)
					{
						this.characterBody.AddTimedBuff(BuffIndex.Energized, (float)(8 + 4 * (itemCount - 1)));
					}
				}
			}
		}

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06000A9C RID: 2716 RVA: 0x0002E6E4 File Offset: 0x0002C8E4
		// (remove) Token: 0x06000A9D RID: 2717 RVA: 0x0002E718 File Offset: 0x0002C918
		public static event Action<EquipmentSlot, EquipmentIndex> onServerEquipmentActivated;

		// Token: 0x06000A9E RID: 2718 RVA: 0x0002E74C File Offset: 0x0002C94C
		private void FireMissile()
		{
			GameObject prefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
			float num = 3f;
			bool crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
			Vector3 position = this.inputBank ? this.inputBank.aimOrigin : base.transform.position;
			Vector3 vector = this.inputBank ? this.inputBank.aimDirection : base.transform.forward;
			Vector3 a = Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f;
			ProjectileManager.instance.FireProjectile(prefab, position, Util.QuaternionSafeLookRotation(a + UnityEngine.Random.insideUnitSphere * 0f), base.gameObject, this.characterBody.damage * num, 200f, crit, DamageColorIndex.Item, null, -1f);
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0002E838 File Offset: 0x0002CA38
		[Server]
		private bool PerformEquipmentAction(EquipmentIndex equipmentIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.EquipmentSlot::PerformEquipmentAction(RoR2.EquipmentIndex)' called on client");
				return false;
			}
			switch (equipmentIndex)
			{
			case EquipmentIndex.CommandMissile:
				this.remainingMissiles += 12;
				return true;
			case EquipmentIndex.Saw:
			{
				Vector3 position = base.transform.position;
				Ray aimRay = this.GetAimRay();
				bool crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.characterBody.damage, 0f, crit, DamageColorIndex.Default, null, -1f);
				return true;
			}
			case EquipmentIndex.Fruit:
				if (this.healthComponent)
				{
					Util.PlaySound("Play_item_use_fruit", base.gameObject);
					EffectData effectData = new EffectData();
					effectData.origin = base.transform.position;
					effectData.SetNetworkedObjectReference(base.gameObject);
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FruitHealEffect"), effectData, true);
					this.healthComponent.HealFraction(0.5f, default(ProcChainMask));
					return true;
				}
				return true;
			case EquipmentIndex.Meteor:
			{
				MeteorStormController component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"), this.characterBody.corePosition, Quaternion.identity).GetComponent<MeteorStormController>();
				component.owner = base.gameObject;
				component.ownerDamage = this.characterBody.damage;
				component.isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				NetworkServer.Spawn(component.gameObject);
				return true;
			}
			case EquipmentIndex.SoulJar:
				return true;
			case EquipmentIndex.Blackhole:
			{
				Vector3 position2 = base.transform.position;
				Ray aimRay2 = this.GetAimRay();
				ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/GravSphere"), position2, Util.QuaternionSafeLookRotation(aimRay2.direction), base.gameObject, 0f, 0f, false, DamageColorIndex.Default, null, -1f);
				return true;
			}
			case EquipmentIndex.GhostGun:
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GhostGun"), base.transform.position, Quaternion.identity);
				gameObject.GetComponent<GhostGunController>().owner = base.gameObject;
				NetworkServer.Spawn(gameObject);
				return true;
			}
			case EquipmentIndex.CritOnUse:
				this.characterBody.AddTimedBuff(BuffIndex.FullCrit, 8f);
				return true;
			case EquipmentIndex.DroneBackup:
			{
				Util.PlaySound("Play_item_use_radio", base.gameObject);
				int sliceCount = 4;
				float num = 25f;
				if (NetworkServer.active)
				{
					float y = Quaternion.LookRotation(this.GetAimRay().direction).eulerAngles.y;
					float d = 3f;
					foreach (float num2 in new DegreeSlices(sliceCount, 0.5f))
					{
						Quaternion rotation = Quaternion.Euler(0f, y + num2, 0f);
						Quaternion rotation2 = Quaternion.Euler(0f, y + num2 + 180f, 0f);
						Vector3 position3 = base.transform.position + rotation * (Vector3.forward * d);
						CharacterMaster characterMaster = this.SummonMaster(Resources.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster"), position3, rotation2);
						if (characterMaster)
						{
							characterMaster.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = num + UnityEngine.Random.Range(0f, 3f);
						}
					}
				}
				this.subcooldownTimer = 0.5f;
				return true;
			}
			case EquipmentIndex.OrbitalLaser:
			{
				Vector3 position4 = base.transform.position;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.GetAimRay(), out raycastHit, 900f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					position4 = raycastHit.point;
				}
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/NetworkedObjects/OrbitalLaser"), position4, Quaternion.identity);
				gameObject2.GetComponent<OrbitalLaserController>().ownerBody = this.characterBody;
				NetworkServer.Spawn(gameObject2);
				return true;
			}
			case EquipmentIndex.BFG:
				this.bfgChargeTimer = 2f;
				this.subcooldownTimer = 2.2f;
				return true;
			case EquipmentIndex.Enigma:
			{
				EquipmentIndex equipmentIndex2 = EquipmentCatalog.enigmaEquipmentList[UnityEngine.Random.Range(0, EquipmentCatalog.enigmaEquipmentList.Count - 1)];
				this.PerformEquipmentAction(equipmentIndex2);
				return true;
			}
			case EquipmentIndex.Jetpack:
			{
				JetpackController jetpackController = JetpackController.FindJetpackController(base.gameObject);
				if (!jetpackController)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/JetpackController"));
					jetpackController = gameObject3.GetComponent<JetpackController>();
					jetpackController.NetworktargetObject = base.gameObject;
					NetworkServer.Spawn(gameObject3);
					return true;
				}
				jetpackController.ResetTimer();
				return true;
			}
			case EquipmentIndex.Lightning:
			{
				this.UpdateTargets();
				HurtBox hurtBox = this.currentTargetHurtBox;
				if (hurtBox)
				{
					this.subcooldownTimer = 0.2f;
					OrbManager.instance.AddOrb(new LightningStrikeOrb
					{
						attacker = base.gameObject,
						damageColorIndex = DamageColorIndex.Item,
						damageValue = this.characterBody.damage * 30f,
						isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master),
						procChainMask = default(ProcChainMask),
						procCoefficient = 1f,
						target = hurtBox
					});
					this.InvalidateCurrentTarget();
					return true;
				}
				return false;
			}
			case EquipmentIndex.PassiveHealing:
				if (this.passiveHealingFollower)
				{
					this.UpdateTargets();
					this.passiveHealingFollower.AssignNewTarget(this.currentTargetBodyObject);
					this.InvalidateCurrentTarget();
					return true;
				}
				return true;
			case EquipmentIndex.BurnNearby:
				if (this.characterBody)
				{
					this.characterBody.AddHelfireDuration(8f);
					return true;
				}
				return true;
			case EquipmentIndex.SoulCorruptor:
			{
				this.UpdateTargets();
				HurtBox hurtBox2 = this.currentTargetHurtBox;
				if (!hurtBox2)
				{
					return false;
				}
				if (!hurtBox2.healthComponent || hurtBox2.healthComponent.combinedHealthFraction > 0.25f)
				{
					return false;
				}
				Util.TryToCreateGhost(hurtBox2.healthComponent.body, this.characterBody, 30);
				hurtBox2.healthComponent.Suicide(base.gameObject, null, DamageType.Generic);
				this.InvalidateCurrentTarget();
				return true;
			}
			case EquipmentIndex.Scanner:
				NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), this.characterBody.corePosition, Quaternion.identity));
				return true;
			case EquipmentIndex.CrippleWard:
				NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/CrippleWard"), this.characterBody.corePosition, Quaternion.identity));
				this.inventory.SetEquipmentIndex(EquipmentIndex.None);
				return true;
			case EquipmentIndex.Gateway:
				return this.FireGateway();
			case EquipmentIndex.Tonic:
				this.characterBody.AddTimedBuff(BuffIndex.TonicBuff, EquipmentSlot.tonicBuffDuration);
				if (!Util.CheckRoll(80f, this.characterBody.master))
				{
					this.characterBody.pendingTonicAfflictionCount++;
					return true;
				}
				return true;
			case EquipmentIndex.QuestVolatileBattery:
				return false;
			case EquipmentIndex.Cleanse:
			{
				Vector3 corePosition = this.characterBody.corePosition;
				EffectData effectData2 = new EffectData
				{
					origin = corePosition
				};
				effectData2.SetHurtBoxReference(this.characterBody.mainHurtBox);
				EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CleanseEffect"), effectData2, true);
				BuffIndex buffIndex = BuffIndex.Slow50;
				BuffIndex buffCount = (BuffIndex)BuffCatalog.buffCount;
				while (buffIndex < buffCount)
				{
					if (BuffCatalog.GetBuffDef(buffIndex).isDebuff)
					{
						this.characterBody.ClearTimedBuffs(buffIndex);
					}
					buffIndex++;
				}
				DotController.RemoveAllDots(base.gameObject);
				SetStateOnHurt component2 = base.GetComponent<SetStateOnHurt>();
				if (component2)
				{
					component2.Cleanse();
				}
				float num3 = 6f;
				float num4 = num3 * num3;
				TeamIndex teamIndex = this.teamComponent.teamIndex;
				List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
				List<ProjectileController> list = new List<ProjectileController>();
				int i = 0;
				int count = instancesList.Count;
				while (i < count)
				{
					ProjectileController projectileController = instancesList[i];
					if (projectileController.teamFilter.teamIndex != teamIndex && (projectileController.transform.position - corePosition).sqrMagnitude < num4)
					{
						list.Add(projectileController);
					}
					i++;
				}
				int j = 0;
				int count2 = list.Count;
				while (j < count2)
				{
					ProjectileController projectileController2 = list[j];
					if (projectileController2)
					{
						UnityEngine.Object.Destroy(projectileController2.gameObject);
					}
					j++;
				}
				return true;
			}
			case EquipmentIndex.FireBallDash:
			{
				Ray aimRay3 = this.GetAimRay();
				GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/FireballVehicle"), aimRay3.origin, Quaternion.LookRotation(aimRay3.direction));
				gameObject4.GetComponent<VehicleSeat>().AssignPassenger(base.gameObject);
				CharacterBody characterBody = this.characterBody;
				NetworkUser networkUser;
				if (characterBody == null)
				{
					networkUser = null;
				}
				else
				{
					CharacterMaster master = characterBody.master;
					if (master == null)
					{
						networkUser = null;
					}
					else
					{
						PlayerCharacterMasterController playerCharacterMasterController = master.playerCharacterMasterController;
						networkUser = ((playerCharacterMasterController != null) ? playerCharacterMasterController.networkUser : null);
					}
				}
				NetworkUser networkUser2 = networkUser;
				if (networkUser2)
				{
					NetworkServer.SpawnWithClientAuthority(gameObject4, networkUser2.gameObject);
				}
				else
				{
					NetworkServer.Spawn(gameObject4);
				}
				this.subcooldownTimer = 2f;
				return true;
			}
			case EquipmentIndex.GainArmor:
				this.characterBody.AddTimedBuff(BuffIndex.ElephantArmorBoost, 5f);
				return true;
			}
			return false;
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x0002F194 File Offset: 0x0002D394
		private bool FireGateway()
		{
			Vector3 footPosition = this.characterBody.footPosition;
			Ray aimRay = this.GetAimRay();
			float num = 2f;
			float num2 = num * 2f;
			float maxDistance = 1000f;
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (!component)
			{
				return false;
			}
			Vector3 position = base.transform.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(aimRay, out raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				Vector3 vector = raycastHit.point + raycastHit.normal * num;
				Vector3 vector2 = vector - position;
				Vector3 normalized = vector2.normalized;
				Vector3 pointBPosition = vector;
				RaycastHit raycastHit2;
				if (component.SweepTest(normalized, out raycastHit2, vector2.magnitude))
				{
					if (raycastHit2.distance < num2)
					{
						return false;
					}
					pointBPosition = position + normalized * raycastHit2.distance;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Zipline"));
				ZiplineController component2 = gameObject.GetComponent<ZiplineController>();
				component2.SetPointAPosition(position + normalized * num);
				component2.SetPointBPosition(pointBPosition);
				gameObject.AddComponent<DestroyOnTimer>().duration = 30f;
				NetworkServer.Spawn(gameObject);
				return true;
			}
			return false;
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0002F2B8 File Offset: 0x0002D4B8
		private Ray GetAimRay()
		{
			return new Ray
			{
				direction = this.inputBank.aimDirection,
				origin = this.inputBank.aimOrigin
			};
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0002F2F4 File Offset: 0x0002D4F4
		[Server]
		[CanBeNull]
		private CharacterMaster SummonMaster([NotNull] GameObject masterPrefab, Vector3 position, Quaternion rotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.EquipmentSlot::SummonMaster(UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return null;
			}
			return new MasterSummon
			{
				masterPrefab = masterPrefab,
				position = position,
				rotation = rotation,
				summonerBodyObject = base.gameObject,
				ignoreTeamMemberLimit = false
			}.Perform();
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0002F354 File Offset: 0x0002D554
		private void ConfigureTargetFinderBase()
		{
			this.targetFinder.teamMaskFilter = TeamMask.allButNeutral;
			this.targetFinder.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.targetFinder.sortMode = BullseyeSearch.SortMode.Angle;
			this.targetFinder.filterByLoS = true;
			float num;
			Ray ray = CameraRigController.ModifyAimRayIfApplicable(this.GetAimRay(), base.gameObject, out num);
			this.targetFinder.searchOrigin = ray.origin;
			this.targetFinder.searchDirection = ray.direction;
			this.targetFinder.maxAngleFilter = 10f;
			this.targetFinder.viewer = this.characterBody;
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x0002F3FD File Offset: 0x0002D5FD
		private void ConfigureTargetFinderForEnemies()
		{
			this.ConfigureTargetFinderBase();
			this.targetFinder.teamMaskFilter = TeamMask.all;
			this.targetFinder.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.targetFinder.RefreshCandidates();
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x0002F43C File Offset: 0x0002D63C
		private void ConfigureTargetFinderForFriendlies()
		{
			this.ConfigureTargetFinderBase();
			this.targetFinder.teamMaskFilter = TeamMask.none;
			this.targetFinder.teamMaskFilter.AddTeam(this.teamComponent.teamIndex);
			this.targetFinder.RefreshCandidates();
			this.targetFinder.FilterOutGameObject(base.gameObject);
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0002F496 File Offset: 0x0002D696
		private void InvalidateCurrentTarget()
		{
			this.currentTargetHurtBox = null;
			this.currentTargetBodyObject = null;
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x0002F4A8 File Offset: 0x0002D6A8
		private void UpdateTargets()
		{
			bool flag = this.stock > 0;
			bool flag2 = (this.equipmentIndex == EquipmentIndex.Lightning || this.equipmentIndex == EquipmentIndex.SoulCorruptor) && flag;
			bool flag3 = this.equipmentIndex == EquipmentIndex.PassiveHealing && flag;
			if (flag2 || flag3)
			{
				if (flag2)
				{
					this.ConfigureTargetFinderForEnemies();
				}
				if (flag3)
				{
					this.ConfigureTargetFinderForFriendlies();
				}
				this.currentTargetHurtBox = this.targetFinder.GetResults().FirstOrDefault<HurtBox>();
				this.currentTargetBodyObject = (this.currentTargetHurtBox ? this.currentTargetHurtBox.healthComponent.gameObject : null);
			}
			else
			{
				this.currentTargetHurtBox = null;
			}
			bool flag4 = this.currentTargetHurtBox;
			if (flag4)
			{
				EquipmentIndex equipmentIndex = this.equipmentIndex;
				if (equipmentIndex != EquipmentIndex.Lightning)
				{
					if (equipmentIndex != EquipmentIndex.PassiveHealing)
					{
						this.targetIndicator.visualizerPrefab = Resources.Load<GameObject>("Prefabs/LightningIndicator");
					}
					else
					{
						this.targetIndicator.visualizerPrefab = Resources.Load<GameObject>("Prefabs/WoodSpriteIndicator");
					}
				}
				else
				{
					this.targetIndicator.visualizerPrefab = Resources.Load<GameObject>("Prefabs/LightningIndicator");
				}
			}
			this.targetIndicator.active = flag4;
			this.targetIndicator.targetTransform = (flag4 ? this.currentTargetHurtBox.transform : null);
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x0002F5D2 File Offset: 0x0002D7D2
		private void Update()
		{
			this.UpdateTargets();
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x0002F5F0 File Offset: 0x0002D7F0
		static EquipmentSlot()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(EquipmentSlot), EquipmentSlot.kCmdCmdExecuteIfReady, new NetworkBehaviour.CmdDelegate(EquipmentSlot.InvokeCmdCmdExecuteIfReady));
			EquipmentSlot.kRpcRpcOnClientEquipmentActivationRecieved = 1342577121;
			NetworkBehaviour.RegisterRpcDelegate(typeof(EquipmentSlot), EquipmentSlot.kRpcRpcOnClientEquipmentActivationRecieved, new NetworkBehaviour.CmdDelegate(EquipmentSlot.InvokeRpcRpcOnClientEquipmentActivationRecieved));
			NetworkCRC.RegisterBehaviour("EquipmentSlot", 0);
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x0002F674 File Offset: 0x0002D874
		protected static void InvokeCmdCmdExecuteIfReady(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdExecuteIfReady called on client.");
				return;
			}
			((EquipmentSlot)obj).CmdExecuteIfReady();
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x0002F698 File Offset: 0x0002D898
		public void CallCmdExecuteIfReady()
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdExecuteIfReady called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdExecuteIfReady();
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)EquipmentSlot.kCmdCmdExecuteIfReady);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			base.SendCommandInternal(networkWriter, 0, "CmdExecuteIfReady");
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x0002F714 File Offset: 0x0002D914
		protected static void InvokeRpcRpcOnClientEquipmentActivationRecieved(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcOnClientEquipmentActivationRecieved called on server.");
				return;
			}
			((EquipmentSlot)obj).RpcOnClientEquipmentActivationRecieved();
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x0002F738 File Offset: 0x0002D938
		public void CallRpcOnClientEquipmentActivationRecieved()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcOnClientEquipmentActivationRecieved called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)EquipmentSlot.kRpcRpcOnClientEquipmentActivationRecieved);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcOnClientEquipmentActivationRecieved");
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x0002F7A4 File Offset: 0x0002D9A4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04000AEC RID: 2796
		private Inventory inventory;

		// Token: 0x04000AF1 RID: 2801
		private Run.FixedTimeStamp _rechargeTime;

		// Token: 0x04000AF2 RID: 2802
		private bool hasEffectiveAuthority;

		// Token: 0x04000AF4 RID: 2804
		private HealthComponent healthComponent;

		// Token: 0x04000AF5 RID: 2805
		private InputBankTest inputBank;

		// Token: 0x04000AF6 RID: 2806
		private TeamComponent teamComponent;

		// Token: 0x04000AF7 RID: 2807
		private const float fullCritDuration = 8f;

		// Token: 0x04000AF8 RID: 2808
		private static readonly float tonicBuffDuration = 20f;

		// Token: 0x04000AF9 RID: 2809
		public static string equipmentActivateString = "Play_UI_equipment_activate";

		// Token: 0x04000AFA RID: 2810
		private float missileTimer;

		// Token: 0x04000AFB RID: 2811
		private float bfgChargeTimer;

		// Token: 0x04000AFC RID: 2812
		private float subcooldownTimer;

		// Token: 0x04000AFD RID: 2813
		private const float missileInterval = 0.125f;

		// Token: 0x04000AFE RID: 2814
		private int remainingMissiles;

		// Token: 0x04000AFF RID: 2815
		private HealingFollowerController passiveHealingFollower;

		// Token: 0x04000B00 RID: 2816
		private GameObject goldgatControllerObject;

		// Token: 0x04000B02 RID: 2818
		private Indicator targetIndicator;

		// Token: 0x04000B03 RID: 2819
		private BullseyeSearch targetFinder = new BullseyeSearch();

		// Token: 0x04000B04 RID: 2820
		private HurtBox currentTargetHurtBox;

		// Token: 0x04000B05 RID: 2821
		private GameObject currentTargetBodyObject;

		// Token: 0x04000B06 RID: 2822
		private static int kRpcRpcOnClientEquipmentActivationRecieved;

		// Token: 0x04000B07 RID: 2823
		private static int kCmdCmdExecuteIfReady = -303452611;
	}
}
