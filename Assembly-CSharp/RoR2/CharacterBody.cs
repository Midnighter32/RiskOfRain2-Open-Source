using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.CharacterAI;
using RoR2.Networking;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200027F RID: 639
	[RequireComponent(typeof(TeamComponent))]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SkillLocator))]
	public class CharacterBody : NetworkBehaviour, ILifeBehavior, IDisplayNameProvider
	{
		// Token: 0x06000C15 RID: 3093 RVA: 0x0003C2B3 File Offset: 0x0003A4B3
		public string GetDisplayName()
		{
			return Language.GetString(this.baseNameToken);
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0003C2C0 File Offset: 0x0003A4C0
		public string GetSubtitle()
		{
			return Language.GetString(this.subtitleNameToken);
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0003C2D0 File Offset: 0x0003A4D0
		public string GetUserName()
		{
			string text = "";
			if (this.master)
			{
				PlayerCharacterMasterController component = this.master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					text = component.GetDisplayName();
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = this.GetDisplayName();
			}
			return text;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0003C31C File Offset: 0x0003A51C
		public string GetColoredUserName()
		{
			Color32 userColor = new Color32(127, 127, 127, byte.MaxValue);
			string text = null;
			if (this.master)
			{
				PlayerCharacterMasterController component = this.master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					GameObject networkUserObject = component.networkUserObject;
					if (networkUserObject)
					{
						NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
						if (component2)
						{
							userColor = component2.userColor;
							text = component2.userName;
						}
					}
				}
			}
			if (text == null)
			{
				text = this.GetDisplayName();
			}
			return Util.GenerateColoredString(text, userColor);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0003C39F File Offset: 0x0003A59F
		[Server]
		public void AddBuff(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddBuff(RoR2.BuffIndex)' called on client");
				return;
			}
			this.SetBuffCount(buffType, this.buffs[(int)buffType] + 1);
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0003C3C8 File Offset: 0x0003A5C8
		[Server]
		public void RemoveBuff(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::RemoveBuff(RoR2.BuffIndex)' called on client");
				return;
			}
			this.SetBuffCount(buffType, this.buffs[(int)buffType] - 1);
			if (buffType == BuffIndex.MedkitHeal && this.GetBuffCount(BuffIndex.MedkitHeal) == 0)
			{
				int itemCount = this.inventory.GetItemCount(ItemIndex.Medkit);
				this.healthComponent.Heal((float)itemCount * 10f, default(ProcChainMask), true);
				Util.PlaySound("Play_item_proc_crit_heal", base.gameObject);
				EffectData effectData = new EffectData();
				effectData.origin = this.transform.position;
				effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/MedkitHealEffect"), effectData, true);
			}
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0003C484 File Offset: 0x0003A684
		[Server]
		private void SetBuffCount(BuffIndex buffType, int newCount)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::SetBuffCount(RoR2.BuffIndex,System.Int32)' called on client");
				return;
			}
			if (newCount == this.buffs[(int)buffType])
			{
				return;
			}
			this.buffs[(int)buffType] = newCount;
			BuffMask a = (newCount == 0) ? this.buffMask.GetBuffRemoved(buffType) : this.buffMask.GetBuffAdded(buffType);
			if (a != this.buffMask)
			{
				BuffMask oldBuffMask = this.buffMask;
				this.buffMask = a;
				if (NetworkClient.active)
				{
					this.OnClientBuffsChanged(oldBuffMask);
				}
				base.SetDirtyBit(2u);
			}
			if (buffType == BuffIndex.AttackSpeedOnCrit)
			{
				base.SetDirtyBit(4u);
			}
			this.statsDirty = true;
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x0003C51C File Offset: 0x0003A71C
		public int GetBuffCount(BuffIndex buffType)
		{
			if (NetworkServer.active)
			{
				return this.buffs[(int)buffType];
			}
			if (BuffCatalog.GetBuffDef(buffType).canStack)
			{
				return this.buffs[(int)buffType];
			}
			if (!this.HasBuff(buffType))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0003C550 File Offset: 0x0003A750
		public bool HasBuff(BuffIndex buffType)
		{
			return this.buffMask.HasBuff(buffType);
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0003C560 File Offset: 0x0003A760
		[Server]
		public void AddTimedBuff(BuffIndex buffType, float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddTimedBuff(RoR2.BuffIndex,System.Single)' called on client");
				return;
			}
			if (buffType != BuffIndex.AttackSpeedOnCrit)
			{
				if (buffType != BuffIndex.BeetleJuice)
				{
					bool flag = false;
					for (int i = 0; i < this.timedBuffs.Count; i++)
					{
						if (this.timedBuffs[i].buffIndex == buffType)
						{
							flag = true;
							this.timedBuffs[i].timer = Mathf.Max(this.timedBuffs[i].timer, duration);
							break;
						}
					}
					if (!flag)
					{
						this.timedBuffs.Add(new CharacterBody.TimedBuff
						{
							buffIndex = buffType,
							timer = duration
						});
						this.AddBuff(buffType);
					}
				}
				else
				{
					int num = 0;
					for (int j = 0; j < this.timedBuffs.Count; j++)
					{
						if (this.timedBuffs[j].buffIndex == buffType)
						{
							num++;
							if (this.timedBuffs[j].timer < duration)
							{
								this.timedBuffs[j].timer = duration;
							}
						}
					}
					if (num < 10)
					{
						this.timedBuffs.Add(new CharacterBody.TimedBuff
						{
							buffIndex = buffType,
							timer = duration
						});
						this.AddBuff(buffType);
						return;
					}
				}
				return;
			}
			int num2 = this.inventory ? this.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit) : 0;
			int num3 = 0;
			int num4 = -1;
			float num5 = 999f;
			for (int k = 0; k < this.timedBuffs.Count; k++)
			{
				if (this.timedBuffs[k].buffIndex == buffType)
				{
					num3++;
					if (this.timedBuffs[k].timer < num5)
					{
						num4 = k;
						num5 = this.timedBuffs[k].timer;
					}
				}
			}
			if (num3 < 1 + num2 * 2)
			{
				this.timedBuffs.Add(new CharacterBody.TimedBuff
				{
					buffIndex = buffType,
					timer = duration
				});
				this.AddBuff(buffType);
				ChildLocator component = this.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("HandL");
					Transform transform2 = component.FindChild("HandR");
					if (transform)
					{
						UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/WolfProcEffect"), transform).transform.localScale = Vector3.one * (float)num3;
					}
					if (transform2)
					{
						UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/WolfProcEffect"), transform2).transform.localScale = Vector3.one * (float)num3;
					}
				}
			}
			else if (num4 > -1)
			{
				this.timedBuffs[num4].timer = duration;
			}
			Util.PlaySound("Play_item_proc_crit_attack_speed" + Mathf.Min(3, num3 + 1), base.gameObject);
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0003C83C File Offset: 0x0003AA3C
		[Server]
		public void ClearTimedBuffs(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::ClearTimedBuffs(RoR2.BuffIndex)' called on client");
				return;
			}
			for (int i = this.timedBuffs.Count - 1; i >= 0; i--)
			{
				if (this.timedBuffs[i].buffIndex == buffType)
				{
					this.RemoveBuff(this.timedBuffs[i].buffIndex);
					this.timedBuffs.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0003C8B0 File Offset: 0x0003AAB0
		[Server]
		private void UpdateBuffs(float deltaTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateBuffs(System.Single)' called on client");
				return;
			}
			for (int i = this.timedBuffs.Count - 1; i >= 0; i--)
			{
				this.timedBuffs[i].timer -= deltaTime;
				if (this.timedBuffs[i].timer <= 0f)
				{
					this.RemoveBuff(this.timedBuffs[i].buffIndex);
					this.timedBuffs.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0003C940 File Offset: 0x0003AB40
		[Client]
		private void OnClientBuffsChanged(BuffMask oldBuffMask)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::OnClientBuffsChanged(RoR2.BuffMask)' called on server");
				return;
			}
			bool flag = this.buffMask.HasBuff(BuffIndex.WarCryBuff);
			if (!flag && this.warCryEffectInstance)
			{
				UnityEngine.Object.Destroy(this.warCryEffectInstance);
			}
			if (flag && !this.warCryEffectInstance)
			{
				Debug.Log("should spawn warcry");
				Transform transform = this.mainHurtBox ? this.mainHurtBox.transform : this.transform;
				if (transform)
				{
					Debug.Log("main hurtbox found");
					this.warCryEffectInstance = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Effects/WarCryEffect"), transform.position, Quaternion.identity, transform);
				}
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000C22 RID: 3106 RVA: 0x0003C9FC File Offset: 0x0003ABFC
		public CharacterMaster master
		{
			get
			{
				if (!this.masterObject)
				{
					return null;
				}
				return this._master;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000C23 RID: 3107 RVA: 0x0003CA13 File Offset: 0x0003AC13
		// (set) Token: 0x06000C24 RID: 3108 RVA: 0x0003CA1B File Offset: 0x0003AC1B
		public Inventory inventory { get; private set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000C25 RID: 3109 RVA: 0x0003CA24 File Offset: 0x0003AC24
		// (set) Token: 0x06000C26 RID: 3110 RVA: 0x0003CA2C File Offset: 0x0003AC2C
		public bool isPlayerControlled { get; private set; }

		// Token: 0x06000C27 RID: 3111 RVA: 0x0003CA38 File Offset: 0x0003AC38
		private void OnInventoryChanged()
		{
			EquipmentIndex currentEquipmentIndex = this.inventory.currentEquipmentIndex;
			if (currentEquipmentIndex != this.previousEquipmentIndex)
			{
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(this.previousEquipmentIndex);
				EquipmentDef equipmentDef2 = EquipmentCatalog.GetEquipmentDef(currentEquipmentIndex);
				if (equipmentDef != null)
				{
					this.OnEquipmentLost(equipmentDef);
				}
				if (equipmentDef2 != null)
				{
					this.OnEquipmentGained(equipmentDef2);
				}
				this.previousEquipmentIndex = currentEquipmentIndex;
			}
			this.statsDirty = true;
			bool flag = this.inventory.GetItemCount(ItemIndex.Ghost) > 0;
			if (flag != this.disablingHurtBoxes)
			{
				if (this.hurtBoxGroup)
				{
					if (flag)
					{
						HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
						int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
						hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
					}
					else
					{
						HurtBoxGroup hurtBoxGroup2 = this.hurtBoxGroup;
						int hurtBoxesDeactivatorCounter = hurtBoxGroup2.hurtBoxesDeactivatorCounter - 1;
						hurtBoxGroup2.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
					}
				}
				this.disablingHurtBoxes = flag;
			}
			this.AddItemBehavior<CharacterBody.MushroomItemBehavior>(this.inventory.GetItemCount(ItemIndex.Mushroom));
			this.AddItemBehavior<CharacterBody.IcicleItemBehavior>(this.inventory.GetItemCount(ItemIndex.Icicle));
			this.AddItemBehavior<CharacterBody.HeadstomperItemBehavior>(this.inventory.GetItemCount(ItemIndex.FallBoots));
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x0003CB3C File Offset: 0x0003AD3C
		private void OnEquipmentLost(EquipmentDef equipmentDef)
		{
			if (NetworkServer.active && equipmentDef.passiveBuff != BuffIndex.None)
			{
				this.RemoveBuff(equipmentDef.passiveBuff);
			}
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x0003CB5A File Offset: 0x0003AD5A
		private void OnEquipmentGained(EquipmentDef equipmentDef)
		{
			if (NetworkServer.active && equipmentDef.passiveBuff != BuffIndex.None)
			{
				this.AddBuff(equipmentDef.passiveBuff);
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000C2A RID: 3114 RVA: 0x0003CB78 File Offset: 0x0003AD78
		// (remove) Token: 0x06000C2B RID: 3115 RVA: 0x0003CBB0 File Offset: 0x0003ADB0
		public event Action onInventoryChanged;

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000C2C RID: 3116 RVA: 0x0003CBE8 File Offset: 0x0003ADE8
		// (set) Token: 0x06000C2D RID: 3117 RVA: 0x0003CCFA File Offset: 0x0003AEFA
		public GameObject masterObject
		{
			get
			{
				if (!this._masterObject)
				{
					if (NetworkServer.active)
					{
						this._masterObject = NetworkServer.FindLocalObject(this.masterObjectId);
					}
					else if (NetworkClient.active)
					{
						this._masterObject = ClientScene.FindLocalObject(this.masterObjectId);
					}
					this._master = (this._masterObject ? this._masterObject.GetComponent<CharacterMaster>() : null);
					if (this._master)
					{
						this.isPlayerControlled = (this._masterObject && this._masterObject.GetComponent<PlayerCharacterMasterController>());
						if (this.inventory)
						{
							this.inventory.onInventoryChanged -= this.OnInventoryChanged;
						}
						this.inventory = this._master.inventory;
						if (this.inventory)
						{
							this.inventory.onInventoryChanged += this.OnInventoryChanged;
							this.OnInventoryChanged();
						}
						this.statsDirty = true;
					}
				}
				return this._masterObject;
			}
			set
			{
				this.masterObjectId = value.GetComponent<NetworkIdentity>().netId;
				this.statsDirty = true;
			}
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x0003CD14 File Offset: 0x0003AF14
		private void UpdateMasterLink()
		{
			if (!this.linkedToMaster && this.master && this.master)
			{
				this.master.OnBodyStart(this);
				this.linkedToMaster = true;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000C2F RID: 3119 RVA: 0x0003CD4B File Offset: 0x0003AF4B
		// (set) Token: 0x06000C30 RID: 3120 RVA: 0x0003CD53 File Offset: 0x0003AF53
		public TeamComponent teamComponent { get; private set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000C31 RID: 3121 RVA: 0x0003CD5C File Offset: 0x0003AF5C
		// (set) Token: 0x06000C32 RID: 3122 RVA: 0x0003CD64 File Offset: 0x0003AF64
		public HealthComponent healthComponent { get; private set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000C33 RID: 3123 RVA: 0x0003CD6D File Offset: 0x0003AF6D
		// (set) Token: 0x06000C34 RID: 3124 RVA: 0x0003CD75 File Offset: 0x0003AF75
		public EquipmentSlot equipmentSlot { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000C35 RID: 3125 RVA: 0x0003CD7E File Offset: 0x0003AF7E
		// (set) Token: 0x06000C36 RID: 3126 RVA: 0x0003CD86 File Offset: 0x0003AF86
		public ModelLocator modelLocator { get; private set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000C37 RID: 3127 RVA: 0x0003CD8F File Offset: 0x0003AF8F
		// (set) Token: 0x06000C38 RID: 3128 RVA: 0x0003CD97 File Offset: 0x0003AF97
		public HurtBoxGroup hurtBoxGroup { get; private set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000C39 RID: 3129 RVA: 0x0003CDA0 File Offset: 0x0003AFA0
		// (set) Token: 0x06000C3A RID: 3130 RVA: 0x0003CDA8 File Offset: 0x0003AFA8
		public HurtBox mainHurtBox { get; private set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000C3B RID: 3131 RVA: 0x0003CDB1 File Offset: 0x0003AFB1
		// (set) Token: 0x06000C3C RID: 3132 RVA: 0x0003CDB9 File Offset: 0x0003AFB9
		public Transform coreTransform { get; private set; }

		// Token: 0x06000C3D RID: 3133 RVA: 0x0003CDC4 File Offset: 0x0003AFC4
		private void Awake()
		{
			this.transform = base.transform;
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.equipmentSlot = base.GetComponent<EquipmentSlot>();
			this.skillLocator = base.GetComponent<SkillLocator>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.characterMotor = base.GetComponent<CharacterMotor>();
			this.sfxLocator = base.GetComponent<SfxLocator>();
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				this.hurtBoxGroup = ((modelTransform != null) ? modelTransform.GetComponent<HurtBoxGroup>() : null);
				if (this.hurtBoxGroup)
				{
					this.mainHurtBox = this.hurtBoxGroup.mainHurtBox;
				}
			}
			HurtBox mainHurtBox = this.mainHurtBox;
			this.coreTransform = (((mainHurtBox != null) ? mainHurtBox.transform : null) ?? this.transform);
			this.radius = 1f;
			CapsuleCollider component = base.GetComponent<CapsuleCollider>();
			if (component)
			{
				this.radius = component.radius;
				return;
			}
			SphereCollider component2 = base.GetComponent<SphereCollider>();
			if (component2)
			{
				this.radius = component2.radius;
			}
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x0003CEE0 File Offset: 0x0003B0E0
		private void Start()
		{
			bool flag = (this.bodyFlags & CharacterBody.BodyFlags.Masterless) > CharacterBody.BodyFlags.None;
			this.outOfCombatStopwatch = float.PositiveInfinity;
			this.outOfDangerStopwatch = float.PositiveInfinity;
			this.notMovingStopwatch = 0f;
			this.warCryTimer = 30f;
			if (NetworkServer.active)
			{
				this.outOfCombat = true;
				this.outOfDanger = true;
			}
			GlobalEventManager.instance.OnCharacterBodyStart(this);
			this.RecalculateStats();
			this.UpdateMasterLink();
			if (flag)
			{
				this.healthComponent.Networkhealth = this.maxHealth;
			}
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x0003CF63 File Offset: 0x0003B163
		public void Update()
		{
			this.UpdateSpreadBloom(Time.deltaTime);
			this.UpdateAllTemporaryVisualEffects();
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x0003CF78 File Offset: 0x0003B178
		public void FixedUpdate()
		{
			this.outOfCombatStopwatch += Time.fixedDeltaTime;
			this.outOfDangerStopwatch += Time.fixedDeltaTime;
			this.aimTimer = Mathf.Max(this.aimTimer - Time.fixedDeltaTime, 0f);
			if (NetworkServer.active)
			{
				this.UpdateMultiKill(Time.fixedDeltaTime);
			}
			this.UpdateMasterLink();
			bool outOfCombat = this.outOfCombat;
			bool flag = outOfCombat;
			if (NetworkServer.active || base.hasAuthority)
			{
				flag = (this.outOfCombatStopwatch >= 5f);
				if (this.outOfCombat != flag)
				{
					if (NetworkServer.active)
					{
						base.SetDirtyBit(8u);
					}
					this.outOfCombat = flag;
					this.statsDirty = true;
				}
			}
			if (NetworkServer.active)
			{
				this.UpdateBuffs(Time.fixedDeltaTime);
				bool flag2 = this.outOfDangerStopwatch >= 7f;
				bool outOfDanger = this.outOfDanger;
				bool flag3 = outOfCombat && outOfDanger;
				bool flag4 = flag && flag2;
				if (this.outOfDanger != flag2)
				{
					base.SetDirtyBit(16u);
					this.outOfDanger = flag2;
					this.statsDirty = true;
				}
				if (flag4 && !flag3)
				{
					this.OnOutOfCombatAndDangerServer();
				}
				Vector3 position = this.transform.position;
				float num = 0.1f * Time.fixedDeltaTime;
				if ((position - this.previousPosition).sqrMagnitude <= num * num)
				{
					this.notMovingStopwatch += Time.fixedDeltaTime;
				}
				else
				{
					this.notMovingStopwatch = 0f;
				}
				this.previousPosition = position;
				this.UpdateTeslaCoil();
				this.UpdateBeetleGuardAllies();
				this.UpdateHelfire();
				int num2 = 0;
				if (this.inventory)
				{
					num2 = this.inventory.GetItemCount(ItemIndex.WarCryOnCombat);
				}
				if (num2 > 0)
				{
					this.warCryTimer -= Time.fixedDeltaTime;
					this.warCryReady = (this.warCryTimer <= 0f);
					if (this.warCryReady && (!this.outOfCombat && outOfCombat))
					{
						this.warCryTimer = 30f;
						this.ActivateWarCryAura(num2);
					}
				}
			}
			if (this.statsDirty)
			{
				this.RecalculateStats();
			}
			this.UpdateFireTrail();
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x0003D183 File Offset: 0x0003B383
		public void OnDeathStart()
		{
			base.enabled = false;
			if (this.master)
			{
				this.master.OnBodyDeath();
			}
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x0003D1A4 File Offset: 0x0003B3A4
		public void OnTakeDamage(DamageInfo damageInfo)
		{
			this.outOfDangerStopwatch = 0f;
			if (this.master)
			{
				this.master.OnBodyDamaged(damageInfo);
			}
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x0003D1CA File Offset: 0x0003B3CA
		public void OnSkillActivated(GenericSkill skill)
		{
			if (skill.isCombatSkill)
			{
				this.outOfCombatStopwatch = 0f;
			}
			if (!NetworkServer.active)
			{
				this.CallCmdOnSkillActivated((sbyte)this.skillLocator.FindSkillSlot(skill));
				return;
			}
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x00004507 File Offset: 0x00002707
		public void OnDamageDealt(DamageReport damageReport)
		{
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0003D1F9 File Offset: 0x0003B3F9
		public void OnDestroy()
		{
			if (this.inventory)
			{
				this.inventory.onInventoryChanged -= this.OnInventoryChanged;
			}
			if (this.master)
			{
				this.master.OnBodyDestroyed();
			}
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x0003D238 File Offset: 0x0003B438
		public float GetNormalizedThreatValue()
		{
			if (Run.instance)
			{
				return (this.master ? this.master.money : 0f) / Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 2f);
			}
			return 0f;
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x0003D28E File Offset: 0x0003B48E
		private void OnEnable()
		{
			CharacterBody.instancesList.Add(this);
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x0003D29B File Offset: 0x0003B49B
		private void OnDisable()
		{
			CharacterBody.instancesList.Remove(this);
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000C49 RID: 3145 RVA: 0x0003D2A9 File Offset: 0x0003B4A9
		// (set) Token: 0x06000C4A RID: 3146 RVA: 0x0003D2B4 File Offset: 0x0003B4B4
		public bool isSprinting
		{
			get
			{
				return this._isSprinting;
			}
			set
			{
				if (this._isSprinting != value)
				{
					this._isSprinting = value;
					this.RecalculateStats();
					if (value)
					{
						this.OnSprintStart();
					}
					else
					{
						this.OnSprintStop();
					}
					if (NetworkServer.active)
					{
						base.SetDirtyBit(32u);
						return;
					}
					if (base.hasAuthority)
					{
						this.CallCmdUpdateSprint(value);
					}
				}
			}
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x00004507 File Offset: 0x00002707
		private void OnSprintStart()
		{
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00004507 File Offset: 0x00002707
		private void OnSprintStop()
		{
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x0003D307 File Offset: 0x0003B507
		[Command]
		private void CmdUpdateSprint(bool newIsSprinting)
		{
			this.isSprinting = newIsSprinting;
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x0003D310 File Offset: 0x0003B510
		[Command]
		private void CmdOnSkillActivated(sbyte skillIndex)
		{
			this.OnSkillActivated(this.skillLocator.GetSkill((SkillSlot)skillIndex));
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x0003D324 File Offset: 0x0003B524
		// (set) Token: 0x06000C50 RID: 3152 RVA: 0x0003D32C File Offset: 0x0003B52C
		public bool outOfCombat { get; private set; } = true;

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x0003D335 File Offset: 0x0003B535
		// (set) Token: 0x06000C52 RID: 3154 RVA: 0x0003D33D File Offset: 0x0003B53D
		public bool outOfDanger
		{
			get
			{
				return this._outOfDanger;
			}
			private set
			{
				if (this._outOfDanger == value)
				{
					return;
				}
				this._outOfDanger = value;
				this.OnOutOfDangerChanged();
			}
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x0003D356 File Offset: 0x0003B556
		private void OnOutOfDangerChanged()
		{
			if (this.outOfDanger && this.healthComponent.shield != this.healthComponent.fullShield)
			{
				Util.PlaySound("Play_item_proc_personal_shield_recharge", base.gameObject);
			}
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x0003D38C File Offset: 0x0003B58C
		[Server]
		private void OnOutOfCombatAndDangerServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::OnOutOfCombatAndDangerServer()' called on client");
				return;
			}
			if (this.inventory && this.inventory.GetItemCount(ItemIndex.SprintOutOfCombat) > 0)
			{
				EffectData effectData = new EffectData();
				effectData.origin = this.corePosition;
				CharacterDirection component = base.GetComponent<CharacterDirection>();
				bool flag = false;
				if (component && component.moveVector != Vector3.zero)
				{
					effectData.rotation = Util.QuaternionSafeLookRotation(component.moveVector);
					flag = true;
				}
				if (!flag)
				{
					effectData.rotation = this.transform.rotation;
				}
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/SprintActivate"), effectData, true);
			}
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x0003D440 File Offset: 0x0003B640
		[Server]
		public bool GetNotMoving()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.CharacterBody::GetNotMoving()' called on client");
				return false;
			}
			return this.notMovingStopwatch >= 2f;
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x0003D468 File Offset: 0x0003B668
		// (set) Token: 0x06000C57 RID: 3159 RVA: 0x0003D470 File Offset: 0x0003B670
		public float experience { get; private set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x0003D479 File Offset: 0x0003B679
		// (set) Token: 0x06000C59 RID: 3161 RVA: 0x0003D481 File Offset: 0x0003B681
		public float level { get; private set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0003D48A File Offset: 0x0003B68A
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x0003D492 File Offset: 0x0003B692
		public float maxHealth { get; private set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0003D49B File Offset: 0x0003B69B
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x0003D4A3 File Offset: 0x0003B6A3
		public float regen { get; private set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x0003D4AC File Offset: 0x0003B6AC
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x0003D4B4 File Offset: 0x0003B6B4
		public float maxShield { get; private set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x0003D4BD File Offset: 0x0003B6BD
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x0003D4C5 File Offset: 0x0003B6C5
		public float moveSpeed { get; private set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000C62 RID: 3170 RVA: 0x0003D4CE File Offset: 0x0003B6CE
		// (set) Token: 0x06000C63 RID: 3171 RVA: 0x0003D4D6 File Offset: 0x0003B6D6
		public float acceleration { get; private set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x0003D4DF File Offset: 0x0003B6DF
		// (set) Token: 0x06000C65 RID: 3173 RVA: 0x0003D4E7 File Offset: 0x0003B6E7
		public float jumpPower { get; private set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x0003D4F0 File Offset: 0x0003B6F0
		// (set) Token: 0x06000C67 RID: 3175 RVA: 0x0003D4F8 File Offset: 0x0003B6F8
		public int maxJumpCount { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x0003D501 File Offset: 0x0003B701
		// (set) Token: 0x06000C69 RID: 3177 RVA: 0x0003D509 File Offset: 0x0003B709
		public float maxJumpHeight { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x0003D512 File Offset: 0x0003B712
		// (set) Token: 0x06000C6B RID: 3179 RVA: 0x0003D51A File Offset: 0x0003B71A
		public float damage { get; private set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x0003D523 File Offset: 0x0003B723
		// (set) Token: 0x06000C6D RID: 3181 RVA: 0x0003D52B File Offset: 0x0003B72B
		public float attackSpeed { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000C6E RID: 3182 RVA: 0x0003D534 File Offset: 0x0003B734
		// (set) Token: 0x06000C6F RID: 3183 RVA: 0x0003D53C File Offset: 0x0003B73C
		public float crit { get; private set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x0003D545 File Offset: 0x0003B745
		// (set) Token: 0x06000C71 RID: 3185 RVA: 0x0003D54D File Offset: 0x0003B74D
		public float armor { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x0003D556 File Offset: 0x0003B756
		// (set) Token: 0x06000C73 RID: 3187 RVA: 0x0003D55E File Offset: 0x0003B75E
		public float critHeal { get; private set; }

		// Token: 0x06000C74 RID: 3188 RVA: 0x0003D567 File Offset: 0x0003B767
		public float CalcLunarDaggerPower()
		{
			if (this.inventory)
			{
				return Mathf.Pow(2f, (float)this.inventory.GetItemCount(ItemIndex.LunarDagger));
			}
			return 1f;
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x0003D594 File Offset: 0x0003B794
		public void RecalculateStats()
		{
			this.experience = TeamManager.instance.GetTeamExperience(this.teamComponent.teamIndex);
			this.level = TeamManager.instance.GetTeamLevel(this.teamComponent.teamIndex);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int bonusStockFromBody = 0;
			int num19 = 0;
			int num20 = 0;
			int num21 = 0;
			int num22 = 0;
			float num23 = 1f;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			uint num24 = 0u;
			if (this.inventory)
			{
				this.level += (float)this.inventory.GetItemCount(ItemIndex.LevelBonus);
				num = this.inventory.GetItemCount(ItemIndex.Infusion);
				num2 = this.inventory.GetItemCount(ItemIndex.HealWhileSafe);
				num3 = this.inventory.GetItemCount(ItemIndex.PersonalShield);
				num4 = this.inventory.GetItemCount(ItemIndex.Hoof);
				num5 = this.inventory.GetItemCount(ItemIndex.SprintOutOfCombat);
				num6 = this.inventory.GetItemCount(ItemIndex.Feather);
				num7 = this.inventory.GetItemCount(ItemIndex.Syringe);
				num8 = this.inventory.GetItemCount(ItemIndex.CritGlasses);
				num9 = this.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit);
				num10 = this.inventory.GetItemCount(ItemIndex.CooldownOnCrit);
				num11 = this.inventory.GetItemCount(ItemIndex.HealOnCrit);
				num12 = this.GetBuffCount(BuffIndex.BeetleJuice);
				num13 = this.inventory.GetItemCount(ItemIndex.ShieldOnly);
				num14 = this.inventory.GetItemCount(ItemIndex.AlienHead);
				num15 = this.inventory.GetItemCount(ItemIndex.Knurl);
				num16 = this.inventory.GetItemCount(ItemIndex.BoostHp);
				num17 = this.inventory.GetItemCount(ItemIndex.CritHeal);
				num18 = this.inventory.GetItemCount(ItemIndex.SprintBonus);
				bonusStockFromBody = this.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine);
				num19 = this.inventory.GetItemCount(ItemIndex.SprintArmor);
				num20 = this.inventory.GetItemCount(ItemIndex.UtilitySkillMagazine);
				num21 = this.inventory.GetItemCount(ItemIndex.HealthDecay);
				num23 = this.CalcLunarDaggerPower();
				equipmentIndex = this.inventory.currentEquipmentIndex;
				num24 = this.inventory.infusionBonus;
				num22 = this.inventory.GetItemCount(ItemIndex.DrizzlePlayerHelper);
			}
			float num25 = this.level - 1f;
			this.isElite = this.buffMask.containsEliteBuff;
			float num26 = this.baseMaxHealth + this.levelMaxHealth * num25;
			float num27 = 1f;
			num27 += (float)num16 * 0.1f;
			if (num > 0)
			{
				num26 += num24;
			}
			num26 += (float)num15 * 40f;
			num26 *= num27;
			if (this.HasBuff(BuffIndex.AffixBlue))
			{
				num26 *= 0.5f;
			}
			num26 /= num23;
			this.maxHealth = num26;
			float num28 = this.baseRegen + this.levelRegen * num25;
			num28 *= 2.5f;
			if (this.outOfDanger && num2 > 0)
			{
				num28 *= 2.5f + (float)(num2 - 1) * 1.5f;
			}
			num28 += (float)num15 * 1.6f;
			if (num21 > 0)
			{
				num28 -= this.maxHealth / (float)num21;
			}
			this.regen = num28;
			float num29 = this.baseMaxShield + this.levelMaxShield * num25;
			num29 += (float)num3 * 25f;
			if (this.HasBuff(BuffIndex.EngiShield))
			{
				num29 += this.maxHealth * 1f;
			}
			if (this.HasBuff(BuffIndex.EngiTeamShield))
			{
				num29 += this.maxHealth * 0.5f;
			}
			if (this.HasBuff(BuffIndex.AffixBlue))
			{
				num29 += this.maxHealth;
			}
			if (num13 > 0)
			{
				num29 += this.maxHealth * (1.5f + (float)(num13 - 1) * 0.25f);
				this.maxHealth = 1f;
			}
			this.maxShield = num29;
			float num30 = this.baseMoveSpeed + this.levelMoveSpeed * num25;
			float num31 = 1f;
			if (Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Spirit))
			{
				float num32 = 1f;
				if (this.healthComponent)
				{
					num32 = this.healthComponent.combinedHealthFraction;
				}
				num31 += 1f - num32;
			}
			if (equipmentIndex == EquipmentIndex.AffixYellow)
			{
				num30 += 2f;
			}
			if (this.isSprinting)
			{
				num30 *= this.sprintingSpeedMultiplier;
			}
			if (this.outOfCombat && this.outOfDanger && num5 > 0)
			{
				num31 += (float)num5 * 0.3f;
			}
			num31 += (float)num4 * 0.14f;
			if (this.isSprinting && num18 > 0)
			{
				num31 += (0.1f + 0.2f * (float)num18) / this.sprintingSpeedMultiplier;
			}
			if (this.HasBuff(BuffIndex.BugWings))
			{
				num31 += 0.2f;
			}
			if (this.HasBuff(BuffIndex.Warbanner))
			{
				num31 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.EnrageAncientWisp))
			{
				num31 += 0.4f;
			}
			if (this.HasBuff(BuffIndex.CloakSpeed))
			{
				num31 += 0.4f;
			}
			if (this.HasBuff(BuffIndex.TempestSpeed))
			{
				num31 += 1f;
			}
			if (this.HasBuff(BuffIndex.WarCryBuff))
			{
				num31 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.EngiTeamShield))
			{
				num31 += 0.3f;
			}
			float num33 = 1f;
			if (this.HasBuff(BuffIndex.Slow50))
			{
				num33 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.Slow60))
			{
				num33 += 0.6f;
			}
			if (this.HasBuff(BuffIndex.Slow80))
			{
				num33 += 0.8f;
			}
			if (this.HasBuff(BuffIndex.ClayGoo))
			{
				num33 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.Slow30))
			{
				num33 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.Cripple))
			{
				num33 += 1f;
			}
			num30 *= num31 / num33;
			if (num12 > 0)
			{
				num30 *= 1f - 0.05f * (float)num12;
			}
			this.moveSpeed = num30;
			this.acceleration = this.moveSpeed / this.baseMoveSpeed * this.baseAcceleration;
			float jumpPower = this.baseJumpPower + this.levelJumpPower * num25;
			this.jumpPower = jumpPower;
			this.maxJumpHeight = Trajectory.CalculateApex(this.jumpPower);
			this.maxJumpCount = this.baseJumpCount + num6;
			float num34 = this.baseDamage + this.levelDamage * num25;
			float num35 = 1f;
			int num36 = this.inventory ? this.inventory.GetItemCount(ItemIndex.BoostDamage) : 0;
			if (num36 > 0)
			{
				num35 += (float)num36 * 0.1f;
			}
			if (num12 > 0)
			{
				num35 -= 0.05f * (float)num12;
			}
			if (this.HasBuff(BuffIndex.GoldEmpowered))
			{
				num35 += 1f;
			}
			num35 += num23 - 1f;
			num34 *= num35;
			this.damage = num34;
			float num37 = this.baseAttackSpeed + this.levelAttackSpeed * num25;
			float num38 = 1f;
			num38 += (float)num7 * 0.15f;
			if (equipmentIndex == EquipmentIndex.AffixYellow)
			{
				num38 += 0.5f;
			}
			num38 += (float)this.buffs[2] * 0.12f;
			if (this.HasBuff(BuffIndex.Warbanner))
			{
				num38 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.EnrageAncientWisp))
			{
				num38 += 2f;
			}
			if (this.HasBuff(BuffIndex.WarCryBuff))
			{
				num38 += 1f;
			}
			num37 *= num38;
			if (num12 > 0)
			{
				num37 *= 1f - 0.05f * (float)num12;
			}
			this.attackSpeed = num37;
			float num39 = this.baseCrit + this.levelCrit * num25;
			num39 += (float)num8 * 10f;
			if (num9 > 0)
			{
				num39 += 5f;
			}
			if (num10 > 0)
			{
				num39 += 5f;
			}
			if (num11 > 0)
			{
				num39 += 5f;
			}
			if (num17 > 0)
			{
				num39 += 5f;
			}
			if (this.HasBuff(BuffIndex.FullCrit))
			{
				num39 += 100f;
			}
			this.crit = num39;
			this.armor = this.baseArmor + this.levelArmor * num25 + (this.HasBuff(BuffIndex.ArmorBoost) ? 200f : 0f);
			this.armor += (float)num22 * 70f;
			if (this.HasBuff(BuffIndex.Cripple))
			{
				this.armor -= 20f;
			}
			if (this.isSprinting && num19 > 0)
			{
				this.armor += (float)(num19 * 30);
			}
			float num40 = 1f;
			if (this.HasBuff(BuffIndex.GoldEmpowered))
			{
				num40 *= 0.25f;
			}
			for (int i = 0; i < num14; i++)
			{
				num40 *= 0.75f;
			}
			if (this.HasBuff(BuffIndex.NoCooldowns))
			{
				num40 = 0f;
			}
			if (this.skillLocator.primary)
			{
				this.skillLocator.primary.cooldownScale = num40;
			}
			if (this.skillLocator.secondary)
			{
				this.skillLocator.secondary.cooldownScale = num40;
				this.skillLocator.secondary.SetBonusStockFromBody(bonusStockFromBody);
			}
			if (this.skillLocator.utility)
			{
				float num41 = num40;
				if (num20 > 0)
				{
					num41 *= 0.6666667f;
				}
				this.skillLocator.utility.cooldownScale = num41;
				this.skillLocator.utility.SetBonusStockFromBody(num20 * 2);
			}
			if (this.skillLocator.special)
			{
				this.skillLocator.special.cooldownScale = num40;
			}
			this.critHeal = 0f;
			if (num17 > 0)
			{
				float crit = this.crit;
				this.crit /= (float)(num17 + 1);
				this.critHeal = crit - this.crit;
			}
			this.statsDirty = false;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x0003DF2D File Offset: 0x0003C12D
		public void OnLevelChanged()
		{
			this.statsDirty = true;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x0003DF36 File Offset: 0x0003C136
		public void SetAimTimer(float duration)
		{
			this.aimTimer = duration;
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x0003DF3F File Offset: 0x0003C13F
		public bool shouldAim
		{
			get
			{
				return this.aimTimer > 0f && !this.isSprinting;
			}
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x0003DF5C File Offset: 0x0003C15C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			if ((b & 1) != 0)
			{
				NetworkInstanceId c = reader.ReadNetworkId();
				if (c != this.masterObjectId)
				{
					this.masterObjectId = c;
					this.statsDirty = true;
				}
			}
			if ((b & 2) != 0)
			{
				BuffMask a = reader.ReadBuffMask();
				if (a != this.buffMask)
				{
					BuffMask oldBuffMask = this.buffMask;
					this.buffMask = a;
					this.statsDirty = true;
					this.OnClientBuffsChanged(oldBuffMask);
				}
			}
			if ((b & 4) != 0)
			{
				byte b2 = reader.ReadByte();
				if (this.buffs[2] != (int)b2)
				{
					this.buffs[2] = (int)b2;
					this.statsDirty = true;
				}
			}
			if ((b & 8) != 0)
			{
				bool flag = reader.ReadBoolean();
				if (!base.hasAuthority && flag != this.outOfCombat)
				{
					this.outOfCombat = flag;
					this.statsDirty = true;
				}
			}
			if ((b & 16) != 0)
			{
				bool flag2 = reader.ReadBoolean();
				if (flag2 != this.outOfDanger)
				{
					this.outOfDanger = flag2;
					this.statsDirty = true;
				}
			}
			if ((b & 32) != 0)
			{
				bool flag3 = reader.ReadBoolean();
				if (flag3 != this.isSprinting && !base.hasAuthority)
				{
					this.statsDirty = true;
					this.isSprinting = flag3;
				}
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0003E078 File Offset: 0x0003C278
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 63u;
			}
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 2u) > 0u;
			bool flag3 = (num & 4u) > 0u;
			bool flag4 = (num & 8u) > 0u;
			bool flag5 = (num & 16u) > 0u;
			bool flag6 = (num & 32u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this.masterObjectId);
			}
			if (flag2)
			{
				writer.WriteBuffMask(this.buffMask);
			}
			if (flag3)
			{
				writer.Write((byte)this.buffs[2]);
			}
			if (flag4)
			{
				writer.Write(this.outOfCombat);
			}
			if (flag5)
			{
				writer.Write(this.outOfDanger);
			}
			if (flag6)
			{
				writer.Write(this.isSprinting);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0003E130 File Offset: 0x0003C330
		public T AddItemBehavior<T>(int stack) where T : CharacterBody.ItemBehavior
		{
			T t = base.GetComponent<T>();
			if (stack > 0)
			{
				if (!t)
				{
					t = base.gameObject.AddComponent<T>();
					t.body = this;
				}
				t.stack = stack;
				return t;
			}
			if (t)
			{
				UnityEngine.Object.Destroy(t);
			}
			return default(T);
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000C7C RID: 3196 RVA: 0x0003E19C File Offset: 0x0003C39C
		// (set) Token: 0x06000C7D RID: 3197 RVA: 0x0003E1A4 File Offset: 0x0003C3A4
		public bool warCryReady
		{
			get
			{
				return this._warCryReady;
			}
			private set
			{
				if (this._warCryReady != value)
				{
					this._warCryReady = value;
					if (NetworkServer.active)
					{
						this.CallRpcSyncWarCryReady(value);
					}
				}
			}
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x0003E1C4 File Offset: 0x0003C3C4
		[Server]
		private void ActivateWarCryAura(int stacks)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::ActivateWarCryAura(System.Int32)' called on client");
				return;
			}
			if (this.warCryAuraController)
			{
				UnityEngine.Object.Destroy(this.warCryAuraController);
			}
			this.warCryAuraController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarCryAura"), this.transform.position, this.transform.rotation, this.transform);
			this.warCryAuraController.GetComponent<TeamFilter>().teamIndex = this.teamComponent.teamIndex;
			BuffWard component = this.warCryAuraController.GetComponent<BuffWard>();
			component.expireDuration = 2f + 4f * (float)stacks;
			component.Networkradius = 8f + 4f * (float)stacks;
			this.warCryAuraController.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x0003E292 File Offset: 0x0003C492
		[ClientRpc]
		private void RpcSyncWarCryReady(bool value)
		{
			if (!NetworkServer.active)
			{
				this.warCryReady = value;
			}
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x0003E2A4 File Offset: 0x0003C4A4
		private void OnKilledOther(DamageReport damageReport)
		{
			this.killCount++;
			this.AddMultiKill(1);
			CharacterBody.IcicleItemBehavior component = base.GetComponent<CharacterBody.IcicleItemBehavior>();
			if (component)
			{
				component.OnOwnerKillOther();
			}
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x0003E2DC File Offset: 0x0003C4DC
		[Server]
		private void UpdateTeslaCoil()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateTeslaCoil()' called on client");
				return;
			}
			if (this.inventory)
			{
				int itemCount = this.inventory.GetItemCount(ItemIndex.ShockNearby);
				if (itemCount > 0)
				{
					this.teslaBuffRollTimer += Time.fixedDeltaTime;
					if (this.teslaBuffRollTimer >= 10f)
					{
						this.teslaBuffRollTimer = 0f;
						this.teslaCrit = Util.CheckRoll(this.crit, this.master);
						if (this.HasBuff(BuffIndex.TeslaField))
						{
							this.AddBuff(BuffIndex.TeslaField);
						}
						else
						{
							this.RemoveBuff(BuffIndex.TeslaField);
						}
					}
					if (this.HasBuff(BuffIndex.TeslaField))
					{
						this.teslaFireTimer += Time.fixedDeltaTime;
						this.teslaResetListTimer += Time.fixedDeltaTime;
						if (this.teslaFireTimer >= 0.083333336f)
						{
							this.teslaFireTimer = 0f;
							LightningOrb lightningOrb = new LightningOrb();
							lightningOrb.origin = this.corePosition;
							lightningOrb.damageValue = this.damage * 2f;
							lightningOrb.isCrit = this.teslaCrit;
							lightningOrb.bouncesRemaining = 2 * itemCount;
							lightningOrb.teamIndex = this.teamComponent.teamIndex;
							lightningOrb.attacker = base.gameObject;
							lightningOrb.procCoefficient = 0.3f;
							lightningOrb.bouncedObjects = this.previousTeslaTargetList;
							lightningOrb.lightningType = LightningOrb.LightningType.Tesla;
							lightningOrb.damageColorIndex = DamageColorIndex.Item;
							lightningOrb.range = 35f;
							HurtBox hurtBox = lightningOrb.PickNextTarget(this.transform.position);
							if (hurtBox)
							{
								this.previousTeslaTargetList.Add(hurtBox.healthComponent);
								lightningOrb.target = hurtBox;
								OrbManager.instance.AddOrb(lightningOrb);
							}
						}
						if (this.teslaResetListTimer >= this.teslaResetListInterval)
						{
							this.teslaResetListTimer -= this.teslaResetListInterval;
							this.previousTeslaTargetList.Clear();
						}
					}
				}
			}
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x0003E4BB File Offset: 0x0003C6BB
		public void AddHelfireDuration(float duration)
		{
			this.helfireLifetime = duration;
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x0003E4C4 File Offset: 0x0003C6C4
		[Server]
		private void UpdateHelfire()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateHelfire()' called on client");
				return;
			}
			this.helfireLifetime -= Time.fixedDeltaTime;
			bool flag = false;
			if (this.inventory)
			{
				flag = (this.inventory.GetItemCount(ItemIndex.BurnNearby) > 0 || this.helfireLifetime > 0f);
			}
			if (this.helfireController != flag)
			{
				if (flag)
				{
					this.helfireController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HelfireController")).GetComponent<HelfireController>();
					this.helfireController.networkedBodyAttachment.AttachToGameObjectAndSpawn(base.gameObject);
					return;
				}
				UnityEngine.Object.Destroy(this.helfireController.gameObject);
				this.helfireController = null;
			}
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0003E584 File Offset: 0x0003C784
		private void UpdateFireTrail()
		{
			bool flag = this.HasBuff(BuffIndex.AffixRed);
			if (flag != this.fireTrail)
			{
				if (flag)
				{
					this.fireTrail = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireTrail"), this.transform).GetComponent<DamageTrail>();
					this.fireTrail.transform.position = this.footPosition;
					this.fireTrail.owner = base.gameObject;
					this.fireTrail.radius *= this.radius;
				}
				else
				{
					UnityEngine.Object.Destroy(this.fireTrail.gameObject);
					this.fireTrail = null;
				}
			}
			if (this.fireTrail)
			{
				this.fireTrail.damagePerSecond = this.damage * 1.5f;
			}
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0003E648 File Offset: 0x0003C848
		private void UpdateBeetleGuardAllies()
		{
			if (NetworkServer.active)
			{
				int num = this.inventory ? this.inventory.GetItemCount(ItemIndex.BeetleGland) : 0;
				if (num > 0 && this.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly) < num)
				{
					this.guardResummonCooldown -= Time.fixedDeltaTime;
					if (this.guardResummonCooldown <= 0f)
					{
						this.guardResummonCooldown = 30f;
						GameObject gameObject = DirectorCore.instance.TrySpawnObject((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.Approximate,
							minDistance = 3f,
							maxDistance = 40f,
							spawnOnTarget = this.transform
						}, RoR2Application.rng);
						if (gameObject)
						{
							CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
							AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
							BaseAI component3 = gameObject.GetComponent<BaseAI>();
							if (component)
							{
								component.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
								component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
								component.inventory.GiveItem(ItemIndex.BoostHp, 10);
								GameObject bodyObject = component.GetBodyObject();
								if (bodyObject)
								{
									Deployable component4 = bodyObject.GetComponent<Deployable>();
									this.master.AddDeployable(component4, DeployableSlot.BeetleGuardAlly);
								}
							}
							if (component2)
							{
								component2.ownerMaster = this.master;
							}
							if (component3)
							{
								component3.leader.gameObject = base.gameObject;
							}
						}
					}
				}
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x0003E7BF File Offset: 0x0003C9BF
		private float bestFitRadius
		{
			get
			{
				return Mathf.Max(this.radius, this.characterMotor ? this.characterMotor.capsuleHeight : 1f);
			}
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x0003E7EC File Offset: 0x0003C9EC
		private void UpdateAllTemporaryVisualEffects()
		{
			this.UpdateSingleTemporaryVisualEffect(ref this.engiShieldTempEffect, "Prefabs/TemporaryVisualEffects/EngiShield", this.bestFitRadius, this.healthComponent.shield > 0f && this.HasBuff(BuffIndex.EngiShield));
			this.UpdateSingleTemporaryVisualEffect(ref this.bucklerShieldTempEffect, "Prefabs/TemporaryVisualEffects/BucklerDefense", this.radius, this.isSprinting && this.inventory.GetItemCount(ItemIndex.SprintArmor) > 0);
			this.UpdateSingleTemporaryVisualEffect(ref this.slowDownTimeTempEffect, "Prefabs/TemporaryVisualEffects/SlowDownTime", this.radius, this.HasBuff(BuffIndex.Slow60));
			this.UpdateSingleTemporaryVisualEffect(ref this.crippleEffect, "Prefabs/TemporaryVisualEffects/CrippleEffect", this.radius, this.HasBuff(BuffIndex.Cripple));
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0003E8A0 File Offset: 0x0003CAA0
		private void UpdateSingleTemporaryVisualEffect(ref TemporaryVisualEffect tempEffect, string resourceString, float effectRadius, bool active)
		{
			bool flag = tempEffect != null;
			if (flag != active)
			{
				if (active)
				{
					if (!flag)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(resourceString), this.corePosition, Quaternion.identity);
						tempEffect = gameObject.GetComponent<TemporaryVisualEffect>();
						tempEffect.parentTransform = this.coreTransform;
						tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
						tempEffect.healthComponent = this.healthComponent;
						tempEffect.radius = effectRadius;
						return;
					}
				}
				else if (tempEffect)
				{
					tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
				}
			}
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0003E920 File Offset: 0x0003CB20
		public VisibilityLevel GetVisibilityLevel(CharacterBody observer)
		{
			if (!this.HasBuff(BuffIndex.Cloak))
			{
				return VisibilityLevel.Visible;
			}
			if (!observer)
			{
				return VisibilityLevel.Revealed;
			}
			TeamIndex teamIndex = this.teamComponent ? this.teamComponent.teamIndex : TeamIndex.Neutral;
			TeamIndex teamIndex2 = observer.teamComponent ? observer.teamComponent.teamIndex : TeamIndex.Neutral;
			if (teamIndex != teamIndex2)
			{
				return VisibilityLevel.Cloaked;
			}
			return VisibilityLevel.Revealed;
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x0003E97F File Offset: 0x0003CB7F
		public void AddSpreadBloom(float value)
		{
			this.spreadBloomInternal = Mathf.Min(this.spreadBloomInternal + value, 1f);
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x0003E999 File Offset: 0x0003CB99
		public void SetSpreadBloom(float value, bool canOnlyIncreaseBloom = true)
		{
			if (canOnlyIncreaseBloom)
			{
				this.spreadBloomInternal = Mathf.Clamp(value, this.spreadBloomInternal, 1f);
				return;
			}
			this.spreadBloomInternal = Mathf.Min(value, 1f);
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x0003E9C8 File Offset: 0x0003CBC8
		private void UpdateSpreadBloom(float dt)
		{
			float num = 1f / this.spreadBloomDecayTime;
			this.spreadBloomInternal = Mathf.Max(this.spreadBloomInternal - num * dt, 0f);
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000C8D RID: 3213 RVA: 0x0003E9FC File Offset: 0x0003CBFC
		public float spreadBloomAngle
		{
			get
			{
				return this.spreadBloomCurve.Evaluate(this.spreadBloomInternal);
			}
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x0003EA10 File Offset: 0x0003CC10
		[Client]
		public void SendConstructTurret(CharacterBody builder, Vector3 position, Quaternion rotation)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::SendConstructTurret(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion)' called on server");
				return;
			}
			CharacterBody.ConstructTurretMessage constructTurretMessage = new CharacterBody.ConstructTurretMessage();
			constructTurretMessage.builder = builder.gameObject;
			constructTurretMessage.position = position;
			constructTurretMessage.rotation = rotation;
			ClientScene.readyConnection.Send(62, constructTurretMessage);
		}

		// Token: 0x06000C8F RID: 3215 RVA: 0x0003EA60 File Offset: 0x0003CC60
		[NetworkMessageHandler(msgType = 62, server = true)]
		private static void HandleConstructTurret(NetworkMessage netMsg)
		{
			CharacterBody.ConstructTurretMessage constructTurretMessage = netMsg.ReadMessage<CharacterBody.ConstructTurretMessage>();
			if (constructTurretMessage.builder)
			{
				CharacterBody component = constructTurretMessage.builder.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						GameObject original = MasterCatalog.FindMasterPrefab("EngiTurretMaster");
						GameObject bodyPrefab = BodyCatalog.FindBodyPrefab("EngiTurretBody");
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, constructTurretMessage.position, constructTurretMessage.rotation);
						CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
						component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
						Inventory component3 = gameObject.GetComponent<Inventory>();
						component3.CopyItemsFrom(master.inventory);
						component3.ResetItem(ItemIndex.WardOnLevel);
						component3.ResetItem(ItemIndex.BeetleGland);
						component3.ResetItem(ItemIndex.CrippleWardOnLevel);
						NetworkServer.Spawn(gameObject);
						Deployable deployable = gameObject.AddComponent<Deployable>();
						deployable.onUndeploy = new UnityEvent();
						deployable.onUndeploy.AddListener(new UnityAction(component2.TrueKill));
						master.AddDeployable(deployable, DeployableSlot.EngiTurret);
						component2.SpawnBody(bodyPrefab, constructTurretMessage.position, constructTurretMessage.rotation);
					}
				}
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0003EB66 File Offset: 0x0003CD66
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x0003EB6E File Offset: 0x0003CD6E
		public int multiKillCount { get; private set; }

		// Token: 0x06000C92 RID: 3218 RVA: 0x0003EB78 File Offset: 0x0003CD78
		[Server]
		public void AddMultiKill(int kills)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddMultiKill(System.Int32)' called on client");
				return;
			}
			this.multiKillTimer = 1f;
			this.multiKillCount += kills;
			int num = this.inventory ? this.inventory.GetItemCount(ItemIndex.WarCryOnMultiKill) : 0;
			if (num > 0 && this.multiKillCount >= 4)
			{
				this.AddTimedBuff(BuffIndex.WarCryBuff, 2f + 4f * (float)num);
			}
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x0003EBF4 File Offset: 0x0003CDF4
		[Server]
		private void UpdateMultiKill(float deltaTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateMultiKill(System.Single)' called on client");
				return;
			}
			this.multiKillTimer -= deltaTime;
			if (this.multiKillTimer <= 0f)
			{
				this.multiKillTimer = 0f;
				this.multiKillCount = 0;
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x0003EC43 File Offset: 0x0003CE43
		public Vector3 corePosition
		{
			get
			{
				return this.coreTransform.position;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000C95 RID: 3221 RVA: 0x0003EC50 File Offset: 0x0003CE50
		public Vector3 footPosition
		{
			get
			{
				Vector3 position = this.transform.position;
				if (this.characterMotor)
				{
					position.y -= this.characterMotor.capsuleHeight * 0.5f;
				}
				return position;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000C96 RID: 3222 RVA: 0x0003EC93 File Offset: 0x0003CE93
		// (set) Token: 0x06000C97 RID: 3223 RVA: 0x0003EC9B File Offset: 0x0003CE9B
		public float radius { get; private set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000C98 RID: 3224 RVA: 0x0003ECA4 File Offset: 0x0003CEA4
		public Vector3 aimOrigin
		{
			get
			{
				if (!this.aimOriginTransform)
				{
					return this.corePosition;
				}
				return this.aimOriginTransform.position;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000C99 RID: 3225 RVA: 0x0003ECC5 File Offset: 0x0003CEC5
		// (set) Token: 0x06000C9A RID: 3226 RVA: 0x0003ECCD File Offset: 0x0003CECD
		public bool isElite { get; private set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000C9B RID: 3227 RVA: 0x0003ECD6 File Offset: 0x0003CED6
		public bool isBoss
		{
			get
			{
				return this.master && this.master.isBoss;
			}
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0003ECF2 File Offset: 0x0003CEF2
		[ClientRpc]
		public void RpcBark()
		{
			if (this.sfxLocator)
			{
				Util.PlaySound(this.sfxLocator.barkSound, base.gameObject);
			}
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0003ED84 File Offset: 0x0003CF84
		static CharacterBody()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdUpdateSprint, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdUpdateSprint));
			CharacterBody.kCmdCmdOnSkillActivated = 384138986;
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdOnSkillActivated, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdOnSkillActivated));
			CharacterBody.kRpcRpcSyncWarCryReady = 1893254821;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcSyncWarCryReady, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcSyncWarCryReady));
			CharacterBody.kRpcRpcBark = -76716871;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcBark, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcBark));
			NetworkCRC.RegisterBehaviour("CharacterBody", 0);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0003EE61 File Offset: 0x0003D061
		protected static void InvokeCmdCmdUpdateSprint(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateSprint called on client.");
				return;
			}
			((CharacterBody)obj).CmdUpdateSprint(reader.ReadBoolean());
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0003EE8A File Offset: 0x0003D08A
		protected static void InvokeCmdCmdOnSkillActivated(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdOnSkillActivated called on client.");
				return;
			}
			((CharacterBody)obj).CmdOnSkillActivated((sbyte)reader.ReadPackedUInt32());
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x0003EEB4 File Offset: 0x0003D0B4
		public void CallCmdUpdateSprint(bool newIsSprinting)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdUpdateSprint called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdUpdateSprint(newIsSprinting);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdUpdateSprint);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(newIsSprinting);
			base.SendCommandInternal(networkWriter, 0, "CmdUpdateSprint");
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x0003EF40 File Offset: 0x0003D140
		public void CallCmdOnSkillActivated(sbyte skillIndex)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdOnSkillActivated called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdOnSkillActivated(skillIndex);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdOnSkillActivated);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32((uint)skillIndex);
			base.SendCommandInternal(networkWriter, 0, "CmdOnSkillActivated");
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x0003EFCA File Offset: 0x0003D1CA
		protected static void InvokeRpcRpcSyncWarCryReady(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSyncWarCryReady called on server.");
				return;
			}
			((CharacterBody)obj).RpcSyncWarCryReady(reader.ReadBoolean());
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x0003EFF3 File Offset: 0x0003D1F3
		protected static void InvokeRpcRpcBark(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcBark called on server.");
				return;
			}
			((CharacterBody)obj).RpcBark();
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0003F018 File Offset: 0x0003D218
		public void CallRpcSyncWarCryReady(bool value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSyncWarCryReady called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcSyncWarCryReady);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(value);
			this.SendRPCInternal(networkWriter, 0, "RpcSyncWarCryReady");
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x0003F08C File Offset: 0x0003D28C
		public void CallRpcBark()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcBark called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcBark);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcBark");
		}

		// Token: 0x04001047 RID: 4167
		[Tooltip("The language token to use as the base name of this character.")]
		public string baseNameToken;

		// Token: 0x04001048 RID: 4168
		public string subtitleNameToken;

		// Token: 0x04001049 RID: 4169
		private int[] buffs = new int[31];

		// Token: 0x0400104A RID: 4170
		private List<CharacterBody.TimedBuff> timedBuffs = new List<CharacterBody.TimedBuff>();

		// Token: 0x0400104B RID: 4171
		private BuffMask buffMask;

		// Token: 0x0400104C RID: 4172
		private GameObject warCryEffectInstance;

		// Token: 0x0400104D RID: 4173
		[EnumMask(typeof(CharacterBody.BodyFlags))]
		public CharacterBody.BodyFlags bodyFlags;

		// Token: 0x0400104E RID: 4174
		private NetworkInstanceId masterObjectId;

		// Token: 0x0400104F RID: 4175
		private GameObject _masterObject;

		// Token: 0x04001050 RID: 4176
		private CharacterMaster _master;

		// Token: 0x04001052 RID: 4178
		private bool linkedToMaster;

		// Token: 0x04001054 RID: 4180
		private bool disablingHurtBoxes;

		// Token: 0x04001055 RID: 4181
		private EquipmentIndex previousEquipmentIndex;

		// Token: 0x04001057 RID: 4183
		private new Transform transform;

		// Token: 0x04001058 RID: 4184
		private CharacterMotor characterMotor;

		// Token: 0x0400105C RID: 4188
		private SkillLocator skillLocator;

		// Token: 0x0400105D RID: 4189
		private SfxLocator sfxLocator;

		// Token: 0x04001062 RID: 4194
		private static List<CharacterBody> instancesList = new List<CharacterBody>();

		// Token: 0x04001063 RID: 4195
		public static readonly ReadOnlyCollection<CharacterBody> readOnlyInstancesList = new ReadOnlyCollection<CharacterBody>(CharacterBody.instancesList);

		// Token: 0x04001064 RID: 4196
		private bool _isSprinting;

		// Token: 0x04001065 RID: 4197
		private float sprintingSpeedMultiplier = 1.45f;

		// Token: 0x04001066 RID: 4198
		private const float outOfCombatDelay = 5f;

		// Token: 0x04001067 RID: 4199
		private const float outOfDangerDelay = 7f;

		// Token: 0x04001068 RID: 4200
		private float outOfCombatStopwatch;

		// Token: 0x04001069 RID: 4201
		private float outOfDangerStopwatch;

		// Token: 0x0400106B RID: 4203
		private bool _outOfDanger = true;

		// Token: 0x0400106C RID: 4204
		private Vector3 previousPosition;

		// Token: 0x0400106D RID: 4205
		private const float notMovingWait = 2f;

		// Token: 0x0400106E RID: 4206
		private float notMovingStopwatch;

		// Token: 0x0400106F RID: 4207
		public bool rootMotionInMainState;

		// Token: 0x04001070 RID: 4208
		public float mainRootSpeed;

		// Token: 0x04001071 RID: 4209
		public float baseMaxHealth;

		// Token: 0x04001072 RID: 4210
		public float baseRegen;

		// Token: 0x04001073 RID: 4211
		public float baseMaxShield;

		// Token: 0x04001074 RID: 4212
		public float baseMoveSpeed;

		// Token: 0x04001075 RID: 4213
		public float baseAcceleration;

		// Token: 0x04001076 RID: 4214
		public float baseJumpPower;

		// Token: 0x04001077 RID: 4215
		public float baseDamage;

		// Token: 0x04001078 RID: 4216
		public float baseAttackSpeed;

		// Token: 0x04001079 RID: 4217
		public float baseCrit;

		// Token: 0x0400107A RID: 4218
		public float baseArmor;

		// Token: 0x0400107B RID: 4219
		public int baseJumpCount = 1;

		// Token: 0x0400107C RID: 4220
		public bool autoCalculateLevelStats;

		// Token: 0x0400107D RID: 4221
		public float levelMaxHealth;

		// Token: 0x0400107E RID: 4222
		public float levelRegen;

		// Token: 0x0400107F RID: 4223
		public float levelMaxShield;

		// Token: 0x04001080 RID: 4224
		public float levelMoveSpeed;

		// Token: 0x04001081 RID: 4225
		public float levelJumpPower;

		// Token: 0x04001082 RID: 4226
		public float levelDamage;

		// Token: 0x04001083 RID: 4227
		public float levelAttackSpeed;

		// Token: 0x04001084 RID: 4228
		public float levelCrit;

		// Token: 0x04001085 RID: 4229
		public float levelArmor;

		// Token: 0x04001095 RID: 4245
		private bool statsDirty;

		// Token: 0x04001096 RID: 4246
		private float aimTimer;

		// Token: 0x04001097 RID: 4247
		private const uint masterDirtyBit = 1u;

		// Token: 0x04001098 RID: 4248
		private const uint buffMaskBit = 2u;

		// Token: 0x04001099 RID: 4249
		private const uint attackSpeedOnCritBuffBit = 4u;

		// Token: 0x0400109A RID: 4250
		private const uint outOfCombatBit = 8u;

		// Token: 0x0400109B RID: 4251
		private const uint outOfDangerBit = 16u;

		// Token: 0x0400109C RID: 4252
		private const uint sprintingBit = 32u;

		// Token: 0x0400109D RID: 4253
		private GameObject warCryAuraController;

		// Token: 0x0400109E RID: 4254
		private float warCryTimer;

		// Token: 0x0400109F RID: 4255
		private const float warCryChargeDuration = 30f;

		// Token: 0x040010A0 RID: 4256
		private bool _warCryReady;

		// Token: 0x040010A1 RID: 4257
		[HideInInspector]
		public int killCount;

		// Token: 0x040010A2 RID: 4258
		private float teslaBuffRollTimer;

		// Token: 0x040010A3 RID: 4259
		private const float teslaRollInterval = 10f;

		// Token: 0x040010A4 RID: 4260
		private float teslaFireTimer;

		// Token: 0x040010A5 RID: 4261
		private float teslaResetListTimer;

		// Token: 0x040010A6 RID: 4262
		private float teslaResetListInterval = 0.5f;

		// Token: 0x040010A7 RID: 4263
		private const float teslaFireInterval = 0.083333336f;

		// Token: 0x040010A8 RID: 4264
		private bool teslaCrit;

		// Token: 0x040010A9 RID: 4265
		private List<HealthComponent> previousTeslaTargetList = new List<HealthComponent>();

		// Token: 0x040010AA RID: 4266
		private HelfireController helfireController;

		// Token: 0x040010AB RID: 4267
		private float helfireLifetime;

		// Token: 0x040010AC RID: 4268
		private DamageTrail fireTrail;

		// Token: 0x040010AD RID: 4269
		public bool wasLucky;

		// Token: 0x040010AE RID: 4270
		private const float timeBetweenGuardResummons = 30f;

		// Token: 0x040010AF RID: 4271
		private float guardResummonCooldown;

		// Token: 0x040010B0 RID: 4272
		private TemporaryVisualEffect engiShieldTempEffect;

		// Token: 0x040010B1 RID: 4273
		private TemporaryVisualEffect bucklerShieldTempEffect;

		// Token: 0x040010B2 RID: 4274
		private TemporaryVisualEffect slowDownTimeTempEffect;

		// Token: 0x040010B3 RID: 4275
		private TemporaryVisualEffect crippleEffect;

		// Token: 0x040010B4 RID: 4276
		[Tooltip("How long it takes for spread bloom to reset from full.")]
		public float spreadBloomDecayTime = 0.45f;

		// Token: 0x040010B5 RID: 4277
		[Tooltip("The spread bloom interpretation curve.")]
		public AnimationCurve spreadBloomCurve;

		// Token: 0x040010B6 RID: 4278
		private float spreadBloomInternal;

		// Token: 0x040010B7 RID: 4279
		[Tooltip("The crosshair prefab used for this body.")]
		public GameObject crosshairPrefab;

		// Token: 0x040010B8 RID: 4280
		[HideInInspector]
		public bool hideCrosshair;

		// Token: 0x040010B9 RID: 4281
		private const float multiKillMaxInterval = 1f;

		// Token: 0x040010BA RID: 4282
		private float multiKillTimer;

		// Token: 0x040010BC RID: 4284
		private const int multiKillThresholdForWarcry = 4;

		// Token: 0x040010BE RID: 4286
		[Tooltip("The child transform to be used as the aiming origin.")]
		public Transform aimOriginTransform;

		// Token: 0x040010BF RID: 4287
		[Tooltip("The hull size to use when pathfinding for this object.")]
		public HullClassification hullClassification;

		// Token: 0x040010C0 RID: 4288
		[Tooltip("The icon displayed for ally healthbars")]
		public Texture portraitIcon;

		// Token: 0x040010C1 RID: 4289
		[Tooltip("Whether or not this is a boss for dropping items on death.")]
		[FormerlySerializedAs("isBoss")]
		public bool isChampion;

		// Token: 0x040010C3 RID: 4291
		private static int kCmdCmdUpdateSprint = -1006016914;

		// Token: 0x040010C4 RID: 4292
		private static int kCmdCmdOnSkillActivated;

		// Token: 0x040010C5 RID: 4293
		private static int kRpcRpcSyncWarCryReady;

		// Token: 0x040010C6 RID: 4294
		private static int kRpcRpcBark;

		// Token: 0x02000280 RID: 640
		private class TimedBuff
		{
			// Token: 0x040010C7 RID: 4295
			public BuffIndex buffIndex;

			// Token: 0x040010C8 RID: 4296
			public float timer;
		}

		// Token: 0x02000281 RID: 641
		[Flags]
		public enum BodyFlags : byte
		{
			// Token: 0x040010CA RID: 4298
			None = 0,
			// Token: 0x040010CB RID: 4299
			IgnoreFallDamage = 1,
			// Token: 0x040010CC RID: 4300
			Mechanical = 2,
			// Token: 0x040010CD RID: 4301
			Masterless = 4,
			// Token: 0x040010CE RID: 4302
			ImmuneToGoo = 8
		}

		// Token: 0x02000282 RID: 642
		public class ItemBehavior : MonoBehaviour
		{
			// Token: 0x040010CF RID: 4303
			public CharacterBody body;

			// Token: 0x040010D0 RID: 4304
			public int stack;
		}

		// Token: 0x02000283 RID: 643
		public class MushroomItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x06000CAA RID: 3242 RVA: 0x0003F0F8 File Offset: 0x0003D2F8
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				int stack = this.stack;
				bool flag = stack > 0 && this.body.GetNotMoving();
				float networkradius = 1.5f + 1.5f * (float)stack;
				if (this.mushroomWard != flag)
				{
					if (flag)
					{
						this.mushroomWard = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MushroomWard"), this.body.footPosition, Quaternion.identity);
						this.mushroomWard.GetComponent<TeamFilter>().teamIndex = this.body.teamComponent.teamIndex;
						NetworkServer.Spawn(this.mushroomWard);
					}
					else
					{
						UnityEngine.Object.Destroy(this.mushroomWard);
						this.mushroomWard = null;
					}
				}
				if (this.mushroomWard)
				{
					HealingWard component = this.mushroomWard.GetComponent<HealingWard>();
					component.healFraction = 0.0225f + 0.0225f * (float)stack;
					component.healPoints = 0f;
					component.Networkradius = networkradius;
				}
			}

			// Token: 0x06000CAB RID: 3243 RVA: 0x0003F1E8 File Offset: 0x0003D3E8
			private void OnDisable()
			{
				if (this.mushroomWard)
				{
					UnityEngine.Object.Destroy(this.mushroomWard);
				}
			}

			// Token: 0x040010D1 RID: 4305
			private GameObject mushroomWard;
		}

		// Token: 0x02000284 RID: 644
		public class IcicleItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x06000CAD RID: 3245 RVA: 0x0003F20C File Offset: 0x0003D40C
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				bool flag = this.stack > 0;
				if (this.icicleAura != flag)
				{
					if (flag)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/IcicleAura"), base.transform.position, Quaternion.identity);
						this.icicleAura = gameObject.GetComponent<IcicleAuraController>();
						this.icicleAura.Networkowner = base.gameObject;
						NetworkServer.Spawn(gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.icicleAura.gameObject);
					this.icicleAura = null;
				}
			}

			// Token: 0x06000CAE RID: 3246 RVA: 0x0003F297 File Offset: 0x0003D497
			public void OnOwnerKillOther()
			{
				if (this.icicleAura)
				{
					this.icicleAura.OnOwnerKillOther();
				}
			}

			// Token: 0x06000CAF RID: 3247 RVA: 0x0003F2B1 File Offset: 0x0003D4B1
			private void OnDisable()
			{
				if (this.icicleAura)
				{
					UnityEngine.Object.Destroy(this.icicleAura);
				}
			}

			// Token: 0x040010D2 RID: 4306
			private IcicleAuraController icicleAura;
		}

		// Token: 0x02000285 RID: 645
		public class HeadstomperItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x06000CB1 RID: 3249 RVA: 0x0003F2CC File Offset: 0x0003D4CC
			private void FixedUpdate()
			{
				bool flag = this.stack > 0;
				if (flag != this.headstompersControllerObject)
				{
					if (flag)
					{
						this.headstompersControllerObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HeadstompersController"));
						this.headstompersControllerObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.body.gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.headstompersControllerObject);
				}
			}

			// Token: 0x06000CB2 RID: 3250 RVA: 0x0003F330 File Offset: 0x0003D530
			private void OnDisable()
			{
				if (this.headstompersControllerObject)
				{
					UnityEngine.Object.Destroy(this.headstompersControllerObject);
				}
			}

			// Token: 0x040010D3 RID: 4307
			private GameObject headstompersControllerObject;
		}

		// Token: 0x02000286 RID: 646
		private class ConstructTurretMessage : MessageBase
		{
			// Token: 0x06000CB5 RID: 3253 RVA: 0x0003F34A File Offset: 0x0003D54A
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.builder);
				writer.Write(this.position);
				writer.Write(this.rotation);
			}

			// Token: 0x06000CB6 RID: 3254 RVA: 0x0003F370 File Offset: 0x0003D570
			public override void Deserialize(NetworkReader reader)
			{
				this.builder = reader.ReadGameObject();
				this.position = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
			}

			// Token: 0x040010D4 RID: 4308
			public GameObject builder;

			// Token: 0x040010D5 RID: 4309
			public Vector3 position;

			// Token: 0x040010D6 RID: 4310
			public Quaternion rotation;
		}
	}
}
