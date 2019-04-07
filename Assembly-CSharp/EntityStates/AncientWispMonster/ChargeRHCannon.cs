using System;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D2 RID: 210
	internal class ChargeRHCannon : BaseState
	{
		// Token: 0x0600041E RID: 1054 RVA: 0x00011214 File Offset: 0x0000F414
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

		// Token: 0x0600041F RID: 1055 RVA: 0x000112DD File Offset: 0x0000F4DD
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x000112FC File Offset: 0x0000F4FC
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

		// Token: 0x06000422 RID: 1058 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003E0 RID: 992
		public static float baseDuration = 3f;

		// Token: 0x040003E1 RID: 993
		public static GameObject effectPrefab;

		// Token: 0x040003E2 RID: 994
		private float duration;

		// Token: 0x040003E3 RID: 995
		private GameObject chargeEffectLeft;

		// Token: 0x040003E4 RID: 996
		private GameObject chargeEffectRight;
	}
}
