using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002AF RID: 687
	public class OnEnableEvent : MonoBehaviour
	{
		// Token: 0x06000F9A RID: 3994 RVA: 0x000446B4 File Offset: 0x000428B4
		private void OnEnable()
		{
			this.action.Invoke();
		}

		// Token: 0x04000F03 RID: 3843
		public UnityEvent action;
	}
}
