using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000587 RID: 1415
	public class NetworkActivation : NetworkBehaviour
	{
		// Token: 0x06001FDC RID: 8156 RVA: 0x0009615E File Offset: 0x0009435E
		public void OnEnable()
		{
			base.SetDirtyBit(1u);
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x0009615E File Offset: 0x0009435E
		public void OnDisable()
		{
			base.SetDirtyBit(1u);
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x00096167 File Offset: 0x00094367
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(base.gameObject.activeSelf);
			return true;
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x0009617B File Offset: 0x0009437B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.gameObject.SetActive(reader.ReadBoolean());
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}
	}
}
