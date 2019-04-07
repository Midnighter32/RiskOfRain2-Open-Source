using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B7 RID: 183
	public abstract class GenericCharacterSpawnState : BaseState
	{
		// Token: 0x060003A0 RID: 928 RVA: 0x0000F027 File Offset: 0x0000D227
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn1", "Spawn1.playbackRate", this.duration);
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000F05C File Offset: 0x0000D25C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000362 RID: 866
		[SerializeField]
		public float duration = 2f;

		// Token: 0x04000363 RID: 867
		[SerializeField]
		public string spawnSoundString;
	}
}
