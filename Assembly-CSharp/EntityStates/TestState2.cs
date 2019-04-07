using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000C3 RID: 195
	public class TestState2 : EntityState
	{
		// Token: 0x060003CD RID: 973 RVA: 0x0000F985 File Offset: 0x0000DB85
		public override void OnEnter()
		{
			Debug.LogFormat("{0} Entering TestState2.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0000F9A0 File Offset: 0x0000DBA0
		public override void OnExit()
		{
			Debug.LogFormat("{0} Exiting TestState2.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0000F9BB File Offset: 0x0000DBBB
		public override void FixedUpdate()
		{
			if (base.isAuthority && Input.GetButton("Fire2"))
			{
				this.outer.SetNextState(new TestState1());
			}
		}
	}
}
