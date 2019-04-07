using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D1 RID: 465
	internal class ChargeSpit : BaseState
	{
		// Token: 0x0600090D RID: 2317 RVA: 0x0002D7FC File Offset: 0x0002B9FC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeSpit.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayCrossfade("Gesture", "ChargeSpit", "ChargeSpit.playbackRate", this.duration, 0.2f);
			Util.PlaySound(ChargeSpit.attackSoundString, base.gameObject);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component && ChargeSpit.effectPrefab)
				{
					Transform transform = component.FindChild("Mouth");
					if (transform)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ChargeSpit.effectPrefab, transform.position, transform.rotation);
						this.chargeEffect.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeEffect.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
				}
			}
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0002D8DD File Offset: 0x0002BADD
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffect);
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x0002D8F0 File Offset: 0x0002BAF0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.characterDirection)
			{
				base.characterDirection.moveVector = base.GetAimRay().direction;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireSpit nextState = new FireSpit();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000C42 RID: 3138
		public static float baseDuration = 3f;

		// Token: 0x04000C43 RID: 3139
		public static GameObject effectPrefab;

		// Token: 0x04000C44 RID: 3140
		public static string attackSoundString;

		// Token: 0x04000C45 RID: 3141
		private float duration;

		// Token: 0x04000C46 RID: 3142
		private GameObject chargeEffect;
	}
}
