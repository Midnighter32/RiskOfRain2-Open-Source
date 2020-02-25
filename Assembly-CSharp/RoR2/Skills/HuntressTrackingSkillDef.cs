using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Skills
{
	// Token: 0x020004B4 RID: 1204
	[CreateAssetMenu(menuName = "RoR2/SkillDef/HuntressTrackingSkillDef")]
	public class HuntressTrackingSkillDef : SkillDef
	{
		// Token: 0x06001D14 RID: 7444 RVA: 0x0007C72A File Offset: 0x0007A92A
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new HuntressTrackingSkillDef.InstanceData
			{
				huntressTracker = skillSlot.GetComponent<HuntressTracker>()
			};
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x0007C73D File Offset: 0x0007A93D
		private static bool HasTarget([NotNull] GenericSkill skillSlot)
		{
			HuntressTracker huntressTracker = ((HuntressTrackingSkillDef.InstanceData)skillSlot.skillInstanceData).huntressTracker;
			return (huntressTracker != null) ? huntressTracker.GetTrackingTarget() : null;
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x0007C765 File Offset: 0x0007A965
		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return HuntressTrackingSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x0007C778 File Offset: 0x0007A978
		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && HuntressTrackingSkillDef.HasTarget(skillSlot);
		}

		// Token: 0x020004B5 RID: 1205
		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			// Token: 0x04001A24 RID: 6692
			public HuntressTracker huntressTracker;
		}
	}
}
