using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020001BD RID: 445
	public class SpawnState : BaseState
	{
		// Token: 0x060008B4 RID: 2228 RVA: 0x0002BB48 File Offset: 0x00029D48
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

		// Token: 0x060008B5 RID: 2229 RVA: 0x0002BBC4 File Offset: 0x00029DC4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000BAC RID: 2988
		public static float duration;

		// Token: 0x04000BAD RID: 2989
		public static string spawnSoundString;

		// Token: 0x04000BAE RID: 2990
		public static GameObject spawnEffectPrefab;

		// Token: 0x04000BAF RID: 2991
		public static string spawnEffectChildString;
	}
}
