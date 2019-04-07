using System;
using RoR2;

namespace EntityStates.Bell
{
	// Token: 0x020001C6 RID: 454
	public class SpawnState : BaseState
	{
		// Token: 0x060008DD RID: 2269 RVA: 0x0002CB59 File Offset: 0x0002AD59
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0002CB8C File Offset: 0x0002AD8C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000C05 RID: 3077
		public static float duration = 4f;

		// Token: 0x04000C06 RID: 3078
		public static string spawnSoundString;
	}
}
