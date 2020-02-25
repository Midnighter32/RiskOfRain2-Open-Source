using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002B0 RID: 688
	[RequireComponent(typeof(Collider))]
	public class OnPlayerEnterEvent : MonoBehaviour
	{
		// Token: 0x06000F9C RID: 3996 RVA: 0x000446C4 File Offset: 0x000428C4
		private void OnTriggerEnter(Collider other)
		{
			if ((this.serverOnly && !NetworkServer.active) || this.calledAction)
			{
				return;
			}
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component)
			{
				CharacterMaster master = component.master;
				if (master && master.GetComponent<PlayerCharacterMasterController>())
				{
					this.calledAction = true;
					UnityEvent unityEvent = this.action;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
			}
		}

		// Token: 0x04000F04 RID: 3844
		public bool serverOnly;

		// Token: 0x04000F05 RID: 3845
		public UnityEvent action;

		// Token: 0x04000F06 RID: 3846
		private bool calledAction;
	}
}
