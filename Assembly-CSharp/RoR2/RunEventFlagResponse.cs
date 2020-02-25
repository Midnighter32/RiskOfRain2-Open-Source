using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030C RID: 780
	public class RunEventFlagResponse : MonoBehaviour
	{
		// Token: 0x06001255 RID: 4693 RVA: 0x0004F1B0 File Offset: 0x0004D3B0
		private void Awake()
		{
			if (NetworkServer.active)
			{
				if (Run.instance)
				{
					UnityEvent unityEvent = Run.instance.GetEventFlag(this.flagName) ? this.onAwakeWithFlagSetServer : this.onAwakeWithFlagUnsetServer;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
					return;
				}
				else
				{
					Debug.LogErrorFormat("Cannot handle run event flag response {0}: No run exists.", new object[]
					{
						base.gameObject.name
					});
				}
			}
		}

		// Token: 0x04001149 RID: 4425
		public string flagName;

		// Token: 0x0400114A RID: 4426
		public UnityEvent onAwakeWithFlagSetServer;

		// Token: 0x0400114B RID: 4427
		public UnityEvent onAwakeWithFlagUnsetServer;
	}
}
