using System;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x0200085B RID: 2139
	public class SpawnState : BaseState
	{
		// Token: 0x0600304C RID: 12364 RVA: 0x000CFE24 File Offset: 0x000CE024
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

		// Token: 0x0600304D RID: 12365 RVA: 0x000CFE98 File Offset: 0x000CE098
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002E76 RID: 11894
		public static float duration = 4f;

		// Token: 0x04002E77 RID: 11895
		public static GameObject burrowPrefab;

		// Token: 0x04002E78 RID: 11896
		public static string spawnSoundString;
	}
}
