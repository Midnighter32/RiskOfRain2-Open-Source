using System;
using RoR2;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020008F6 RID: 2294
	public class SpawnState : BaseState
	{
		// Token: 0x06003347 RID: 13127 RVA: 0x000DE7E4 File Offset: 0x000DC9E4
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn1", "Spawn1.playbackRate", SpawnState.duration);
		}

		// Token: 0x06003348 RID: 13128 RVA: 0x000DE817 File Offset: 0x000DCA17
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06003349 RID: 13129 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040032CB RID: 13003
		public static float duration = 4f;

		// Token: 0x040032CC RID: 13004
		public static string spawnSoundString;
	}
}
