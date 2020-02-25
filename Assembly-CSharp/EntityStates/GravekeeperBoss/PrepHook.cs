using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GravekeeperBoss
{
	// Token: 0x0200084D RID: 2125
	public class PrepHook : BaseState
	{
		// Token: 0x06003010 RID: 12304 RVA: 0x000CE328 File Offset: 0x000CC528
		public override void OnEnter()
		{
			base.OnEnter();
			base.fixedAge = 0f;
			this.duration = PrepHook.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Body", "PrepHook", "PrepHook.playbackRate", this.duration, 0.5f);
				this.modelAnimator.GetComponent<AimAnimator>().enabled = true;
			}
			if (base.characterDirection)
			{
				base.characterDirection.moveVector = base.inputBank.aimDirection;
			}
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(PrepHook.muzzleString);
					if (transform && PrepHook.chargeEffectPrefab)
					{
						this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(PrepHook.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeInstance.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
				}
			}
			Util.PlayScaledSound(PrepHook.attackString, base.gameObject, this.attackSpeedStat);
		}

		// Token: 0x06003011 RID: 12305 RVA: 0x000CE467 File Offset: 0x000CC667
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireHook());
				return;
			}
		}

		// Token: 0x06003012 RID: 12306 RVA: 0x000CE496 File Offset: 0x000CC696
		public override void OnExit()
		{
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
			base.OnExit();
		}

		// Token: 0x06003013 RID: 12307 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002DFD RID: 11773
		public static float baseDuration = 3f;

		// Token: 0x04002DFE RID: 11774
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002DFF RID: 11775
		public static string muzzleString;

		// Token: 0x04002E00 RID: 11776
		public static string attackString;

		// Token: 0x04002E01 RID: 11777
		private float duration;

		// Token: 0x04002E02 RID: 11778
		private GameObject chargeInstance;

		// Token: 0x04002E03 RID: 11779
		private Animator modelAnimator;
	}
}
