using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x0200019B RID: 411
	public class AltarSkeletonDeath : BaseState
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060007F3 RID: 2035 RVA: 0x00027708 File Offset: 0x00025908
		// (remove) Token: 0x060007F4 RID: 2036 RVA: 0x0002773C File Offset: 0x0002593C
		public static event Action onDeath;

		// Token: 0x060007F5 RID: 2037 RVA: 0x0002776F File Offset: 0x0002596F
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(AltarSkeletonDeath.deathSoundString, base.gameObject);
			this.Explode();
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0000F633 File Offset: 0x0000D833
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x00027790 File Offset: 0x00025990
		private void Explode()
		{
			if (base.modelLocator)
			{
				if (base.modelLocator.modelBaseTransform)
				{
					EntityState.Destroy(base.modelLocator.modelBaseTransform.gameObject);
				}
				if (base.modelLocator.modelTransform)
				{
					EntityState.Destroy(base.modelLocator.modelTransform.gameObject);
				}
			}
			if (AltarSkeletonDeath.explosionEffectPrefab && NetworkServer.active)
			{
				EffectManager.instance.SpawnEffect(AltarSkeletonDeath.explosionEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = AltarSkeletonDeath.explosionRadius,
					rotation = Quaternion.identity
				}, false);
			}
			Action action = AltarSkeletonDeath.onDeath;
			if (action != null)
			{
				action();
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000A68 RID: 2664
		public static GameObject explosionEffectPrefab;

		// Token: 0x04000A69 RID: 2665
		public static float explosionRadius;

		// Token: 0x04000A6A RID: 2666
		public static string deathSoundString;

		// Token: 0x04000A6C RID: 2668
		private float stopwatch;
	}
}
