using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002CB RID: 715
	public class Deployable : MonoBehaviour
	{
		// Token: 0x06000E6D RID: 3693 RVA: 0x00047378 File Offset: 0x00045578
		private void OnDestroy()
		{
			if (NetworkServer.active && this.ownerMaster)
			{
				this.ownerMaster.RemoveDeployable(this);
			}
		}

		// Token: 0x04001276 RID: 4726
		[NonSerialized]
		public CharacterMaster ownerMaster;

		// Token: 0x04001277 RID: 4727
		public UnityEvent onUndeploy;
	}
}
