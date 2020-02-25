using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002AE RID: 686
	public class OnDisableEvent : MonoBehaviour
	{
		// Token: 0x06000F98 RID: 3992 RVA: 0x000446A7 File Offset: 0x000428A7
		private void OnDisable()
		{
			this.action.Invoke();
		}

		// Token: 0x04000F02 RID: 3842
		public UnityEvent action;
	}
}
