using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000532 RID: 1330
	public class PlaySoundOnEnter : StateMachineBehaviour
	{
		// Token: 0x06001F6A RID: 8042 RVA: 0x0008872B File Offset: 0x0008692B
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.soundString, animator.gameObject);
		}

		// Token: 0x06001F6B RID: 8043 RVA: 0x00088748 File Offset: 0x00086948
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Util.PlaySound(this.stopSoundString, animator.gameObject);
		}

		// Token: 0x04001D1C RID: 7452
		public string soundString;

		// Token: 0x04001D1D RID: 7453
		public string stopSoundString;
	}
}
