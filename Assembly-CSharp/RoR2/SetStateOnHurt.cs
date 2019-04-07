using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D5 RID: 981
	public class SetStateOnHurt : NetworkBehaviour
	{
		// Token: 0x0600153E RID: 5438 RVA: 0x0006603E File Offset: 0x0006423E
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x0006604C File Offset: 0x0006424C
		private void OnTakeDamage(DamageInfo damageInfo)
		{
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				float num = damageInfo.crit ? (damageInfo.damage * 2f) : damageInfo.damage;
				if ((damageInfo.damageType & DamageType.Freeze2s) != DamageType.Generic)
				{
					this.SetFrozen(2f);
					return;
				}
				if (!this.characterBody.healthComponent.isFrozen)
				{
					if ((damageInfo.damageType & DamageType.Stun1s) != DamageType.Generic)
					{
						this.SetStun(1f);
						return;
					}
					if (num > this.characterBody.maxHealth * this.hitThreshold)
					{
						this.SetPain();
					}
				}
			}
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x000660FC File Offset: 0x000642FC
		public void SetStun(float duration)
		{
			if (this.targetStateMachine)
			{
				StunState stunState = new StunState();
				stunState.stunDuration = duration;
				this.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x00066154 File Offset: 0x00064354
		public void SetFrozen(float duration)
		{
			if (this.targetStateMachine)
			{
				FrozenState frozenState = new FrozenState();
				frozenState.freezeDuration = duration;
				this.targetStateMachine.SetInterruptState(frozenState, InterruptPriority.Death);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x000661AC File Offset: 0x000643AC
		public void SetPain()
		{
			if (this.targetStateMachine)
			{
				this.targetStateMachine.SetInterruptState(EntityState.Instantiate(this.hurtState), InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x00066214 File Offset: 0x00064414
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001893 RID: 6291
		[Tooltip("The percentage of their max HP they need to take to get stunned. Ranges from 0-1.")]
		public float hitThreshold = 0.1f;

		// Token: 0x04001894 RID: 6292
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x04001895 RID: 6293
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x04001896 RID: 6294
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x04001897 RID: 6295
		private CharacterBody characterBody;
	}
}
