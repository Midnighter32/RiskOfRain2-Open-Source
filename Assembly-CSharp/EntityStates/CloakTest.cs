using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x02000701 RID: 1793
	public class CloakTest : BaseState
	{
		// Token: 0x060029AE RID: 10670 RVA: 0x000AF9CC File Offset: 0x000ADBCC
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.Cloak);
				base.characterBody.AddBuff(BuffIndex.CloakSpeed);
			}
		}

		// Token: 0x060029AF RID: 10671 RVA: 0x000AFA00 File Offset: 0x000ADC00
		public override void OnExit()
		{
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.Cloak);
				base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
			}
			base.OnExit();
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x000AFA34 File Offset: 0x000ADC34
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x040025AF RID: 9647
		private float duration = 3f;
	}
}
