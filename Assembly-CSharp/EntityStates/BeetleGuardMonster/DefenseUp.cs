using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020008F2 RID: 2290
	public class DefenseUp : BaseState
	{
		// Token: 0x06003331 RID: 13105 RVA: 0x000DE0C4 File Offset: 0x000DC2C4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = DefenseUp.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Body", "DefenseUp", "DefenseUp.playbackRate", this.duration, 0.2f);
			}
		}

		// Token: 0x06003332 RID: 13106 RVA: 0x000DE124 File Offset: 0x000DC324
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("DefenseUp.activate") > 0.5f && !this.hasCastBuff)
			{
				ScaleParticleSystemDuration component = UnityEngine.Object.Instantiate<GameObject>(DefenseUp.defenseUpPrefab, base.transform.position, Quaternion.identity, base.transform).GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = DefenseUp.buffDuration;
				}
				this.hasCastBuff = true;
				if (NetworkServer.active)
				{
					base.characterBody.AddTimedBuff(BuffIndex.EnrageAncientWisp, DefenseUp.buffDuration);
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003333 RID: 13107 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040032A5 RID: 12965
		public static float baseDuration = 3.5f;

		// Token: 0x040032A6 RID: 12966
		public static float buffDuration = 8f;

		// Token: 0x040032A7 RID: 12967
		public static GameObject defenseUpPrefab;

		// Token: 0x040032A8 RID: 12968
		private Animator modelAnimator;

		// Token: 0x040032A9 RID: 12969
		private float duration;

		// Token: 0x040032AA RID: 12970
		private bool hasCastBuff;
	}
}
