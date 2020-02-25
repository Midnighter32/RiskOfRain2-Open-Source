using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200070E RID: 1806
	public abstract class GenericCharacterSpawnState : BaseState
	{
		// Token: 0x06002A2C RID: 10796 RVA: 0x000B17BA File Offset: 0x000AF9BA
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn1", "Spawn1.playbackRate", this.duration);
		}

		// Token: 0x06002A2D RID: 10797 RVA: 0x000B17EF File Offset: 0x000AF9EF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040025FD RID: 9725
		[SerializeField]
		public float duration = 2f;

		// Token: 0x040025FE RID: 9726
		[SerializeField]
		public string spawnSoundString;
	}
}
