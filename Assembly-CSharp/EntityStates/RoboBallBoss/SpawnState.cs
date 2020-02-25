using System;
using RoR2;
using UnityEngine;

namespace EntityStates.RoboBallBoss
{
	// Token: 0x0200079A RID: 1946
	public class SpawnState : GenericCharacterSpawnState
	{
		// Token: 0x06002C98 RID: 11416 RVA: 0x000BC1C9 File Offset: 0x000BA3C9
		public override void OnEnter()
		{
			base.OnEnter();
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.SpawnEffect(SpawnState.spawnEffectPrefab, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = SpawnState.spawnEffectRadius
				}, false);
			}
		}

		// Token: 0x040028AF RID: 10415
		public static GameObject spawnEffectPrefab;

		// Token: 0x040028B0 RID: 10416
		public static float spawnEffectRadius;
	}
}
