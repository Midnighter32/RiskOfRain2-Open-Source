using System;
using RoR2;

namespace EntityStates.JellyfishMonster
{
	// Token: 0x02000134 RID: 308
	public class SpawnState : BaseState
	{
		// Token: 0x060005EF RID: 1519 RVA: 0x0001B546 File Offset: 0x00019746
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0001B579 File Offset: 0x00019779
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040006E4 RID: 1764
		public static float duration = 4f;

		// Token: 0x040006E5 RID: 1765
		public static string spawnSoundString;
	}
}
