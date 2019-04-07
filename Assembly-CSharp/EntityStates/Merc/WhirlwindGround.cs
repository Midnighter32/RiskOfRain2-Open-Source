using System;

namespace EntityStates.Merc
{
	// Token: 0x0200010D RID: 269
	public class WhirlwindGround : WhirlwindBase
	{
		// Token: 0x0600052C RID: 1324 RVA: 0x00016C9B File Offset: 0x00014E9B
		protected override void PlayAnim()
		{
			base.PlayCrossfade("FullBody, Override", "WhirlwindGround", "Whirlwind.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00016CC0 File Offset: 0x00014EC0
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
