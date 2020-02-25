using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000537 RID: 1335
	public class StayInStateForDuration : StateMachineBehaviour
	{
		// Token: 0x06001F76 RID: 8054 RVA: 0x00088884 File Offset: 0x00086A84
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetBool(this.deactivationBoolParameterName, true);
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x0008889C File Offset: 0x00086A9C
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

		// Token: 0x04001D29 RID: 7465
		[Tooltip("The reference float - this is how long we will stay in this state")]
		public string durationFloatParameterName;

		// Token: 0x04001D2A RID: 7466
		[Tooltip("The counter float - this is exposed incase we want to reset it")]
		public string stopwatchFloatParameterName;

		// Token: 0x04001D2B RID: 7467
		[Tooltip("The bool that will be set to 'false' once the duration is up, and 'true' when entering this state.")]
		public string deactivationBoolParameterName;
	}
}
