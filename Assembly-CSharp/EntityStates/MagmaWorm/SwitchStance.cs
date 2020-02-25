using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.MagmaWorm
{
	// Token: 0x02000818 RID: 2072
	public class SwitchStance : BaseState
	{
		// Token: 0x06002EF5 RID: 12021 RVA: 0x000C7F96 File Offset: 0x000C6196
		public override void OnEnter()
		{
			base.OnEnter();
			this.SetStanceParameters(true);
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x000C7FA5 File Offset: 0x000C61A5
		public override void OnExit()
		{
			base.OnExit();
			this.SetStanceParameters(false);
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x000C7FB4 File Offset: 0x000C61B4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SwitchStance.leapingDuration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002EF8 RID: 12024 RVA: 0x000C7FD4 File Offset: 0x000C61D4
		private void SetStanceParameters(bool leaping)
		{
			if (NetworkServer.active)
			{
				WormBodyPositions2 component = base.GetComponent<WormBodyPositions2>();
				if (!component)
				{
					return;
				}
				if (leaping)
				{
					component.ySpringConstant = SwitchStance.leapStanceSpring;
					component.yDamperConstant = SwitchStance.leapStanceDamping;
					component.speedMultiplier = SwitchStance.leapStanceSpeedMultiplier;
					component.allowShoving = false;
				}
				else
				{
					component.ySpringConstant = SwitchStance.groundStanceSpring;
					component.yDamperConstant = SwitchStance.groundStanceDamping;
					component.speedMultiplier = SwitchStance.groundStanceSpeedMultiplier;
					component.allowShoving = true;
				}
				component.shouldFireMeatballsOnImpact = leaping;
			}
		}

		// Token: 0x04002C40 RID: 11328
		public static float leapingDuration = 10f;

		// Token: 0x04002C41 RID: 11329
		public static float groundStanceSpring = 3f;

		// Token: 0x04002C42 RID: 11330
		public static float groundStanceDamping = 3f;

		// Token: 0x04002C43 RID: 11331
		public static float groundStanceSpeedMultiplier = 1.5f;

		// Token: 0x04002C44 RID: 11332
		public static float leapStanceSpring = 3f;

		// Token: 0x04002C45 RID: 11333
		public static float leapStanceDamping = 1f;

		// Token: 0x04002C46 RID: 11334
		public static float leapStanceSpeedMultiplier = 1f;
	}
}
