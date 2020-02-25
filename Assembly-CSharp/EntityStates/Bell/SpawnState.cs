using System;
using RoR2;

namespace EntityStates.Bell
{
	// Token: 0x020008E1 RID: 2273
	public class SpawnState : BaseState
	{
		// Token: 0x060032DD RID: 13021 RVA: 0x000DC7CF File Offset: 0x000DA9CF
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x060032DE RID: 13022 RVA: 0x000DC802 File Offset: 0x000DAA02
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060032DF RID: 13023 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400322D RID: 12845
		public static float duration = 4f;

		// Token: 0x0400322E RID: 12846
		public static string spawnSoundString;
	}
}
