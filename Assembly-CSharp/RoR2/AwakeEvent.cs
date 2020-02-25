using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000158 RID: 344
	public class AwakeEvent : MonoBehaviour
	{
		// Token: 0x0600062C RID: 1580 RVA: 0x00019B40 File Offset: 0x00017D40
		private void Awake()
		{
			this.action.Invoke();
		}

		// Token: 0x040006A2 RID: 1698
		public UnityEvent action;
	}
}
