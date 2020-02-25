using System;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x02000725 RID: 1829
	public class FireState : BaseState
	{
		// Token: 0x06002A8F RID: 10895 RVA: 0x000B32A8 File Offset: 0x000B14A8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireState.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			base.characterMotor.muteWalkMotion = true;
			base.PlayCrossfade("Body", "Fire", "Fire.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x06002A90 RID: 10896 RVA: 0x000B3308 File Offset: 0x000B1508
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Vector3 zero = Vector3.zero;
			Vector3 position = base.transform.position;
			if (base.isAuthority && this.modelAnimator)
			{
				this.modelAnimator.GetFloat("Fire.begin");
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new MainState());
				return;
			}
		}

		// Token: 0x06002A91 RID: 10897 RVA: 0x000B3380 File Offset: 0x000B1580
		public override void OnExit()
		{
			base.OnExit();
			base.characterMotor.muteWalkMotion = false;
		}

		// Token: 0x06002A92 RID: 10898 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002672 RID: 9842
		public static float baseDuration = 3.5f;

		// Token: 0x04002673 RID: 9843
		public static float damage = 10f;

		// Token: 0x04002674 RID: 9844
		public static float forceMagnitude = 16f;

		// Token: 0x04002675 RID: 9845
		public static GameObject projectilePrefab;

		// Token: 0x04002676 RID: 9846
		private Animator modelAnimator;

		// Token: 0x04002677 RID: 9847
		private RootMotionAccumulator modelRootMotionAccumulator;

		// Token: 0x04002678 RID: 9848
		private float duration;
	}
}
