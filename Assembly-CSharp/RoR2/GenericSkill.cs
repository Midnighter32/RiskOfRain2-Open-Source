using System;
using System.Collections.Generic;
using EntityStates;
using JetBrains.Annotations;
using RoR2.Skills;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000210 RID: 528
	[RequireComponent(typeof(CharacterBody))]
	public sealed class GenericSkill : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000B67 RID: 2919 RVA: 0x00032308 File Offset: 0x00030508
		// (set) Token: 0x06000B68 RID: 2920 RVA: 0x00032310 File Offset: 0x00030510
		public SkillDef skillDef { get; private set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x00032319 File Offset: 0x00030519
		public SkillFamily skillFamily
		{
			get
			{
				return this._skillFamily;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x00032321 File Offset: 0x00030521
		// (set) Token: 0x06000B6B RID: 2923 RVA: 0x00032329 File Offset: 0x00030529
		public SkillDef baseSkill { get; private set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x00032332 File Offset: 0x00030532
		public string skillNameToken
		{
			get
			{
				return this.skillDef.skillNameToken;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000B6D RID: 2925 RVA: 0x0003233F File Offset: 0x0003053F
		public string skillDescriptionToken
		{
			get
			{
				return this.skillDef.skillDescriptionToken;
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000B6E RID: 2926 RVA: 0x0003234C File Offset: 0x0003054C
		public float baseRechargeInterval
		{
			get
			{
				return this.skillDef.GetRechargeInterval(this);
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000B6F RID: 2927 RVA: 0x0003235A File Offset: 0x0003055A
		public int rechargeStock
		{
			get
			{
				return this.skillDef.GetRechargeStock(this);
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000B70 RID: 2928 RVA: 0x00032368 File Offset: 0x00030568
		public bool isBullets
		{
			get
			{
				return this.skillDef.isBullets;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000B71 RID: 2929 RVA: 0x00032375 File Offset: 0x00030575
		public bool beginSkillCooldownOnSkillEnd
		{
			get
			{
				return this.skillDef.beginSkillCooldownOnSkillEnd;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000B72 RID: 2930 RVA: 0x00032382 File Offset: 0x00030582
		public SerializableEntityStateType activationState
		{
			get
			{
				return this.skillDef.activationState;
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000B73 RID: 2931 RVA: 0x0003238F File Offset: 0x0003058F
		public InterruptPriority interruptPriority
		{
			get
			{
				return this.skillDef.interruptPriority;
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000B74 RID: 2932 RVA: 0x0003239C File Offset: 0x0003059C
		public bool isCombatSkill
		{
			get
			{
				return this.skillDef.isCombatSkill;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000B75 RID: 2933 RVA: 0x000323A9 File Offset: 0x000305A9
		public bool mustKeyPress
		{
			get
			{
				return this.skillDef.mustKeyPress;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x000323B6 File Offset: 0x000305B6
		public Sprite icon
		{
			get
			{
				return this.skillDef.GetCurrentIcon(this);
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x000323C4 File Offset: 0x000305C4
		// (set) Token: 0x06000B78 RID: 2936 RVA: 0x000323CC File Offset: 0x000305CC
		[CanBeNull]
		public EntityStateMachine stateMachine { get; private set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000B79 RID: 2937 RVA: 0x000323D5 File Offset: 0x000305D5
		// (set) Token: 0x06000B7A RID: 2938 RVA: 0x000323DD File Offset: 0x000305DD
		[CanBeNull]
		public SkillDef.BaseSkillInstanceData skillInstanceData { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000B7B RID: 2939 RVA: 0x000323E6 File Offset: 0x000305E6
		// (set) Token: 0x06000B7C RID: 2940 RVA: 0x000323EE File Offset: 0x000305EE
		public CharacterBody characterBody { get; private set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000B7D RID: 2941 RVA: 0x000323F7 File Offset: 0x000305F7
		// (set) Token: 0x06000B7E RID: 2942 RVA: 0x000323FF File Offset: 0x000305FF
		public SkillDef defaultSkillDef { get; private set; }

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06000B7F RID: 2943 RVA: 0x00032408 File Offset: 0x00030608
		// (remove) Token: 0x06000B80 RID: 2944 RVA: 0x00032440 File Offset: 0x00030640
		public event Action<GenericSkill> onSkillChanged;

		// Token: 0x06000B81 RID: 2945 RVA: 0x00032478 File Offset: 0x00030678
		private int FindSkillOverrideIndex(ref GenericSkill.SkillOverride skillOverride)
		{
			for (int i = 0; i < this.skillOverrides.Length; i++)
			{
				if (this.skillOverrides[i].Equals(skillOverride))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x000324B4 File Offset: 0x000306B4
		public void SetSkillOverride(object source, SkillDef skillDef, GenericSkill.SkillOverridePriority priority)
		{
			GenericSkill.SkillOverride skillOverride = new GenericSkill.SkillOverride(source, skillDef, priority);
			if (this.FindSkillOverrideIndex(ref skillOverride) == -1)
			{
				HGArrayUtilities.ArrayAppend<GenericSkill.SkillOverride>(ref this.skillOverrides, ref skillOverride);
				this.PickCurrentOverride();
			}
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x000324EC File Offset: 0x000306EC
		public void UnsetSkillOverride(object source, SkillDef skillDef, GenericSkill.SkillOverridePriority priority)
		{
			GenericSkill.SkillOverride skillOverride = new GenericSkill.SkillOverride(source, skillDef, priority);
			int num = this.FindSkillOverrideIndex(ref skillOverride);
			if (num != -1)
			{
				HGArrayUtilities.ArrayRemoveAtAndResize<GenericSkill.SkillOverride>(ref this.skillOverrides, num, 1);
				this.PickCurrentOverride();
			}
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x00032524 File Offset: 0x00030724
		private void PickCurrentOverride()
		{
			int num = -1;
			GenericSkill.SkillOverridePriority skillOverridePriority = GenericSkill.SkillOverridePriority.Default;
			for (int i = 0; i < this.skillOverrides.Length; i++)
			{
				GenericSkill.SkillOverridePriority priority = this.skillOverrides[i].priority;
				if (skillOverridePriority <= priority)
				{
					num = i;
					skillOverridePriority = priority;
				}
			}
			if (num == -1)
			{
				this.SetSkillInternal(this.baseSkill);
				return;
			}
			this.SetSkillInternal(this.skillOverrides[num].skillDef);
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0003258A File Offset: 0x0003078A
		private void SetSkillInternal(SkillDef newSkillDef)
		{
			if (this.skillDef == newSkillDef)
			{
				return;
			}
			this.UnassignSkill();
			this.AssignSkill(newSkillDef);
			Action<GenericSkill> action = this.onSkillChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x000325B9 File Offset: 0x000307B9
		public void SetBaseSkill(SkillDef newSkillDef)
		{
			this.baseSkill = newSkillDef;
			this.PickCurrentOverride();
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x000325C8 File Offset: 0x000307C8
		private void UnassignSkill()
		{
			if (this.skillDef == null)
			{
				return;
			}
			this.skillDef.OnUnassigned(this);
			this.skillInstanceData = null;
			this.skillDef = null;
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x000325F0 File Offset: 0x000307F0
		private void AssignSkill(SkillDef newSkillDef)
		{
			this.skillDef = newSkillDef;
			if (this.skillDef == null)
			{
				return;
			}
			EntityStateMachine stateMachine = this.stateMachine;
			if (((stateMachine != null) ? stateMachine.customName : null) != this.skillDef.activationStateMachineName)
			{
				this.stateMachine = null;
				base.GetComponents<EntityStateMachine>(GenericSkill.stateMachineLookupBuffer);
				int i = 0;
				int count = GenericSkill.stateMachineLookupBuffer.Count;
				while (i < count)
				{
					EntityStateMachine entityStateMachine = GenericSkill.stateMachineLookupBuffer[i];
					if (entityStateMachine.customName == this.skillDef.activationStateMachineName)
					{
						this.stateMachine = entityStateMachine;
						break;
					}
					i++;
				}
			}
			if (this.skillDef.fullRestockOnAssign)
			{
				this.RecalculateMaxStock();
				this.stock = this.maxStock;
			}
			this.skillInstanceData = this.skillDef.OnAssigned(this);
			this.RecalculateFinalRechargeInterval();
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x000326BE File Offset: 0x000308BE
		public void SetBonusStockFromBody(int newBonusStockFromBody)
		{
			this.bonusStockFromBody = newBonusStockFromBody;
			this.RecalculateMaxStock();
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000B8A RID: 2954 RVA: 0x000326CD File Offset: 0x000308CD
		// (set) Token: 0x06000B8B RID: 2955 RVA: 0x000326D5 File Offset: 0x000308D5
		public int maxStock { get; private set; }

		// Token: 0x06000B8C RID: 2956 RVA: 0x000326DE File Offset: 0x000308DE
		private void RecalculateMaxStock()
		{
			this.maxStock = this.skillDef.GetMaxStock(this) + this.bonusStockFromBody;
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000B8D RID: 2957 RVA: 0x000326F9 File Offset: 0x000308F9
		// (set) Token: 0x06000B8E RID: 2958 RVA: 0x00032701 File Offset: 0x00030901
		public int stock { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000B8F RID: 2959 RVA: 0x0003270A File Offset: 0x0003090A
		// (set) Token: 0x06000B90 RID: 2960 RVA: 0x00032712 File Offset: 0x00030912
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

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x0003272B File Offset: 0x0003092B
		// (set) Token: 0x06000B92 RID: 2962 RVA: 0x00032733 File Offset: 0x00030933
		public float rechargeStopwatch { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000B93 RID: 2963 RVA: 0x0003273C File Offset: 0x0003093C
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

		// Token: 0x06000B94 RID: 2964 RVA: 0x0003275F File Offset: 0x0003095F
		private void Awake()
		{
			this.defaultSkillDef = this.skillFamily.defaultSkillDef;
			this.baseSkill = this.defaultSkillDef;
			this.characterBody = base.GetComponent<CharacterBody>();
			this.AssignSkill(this.defaultSkillDef);
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00032796 File Offset: 0x00030996
		private void OnDestroy()
		{
			this.UnassignSkill();
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x0003279E File Offset: 0x0003099E
		private void Start()
		{
			this.RecalculateMaxStock();
			this.stock = this.maxStock;
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x000327B2 File Offset: 0x000309B2
		private void FixedUpdate()
		{
			SkillDef skillDef = this.skillDef;
			if (skillDef == null)
			{
				return;
			}
			skillDef.OnFixedUpdate(this);
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x00022B74 File Offset: 0x00020D74
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x000327C5 File Offset: 0x000309C5
		public bool CanExecute()
		{
			SkillDef skillDef = this.skillDef;
			return skillDef != null && skillDef.CanExecute(this);
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x000327D9 File Offset: 0x000309D9
		public bool IsReady()
		{
			SkillDef skillDef = this.skillDef;
			return skillDef != null && skillDef.IsReady(this);
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x000327ED File Offset: 0x000309ED
		public bool ExecuteIfReady()
		{
			this.hasExecutedSuccessfully = this.CanExecute();
			if (this.hasExecutedSuccessfully)
			{
				this.OnExecute();
				return true;
			}
			return false;
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0003280C File Offset: 0x00030A0C
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

		// Token: 0x06000B9D RID: 2973 RVA: 0x00032876 File Offset: 0x00030A76
		public void Reset()
		{
			this.rechargeStopwatch = 0f;
			this.stock = this.maxStock;
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0003288F File Offset: 0x00030A8F
		public bool CanApplyAmmoPack()
		{
			return this.stock < this.maxStock;
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x000328A4 File Offset: 0x00030AA4
		public void AddOneStock()
		{
			int stock = this.stock + 1;
			this.stock = stock;
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x000328CC File Offset: 0x00030ACC
		public void RemoveAllStocks()
		{
			this.stock = 0;
			this.rechargeStopwatch = 0f;
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x000328E0 File Offset: 0x00030AE0
		public void DeductStock(int count)
		{
			this.stock = Mathf.Max(0, this.stock - count);
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x000328F6 File Offset: 0x00030AF6
		private void OnExecute()
		{
			this.skillDef.OnExecute(this);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x00032904 File Offset: 0x00030B04
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

		// Token: 0x06000BA4 RID: 2980 RVA: 0x0003299C File Offset: 0x00030B9C
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

		// Token: 0x06000BA5 RID: 2981 RVA: 0x000329F7 File Offset: 0x00030BF7
		public float CalculateFinalRechargeInterval()
		{
			return Mathf.Min(this.baseRechargeInterval, Mathf.Max(0.5f, this.baseRechargeInterval * this.cooldownScale));
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00032A1B File Offset: 0x00030C1B
		private void RecalculateFinalRechargeInterval()
		{
			this.finalRechargeInterval = this.CalculateFinalRechargeInterval();
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x00032A29 File Offset: 0x00030C29
		public void RecalculateValues()
		{
			this.RecalculateMaxStock();
			this.RecalculateFinalRechargeInterval();
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x00032A37 File Offset: 0x00030C37
		[AssetCheck(typeof(GenericSkill))]
		private static void CheckGenericSkillStateMachine(AssetCheckArgs args)
		{
			if (((GenericSkill)args.asset).stateMachine.customName == string.Empty)
			{
				args.LogError("Unnamed state machine.", ((GenericSkill)args.asset).gameObject);
			}
		}

		// Token: 0x04000BA7 RID: 2983
		[SerializeField]
		private SkillFamily _skillFamily;

		// Token: 0x04000BA9 RID: 2985
		public string skillName;

		// Token: 0x04000BAF RID: 2991
		private static readonly List<EntityStateMachine> stateMachineLookupBuffer = new List<EntityStateMachine>();

		// Token: 0x04000BB0 RID: 2992
		private GenericSkill.SkillOverride[] skillOverrides = Array.Empty<GenericSkill.SkillOverride>();

		// Token: 0x04000BB1 RID: 2993
		private int bonusStockFromBody;

		// Token: 0x04000BB4 RID: 2996
		private float finalRechargeInterval;

		// Token: 0x04000BB5 RID: 2997
		private float _cooldownScale = 1f;

		// Token: 0x04000BB7 RID: 2999
		[HideInInspector]
		public bool hasExecutedSuccessfully;

		// Token: 0x02000211 RID: 529
		public struct SkillOverride : IEquatable<GenericSkill.SkillOverride>
		{
			// Token: 0x06000BAB RID: 2987 RVA: 0x00032A9F File Offset: 0x00030C9F
			public SkillOverride(object source, SkillDef skillDef, GenericSkill.SkillOverridePriority priority)
			{
				this.source = source;
				this.skillDef = skillDef;
				this.priority = priority;
			}

			// Token: 0x06000BAC RID: 2988 RVA: 0x00032AB6 File Offset: 0x00030CB6
			public bool Equals(GenericSkill.SkillOverride other)
			{
				return object.Equals(this.source, other.source) && object.Equals(this.skillDef, other.skillDef) && this.priority == other.priority;
			}

			// Token: 0x06000BAD RID: 2989 RVA: 0x00032AF0 File Offset: 0x00030CF0
			public override bool Equals(object obj)
			{
				if (obj is GenericSkill.SkillOverride)
				{
					GenericSkill.SkillOverride other = (GenericSkill.SkillOverride)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06000BAE RID: 2990 RVA: 0x00032B18 File Offset: 0x00030D18
			public override int GetHashCode()
			{
				return (((this.source != null) ? this.source.GetHashCode() : 0) * 397 ^ ((this.skillDef != null) ? this.skillDef.GetHashCode() : 0)) * 397 ^ (int)this.priority;
			}

			// Token: 0x04000BB8 RID: 3000
			public readonly object source;

			// Token: 0x04000BB9 RID: 3001
			public readonly SkillDef skillDef;

			// Token: 0x04000BBA RID: 3002
			public readonly GenericSkill.SkillOverridePriority priority;
		}

		// Token: 0x02000212 RID: 530
		public enum SkillOverridePriority
		{
			// Token: 0x04000BBC RID: 3004
			Default,
			// Token: 0x04000BBD RID: 3005
			Loadout,
			// Token: 0x04000BBE RID: 3006
			Upgrade,
			// Token: 0x04000BBF RID: 3007
			Replacement,
			// Token: 0x04000BC0 RID: 3008
			Contextual,
			// Token: 0x04000BC1 RID: 3009
			Network
		}

		// Token: 0x02000213 RID: 531
		public class SkillOverrideHandle : IDisposable
		{
			// Token: 0x06000BAF RID: 2991 RVA: 0x0000409B File Offset: 0x0000229B
			public void Dispose()
			{
			}

			// Token: 0x04000BC2 RID: 3010
			public readonly object source;

			// Token: 0x04000BC3 RID: 3011
			public readonly SkillDef skill;

			// Token: 0x04000BC4 RID: 3012
			public readonly GenericSkill skillSlot;

			// Token: 0x04000BC5 RID: 3013
			public readonly GenericSkill.SkillOverridePriority priority;
		}
	}
}
