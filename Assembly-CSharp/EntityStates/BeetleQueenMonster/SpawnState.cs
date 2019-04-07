using System;
using RoR2;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D3 RID: 467
	public class SpawnState : BaseState
	{
		// Token: 0x0600091B RID: 2331 RVA: 0x0002DC38 File Offset: 0x0002BE38
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

		// Token: 0x0600091C RID: 2332 RVA: 0x0002DCAC File Offset: 0x0002BEAC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000C54 RID: 3156
		public static float duration = 4f;

		// Token: 0x04000C55 RID: 3157
		public static GameObject burrowPrefab;

		// Token: 0x04000C56 RID: 3158
		public static string spawnSoundString;
	}
}
