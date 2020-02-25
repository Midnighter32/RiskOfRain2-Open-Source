using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GreaterWispMonster
{
	// Token: 0x0200072B RID: 1835
	public class ChargeCannons : BaseState
	{
		// Token: 0x06002AA1 RID: 10913 RVA: 0x000B34BC File Offset: 0x000B16BC
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlayScaledSound(this.attackString, base.gameObject, this.attackSpeedStat * (2f / this.baseDuration));
			this.duration = this.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.GetModelAnimator();
			base.PlayAnimation("Gesture", "ChargeCannons", "ChargeCannons.playbackRate", this.duration);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component && this.effectPrefab)
				{
					Transform transform = component.FindChild("MuzzleLeft");
					Transform transform2 = component.FindChild("MuzzleRight");
					if (transform)
					{
						this.chargeEffectLeft = UnityEngine.Object.Instantiate<GameObject>(this.effectPrefab, transform.position, transform.rotation);
						this.chargeEffectLeft.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeEffectLeft.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
					if (transform2)
					{
						this.chargeEffectRight = UnityEngine.Object.Instantiate<GameObject>(this.effectPrefab, transform2.position, transform2.rotation);
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

		// Token: 0x06002AA2 RID: 10914 RVA: 0x000B3645 File Offset: 0x000B1845
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		// Token: 0x06002AA3 RID: 10915 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002AA4 RID: 10916 RVA: 0x000B3664 File Offset: 0x000B1864
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

		// Token: 0x06002AA5 RID: 10917 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400267C RID: 9852
		[SerializeField]
		public float baseDuration = 3f;

		// Token: 0x0400267D RID: 9853
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x0400267E RID: 9854
		[SerializeField]
		public string attackString;

		// Token: 0x0400267F RID: 9855
		protected float duration;

		// Token: 0x04002680 RID: 9856
		private GameObject chargeEffectLeft;

		// Token: 0x04002681 RID: 9857
		private GameObject chargeEffectRight;

		// Token: 0x04002682 RID: 9858
		private const float soundDuration = 2f;
	}
}
