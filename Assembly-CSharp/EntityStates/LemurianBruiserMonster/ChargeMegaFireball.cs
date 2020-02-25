using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x020007ED RID: 2029
	public class ChargeMegaFireball : BaseState
	{
		// Token: 0x06002E27 RID: 11815 RVA: 0x000C4504 File Offset: 0x000C2704
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

		// Token: 0x06002E28 RID: 11816 RVA: 0x000C45F8 File Offset: 0x000C27F8
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x06002E29 RID: 11817 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002E2A RID: 11818 RVA: 0x000C4618 File Offset: 0x000C2818
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

		// Token: 0x06002E2B RID: 11819 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002B32 RID: 11058
		public static float baseDuration = 1f;

		// Token: 0x04002B33 RID: 11059
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002B34 RID: 11060
		public static string attackString;

		// Token: 0x04002B35 RID: 11061
		private float duration;

		// Token: 0x04002B36 RID: 11062
		private GameObject chargeInstance;
	}
}
