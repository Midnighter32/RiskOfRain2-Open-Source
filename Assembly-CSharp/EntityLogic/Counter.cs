using System;
using UnityEngine;
using UnityEngine.Events;

namespace EntityLogic
{
	// Token: 0x020000A8 RID: 168
	public class Counter : MonoBehaviour
	{
		// Token: 0x06000340 RID: 832 RVA: 0x0000CC98 File Offset: 0x0000AE98
		public void Add(int valueToAdd)
		{
			this.value += valueToAdd;
			if (this.value >= this.threshold)
			{
				this.onTrigger.Invoke();
			}
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0000CCC1 File Offset: 0x0000AEC1
		public void SetValue(int newValue)
		{
			this.value = newValue;
		}

		// Token: 0x040002ED RID: 749
		public int value;

		// Token: 0x040002EE RID: 750
		public int threshold;

		// Token: 0x040002EF RID: 751
		public UnityEvent onTrigger;
	}
}
