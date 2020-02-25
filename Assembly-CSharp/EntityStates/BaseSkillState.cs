using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x020006FA RID: 1786
	public class BaseSkillState : BaseState
	{
		// Token: 0x0600297C RID: 10620 RVA: 0x000AECCC File Offset: 0x000ACECC
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			int num = -1;
			if (base.skillLocator != null)
			{
				num = base.skillLocator.GetSkillSlotIndex(this.activatorSkillSlot);
			}
			writer.Write((sbyte)num);
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x000AED04 File Offset: 0x000ACF04
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			int index = (int)reader.ReadSByte();
			if (base.skillLocator != null)
			{
				this.activatorSkillSlot = base.skillLocator.GetSkillAtIndex(index);
			}
		}

		// Token: 0x0600297E RID: 10622 RVA: 0x000AED3C File Offset: 0x000ACF3C
		public bool IsKeyDownAuthority()
		{
			if (base.skillLocator == null || this.activatorSkillSlot == null || base.inputBank == null)
			{
				return false;
			}
			switch (base.skillLocator.FindSkillSlot(this.activatorSkillSlot))
			{
			case SkillSlot.None:
				return false;
			case SkillSlot.Primary:
				return base.inputBank.skill1.down;
			case SkillSlot.Secondary:
				return base.inputBank.skill2.down;
			case SkillSlot.Utility:
				return base.inputBank.skill3.down;
			case SkillSlot.Special:
				return base.inputBank.skill4.down;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x04002586 RID: 9606
		public GenericSkill activatorSkillSlot;
	}
}
