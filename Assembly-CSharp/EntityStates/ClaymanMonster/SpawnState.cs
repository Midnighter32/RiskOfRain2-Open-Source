using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClaymanMonster
{
	// Token: 0x020008CE RID: 2254
	public class SpawnState : BaseState
	{
		// Token: 0x06003285 RID: 12933 RVA: 0x000DA868 File Offset: 0x000D8A68
		public override void OnEnter()
		{
			base.OnEnter();
			ChildLocator component = base.GetModelTransform().GetComponent<ChildLocator>();
			if (component)
			{
				Transform transform = component.FindChild(SpawnState.spawnEffectChildString);
				if (transform)
				{
					UnityEngine.Object.Instantiate<GameObject>(SpawnState.spawnEffectPrefab, transform.position, Quaternion.identity);
				}
			}
			if (SpawnState.spawnSoundString.Length > 0)
			{
				Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			}
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
		}

		// Token: 0x06003286 RID: 12934 RVA: 0x000DA8F1 File Offset: 0x000D8AF1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003287 RID: 12935 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04003189 RID: 12681
		public static float duration;

		// Token: 0x0400318A RID: 12682
		public static string spawnSoundString;

		// Token: 0x0400318B RID: 12683
		public static GameObject spawnEffectPrefab;

		// Token: 0x0400318C RID: 12684
		public static string spawnEffectChildString;
	}
}
