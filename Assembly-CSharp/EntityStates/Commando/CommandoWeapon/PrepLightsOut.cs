using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C6 RID: 2246
	public class PrepLightsOut : BaseState
	{
		// Token: 0x0600325D RID: 12893 RVA: 0x000D9E24 File Offset: 0x000D8024
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

		// Token: 0x0600325E RID: 12894 RVA: 0x000D9F6C File Offset: 0x000D816C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireLightsOut());
				return;
			}
		}

		// Token: 0x0600325F RID: 12895 RVA: 0x000D9F9B File Offset: 0x000D819B
		public override void OnExit()
		{
			EntityState.Destroy(this.chargeEffect);
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x06003260 RID: 12896 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003152 RID: 12626
		public static float baseDuration = 3f;

		// Token: 0x04003153 RID: 12627
		public static GameObject chargePrefab;

		// Token: 0x04003154 RID: 12628
		public static GameObject specialCrosshairPrefab;

		// Token: 0x04003155 RID: 12629
		public static string prepSoundString;

		// Token: 0x04003156 RID: 12630
		private GameObject chargeEffect;

		// Token: 0x04003157 RID: 12631
		private float duration;

		// Token: 0x04003158 RID: 12632
		private ChildLocator childLocator;

		// Token: 0x04003159 RID: 12633
		private GameObject defaultCrosshairPrefab;
	}
}
