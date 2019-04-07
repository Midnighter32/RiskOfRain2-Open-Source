using System;
using RoR2;
using UnityEngine;

namespace EntityStates.NewtMonster
{
	// Token: 0x02000100 RID: 256
	public class SpawnState : EntityState
	{
		// Token: 0x060004F3 RID: 1267 RVA: 0x00014F24 File Offset: 0x00013124
		public override void OnEnter()
		{
			base.OnEnter();
			base.GetModelAnimator();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "SpawnEffectOrigin", false);
			}
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00014F90 File Offset: 0x00013190
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040004D8 RID: 1240
		public static float duration = 2f;

		// Token: 0x040004D9 RID: 1241
		public static string spawnSoundString;

		// Token: 0x040004DA RID: 1242
		public static GameObject spawnEffectPrefab;
	}
}
