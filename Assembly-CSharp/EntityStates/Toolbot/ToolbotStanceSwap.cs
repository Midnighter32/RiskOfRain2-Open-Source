using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x02000771 RID: 1905
	public class ToolbotStanceSwap : ToolbotStanceBase
	{
		// Token: 0x06002BE3 RID: 11235 RVA: 0x000B97C4 File Offset: 0x000B79C4
		public override void OnEnter()
		{
			base.OnEnter();
			float num = this.baseDuration / this.attackSpeedStat;
			this.endTime = Run.FixedTimeStamp.now + num;
			GenericSkill primarySkill = base.GetPrimarySkill1();
			GenericSkill primarySkill2 = base.GetPrimarySkill2();
			if (this.previousStanceState != typeof(ToolbotStanceA))
			{
				Util.Swap<GenericSkill>(ref primarySkill, ref primarySkill2);
			}
			ToolbotWeaponSkillDef toolbotWeaponSkillDef;
			if (primarySkill2 && (toolbotWeaponSkillDef = (primarySkill2.skillDef as ToolbotWeaponSkillDef)) != null)
			{
				base.SendWeaponStanceToAnimator(toolbotWeaponSkillDef);
				Util.PlaySound(toolbotWeaponSkillDef.entrySound, base.gameObject);
			}
			ToolbotWeaponSkillDef toolbotWeaponSkillDef2;
			if (primarySkill && (toolbotWeaponSkillDef2 = (primarySkill.skillDef as ToolbotWeaponSkillDef)) != null)
			{
				Debug.LogFormat("Leaving skill {0}", new object[]
				{
					toolbotWeaponSkillDef2
				});
				base.PlayAnimation("Stance, Additive", toolbotWeaponSkillDef2.exitAnimState, "StanceSwap.playbackRate", num * 0.5f);
			}
		}

		// Token: 0x06002BE4 RID: 11236 RVA: 0x000B989F File Offset: 0x000B7A9F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.endTime.hasPassed)
			{
				this.outer.SetNextState(EntityState.Instantiate(this.nextStanceState));
				return;
			}
		}

		// Token: 0x06002BE5 RID: 11237 RVA: 0x000B98D4 File Offset: 0x000B7AD4
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			short num = StateIndexTable.TypeToIndex(this.previousStanceState);
			short num2 = StateIndexTable.TypeToIndex(this.nextStanceState);
			writer.Write((ushort)num);
			writer.Write((ushort)num2);
		}

		// Token: 0x06002BE6 RID: 11238 RVA: 0x000B9910 File Offset: 0x000B7B10
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			short stateTypeIndex = reader.ReadInt16();
			short stateTypeIndex2 = reader.ReadInt16();
			this.previousStanceState = StateIndexTable.IndexToType(stateTypeIndex);
			this.nextStanceState = StateIndexTable.IndexToType(stateTypeIndex2);
		}

		// Token: 0x04002805 RID: 10245
		[SerializeField]
		private float baseDuration = 0.5f;

		// Token: 0x04002806 RID: 10246
		private Run.FixedTimeStamp endTime;

		// Token: 0x04002807 RID: 10247
		public Type nextStanceState;

		// Token: 0x04002808 RID: 10248
		public Type previousStanceState;
	}
}
