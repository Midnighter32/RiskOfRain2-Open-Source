using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002C7 RID: 711
	public class DelayedEvent : MonoBehaviour
	{
		// Token: 0x06000E6A RID: 3690 RVA: 0x0004730C File Offset: 0x0004550C
		public void CallDelayed(float timer)
		{
			TimerQueue timerQueue = null;
			switch (this.timeStepType)
			{
			case DelayedEvent.TimeStepType.Time:
				timerQueue = RoR2Application.timeTimers;
				break;
			case DelayedEvent.TimeStepType.UnscaledTime:
				timerQueue = RoR2Application.unscaledTimeTimers;
				break;
			case DelayedEvent.TimeStepType.FixedTime:
				timerQueue = RoR2Application.fixedTimeTimers;
				break;
			}
			if (timerQueue != null)
			{
				timerQueue.CreateTimer(timer, new Action(this.Call));
			}
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00047363 File Offset: 0x00045563
		private void Call()
		{
			if (this)
			{
				this.action.Invoke();
			}
		}

		// Token: 0x04001269 RID: 4713
		public UnityEvent action;

		// Token: 0x0400126A RID: 4714
		public DelayedEvent.TimeStepType timeStepType;

		// Token: 0x020002C8 RID: 712
		public enum TimeStepType
		{
			// Token: 0x0400126C RID: 4716
			Time,
			// Token: 0x0400126D RID: 4717
			UnscaledTime,
			// Token: 0x0400126E RID: 4718
			FixedTime
		}
	}
}
