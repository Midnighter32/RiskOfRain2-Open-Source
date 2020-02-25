using System;
using EntityStates;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Skills
{
	// Token: 0x020004BC RID: 1212
	[CreateAssetMenu(menuName = "RoR2/SkillDef/Generic")]
	public class SkillDef : ScriptableObject
	{
		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06001D3B RID: 7483 RVA: 0x0000AC7F File Offset: 0x00008E7F
		[Obsolete("Accessing UnityEngine.Object.Name causes allocations on read. Look up the name from the catalog instead. If absolutely necessary to perform direct access, cast to ScriptableObject first.")]
		public new string name
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06001D3C RID: 7484 RVA: 0x0007CCA5 File Offset: 0x0007AEA5
		// (set) Token: 0x06001D3D RID: 7485 RVA: 0x0007CCAD File Offset: 0x0007AEAD
		public int skillIndex { get; set; }

		// Token: 0x06001D3E RID: 7486 RVA: 0x0000AC7F File Offset: 0x00008E7F
		public virtual SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return null;
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnUnassigned([NotNull] GenericSkill skillSlot)
		{
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x0007CCB6 File Offset: 0x0007AEB6
		public virtual Sprite GetCurrentIcon([NotNull] GenericSkill skillSlot)
		{
			return this.icon;
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x0007CCC0 File Offset: 0x0007AEC0
		protected bool HasRequiredStockAndDelay([NotNull] GenericSkill skillSlot)
		{
			if (!this.isBullets)
			{
				return skillSlot.stock >= this.requiredStock;
			}
			return (skillSlot.stock >= this.requiredStock && skillSlot.rechargeStopwatch >= this.shootDelay) || skillSlot.stock == skillSlot.maxStock;
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0007CD13 File Offset: 0x0007AF13
		public virtual bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return this.HasRequiredStockAndDelay(skillSlot) && skillSlot.stateMachine && !skillSlot.stateMachine.HasPendingState() && skillSlot.stateMachine.CanInterruptState(this.interruptPriority);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x0007CD4B File Offset: 0x0007AF4B
		public virtual bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return this.HasRequiredStockAndDelay(skillSlot);
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x0007CD54 File Offset: 0x0007AF54
		protected virtual EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
		{
			EntityState entityState = EntityState.Instantiate(this.activationState);
			BaseSkillState baseSkillState;
			if ((baseSkillState = (entityState as BaseSkillState)) != null)
			{
				baseSkillState.activatorSkillSlot = skillSlot;
			}
			return entityState;
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x0007CD80 File Offset: 0x0007AF80
		public virtual void OnExecute([NotNull] GenericSkill skillSlot)
		{
			skillSlot.stateMachine.SetInterruptState(this.InstantiateNextState(skillSlot), this.interruptPriority);
			if (this.noSprint)
			{
				skillSlot.characterBody.isSprinting = false;
			}
			skillSlot.stock -= this.stockToConsume;
			if (this.isBullets)
			{
				skillSlot.rechargeStopwatch = 0f;
			}
			if (skillSlot.characterBody)
			{
				skillSlot.characterBody.OnSkillActivated(skillSlot);
			}
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x0007CDFC File Offset: 0x0007AFFC
		public virtual void OnFixedUpdate([NotNull] GenericSkill skillSlot)
		{
			skillSlot.RunRecharge(Time.fixedDeltaTime);
			if (this.canceledFromSprinting && skillSlot.characterBody.isSprinting && skillSlot.stateMachine.state.GetType() == this.activationState.stateType)
			{
				skillSlot.stateMachine.SetNextStateToMain();
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x0007CE56 File Offset: 0x0007B056
		public bool IsAlreadyInState([NotNull] GenericSkill skillSlot)
		{
			return ((skillSlot != null) ? skillSlot.stateMachine.state.GetType() : null) == this.activationState.stateType;
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x0007CE7E File Offset: 0x0007B07E
		public virtual int GetMaxStock([NotNull] GenericSkill skillSlot)
		{
			return this.baseMaxStock;
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x0007CE86 File Offset: 0x0007B086
		public virtual int GetRechargeStock([NotNull] GenericSkill skillSlot)
		{
			return this.rechargeStock;
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x0007CE8E File Offset: 0x0007B08E
		public virtual float GetRechargeInterval([NotNull] GenericSkill skillSlot)
		{
			return this.baseRechargeInterval;
		}

		// Token: 0x04001A38 RID: 6712
		[Tooltip("The name of the skill. This is mainly for purposes of identification in the inspector and currently has no direct effect.")]
		[Header("Skill Identifier")]
		public string skillName = "";

		// Token: 0x04001A39 RID: 6713
		[Tooltip("The language token with the name of this skill.")]
		[Header("User-Facing Info")]
		public string skillNameToken = "";

		// Token: 0x04001A3A RID: 6714
		[Tooltip("The language token with the description of this skill.")]
		public string skillDescriptionToken = "";

		// Token: 0x04001A3B RID: 6715
		[ShowThumbnail]
		[Tooltip("The icon to display for this skill.")]
		public Sprite icon;

		// Token: 0x04001A3C RID: 6716
		[Header("State Machine Parameters")]
		[Tooltip("The state machine this skill operates upon.")]
		public string activationStateMachineName;

		// Token: 0x04001A3D RID: 6717
		[Tooltip("The state to enter when this skill is activated.")]
		public SerializableEntityStateType activationState;

		// Token: 0x04001A3E RID: 6718
		[Tooltip("The priority of this skill.")]
		public InterruptPriority interruptPriority = InterruptPriority.Skill;

		// Token: 0x04001A3F RID: 6719
		[Header("Stock and Cooldown")]
		[Tooltip("How long it takes for this skill to recharge after being used.")]
		public float baseRechargeInterval = 1f;

		// Token: 0x04001A40 RID: 6720
		[Tooltip("Maximum number of charges this skill can carry.")]
		public int baseMaxStock = 1;

		// Token: 0x04001A41 RID: 6721
		[Tooltip("How much stock to restore on a recharge.")]
		public int rechargeStock = 1;

		// Token: 0x04001A42 RID: 6722
		[Tooltip("Whether or not it has bullet reload behavior")]
		public bool isBullets;

		// Token: 0x04001A43 RID: 6723
		[Tooltip("Time between bullets for bullet-style weapons")]
		public float shootDelay = 0.3f;

		// Token: 0x04001A44 RID: 6724
		[Tooltip("Whether or not the cooldown waits until it leaves the set state")]
		public bool beginSkillCooldownOnSkillEnd;

		// Token: 0x04001A45 RID: 6725
		[Tooltip("How much stock is required to activate this skill.")]
		public int requiredStock = 1;

		// Token: 0x04001A46 RID: 6726
		[Tooltip("How much stock to deduct when the skill is activated.")]
		public int stockToConsume = 1;

		// Token: 0x04001A47 RID: 6727
		[Tooltip("Whether or not this is considered a combat skill.")]
		[Header("Misc Parameters")]
		public bool isCombatSkill = true;

		// Token: 0x04001A48 RID: 6728
		[Tooltip("Whether or not the usage of this skill is mutually exclusive with sprinting.")]
		public bool noSprint = true;

		// Token: 0x04001A49 RID: 6729
		[Tooltip("Sprinting will actively cancel this ability.")]
		public bool canceledFromSprinting;

		// Token: 0x04001A4A RID: 6730
		[Tooltip("The skill can't be activated if the key is held.")]
		public bool mustKeyPress;

		// Token: 0x04001A4B RID: 6731
		[Tooltip("Whether or not to fully restock this skill when it's assigned.")]
		public bool fullRestockOnAssign = true;

		// Token: 0x020004BD RID: 1213
		public class BaseSkillInstanceData
		{
		}
	}
}
