using System;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000731 RID: 1841
	public class ChargeRHCannon : BaseState
	{
		// Token: 0x06002AC7 RID: 10951 RVA: 0x000B4260 File Offset: 0x000B2460
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeRHCannon.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayAnimation("Gesture", "ChargeRHCannon", "ChargeRHCannon.playbackRate", this.duration);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component && ChargeRHCannon.effectPrefab)
				{
					Transform transform = component.FindChild("MuzzleRight");
					if (transform)
					{
						this.chargeEffectRight = UnityEngine.Object.Instantiate<GameObject>(ChargeRHCannon.effectPrefab, transform.position, transform.rotation);
						this.chargeEffectRight.transform.parent = transform;
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x06002AC8 RID: 10952 RVA: 0x000B4329 File Offset: 0x000B2529
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		// Token: 0x06002AC9 RID: 10953 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002ACA RID: 10954 RVA: 0x000B4348 File Offset: 0x000B2548
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireRHCannon nextState = new FireRHCannon();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x06002ACB RID: 10955 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040026A6 RID: 9894
		public static float baseDuration = 3f;

		// Token: 0x040026A7 RID: 9895
		public static GameObject effectPrefab;

		// Token: 0x040026A8 RID: 9896
		private float duration;

		// Token: 0x040026A9 RID: 9897
		private GameObject chargeEffectLeft;

		// Token: 0x040026AA RID: 9898
		private GameObject chargeEffectRight;
	}
}
