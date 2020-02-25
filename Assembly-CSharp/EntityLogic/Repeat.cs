using System;
using UnityEngine;
using UnityEngine.Events;

namespace EntityLogic
{
	// Token: 0x020000A9 RID: 169
	public class Repeat : MonoBehaviour
	{
		// Token: 0x06000343 RID: 835 RVA: 0x0000CCCA File Offset: 0x0000AECA
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

		// Token: 0x040002F0 RID: 752
		public UnityEvent repeatedEvent;
	}
}
