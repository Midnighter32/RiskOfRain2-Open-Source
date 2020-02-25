using System;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000794 RID: 1940
	public class Sit : BaseState
	{
		// Token: 0x06002C7D RID: 11389 RVA: 0x000BBB8A File Offset: 0x000B9D8A
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Body", "SitLoop", 0.1f);
		}

		// Token: 0x06002C7E RID: 11390 RVA: 0x000BBBA7 File Offset: 0x000B9DA7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.inputBank.moveVector.sqrMagnitude >= Mathf.Epsilon && base.fixedAge >= Sit.minimumDuration)
			{
				this.outer.SetNextState(new ExitSit());
			}
		}

		// Token: 0x0400288E RID: 10382
		public static string soundString;

		// Token: 0x0400288F RID: 10383
		public static float minimumDuration;
	}
}
