using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200037B RID: 891
	[RequireComponent(typeof(Collider))]
	public class OnPlayerEnterEvent : MonoBehaviour
	{
		// Token: 0x0600129F RID: 4767 RVA: 0x0005B204 File Offset: 0x00059404
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

		// Token: 0x04001648 RID: 5704
		public bool serverOnly;

		// Token: 0x04001649 RID: 5705
		public UnityEvent action;

		// Token: 0x0400164A RID: 5706
		private bool calledAction;
	}
}
