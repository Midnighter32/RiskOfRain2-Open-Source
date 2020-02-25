using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200071D RID: 1821
	public class TestState2 : EntityState
	{
		// Token: 0x06002A6B RID: 10859 RVA: 0x000B274E File Offset: 0x000B094E
		public override void OnEnter()
		{
			Debug.LogFormat("{0} Entering TestState2.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x06002A6C RID: 10860 RVA: 0x000B2769 File Offset: 0x000B0969
		public override void OnExit()
		{
			Debug.LogFormat("{0} Exiting TestState2.", new object[]
			{
				base.gameObject
			});
		}

		// Token: 0x06002A6D RID: 10861 RVA: 0x000B2784 File Offset: 0x000B0984
		public override void FixedUpdate()
		{
			if (base.isAuthority && Input.GetButton("Fire2"))
			{
				this.outer.SetNextState(new TestState1());
			}
		}
	}
}
