using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianMonster
{
	// Token: 0x02000124 RID: 292
	internal class ChargeFireball : BaseState
	{
		// Token: 0x060005A2 RID: 1442 RVA: 0x00019CF4 File Offset: 0x00017EF4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeFireball.baseDuration / this.attackSpeedStat;
			base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(ChargeFireball.attackString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("MuzzleMouth");
					if (transform && ChargeFireball.effectPrefab)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ChargeFireball.effectPrefab, transform.position, transform.rotation);
						this.chargeEffect.transform.parent = transform;
					}
				}
			}
			base.PlayAnimation("Gesture", "ChargeFireball", "ChargeFireball.playbackRate", this.duration);
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x00019DBD File Offset: 0x00017FBD
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00019DE0 File Offset: 0x00017FE0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireFireball nextState = new FireFireball();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400066F RID: 1647
		public static float baseDuration = 1f;

		// Token: 0x04000670 RID: 1648
		public static GameObject effectPrefab;

		// Token: 0x04000671 RID: 1649
		public static string attackString;

		// Token: 0x04000672 RID: 1650
		private float duration;

		// Token: 0x04000673 RID: 1651
		private GameObject chargeEffect;
	}
}
