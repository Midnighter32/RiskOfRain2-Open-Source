using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D6 RID: 982
	public class SetStateOnWeakened : NetworkBehaviour
	{
		// Token: 0x06001547 RID: 5447 RVA: 0x00066222 File Offset: 0x00064422
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x00066230 File Offset: 0x00064430
		private void OnTakeDamage(DamageInfo damageInfo)
		{
			if (this.consumed)
			{
				return;
			}
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				float num = damageInfo.crit ? (damageInfo.damage * 2f) : damageInfo.damage;
				if ((damageInfo.damageType & DamageType.WeakPointHit) != DamageType.Generic)
				{
					this.accumulatedDamage += num;
					if (this.accumulatedDamage > this.characterBody.maxHealth * this.weakenPercentage)
					{
						this.consumed = true;
						this.SetWeak(damageInfo);
					}
				}
			}
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x000662C8 File Offset: 0x000644C8
		public void SetWeak(DamageInfo damageInfo)
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
			HurtBox[] array2 = this.weakHurtBox;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].gameObject.SetActive(false);
			}
			if (this.selfDamagePercentage > 0f)
			{
				DamageInfo damageInfo2 = new DamageInfo();
				damageInfo2.damage = this.characterBody.maxHealth * this.selfDamagePercentage / 3f;
				damageInfo2.attacker = damageInfo.attacker;
				damageInfo2.crit = true;
				damageInfo2.position = damageInfo.position;
				damageInfo2.damageType = (DamageType.NonLethal | DamageType.WeakPointHit);
				this.characterBody.healthComponent.TakeDamage(damageInfo2);
			}
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x000663BC File Offset: 0x000645BC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001898 RID: 6296
		[Tooltip("The percentage of their max HP they need to take to get weakened. Ranges from 0-1.")]
		public float weakenPercentage = 0.1f;

		// Token: 0x04001899 RID: 6297
		[Tooltip("The percentage of their max HP they deal to themselves once weakened. Ranges from 0-1.")]
		public float selfDamagePercentage;

		// Token: 0x0400189A RID: 6298
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x0400189B RID: 6299
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x0400189C RID: 6300
		[Tooltip("The hurtboxes to set to not a weak point once consumed")]
		public HurtBox[] weakHurtBox;

		// Token: 0x0400189D RID: 6301
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x0400189E RID: 6302
		private float accumulatedDamage;

		// Token: 0x0400189F RID: 6303
		private bool consumed;

		// Token: 0x040018A0 RID: 6304
		private CharacterBody characterBody;
	}
}
