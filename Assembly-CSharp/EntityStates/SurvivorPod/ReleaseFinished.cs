using System;

namespace EntityStates.SurvivorPod
{
	// Token: 0x020000F1 RID: 241
	public class ReleaseFinished : SurvivorPodBaseState
	{
		// Token: 0x0600049F RID: 1183 RVA: 0x000134D9 File Offset: 0x000116D9
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Release");
			if (base.survivorPodController)
			{
				base.survivorPodController.NetworkcharacterBodyObject = null;
			}
		}
	}
}
