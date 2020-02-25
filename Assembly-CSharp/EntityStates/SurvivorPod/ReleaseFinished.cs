using System;

namespace EntityStates.SurvivorPod
{
	// Token: 0x0200077E RID: 1918
	public class ReleaseFinished : SurvivorPodBaseState
	{
		// Token: 0x06002C1B RID: 11291 RVA: 0x000BA50C File Offset: 0x000B870C
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Release");
		}
	}
}
