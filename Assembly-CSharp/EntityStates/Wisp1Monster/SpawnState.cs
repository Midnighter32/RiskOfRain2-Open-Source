using System;
using RoR2;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x02000727 RID: 1831
	public class SpawnState : BaseState
	{
		// Token: 0x06002A98 RID: 10904 RVA: 0x000B33FE File Offset: 0x000B15FE
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x06002A99 RID: 10905 RVA: 0x000B3431 File Offset: 0x000B1631
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002A9A RID: 10906 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400267A RID: 9850
		public static float duration = 4f;

		// Token: 0x0400267B RID: 9851
		public static string spawnSoundString;
	}
}
