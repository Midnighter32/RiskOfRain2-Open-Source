using System;
using EntityStates;
using EntityStates.Merc;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Skills
{
	// Token: 0x020004B8 RID: 1208
	[CreateAssetMenu(menuName = "RoR2/SkillDef/MercDashSkillDef")]
	public class MercDashSkillDef : SkillDef
	{
		// Token: 0x06001D22 RID: 7458 RVA: 0x0007C82E File Offset: 0x0007AA2E
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new MercDashSkillDef.InstanceData();
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x0007C835 File Offset: 0x0007AA35
		public override void OnUnassigned([NotNull] GenericSkill skillSlot)
		{
			base.OnUnassigned(skillSlot);
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x0007C840 File Offset: 0x0007AA40
		public override void OnFixedUpdate([NotNull] GenericSkill skillSlot)
		{
			base.OnFixedUpdate(skillSlot);
			MercDashSkillDef.InstanceData instanceData = (MercDashSkillDef.InstanceData)skillSlot.skillInstanceData;
			Assaulter assaulter;
			if (instanceData.waitingForHit && (assaulter = (skillSlot.stateMachine.state as Assaulter)) != null && assaulter.hasHit)
			{
				instanceData.waitingForHit = false;
				this.AddHit(skillSlot);
			}
			instanceData.timeoutTimer -= Time.fixedDeltaTime;
			if (instanceData.timeoutTimer <= 0f && instanceData.currentDashIndex != 0)
			{
				if (instanceData.hasExtraStock)
				{
					skillSlot.stock--;
					instanceData.hasExtraStock = false;
				}
				instanceData.currentDashIndex = 0;
			}
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x0007C8E0 File Offset: 0x0007AAE0
		protected override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
		{
			EntityState entityState = base.InstantiateNextState(skillSlot);
			Assaulter assaulter;
			if ((assaulter = (entityState as Assaulter)) != null)
			{
				assaulter.dashIndex = ((MercDashSkillDef.InstanceData)skillSlot.skillInstanceData).currentDashIndex;
			}
			return entityState;
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x0007C914 File Offset: 0x0007AB14
		public override void OnExecute([NotNull] GenericSkill skillSlot)
		{
			base.OnExecute(skillSlot);
			MercDashSkillDef.InstanceData instanceData = (MercDashSkillDef.InstanceData)skillSlot.skillInstanceData;
			if (!instanceData.hasExtraStock)
			{
				instanceData.currentDashIndex = 0;
			}
			instanceData.waitingForHit = true;
			instanceData.hasExtraStock = false;
			instanceData.timeoutTimer = this.timeoutDuration;
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x0007C960 File Offset: 0x0007AB60
		protected void AddHit([NotNull] GenericSkill skillSlot)
		{
			MercDashSkillDef.InstanceData instanceData = (MercDashSkillDef.InstanceData)skillSlot.skillInstanceData;
			if (instanceData.currentDashIndex < this.maxDashes - 1)
			{
				instanceData.currentDashIndex++;
				int stock = skillSlot.stock + 1;
				skillSlot.stock = stock;
				instanceData.hasExtraStock = true;
				return;
			}
			instanceData.currentDashIndex = 0;
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x0007C9B8 File Offset: 0x0007ABB8
		public override Sprite GetCurrentIcon([NotNull] GenericSkill skillSlot)
		{
			MercDashSkillDef.InstanceData instanceData = (MercDashSkillDef.InstanceData)skillSlot.skillInstanceData;
			int index = (instanceData != null) ? instanceData.currentDashIndex : 0;
			return HGArrayUtilities.GetSafe<Sprite>(this.icons, index);
		}

		// Token: 0x04001A26 RID: 6694
		public int maxDashes;

		// Token: 0x04001A27 RID: 6695
		public float timeoutDuration;

		// Token: 0x04001A28 RID: 6696
		public Sprite[] icons;

		// Token: 0x020004B9 RID: 1209
		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			// Token: 0x04001A29 RID: 6697
			public int currentDashIndex;

			// Token: 0x04001A2A RID: 6698
			public float timeoutTimer;

			// Token: 0x04001A2B RID: 6699
			public bool waitingForHit;

			// Token: 0x04001A2C RID: 6700
			public bool hasExtraStock;
		}
	}
}
