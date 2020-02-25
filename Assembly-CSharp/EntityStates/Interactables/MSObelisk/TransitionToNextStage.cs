using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.Interactables.MSObelisk
{
	// Token: 0x02000810 RID: 2064
	public class TransitionToNextStage : BaseState
	{
		// Token: 0x06002EDF RID: 11999 RVA: 0x000C7979 File Offset: 0x000C5B79
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge >= TransitionToNextStage.duration)
			{
				Stage.instance.BeginAdvanceStage(SceneCatalog.GetSceneDefFromSceneName("limbo"));
				this.outer.SetNextState(new Idle());
			}
		}

		// Token: 0x04002C2B RID: 11307
		public static float duration;
	}
}
