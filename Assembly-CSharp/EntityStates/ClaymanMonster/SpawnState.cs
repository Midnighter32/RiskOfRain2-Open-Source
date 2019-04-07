using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClaymanMonster
{
	// Token: 0x020001B4 RID: 436
	public class SpawnState : BaseState
	{
		// Token: 0x0600088A RID: 2186 RVA: 0x0002AD34 File Offset: 0x00028F34
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

		// Token: 0x0600088B RID: 2187 RVA: 0x0002ADBD File Offset: 0x00028FBD
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000B68 RID: 2920
		public static float duration;

		// Token: 0x04000B69 RID: 2921
		public static string spawnSoundString;

		// Token: 0x04000B6A RID: 2922
		public static GameObject spawnEffectPrefab;

		// Token: 0x04000B6B RID: 2923
		public static string spawnEffectChildString;
	}
}
