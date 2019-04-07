using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x0200037A RID: 890
	public class OnEnableEvent : MonoBehaviour
	{
		// Token: 0x0600129D RID: 4765 RVA: 0x0005B1F7 File Offset: 0x000593F7
		private void OnEnable()
		{
			this.action.Invoke();
		}

		// Token: 0x04001647 RID: 5703
		public UnityEvent action;
	}
}
