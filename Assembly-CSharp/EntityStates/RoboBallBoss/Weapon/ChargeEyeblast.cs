using System;
using RoR2;
using UnityEngine;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x0200079D RID: 1949
	public class ChargeEyeblast : BaseState
	{
		// Token: 0x06002CA3 RID: 11427 RVA: 0x000BC598 File Offset: 0x000BA798
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeEyeblast.baseDuration / this.attackSpeedStat;
			UnityEngine.Object modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(ChargeEyeblast.attackString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(ChargeEyeblast.muzzleString);
					if (transform && ChargeEyeblast.chargeEffectPrefab)
					{
						this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeEyeblast.chargeEffectPrefab, transform.position, transform.rotation);
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
				base.PlayCrossfade("Gesture, Additive", "ChargeEyeBlast", "ChargeEyeBlast.playbackRate", this.duration, 0.1f);
			}
		}

		// Token: 0x06002CA4 RID: 11428 RVA: 0x000BC68C File Offset: 0x000BA88C
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x06002CA5 RID: 11429 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002CA6 RID: 11430 RVA: 0x000BC6AC File Offset: 0x000BA8AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(this.GetNextState());
				return;
			}
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x000BC6DC File Offset: 0x000BA8DC
		public virtual EntityState GetNextState()
		{
			return new FireEyeBlast();
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040028C3 RID: 10435
		public static float baseDuration = 1f;

		// Token: 0x040028C4 RID: 10436
		public static GameObject chargeEffectPrefab;

		// Token: 0x040028C5 RID: 10437
		public static string attackString;

		// Token: 0x040028C6 RID: 10438
		public static string muzzleString;

		// Token: 0x040028C7 RID: 10439
		private float duration;

		// Token: 0x040028C8 RID: 10440
		private GameObject chargeInstance;
	}
}
