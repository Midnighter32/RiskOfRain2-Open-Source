using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020002FF RID: 767
	[RequireComponent(typeof(CharacterBody))]
	public class GenericSkill : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x06000F9D RID: 3997 RVA: 0x0004CC4C File Offset: 0x0004AE4C
		public void SetBonusStockFromBody(int newBonusStockFromBody)
		{
			this.bonusStockFromBody = newBonusStockFromBody;
			this.RecalculateMaxStock();
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x0004CC5B File Offset: 0x0004AE5B
		// (set) Token: 0x06000F9F RID: 3999 RVA: 0x0004CC63 File Offset: 0x0004AE63
		public int maxStock { get; protected set; }

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0004CC6C File Offset: 0x0004AE6C
		private void RecalculateMaxStock()
		{
			this.maxStock = this.baseMaxStock + this.bonusStockFromBody;
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x0004CC81 File Offset: 0x0004AE81
		// (set) Token: 0x06000FA2 RID: 4002 RVA: 0x0004CC89 File Offset: 0x0004AE89
		public int stock { get; protected set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x0004CC92 File Offset: 0x0004AE92
		// (set) Token: 0x06000FA4 RID: 4004 RVA: 0x0004CC9A File Offset: 0x0004AE9A
		public float cooldownScale
		{
			get
			{
				return this._cooldownScale;
			}
			set
			{
				if (this._cooldownScale == value)
				{
					return;
				}
				this._cooldownScale = value;
				this.RecalculateFinalRechargeInterval();
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x0004CCB3 File Offset: 0x0004AEB3
		public float cooldownRemaining
		{
			get
			{
				if (this.stock != this.maxStock)
				{
					return this.finalRechargeInterval - this.rechargeStopwatch;
				}
				return 0f;
			}
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0004CCD6 File Offset: 0x0004AED6
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.RecalculateFinalRechargeInterval();
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0004CCEA File Offset: 0x0004AEEA
		protected void Start()
		{
			this.RecalculateMaxStock();
			this.stock = this.maxStock;
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0004CD00 File Offset: 0x0004AF00
		protected void FixedUpdate()
		{
			this.RunRecharge(Time.fixedDeltaTime);
			if (this.canceledFromSprinting && this.characterBody.isSprinting && this.stateMachine.state.GetType() == this.activationState.stateType)
			{
				this.stateMachine.SetNextStateToMain();
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0003F5D8 File Offset: 0x0003D7D8
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0004CD5C File Offset: 0x0004AF5C
		public virtual bool CanExecute()
		{
			if (!this.isBullets)
			{
				return this.stock >= this.requiredStock;
			}
			return (this.stock >= this.requiredStock && this.rechargeStopwatch >= this.shootDelay) || this.stock == this.maxStock;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0004CDAF File Offset: 0x0004AFAF
		public bool ExecuteIfReady()
		{
			if (this.CanExecute())
			{
				this.OnExecute();
				return true;
			}
			return false;
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0004CDC4 File Offset: 0x0004AFC4
		public void RunRecharge(float dt)
		{
			if (this.stock < this.maxStock)
			{
				if (!this.beginSkillCooldownOnSkillEnd || this.stateMachine.state.GetType() != this.activationState.stateType)
				{
					this.rechargeStopwatch += dt;
				}
				if (this.rechargeStopwatch >= this.finalRechargeInterval)
				{
					this.RestockSteplike();
				}
			}
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0004CE2B File Offset: 0x0004B02B
		public void Reset()
		{
			this.rechargeStopwatch = 0f;
			this.stock = this.maxStock;
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0004CE44 File Offset: 0x0004B044
		public bool CanApplyAmmoPack()
		{
			return this.stock < this.maxStock;
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0004CE58 File Offset: 0x0004B058
		public void AddOneStock()
		{
			int stock = this.stock + 1;
			this.stock = stock;
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x0004CE75 File Offset: 0x0004B075
		public void RemoveAllStocks()
		{
			this.stock = 0;
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x0004CE89 File Offset: 0x0004B089
		public void DeductStock(int count)
		{
			this.stock = Mathf.Max(0, this.stock - count);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0004CEA0 File Offset: 0x0004B0A0
		protected virtual void OnExecute()
		{
			this.hasExecutedSuccessfully = false;
			if (this.stateMachine && !this.stateMachine.HasPendingState() && this.stateMachine.SetInterruptState(EntityState.Instantiate(this.activationState), this.interruptPriority))
			{
				this.hasExecutedSuccessfully = true;
				if (this.noSprint)
				{
					this.characterBody.isSprinting = false;
				}
				this.stock -= this.stockToConsume;
				if (this.isBullets)
				{
					this.rechargeStopwatch = 0f;
				}
				if (this.characterBody)
				{
					this.characterBody.OnSkillActivated(this);
				}
			}
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0004CF4C File Offset: 0x0004B14C
		private void RestockContinuous()
		{
			if (this.finalRechargeInterval == 0f)
			{
				this.stock = this.maxStock;
				this.rechargeStopwatch = 0f;
				return;
			}
			int num = Mathf.FloorToInt(this.rechargeStopwatch / this.finalRechargeInterval * (float)this.rechargeStock);
			this.stock += num;
			if (this.stock >= this.maxStock)
			{
				this.stock = this.maxStock;
				this.rechargeStopwatch = 0f;
				return;
			}
			this.rechargeStopwatch -= (float)num * this.finalRechargeInterval;
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0004CFE4 File Offset: 0x0004B1E4
		private void RestockSteplike()
		{
			if (!this.isBullets)
			{
				this.stock += this.rechargeStock;
				if (this.stock >= this.maxStock)
				{
					this.stock = this.maxStock;
				}
			}
			else
			{
				this.stock = this.maxStock;
			}
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x0004D03F File Offset: 0x0004B23F
		private void RecalculateFinalRechargeInterval()
		{
			this.finalRechargeInterval = Mathf.Min(this.baseRechargeInterval, Mathf.Max(0.5f, this.baseRechargeInterval * this.cooldownScale));
		}

		// Token: 0x040013A1 RID: 5025
		[Tooltip("The name of the skill. This is mainly for purposes of identification in the inspector and currently has no direct effect.")]
		public string skillName = "";

		// Token: 0x040013A2 RID: 5026
		[Tooltip("The language token with the name of this skill.")]
		public string skillNameToken = "";

		// Token: 0x040013A3 RID: 5027
		[Tooltip("The language token with the description of this skill.")]
		public string skillDescriptionToken = "";

		// Token: 0x040013A4 RID: 5028
		[FormerlySerializedAs("rechargeInterval")]
		[Tooltip("How long it takes for this skill to recharge after being used.")]
		public float baseRechargeInterval = 1f;

		// Token: 0x040013A5 RID: 5029
		[Tooltip("Maximum number of charges this skill can carry.")]
		[FormerlySerializedAs("maxStock")]
		public int baseMaxStock = 1;

		// Token: 0x040013A6 RID: 5030
		[Tooltip("How much stock to restore on a recharge.")]
		public int rechargeStock = 1;

		// Token: 0x040013A7 RID: 5031
		[Tooltip("Whether or not it has bullet reload behavior")]
		public bool isBullets;

		// Token: 0x040013A8 RID: 5032
		[Tooltip("Time between bullets for bullet-style weapons")]
		public float shootDelay = 0.3f;

		// Token: 0x040013A9 RID: 5033
		[Tooltip("Whether or not the cooldown waits until it leaves the set state")]
		public bool beginSkillCooldownOnSkillEnd;

		// Token: 0x040013AA RID: 5034
		[Tooltip("The state machine this skill operates upon.")]
		public EntityStateMachine stateMachine;

		// Token: 0x040013AB RID: 5035
		[Tooltip("The state to enter when this skill is activated.")]
		public SerializableEntityStateType activationState;

		// Token: 0x040013AC RID: 5036
		[Tooltip("The priority of this skill.")]
		public InterruptPriority interruptPriority = InterruptPriority.Skill;

		// Token: 0x040013AD RID: 5037
		[Tooltip("Whether or not this is considered a combat skill.")]
		public bool isCombatSkill = true;

		// Token: 0x040013AE RID: 5038
		[Tooltip("Whether or not the usage of this skill is mutually exclusive with sprinting.")]
		public bool noSprint = true;

		// Token: 0x040013AF RID: 5039
		[Tooltip("Sprinting will actively cancel this ability.")]
		public bool canceledFromSprinting;

		// Token: 0x040013B0 RID: 5040
		[Tooltip("The skill can't be activated if the key is held.")]
		public bool mustKeyPress;

		// Token: 0x040013B1 RID: 5041
		[ShowThumbnail]
		[Tooltip("The icon to display for this skill.")]
		public Sprite icon;

		// Token: 0x040013B2 RID: 5042
		[Tooltip("How much stock is required to activate this skill.")]
		public int requiredStock = 1;

		// Token: 0x040013B3 RID: 5043
		[Tooltip("How much stock to deduct when the skill is activated.")]
		public int stockToConsume = 1;

		// Token: 0x040013B4 RID: 5044
		private CharacterBody characterBody;

		// Token: 0x040013B5 RID: 5045
		protected int bonusStockFromBody;

		// Token: 0x040013B8 RID: 5048
		private float finalRechargeInterval;

		// Token: 0x040013B9 RID: 5049
		private float _cooldownScale = 1f;

		// Token: 0x040013BA RID: 5050
		private float rechargeStopwatch;

		// Token: 0x040013BB RID: 5051
		[HideInInspector]
		public bool hasExecutedSuccessfully;
	}
}
