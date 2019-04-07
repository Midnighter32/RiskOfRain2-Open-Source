using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x0200011E RID: 286
	internal class ChargeMegaFireball : BaseState
	{
		// Token: 0x06000581 RID: 1409 RVA: 0x00019200 File Offset: 0x00017400
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeMegaFireball.baseDuration / this.attackSpeedStat;
			UnityEngine.Object modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(ChargeMegaFireball.attackString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("MuzzleMouth");
					if (transform && ChargeMegaFireball.chargeEffectPrefab)
					{
						this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaFireball.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeInstance.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
				}
			}
			if (modelAnimator)
			{
				base.PlayCrossfade("Gesture, Additive", "ChargeMegaFireball", "ChargeMegaFireball.playbackRate", this.duration, 0.1f);
			}
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x000192F4 File Offset: 0x000174F4
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00019314 File Offset: 0x00017514
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireMegaFireball nextState = new FireMegaFireball();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000631 RID: 1585
		public static float baseDuration = 1f;

		// Token: 0x04000632 RID: 1586
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000633 RID: 1587
		public static string attackString;

		// Token: 0x04000634 RID: 1588
		private float duration;

		// Token: 0x04000635 RID: 1589
		private GameObject chargeInstance;
	}
}
