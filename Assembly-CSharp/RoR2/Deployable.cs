using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001DA RID: 474
	public class Deployable : MonoBehaviour
	{
		// Token: 0x06000A10 RID: 2576 RVA: 0x0002C154 File Offset: 0x0002A354
		private void OnDestroy()
		{
			if (NetworkServer.active && this.ownerMaster)
			{
				this.ownerMaster.RemoveDeployable(this);
			}
		}

		// Token: 0x04000A64 RID: 2660
		[NonSerialized]
		public CharacterMaster ownerMaster;

		// Token: 0x04000A65 RID: 2661
		public UnityEvent onUndeploy;
	}
}
