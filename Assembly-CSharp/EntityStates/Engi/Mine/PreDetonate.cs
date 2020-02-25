using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x0200087F RID: 2175
	public class PreDetonate : BaseMineState
	{
		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x060030F1 RID: 12529 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x060030F2 RID: 12530 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldRevertToWaitForStickOnSurfaceLost
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060030F3 RID: 12531 RVA: 0x000D26B8 File Offset: 0x000D08B8
		public override void OnEnter()
		{
			base.OnEnter();
			base.transform.Find(PreDetonate.pathToPrepForExplosionChildEffect).gameObject.SetActive(true);
			base.rigidbody.AddForce(base.transform.forward * PreDetonate.detachForce);
			base.rigidbody.AddTorque(UnityEngine.Random.onUnitSphere * 200f);
		}

		// Token: 0x060030F4 RID: 12532 RVA: 0x000D2720 File Offset: 0x000D0920
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && PreDetonate.duration <= base.fixedAge)
			{
				this.outer.SetNextState(new Detonate());
			}
		}

		// Token: 0x04002F22 RID: 12066
		public static float duration;

		// Token: 0x04002F23 RID: 12067
		public static string pathToPrepForExplosionChildEffect;

		// Token: 0x04002F24 RID: 12068
		public static float detachForce;
	}
}
