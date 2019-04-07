using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000262 RID: 610
	public class AwakeEvent : MonoBehaviour
	{
		// Token: 0x06000B56 RID: 2902 RVA: 0x00037F9C File Offset: 0x0003619C
		private void Awake()
		{
			this.action.Invoke();
		}

		// Token: 0x04000F5C RID: 3932
		public UnityEvent action;
	}
}
