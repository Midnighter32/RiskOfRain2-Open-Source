using System;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000146 RID: 326
	internal class ChargeSpines : BaseState
	{
		// Token: 0x06000641 RID: 1601 RVA: 0x0001D478 File Offset: 0x0001B678
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeSpines.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("MuzzleMouth");
					if (transform && ChargeSpines.effectPrefab)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ChargeSpines.effectPrefab, transform.position, transform.rotation);
						this.chargeEffect.transform.parent = transform;
					}
				}
			}
			base.PlayAnimation("Gesture", "ChargeSpines", "ChargeSpines.playbackRate", this.duration);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x0001D523 File Offset: 0x0001B723
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0001D544 File Offset: 0x0001B744
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireSpines nextState = new FireSpines();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400076D RID: 1901
		public static float baseDuration = 1f;

		// Token: 0x0400076E RID: 1902
		public static GameObject effectPrefab;

		// Token: 0x0400076F RID: 1903
		private float duration;

		// Token: 0x04000770 RID: 1904
		private GameObject chargeEffect;
	}
}
