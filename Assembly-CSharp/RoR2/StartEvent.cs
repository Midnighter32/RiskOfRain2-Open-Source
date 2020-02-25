using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000342 RID: 834
	public class StartEvent : MonoBehaviour
	{
		// Token: 0x060013E7 RID: 5095 RVA: 0x00055108 File Offset: 0x00053308
		private void Start()
		{
			if (!this.runOnServerOnly || NetworkServer.active)
			{
				this.action.Invoke();
			}
		}

		// Token: 0x040012A6 RID: 4774
		public bool runOnServerOnly;

		// Token: 0x040012A7 RID: 4775
		public UnityEvent action;
	}
}
