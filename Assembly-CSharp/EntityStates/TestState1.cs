using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000C2 RID: 194
	public class TestState1 : EntityState
	{
		// Token: 0x060003C9 RID: 969 RVA: 0x0000F929 File Offset: 0x0000DB29
		public override void OnEnter()
		{
			Debug.LogFormat("{0} Entering TestState1.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0000F944 File Offset: 0x0000DB44
		public override void OnExit()
		{
			Debug.LogFormat("{0} Exiting TestState1.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0000F95F File Offset: 0x0000DB5F
		public override void FixedUpdate()
		{
			if (base.isAuthority && Input.GetButton("Fire1"))
			{
				this.outer.SetNextState(new TestState2());
			}
		}
	}
}
