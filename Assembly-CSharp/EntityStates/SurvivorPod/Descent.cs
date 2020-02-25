using System;
using RoR2;
using UnityEngine;

namespace EntityStates.SurvivorPod
{
	// Token: 0x0200077A RID: 1914
	public class Descent : SurvivorPodBaseState
	{
		// Token: 0x06002C09 RID: 11273 RVA: 0x000BA124 File Offset: 0x000B8324
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

		// Token: 0x06002C0A RID: 11274 RVA: 0x000BA1F4 File Offset: 0x000B83F4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.AuthorityFixedUpdate();
			}
		}

		// Token: 0x06002C0B RID: 11275 RVA: 0x000BA20C File Offset: 0x000B840C
		protected void AuthorityFixedUpdate()
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Base");
				if (layerIndex != -1 && modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
				{
					this.TransitionIntoNextState();
				}
			}
		}

		// Token: 0x06002C0C RID: 11276 RVA: 0x000BA254 File Offset: 0x000B8454
		protected virtual void TransitionIntoNextState()
		{
			this.outer.SetNextState(new Landed());
		}

		// Token: 0x06002C0D RID: 11277 RVA: 0x000BA266 File Offset: 0x000B8466
		public override void OnExit()
		{
			EntityState.Destroy(this.shakeEmitter);
			base.OnExit();
		}

		// Token: 0x04002825 RID: 10277
		private ShakeEmitter shakeEmitter;
	}
}
