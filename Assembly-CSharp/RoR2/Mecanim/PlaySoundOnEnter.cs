using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000568 RID: 1384
	public class PlaySoundOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001EDC RID: 7900 RVA: 0x00091BBA File Offset: 0x0008FDBA
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.soundString, animator.gameObject);
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x00091BD7 File Offset: 0x0008FDD7
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.stopSoundString, animator.gameObject);
		}

		// Token: 0x04002192 RID: 8594
		public string soundString;

		// Token: 0x04002193 RID: 8595
		public string stopSoundString;
	}
}
