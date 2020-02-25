using System;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000822 RID: 2082
	public class ChargeSpines : BaseState
	{
		// Token: 0x06002F2E RID: 12078 RVA: 0x000C97FC File Offset: 0x000C79FC
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

		// Token: 0x06002F2F RID: 12079 RVA: 0x000C98A7 File Offset: 0x000C7AA7
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
		}

		// Token: 0x06002F30 RID: 12080 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002F31 RID: 12081 RVA: 0x000C98C8 File Offset: 0x000C7AC8
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

		// Token: 0x06002F32 RID: 12082 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002CAD RID: 11437
		public static float baseDuration = 1f;

		// Token: 0x04002CAE RID: 11438
		public static GameObject effectPrefab;

		// Token: 0x04002CAF RID: 11439
		private float duration;

		// Token: 0x04002CB0 RID: 11440
		private GameObject chargeEffect;
	}
}
