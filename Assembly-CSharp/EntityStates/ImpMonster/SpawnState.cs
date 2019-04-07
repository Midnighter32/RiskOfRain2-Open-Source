using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x0200014B RID: 331
	public class SpawnState : BaseState
	{
		// Token: 0x0600065B RID: 1627 RVA: 0x0001DB6C File Offset: 0x0001BD6C
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "Base", false);
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0001DBD1 File Offset: 0x0001BDD1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000791 RID: 1937
		private float stopwatch;

		// Token: 0x04000792 RID: 1938
		public static float duration = 4f;

		// Token: 0x04000793 RID: 1939
		public static string spawnSoundString;

		// Token: 0x04000794 RID: 1940
		public static GameObject spawnEffectPrefab;
	}
}
