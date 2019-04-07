using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000183 RID: 387
	internal class EngiOtherShield : BaseState
	{
		// Token: 0x06000770 RID: 1904 RVA: 0x00024924 File Offset: 0x00022B24
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.target)
			{
				this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/EngiShieldRetractIndicator"));
				this.indicator.active = true;
				this.indicator.targetTransform = Util.GetCoreTransform(this.target.gameObject);
				this.target.AddBuff(BuffIndex.EngiShield);
				this.target.RecalculateStats();
				HealthComponent component = this.target.GetComponent<HealthComponent>();
				if (component)
				{
					component.RechargeShieldFull();
				}
			}
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x000249B8 File Offset: 0x00022BB8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.target || !base.characterBody.healthComponent.alive)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x000249EC File Offset: 0x00022BEC
		public override void OnExit()
		{
			base.skillLocator.utility = base.skillLocator.FindSkill("GiveShield");
			if (NetworkServer.active && this.target)
			{
				this.target.RemoveBuff(BuffIndex.EngiShield);
			}
			if (base.isAuthority)
			{
				base.skillLocator.utility.RemoveAllStocks();
			}
			if (this.indicator != null)
			{
				this.indicator.active = false;
			}
			base.OnExit();
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00024A67 File Offset: 0x00022C67
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.fixedAge < this.minimumDuration)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Skill;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00024A7A File Offset: 0x00022C7A
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.target.gameObject);
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00024A94 File Offset: 0x00022C94
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			GameObject gameObject = reader.ReadGameObject();
			if (gameObject)
			{
				this.target = gameObject.GetComponent<CharacterBody>();
			}
		}

		// Token: 0x04000988 RID: 2440
		public CharacterBody target;

		// Token: 0x04000989 RID: 2441
		public float minimumDuration;

		// Token: 0x0400098A RID: 2442
		private Indicator indicator;
	}
}
