using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200056D RID: 1389
	public class StayInStateForDuration : StateMachineBehaviour
	{
		// Token: 0x06001EE8 RID: 7912 RVA: 0x00091D10 File Offset: 0x0008FF10
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetBool(this.deactivationBoolParameterName, true);
		}

		// Token: 0x06001EE9 RID: 7913 RVA: 0x00091D28 File Offset: 0x0008FF28
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, animatorStateInfo, layerIndex);
			float num = animator.GetFloat(this.stopwatchFloatParameterName);
			float @float = animator.GetFloat(this.durationFloatParameterName);
			num += Time.deltaTime;
			if (num >= @float)
			{
				animator.SetFloat(this.stopwatchFloatParameterName, 0f);
				animator.SetBool(this.deactivationBoolParameterName, false);
				return;
			}
			animator.SetFloat(this.stopwatchFloatParameterName, num);
		}

		// Token: 0x0400219F RID: 8607
		[Tooltip("The reference float - this is how long we will stay in this state")]
		public string durationFloatParameterName;

		// Token: 0x040021A0 RID: 8608
		[Tooltip("The counter float - this is exposed incase we want to reset it")]
		public string stopwatchFloatParameterName;

		// Token: 0x040021A1 RID: 8609
		[Tooltip("The bool that will be set to 'false' once the duration is up, and 'true' when entering this state.")]
		public string deactivationBoolParameterName;
	}
}
