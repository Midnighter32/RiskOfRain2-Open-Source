using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x020008A2 RID: 2210
	public class AltarSkeletonDeath : BaseState
	{
		// Token: 0x14000090 RID: 144
		// (add) Token: 0x0600318F RID: 12687 RVA: 0x000D577C File Offset: 0x000D397C
		// (remove) Token: 0x06003190 RID: 12688 RVA: 0x000D57B0 File Offset: 0x000D39B0
		public static event Action onDeath;

		// Token: 0x06003191 RID: 12689 RVA: 0x000D57E3 File Offset: 0x000D39E3
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(AltarSkeletonDeath.deathSoundString, base.gameObject);
			this.Explode();
		}

		// Token: 0x06003192 RID: 12690 RVA: 0x000B23CF File Offset: 0x000B05CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06003193 RID: 12691 RVA: 0x000D5804 File Offset: 0x000D3A04
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
				EffectManager.SpawnEffect(AltarSkeletonDeath.explosionEffectPrefab, new EffectData
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

		// Token: 0x06003194 RID: 12692 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04003006 RID: 12294
		public static GameObject explosionEffectPrefab;

		// Token: 0x04003007 RID: 12295
		public static float explosionRadius;

		// Token: 0x04003008 RID: 12296
		public static string deathSoundString;

		// Token: 0x0400300A RID: 12298
		private float stopwatch;
	}
}
