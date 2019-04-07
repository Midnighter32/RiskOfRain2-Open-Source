using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SurvivorPod
{
	// Token: 0x020000F0 RID: 240
	public class Release : SurvivorPodBaseState
	{
		// Token: 0x0600049B RID: 1179 RVA: 0x00013388 File Offset: 0x00011588
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Release");
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				component.FindChild("Door").gameObject.SetActive(false);
				component.FindChild("ReleaseExhaustFX").gameObject.SetActive(true);
			}
			if (!base.survivorPodController)
			{
				return;
			}
			if (Util.HasEffectiveAuthority(base.survivorPodController.characterBodyObject))
			{
				base.survivorPodController.characterStateMachine.SetNextStateToMain();
				TeleportHelper.TeleportGameObject(base.survivorPodController.characterBodyObject, base.survivorPodController.exitPosition.position);
				CharacterMotor component2 = base.survivorPodController.characterBodyObject.GetComponent<CharacterMotor>();
				if (component2)
				{
					component2.velocity = base.survivorPodController.exitPosition.forward * Release.ejectionSpeed;
				}
			}
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00013474 File Offset: 0x00011674
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && (!base.survivorPodController.characterStateMachine || !(base.survivorPodController.characterStateMachine.state is GenericCharacterPod)))
			{
				this.outer.SetNextState(new ReleaseFinished());
			}
		}

		// Token: 0x0400045A RID: 1114
		public static float ejectionSpeed = 20f;
	}
}
