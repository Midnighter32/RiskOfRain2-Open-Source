using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029E RID: 670
	[DefaultExecutionOrder(-1)]
	public class NetworkRuleChoiceMask : NetworkBehaviour
	{
		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x000422FA File Offset: 0x000404FA
		// (set) Token: 0x06000EEF RID: 3823 RVA: 0x00042302 File Offset: 0x00040502
		public RuleChoiceMask ruleChoiceMask { get; private set; }

		// Token: 0x06000EF0 RID: 3824 RVA: 0x0004230B File Offset: 0x0004050B
		private void Awake()
		{
			this.ruleChoiceMask = new RuleChoiceMask();
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x00042318 File Offset: 0x00040518
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
			base.SetDirtyBit(1U);
			this.ruleChoiceMask.Copy(newRuleChoiceMask);
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x00042354 File Offset: 0x00040554
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1U;
			}
			bool flag = (num & 1U) > 0U;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this.ruleChoiceMask);
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x00042392 File Offset: 0x00040592
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleChoiceMask(this.ruleChoiceMask);
			}
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x04000EC0 RID: 3776
		private const uint maskDirtyBit = 1U;
	}
}
