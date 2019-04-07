using System;
using RoR2;
using UnityEngine;

namespace EntityStates.SurvivorPod
{
	// Token: 0x020000EF RID: 239
	public class PreRelease : SurvivorPodBaseState
	{
		// Token: 0x06000497 RID: 1175 RVA: 0x000132D0 File Offset: 0x000114D0
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

		// Token: 0x06000498 RID: 1176 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00013330 File Offset: 0x00011530
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
