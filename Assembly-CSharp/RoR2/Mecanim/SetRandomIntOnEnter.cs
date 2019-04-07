using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200056C RID: 1388
	public class SetRandomIntOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001EE6 RID: 7910 RVA: 0x00091CE6 File Offset: 0x0008FEE6
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetInteger(this.intParameterName, UnityEngine.Random.Range(this.rangeMin, this.rangeMax + 1));
		}

		// Token: 0x0400219C RID: 8604
		public string intParameterName;

		// Token: 0x0400219D RID: 8605
		public int rangeMin;

		// Token: 0x0400219E RID: 8606
		public int rangeMax;
	}
}
