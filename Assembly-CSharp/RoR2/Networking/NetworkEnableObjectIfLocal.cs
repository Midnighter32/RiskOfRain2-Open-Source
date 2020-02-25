using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200053F RID: 1343
	public class NetworkEnableObjectIfLocal : NetworkBehaviour
	{
		// Token: 0x06001FAD RID: 8109 RVA: 0x00089ACF File Offset: 0x00087CCF
		private void Start()
		{
			if (this.target)
			{
				this.target.SetActive(base.hasAuthority);
			}
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x00089AEF File Offset: 0x00087CEF
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			if (this.target)
			{
				this.target.SetActive(true);
			}
		}

		// Token: 0x06001FAF RID: 8111 RVA: 0x00089B10 File Offset: 0x00087D10
		public override void OnStopAuthority()
		{
			if (this.target)
			{
				this.target.SetActive(false);
			}
			base.OnStopAuthority();
		}

		// Token: 0x06001FB1 RID: 8113 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x00089B34 File Offset: 0x00087D34
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001FB3 RID: 8115 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001D66 RID: 7526
		[Tooltip("The GameObject to enable/disable.")]
		public GameObject target;
	}
}
