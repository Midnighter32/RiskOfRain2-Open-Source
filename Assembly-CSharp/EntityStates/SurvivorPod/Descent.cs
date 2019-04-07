using System;
using RoR2;
using UnityEngine;

namespace EntityStates.SurvivorPod
{
	// Token: 0x020000ED RID: 237
	internal class Descent : SurvivorPodBaseState
	{
		// Token: 0x06000490 RID: 1168 RVA: 0x000130D4 File Offset: 0x000112D4
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "InitialSpawn");
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("Travel");
					if (transform)
					{
						this.shakeEmitter = transform.gameObject.AddComponent<ShakeEmitter>();
						this.shakeEmitter.wave = new Wave
						{
							amplitude = 1f,
							frequency = 180f,
							cycleOffset = 0f
						};
						this.shakeEmitter.duration = 10000f;
						this.shakeEmitter.radius = 400f;
						this.shakeEmitter.amplitudeTimeDecay = false;
					}
				}
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x000131A4 File Offset: 0x000113A4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Base");
				if (layerIndex != -1 && modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
				{
					this.outer.SetNextState(new Landed());
				}
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x000131FC File Offset: 0x000113FC
		public override void OnExit()
		{
			EntityState.Destroy(this.shakeEmitter);
			base.OnExit();
		}

		// Token: 0x04000458 RID: 1112
		private ShakeEmitter shakeEmitter;
	}
}
