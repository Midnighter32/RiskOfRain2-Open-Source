using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianMonster
{
	// Token: 0x020007F3 RID: 2035
	public class ChargeFireball : BaseState
	{
		// Token: 0x06002E48 RID: 11848 RVA: 0x000C4FEC File Offset: 0x000C31EC
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
					if (transform && ChargeFireball.chargeVfxPrefab)
					{
						this.chargeVfxInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeFireball.chargeVfxPrefab, transform.position, transform.rotation);
						this.chargeVfxInstance.transform.parent = transform;
					}
				}
			}
			base.PlayAnimation("Gesture", "ChargeFireball", "ChargeFireball.playbackRate", this.duration);
		}

		// Token: 0x06002E49 RID: 11849 RVA: 0x000C50B5 File Offset: 0x000C32B5
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeVfxInstance)
			{
				EntityState.Destroy(this.chargeVfxInstance);
			}
		}

		// Token: 0x06002E4A RID: 11850 RVA: 0x000C50D8 File Offset: 0x000C32D8
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

		// Token: 0x06002E4B RID: 11851 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002B70 RID: 11120
		public static float baseDuration = 1f;

		// Token: 0x04002B71 RID: 11121
		public static GameObject chargeVfxPrefab;

		// Token: 0x04002B72 RID: 11122
		public static string attackString;

		// Token: 0x04002B73 RID: 11123
		private float duration;

		// Token: 0x04002B74 RID: 11124
		private GameObject chargeVfxInstance;
	}
}
