using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020001D7 RID: 471
	public class DefenseUp : BaseState
	{
		// Token: 0x06000931 RID: 2353 RVA: 0x0002E458 File Offset: 0x0002C658
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

		// Token: 0x06000932 RID: 2354 RVA: 0x0002E4B8 File Offset: 0x0002C6B8
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

		// Token: 0x06000933 RID: 2355 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000C7D RID: 3197
		public static float baseDuration = 3.5f;

		// Token: 0x04000C7E RID: 3198
		public static float buffDuration = 8f;

		// Token: 0x04000C7F RID: 3199
		public static GameObject defenseUpPrefab;

		// Token: 0x04000C80 RID: 3200
		private Animator modelAnimator;

		// Token: 0x04000C81 RID: 3201
		private float duration;

		// Token: 0x04000C82 RID: 3202
		private bool hasCastBuff;
	}
}
