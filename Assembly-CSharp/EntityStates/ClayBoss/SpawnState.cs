using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020008D8 RID: 2264
	public class SpawnState : BaseState
	{
		// Token: 0x060032B3 RID: 12979 RVA: 0x000DB77C File Offset: 0x000D997C
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
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
		}

		// Token: 0x060032B4 RID: 12980 RVA: 0x000DB7F8 File Offset: 0x000D99F8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040031D4 RID: 12756
		public static float duration;

		// Token: 0x040031D5 RID: 12757
		public static string spawnSoundString;

		// Token: 0x040031D6 RID: 12758
		public static GameObject spawnEffectPrefab;

		// Token: 0x040031D7 RID: 12759
		public static string spawnEffectChildString;
	}
}
