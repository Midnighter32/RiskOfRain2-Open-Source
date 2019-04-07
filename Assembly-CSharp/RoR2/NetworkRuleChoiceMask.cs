using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036D RID: 877
	public class NetworkRuleChoiceMask : NetworkBehaviour
	{
		// Token: 0x06001210 RID: 4624 RVA: 0x00059271 File Offset: 0x00057471
		[Server]
		public void SetRuleChoiceMask([NotNull] RuleChoiceMask newRuleChoiceMask)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkRuleChoiceMask::SetRuleChoiceMask(RoR2.RuleChoiceMask)' called on client");
				return;
			}
			if (this.ruleChoiceMask.Equals(newRuleChoiceMask))
			{
				return;
			}
			base.SetDirtyBit(1u);
			this.ruleChoiceMask.Copy(newRuleChoiceMask);
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x000592AC File Offset: 0x000574AC
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1u;
			}
			bool flag = (num & 1u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this.ruleChoiceMask);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x000592EA File Offset: 0x000574EA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleChoiceMask(this.ruleChoiceMask);
			}
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x04001611 RID: 5649
		public readonly RuleChoiceMask ruleChoiceMask = new RuleChoiceMask();

		// Token: 0x04001612 RID: 5650
		private const uint maskDirtyBit = 1u;
	}
}
