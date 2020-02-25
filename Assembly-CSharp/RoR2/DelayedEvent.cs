using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020001D6 RID: 470
	public class DelayedEvent : MonoBehaviour
	{
		// Token: 0x06000A0D RID: 2573 RVA: 0x0002C0E8 File Offset: 0x0002A2E8
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

		// Token: 0x06000A0E RID: 2574 RVA: 0x0002C13F File Offset: 0x0002A33F
		private void Call()
		{
			if (this)
			{
				this.action.Invoke();
			}
		}

		// Token: 0x04000A54 RID: 2644
		public UnityEvent action;

		// Token: 0x04000A55 RID: 2645
		public DelayedEvent.TimeStepType timeStepType;

		// Token: 0x020001D7 RID: 471
		public enum TimeStepType
		{
			// Token: 0x04000A57 RID: 2647
			Time,
			// Token: 0x04000A58 RID: 2648
			UnscaledTime,
			// Token: 0x04000A59 RID: 2649
			FixedTime
		}
	}
}
