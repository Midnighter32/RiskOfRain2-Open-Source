using System;
using RoR2;
using UnityEngine;

namespace EntityStates.SurvivorPod
{
	// Token: 0x0200077C RID: 1916
	public class PreRelease : SurvivorPodBaseState
	{
		// Token: 0x06002C13 RID: 11283 RVA: 0x000BA348 File Offset: 0x000B8548
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "IdleToRelease");
			Util.PlaySound("Play_UI_podBlastDoorOpen", base.gameObject);
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				modelTransform.GetComponent<ChildLocator>().FindChild("InitialExhaustFX").gameObject.SetActive(true);
			}
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x000BA3A8 File Offset: 0x000B85A8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Base");
				if (layerIndex != -1 && modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("IdleToReleaseFinished"))
				{
					this.outer.SetNextState(new Release());
				}
			}
		}
	}
}
