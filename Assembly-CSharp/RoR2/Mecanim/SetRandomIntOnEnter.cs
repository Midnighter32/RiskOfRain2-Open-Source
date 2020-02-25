using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000536 RID: 1334
	public class SetRandomIntOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001F74 RID: 8052 RVA: 0x0008885A File Offset: 0x00086A5A
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetInteger(this.intParameterName, UnityEngine.Random.Range(this.rangeMin, this.rangeMax + 1));
		}

		// Token: 0x04001D26 RID: 7462
		public string intParameterName;

		// Token: 0x04001D27 RID: 7463
		public int rangeMin;

		// Token: 0x04001D28 RID: 7464
		public int rangeMax;
	}
}
