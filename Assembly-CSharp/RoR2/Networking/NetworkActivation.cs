using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000554 RID: 1364
	public class NetworkActivation : NetworkBehaviour
	{
		// Token: 0x06002088 RID: 8328 RVA: 0x0008D046 File Offset: 0x0008B246
		public void OnEnable()
		{
			base.SetDirtyBit(1U);
		}

		// Token: 0x06002089 RID: 8329 RVA: 0x0008D046 File Offset: 0x0008B246
		public void OnDisable()
		{
			base.SetDirtyBit(1U);
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x0008D04F File Offset: 0x0008B24F
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(base.gameObject.activeSelf);
			return true;
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x0008D063 File Offset: 0x0008B263
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.gameObject.SetActive(reader.ReadBoolean());
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}
	}
}
