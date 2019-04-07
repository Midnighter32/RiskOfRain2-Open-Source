using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x020000AA RID: 170
	public class CloakTest : BaseState
	{
		// Token: 0x06000334 RID: 820 RVA: 0x0000D496 File Offset: 0x0000B696
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.Cloak);
				base.characterBody.AddBuff(BuffIndex.CloakSpeed);
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000D4CA File Offset: 0x0000B6CA
		public override void OnExit()
		{
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.Cloak);
				base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
			}
			base.OnExit();
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000D4FE File Offset: 0x0000B6FE
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0400031E RID: 798
		private float duration = 3f;
	}
}
