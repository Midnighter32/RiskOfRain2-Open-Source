using System;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x020000C9 RID: 201
	public class FireState : BaseState
	{
		// Token: 0x060003EA RID: 1002 RVA: 0x000102C8 File Offset: 0x0000E4C8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireState.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			base.characterMotor.muteWalkMotion = true;
			base.PlayCrossfade("Body", "Fire", "Fire.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00010328 File Offset: 0x0000E528
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Vector3 zero = Vector3.zero;
			Vector3 position = base.transform.position;
			if (base.hasAuthority && this.modelAnimator)
			{
				this.modelAnimator.GetFloat("Fire.begin");
			}
			if (base.fixedAge >= this.duration && base.hasAuthority)
			{
				this.outer.SetNextState(new MainState());
				return;
			}
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x000103A0 File Offset: 0x0000E5A0
		public override void OnExit()
		{
			base.OnExit();
			base.characterMotor.muteWalkMotion = false;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040003AE RID: 942
		public static float baseDuration = 3.5f;

		// Token: 0x040003AF RID: 943
		public static float damage = 10f;

		// Token: 0x040003B0 RID: 944
		public static float forceMagnitude = 16f;

		// Token: 0x040003B1 RID: 945
		public static GameObject projectilePrefab;

		// Token: 0x040003B2 RID: 946
		private Animator modelAnimator;

		// Token: 0x040003B3 RID: 947
		private RootMotionAccumulator modelRootMotionAccumulator;

		// Token: 0x040003B4 RID: 948
		private float duration;
	}
}
