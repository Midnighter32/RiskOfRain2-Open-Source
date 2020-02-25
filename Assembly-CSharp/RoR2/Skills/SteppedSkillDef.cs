using System;
using EntityStates;
using JetBrains.Annotations;

namespace RoR2.Skills
{
	// Token: 0x020004C0 RID: 1216
	public class SteppedSkillDef : SkillDef
	{
		// Token: 0x06001D57 RID: 7511 RVA: 0x0007CFC8 File Offset: 0x0007B1C8
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new SteppedSkillDef.InstanceData();
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x0007CFD0 File Offset: 0x0007B1D0
		protected override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
		{
			EntityState entityState = base.InstantiateNextState(skillSlot);
			SteppedSkillDef.InstanceData instanceData = (SteppedSkillDef.InstanceData)skillSlot.skillInstanceData;
			SteppedSkillDef.IStepSetter stepSetter;
			if ((stepSetter = (entityState as SteppedSkillDef.IStepSetter)) != null)
			{
				stepSetter.SetStep(instanceData.step);
			}
			return entityState;
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x0007D008 File Offset: 0x0007B208
		public override void OnExecute([NotNull] GenericSkill skillSlot)
		{
			SteppedSkillDef.InstanceData instanceData = (SteppedSkillDef.InstanceData)skillSlot.skillInstanceData;
			if (base.IsAlreadyInState(skillSlot) || !this.resetStepsOnIdle)
			{
				instanceData.step++;
				if (instanceData.step >= this.stepCount)
				{
					instanceData.step = 0;
				}
			}
			else
			{
				instanceData.step = 0;
			}
			base.OnExecute(skillSlot);
		}

		// Token: 0x04001A52 RID: 6738
		public bool resetStepsOnIdle;

		// Token: 0x04001A53 RID: 6739
		public int stepCount = 2;

		// Token: 0x020004C1 RID: 1217
		public class InstanceData : SkillDef.BaseSkillInstanceData
		{
			// Token: 0x04001A54 RID: 6740
			public int step;
		}

		// Token: 0x020004C2 RID: 1218
		public interface IStepSetter
		{
			// Token: 0x06001D5C RID: 7516
			void SetStep(int i);
		}
	}
}
