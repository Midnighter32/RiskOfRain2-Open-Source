using System;
using RoR2;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x020000CB RID: 203
	public class SpawnState : BaseState
	{
		// Token: 0x060003F3 RID: 1011 RVA: 0x0001041E File Offset: 0x0000E61E
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00010451 File Offset: 0x0000E651
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040003B6 RID: 950
		public static float duration = 4f;

		// Token: 0x040003B7 RID: 951
		public static string spawnSoundString;
	}
}
