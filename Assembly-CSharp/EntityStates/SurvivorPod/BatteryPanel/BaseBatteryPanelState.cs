using System;
using RoR2;
using UnityEngine;

namespace EntityStates.SurvivorPod.BatteryPanel
{
	// Token: 0x02000780 RID: 1920
	public class BaseBatteryPanelState : BaseState
	{
		// Token: 0x06002C23 RID: 11299 RVA: 0x000BA59F File Offset: 0x000B879F
		public override void OnEnter()
		{
			base.OnEnter();
			VehicleSeat componentInParent = base.gameObject.GetComponentInParent<VehicleSeat>();
			this.SetPodObject((componentInParent != null) ? componentInParent.gameObject : null);
		}

		// Token: 0x06002C24 RID: 11300 RVA: 0x000BA5C4 File Offset: 0x000B87C4
		private void SetPodObject(GameObject podObject)
		{
			this.podInfo = default(BaseBatteryPanelState.PodInfo);
			if (!podObject)
			{
				return;
			}
			this.podInfo.podObject = podObject;
			ModelLocator component = podObject.GetComponent<ModelLocator>();
			if (component)
			{
				Transform modelTransform = component.modelTransform;
				if (modelTransform)
				{
					this.podInfo.podAnimator = modelTransform.GetComponent<Animator>();
				}
			}
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x000BA621 File Offset: 0x000B8821
		protected void PlayPodAnimation(string layerName, string animationStateName, string playbackRateParam, float duration)
		{
			if (this.podInfo.podAnimator)
			{
				EntityState.PlayAnimationOnAnimator(this.podInfo.podAnimator, layerName, animationStateName, playbackRateParam, duration);
			}
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x000BA64A File Offset: 0x000B884A
		protected void PlayPodAnimation(string layerName, string animationStateName)
		{
			if (this.podInfo.podAnimator)
			{
				EntityState.PlayAnimationOnAnimator(this.podInfo.podAnimator, layerName, animationStateName);
			}
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x000BA670 File Offset: 0x000B8870
		protected void EnablePickup()
		{
			ChildLocator component = this.podInfo.podAnimator.GetComponent<ChildLocator>();
			if (!component)
			{
				Debug.Log("Could not find pod child locator.");
				return;
			}
			Transform transform = component.FindChild("BatteryAttachmentPoint");
			if (!transform)
			{
				Debug.Log("Could not find battery attachment point.");
				return;
			}
			Transform transform2 = transform.Find("QuestVolatileBatteryWorldPickup(Clone)");
			if (!transform2)
			{
				Debug.Log("Could not find battery transform");
				return;
			}
			GenericPickupController component2 = transform2.GetComponent<GenericPickupController>();
			if (component2)
			{
				component2.enabled = true;
				return;
			}
			Debug.Log("Could not find pickup controller.");
		}

		// Token: 0x04002829 RID: 10281
		protected BaseBatteryPanelState.PodInfo podInfo;

		// Token: 0x02000781 RID: 1921
		protected struct PodInfo
		{
			// Token: 0x0400282A RID: 10282
			public GameObject podObject;

			// Token: 0x0400282B RID: 10283
			public Animator podAnimator;
		}
	}
}
