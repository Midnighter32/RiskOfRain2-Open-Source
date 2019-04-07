using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001B1 RID: 433
	internal class PrepLightsOut : BaseState
	{
		// Token: 0x0600087B RID: 2171 RVA: 0x0002A8A4 File Offset: 0x00028AA4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepLightsOut.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
			base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
			Util.PlaySound(PrepLightsOut.prepSoundString, base.gameObject);
			this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
			base.characterBody.crosshairPrefab = PrepLightsOut.specialCrosshairPrefab;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				if (this.childLocator)
				{
					Transform transform = this.childLocator.FindChild("MuzzlePistol");
					if (transform && PrepLightsOut.chargePrefab)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(PrepLightsOut.chargePrefab, transform.position, transform.rotation);
						this.chargeEffect.transform.parent = transform;
						ScaleParticleSystemDuration component = this.chargeEffect.GetComponent<ScaleParticleSystemDuration>();
						if (component)
						{
							component.newDuration = this.duration;
						}
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x0002A9EC File Offset: 0x00028BEC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireLightsOut());
				return;
			}
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x0002AA1B File Offset: 0x00028C1B
		public override void OnExit()
		{
			EntityState.Destroy(this.chargeEffect);
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000B51 RID: 2897
		public static float baseDuration = 3f;

		// Token: 0x04000B52 RID: 2898
		public static GameObject chargePrefab;

		// Token: 0x04000B53 RID: 2899
		public static GameObject specialCrosshairPrefab;

		// Token: 0x04000B54 RID: 2900
		public static string prepSoundString;

		// Token: 0x04000B55 RID: 2901
		private GameObject chargeEffect;

		// Token: 0x04000B56 RID: 2902
		private float duration;

		// Token: 0x04000B57 RID: 2903
		private ChildLocator childLocator;

		// Token: 0x04000B58 RID: 2904
		private GameObject defaultCrosshairPrefab;
	}
}
