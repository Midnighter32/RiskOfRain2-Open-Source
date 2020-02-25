using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008EC RID: 2284
	public class ChargeSpit : BaseState
	{
		// Token: 0x0600330D RID: 13069 RVA: 0x000DD464 File Offset: 0x000DB664
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

		// Token: 0x0600330E RID: 13070 RVA: 0x000DD545 File Offset: 0x000DB745
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffect);
		}

		// Token: 0x0600330F RID: 13071 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06003310 RID: 13072 RVA: 0x000DD558 File Offset: 0x000DB758
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

		// Token: 0x06003311 RID: 13073 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400326A RID: 12906
		public static float baseDuration = 3f;

		// Token: 0x0400326B RID: 12907
		public static GameObject effectPrefab;

		// Token: 0x0400326C RID: 12908
		public static string attackSoundString;

		// Token: 0x0400326D RID: 12909
		private float duration;

		// Token: 0x0400326E RID: 12910
		private GameObject chargeEffect;
	}
}
