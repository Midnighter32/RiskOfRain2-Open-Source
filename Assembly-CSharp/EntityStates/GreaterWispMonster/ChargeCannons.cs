using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GreaterWispMonster
{
	// Token: 0x020000CC RID: 204
	internal class ChargeCannons : BaseState
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x00010488 File Offset: 0x0000E688
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlayScaledSound(ChargeCannons.attackString, base.gameObject, this.attackSpeedStat);
			this.duration = ChargeCannons.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Gesture");
				modelAnimator.SetFloat("ChargeCannons.playbackRate", 1f);
				modelAnimator.PlayInFixedTime("ChargeCannons", layerIndex, 0f);
				modelAnimator.Update(0f);
			}
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component && ChargeCannons.effectPrefab)
				{
					Transform transform = component.FindChild("MuzzleLeft");
					Transform transform2 = component.FindChild("MuzzleRight");
					if (transform)
					{
						this.chargeEffectLeft = UnityEngine.Object.Instantiate<GameObject>(ChargeCannons.effectPrefab, transform.position, transform.rotation);
						this.chargeEffectLeft.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeEffectLeft.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
					if (transform2)
					{
						this.chargeEffectRight = UnityEngine.Object.Instantiate<GameObject>(ChargeCannons.effectPrefab, transform2.position, transform2.rotation);
						this.chargeEffectRight.transform.parent = transform2;
						ScaleParticleSystemDuration component3 = this.chargeEffectRight.GetComponent<ScaleParticleSystemDuration>();
						if (component3)
						{
							component3.newDuration = this.duration;
						}
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001062E File Offset: 0x0000E82E
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0001064C File Offset: 0x0000E84C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireCannons nextState = new FireCannons();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003B8 RID: 952
		public static float baseDuration = 3f;

		// Token: 0x040003B9 RID: 953
		public static GameObject effectPrefab;

		// Token: 0x040003BA RID: 954
		public static string attackString;

		// Token: 0x040003BB RID: 955
		private float duration;

		// Token: 0x040003BC RID: 956
		private GameObject chargeEffectLeft;

		// Token: 0x040003BD RID: 957
		private GameObject chargeEffectRight;
	}
}
