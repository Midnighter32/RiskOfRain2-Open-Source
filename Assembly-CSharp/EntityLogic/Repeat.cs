using System;
using UnityEngine;
using UnityEngine.Events;

namespace EntityLogic
{
	// Token: 0x020001E8 RID: 488
	public class Repeat : MonoBehaviour
	{
		// Token: 0x06000983 RID: 2435 RVA: 0x0002FF78 File Offset: 0x0002E178
		public void CallRepeat(int repeatNumber)
		{
			while (repeatNumber > 0)
			{
				repeatNumber--;
				UnityEvent unityEvent = this.repeatedEvent;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
			}
		}

		// Token: 0x04000CDE RID: 3294
		public UnityEvent repeatedEvent;
	}
}
