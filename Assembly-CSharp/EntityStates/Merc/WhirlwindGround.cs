using System;

namespace EntityStates.Merc
{
	// Token: 0x020007CB RID: 1995
	public class WhirlwindGround : WhirlwindBase
	{
		// Token: 0x06002D81 RID: 11649 RVA: 0x000C0FE7 File Offset: 0x000BF1E7
		protected override void PlayAnim()
		{
			base.PlayCrossfade("FullBody, Override", "WhirlwindGround", "Whirlwind.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x06002D82 RID: 11650 RVA: 0x000C100C File Offset: 0x000BF20C
		public override void OnExit()
		{
			base.OnExit();
			int layerIndex = this.animator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				this.animator.SetLayerWeight(layerIndex, 3f);
				base.PlayAnimation("Impact", "LightImpact");
			}
		}
	}
}
