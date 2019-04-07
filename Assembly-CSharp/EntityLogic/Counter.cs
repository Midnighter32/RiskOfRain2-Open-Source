using System;
using UnityEngine;
using UnityEngine.Events;

namespace EntityLogic
{
	// Token: 0x020001E7 RID: 487
	public class Counter : MonoBehaviour
	{
		// Token: 0x06000980 RID: 2432 RVA: 0x0002FF46 File Offset: 0x0002E146
		public void Add(int valueToAdd)
		{
			this.value += valueToAdd;
			if (this.value >= this.threshold)
			{
				this.onTrigger.Invoke();
			}
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0002FF6F File Offset: 0x0002E16F
		public void SetValue(int newValue)
		{
			this.value = newValue;
		}

		// Token: 0x04000CDB RID: 3291
		public int value;

		// Token: 0x04000CDC RID: 3292
		public int threshold;

		// Token: 0x04000CDD RID: 3293
		public UnityEvent onTrigger;
	}
}
