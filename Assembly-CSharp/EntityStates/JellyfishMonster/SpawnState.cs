using System;
using RoR2;

namespace EntityStates.JellyfishMonster
{
	// Token: 0x0200080A RID: 2058
	public class SpawnState : BaseState
	{
		// Token: 0x06002EC6 RID: 11974 RVA: 0x000C71F7 File Offset: 0x000C53F7
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x06002EC7 RID: 11975 RVA: 0x000C722A File Offset: 0x000C542A
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002EC8 RID: 11976 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002C08 RID: 11272
		public static float duration = 4f;

		// Token: 0x04002C09 RID: 11273
		public static string spawnSoundString;
	}
}
