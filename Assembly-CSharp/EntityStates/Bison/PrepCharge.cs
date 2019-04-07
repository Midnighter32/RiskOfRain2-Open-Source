using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Bison
{
	// Token: 0x020001C4 RID: 452
	public class PrepCharge : BaseState
	{
		// Token: 0x060008D5 RID: 2261 RVA: 0x0002C8D8 File Offset: 0x0002AAD8
		public override void OnEnter()
		{
			base.OnEnter();
			this.prepDuration = PrepCharge.basePrepDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.prepDuration);
			base.PlayCrossfade("Body", "PrepCharge", "PrepCharge.playbackRate", this.prepDuration, 0.2f);
			Util.PlaySound(PrepCharge.enterSoundString, base.gameObject);
			Transform modelTransform = base.GetModelTransform();
			AimAnimator component = modelTransform.GetComponent<AimAnimator>();
			if (component)
			{
				component.enabled = true;
			}
			if (base.characterDirection)
			{
				base.characterDirection.moveVector = base.GetAimRay().direction;
			}
			if (modelTransform)
			{
				ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					Transform transform = component2.FindChild("ChargeIndicator");
					if (transform && PrepCharge.chargeEffectPrefab)
					{
						this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(PrepCharge.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeEffectInstance.transform.parent = transform;
						ScaleParticleSystemDuration component3 = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
						if (component3)
						{
							component3.newDuration = this.prepDuration;
						}
					}
				}
			}
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x0002CA11 File Offset: 0x0002AC11
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0002CA34 File Offset: 0x0002AC34
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > this.prepDuration && base.isAuthority)
			{
				this.outer.SetNextState(new Charge());
				return;
			}
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000BFA RID: 3066
		public static float basePrepDuration;

		// Token: 0x04000BFB RID: 3067
		public static string enterSoundString;

		// Token: 0x04000BFC RID: 3068
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000BFD RID: 3069
		private float stopwatch;

		// Token: 0x04000BFE RID: 3070
		private float prepDuration;

		// Token: 0x04000BFF RID: 3071
		private GameObject chargeEffectInstance;
	}
}
