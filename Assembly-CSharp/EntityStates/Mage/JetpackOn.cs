using System;
using UnityEngine;

namespace EntityStates.Mage
{
	// Token: 0x020007CE RID: 1998
	public class JetpackOn : BaseState
	{
		// Token: 0x06002D91 RID: 11665 RVA: 0x000C13EB File Offset: 0x000BF5EB
		public override void OnEnter()
		{
			base.OnEnter();
			this.jetOnEffect = base.FindModelChild("JetOn");
			if (this.jetOnEffect)
			{
				this.jetOnEffect.gameObject.SetActive(true);
			}
		}

		// Token: 0x06002D92 RID: 11666 RVA: 0x000C1424 File Offset: 0x000BF624
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				float num = base.characterMotor.velocity.y;
				num = Mathf.MoveTowards(num, JetpackOn.hoverVelocity, JetpackOn.hoverAcceleration * Time.fixedDeltaTime);
				base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
			}
		}

		// Token: 0x06002D93 RID: 11667 RVA: 0x000C1498 File Offset: 0x000BF698
		public override void OnExit()
		{
			base.OnExit();
			if (this.jetOnEffect)
			{
				this.jetOnEffect.gameObject.SetActive(false);
			}
		}

		// Token: 0x04002A2B RID: 10795
		public static float hoverVelocity;

		// Token: 0x04002A2C RID: 10796
		public static float hoverAcceleration;

		// Token: 0x04002A2D RID: 10797
		private Transform jetOnEffect;
	}
}
