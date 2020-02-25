using System;
using RoR2;

namespace EntityStates
{
	// Token: 0x0200071F RID: 1823
	public class PrepFlower2 : BaseState
	{
		// Token: 0x06002A77 RID: 10871 RVA: 0x000B2B80 File Offset: 0x000B0D80
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepFlower2.baseDuration / this.attackSpeedStat;
			Util.PlaySound(PrepFlower2.enterSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "PrepFlower", "PrepFlower.playbackRate", this.duration);
		}

		// Token: 0x06002A78 RID: 10872 RVA: 0x000B2BD1 File Offset: 0x000B0DD1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(new FireFlower2());
			}
		}

		// Token: 0x06002A79 RID: 10873 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002652 RID: 9810
		public static float baseDuration;

		// Token: 0x04002653 RID: 9811
		public static string enterSoundString;

		// Token: 0x04002654 RID: 9812
		private float duration;
	}
}
