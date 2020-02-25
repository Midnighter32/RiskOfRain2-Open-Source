using System;
using RoR2;

namespace EntityStates.Vulture
{
	// Token: 0x0200073C RID: 1852
	public class SpawnState : BaseState
	{
		// Token: 0x06002AFF RID: 11007 RVA: 0x000B512D File Offset: 0x000B332D
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x06002B00 RID: 11008 RVA: 0x000B5160 File Offset: 0x000B3360
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040026D9 RID: 9945
		public static float duration = 4f;

		// Token: 0x040026DA RID: 9946
		public static string spawnSoundString;
	}
}
