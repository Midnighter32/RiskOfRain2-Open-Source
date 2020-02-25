using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000882 RID: 2178
	public class EngiOtherShield : BaseState
	{
		// Token: 0x06003100 RID: 12544 RVA: 0x000D2B8C File Offset: 0x000D0D8C
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

		// Token: 0x06003101 RID: 12545 RVA: 0x000D2C20 File Offset: 0x000D0E20
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.target || !base.characterBody.healthComponent.alive)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06003102 RID: 12546 RVA: 0x000D2C54 File Offset: 0x000D0E54
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

		// Token: 0x06003103 RID: 12547 RVA: 0x000D2CCF File Offset: 0x000D0ECF
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.fixedAge < this.minimumDuration)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Skill;
		}

		// Token: 0x06003104 RID: 12548 RVA: 0x000D2CE2 File Offset: 0x000D0EE2
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.target.gameObject);
		}

		// Token: 0x06003105 RID: 12549 RVA: 0x000D2CFC File Offset: 0x000D0EFC
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			GameObject gameObject = reader.ReadGameObject();
			if (gameObject)
			{
				this.target = gameObject.GetComponent<CharacterBody>();
			}
		}

		// Token: 0x04002F38 RID: 12088
		public CharacterBody target;

		// Token: 0x04002F39 RID: 12089
		public float minimumDuration;

		// Token: 0x04002F3A RID: 12090
		private Indicator indicator;
	}
}
