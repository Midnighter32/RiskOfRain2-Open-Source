using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200071C RID: 1820
	public class TestState1 : EntityState
	{
		// Token: 0x06002A67 RID: 10855 RVA: 0x000B26F2 File Offset: 0x000B08F2
		public override void OnEnter()
		{
			Debug.LogFormat("{0} Entering TestState1.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x06002A68 RID: 10856 RVA: 0x000B270D File Offset: 0x000B090D
		public override void OnExit()
		{
			Debug.LogFormat("{0} Exiting TestState1.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x000B2728 File Offset: 0x000B0928
		public override void FixedUpdate()
		{
			if (base.isAuthority && Input.GetButton("Fire1"))
			{
				this.outer.SetNextState(new TestState2());
			}
		}
	}
}
