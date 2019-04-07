using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000576 RID: 1398
	public class NetworkEnableObjectIfLocal : NetworkBehaviour
	{
		// Token: 0x06001F1E RID: 7966 RVA: 0x00092DAF File Offset: 0x00090FAF
		private void Start()
		{
			if (this.target)
			{
				this.target.SetActive(base.hasAuthority);
			}
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x00092DCF File Offset: 0x00090FCF
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			if (this.target)
			{
				this.target.SetActive(true);
			}
		}

		// Token: 0x06001F20 RID: 7968 RVA: 0x00092DF0 File Offset: 0x00090FF0
		public override void OnStopAuthority()
		{
			if (this.target)
			{
				this.target.SetActive(false);
			}
			base.OnStopAuthority();
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x00092E14 File Offset: 0x00091014
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040021D4 RID: 8660
		[Tooltip("The GameObject to enable/disable.")]
		public GameObject target;
	}
}
