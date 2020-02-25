using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000327 RID: 807
	public class SetStateOnWeakened : NetworkBehaviour, IOnTakeDamageServerReceiver
	{
		// Token: 0x0600130B RID: 4875 RVA: 0x00051A0A File Offset: 0x0004FC0A
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x00051A18 File Offset: 0x0004FC18
		public void OnTakeDamageServer(DamageReport damageReport)
		{
			if (this.consumed)
			{
				return;
			}
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				DamageInfo damageInfo = damageReport.damageInfo;
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

		// Token: 0x0600130D RID: 4877 RVA: 0x00051AB8 File Offset: 0x0004FCB8
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

		// Token: 0x0600130F RID: 4879 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x00051BAC File Offset: 0x0004FDAC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040011D3 RID: 4563
		[Tooltip("The percentage of their max HP they need to take to get weakened. Ranges from 0-1.")]
		public float weakenPercentage = 0.1f;

		// Token: 0x040011D4 RID: 4564
		[Tooltip("The percentage of their max HP they deal to themselves once weakened. Ranges from 0-1.")]
		public float selfDamagePercentage;

		// Token: 0x040011D5 RID: 4565
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x040011D6 RID: 4566
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x040011D7 RID: 4567
		[Tooltip("The hurtboxes to set to not a weak point once consumed")]
		public HurtBox[] weakHurtBox;

		// Token: 0x040011D8 RID: 4568
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x040011D9 RID: 4569
		private float accumulatedDamage;

		// Token: 0x040011DA RID: 4570
		private bool consumed;

		// Token: 0x040011DB RID: 4571
		private CharacterBody characterBody;
	}
}
