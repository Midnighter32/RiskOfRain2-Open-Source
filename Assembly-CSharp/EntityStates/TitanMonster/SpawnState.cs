using System;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000175 RID: 373
	public class SpawnState : BaseState
	{
		// Token: 0x06000731 RID: 1841 RVA: 0x00023024 File Offset: 0x00021224
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

		// Token: 0x06000732 RID: 1842 RVA: 0x00023098 File Offset: 0x00021298
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000909 RID: 2313
		public static float duration = 4f;

		// Token: 0x0400090A RID: 2314
		public static GameObject burrowPrefab;

		// Token: 0x0400090B RID: 2315
		public static string spawnSoundString;
	}
}
