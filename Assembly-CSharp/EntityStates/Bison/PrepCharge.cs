using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Bison
{
	// Token: 0x020008DF RID: 2271
	public class PrepCharge : BaseState
	{
		// Token: 0x060032D4 RID: 13012 RVA: 0x000DC534 File Offset: 0x000DA734
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

		// Token: 0x060032D5 RID: 13013 RVA: 0x000DC66D File Offset: 0x000DA86D
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
		}

		// Token: 0x060032D6 RID: 13014 RVA: 0x000DC690 File Offset: 0x000DA890
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

		// Token: 0x060032D7 RID: 13015 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003223 RID: 12835
		public static float basePrepDuration;

		// Token: 0x04003224 RID: 12836
		public static string enterSoundString;

		// Token: 0x04003225 RID: 12837
		public static GameObject chargeEffectPrefab;

		// Token: 0x04003226 RID: 12838
		private float stopwatch;

		// Token: 0x04003227 RID: 12839
		private float prepDuration;

		// Token: 0x04003228 RID: 12840
		private GameObject chargeEffectInstance;
	}
}
