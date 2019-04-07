using System;
using RoR2;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020001DB RID: 475
	public class SpawnState : BaseState
	{
		// Token: 0x06000947 RID: 2375 RVA: 0x0002EBBE File Offset: 0x0002CDBE
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn1", "Spawn1.playbackRate", SpawnState.duration);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0002EBF1 File Offset: 0x0002CDF1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000CA2 RID: 3234
		public static float duration = 4f;

		// Token: 0x04000CA3 RID: 3235
		public static string spawnSoundString;
	}
}
