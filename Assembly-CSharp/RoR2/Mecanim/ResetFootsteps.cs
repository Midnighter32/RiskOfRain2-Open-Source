using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200056A RID: 1386
	public class ResetFootsteps : StateMachineBehaviour
	{
		// Token: 0x06001EE1 RID: 7905 RVA: 0x00091C64 File Offset: 0x0008FE64
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.GetComponent<FootstepHandler>();
		}
	}
}
