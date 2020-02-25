using System;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Loader
{
	// Token: 0x020007EB RID: 2027
	public class SwingComboFist : LoaderMeleeAttack, SteppedSkillDef.IStepSetter
	{
		// Token: 0x06002E1C RID: 11804 RVA: 0x000C4328 File Offset: 0x000C2528
		void SteppedSkillDef.IStepSetter.SetStep(int i)
		{
			this.gauntlet = i;
		}

		// Token: 0x06002E1D RID: 11805 RVA: 0x000C4334 File Offset: 0x000C2534
		protected override void PlayAnimation()
		{
			string animationStateName = (this.gauntlet == 0) ? "SwingFistRight" : "SwingFistLeft";
			float duration = Mathf.Max(this.duration, 0.2f);
			base.PlayCrossfade("Gesture, Additive", animationStateName, "SwingFist.playbackRate", duration, 0.1f);
			base.PlayCrossfade("Gesture, Override", animationStateName, "SwingFist.playbackRate", duration, 0.1f);
		}

		// Token: 0x06002E1E RID: 11806 RVA: 0x000C4395 File Offset: 0x000C2595
		protected override void BeginMeleeAttack()
		{
			this.swingEffectMuzzleString = ((this.gauntlet == 0) ? "SwingRight" : "SwingLeft");
			base.BeginMeleeAttack();
		}

		// Token: 0x06002E1F RID: 11807 RVA: 0x000C43B7 File Offset: 0x000C25B7
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.gauntlet);
		}

		// Token: 0x06002E20 RID: 11808 RVA: 0x000C43CD File Offset: 0x000C25CD
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.gauntlet = (int)reader.ReadByte();
		}

		// Token: 0x06002E21 RID: 11809 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002B2A RID: 11050
		public int gauntlet;
	}
}
