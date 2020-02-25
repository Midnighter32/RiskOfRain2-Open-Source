using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008EE RID: 2286
	public class SpawnState : BaseState
	{
		// Token: 0x0600331B RID: 13083 RVA: 0x000DD894 File Offset: 0x000DBA94
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			ChildLocator component = base.GetModelTransform().GetComponent<ChildLocator>();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			string childName = "BurrowCenter";
			Transform transform = component.FindChild(childName);
			if (transform)
			{
				UnityEngine.Object.Instantiate<GameObject>(SpawnState.burrowPrefab, transform.position, Quaternion.identity);
			}
		}

		// Token: 0x0600331C RID: 13084 RVA: 0x000DD908 File Offset: 0x000DBB08
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600331D RID: 13085 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400327C RID: 12924
		public static float duration = 4f;

		// Token: 0x0400327D RID: 12925
		public static GameObject burrowPrefab;

		// Token: 0x0400327E RID: 12926
		public static string spawnSoundString;
	}
}
