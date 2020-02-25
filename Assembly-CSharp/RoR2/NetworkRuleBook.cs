using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029D RID: 669
	[DefaultExecutionOrder(-1)]
	public class NetworkRuleBook : NetworkBehaviour
	{
		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x0004224A File Offset: 0x0004044A
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x00042252 File Offset: 0x00040452
		public RuleBook ruleBook { get; private set; }

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0004225B File Offset: 0x0004045B
		private void Awake()
		{
			this.ruleBook = new RuleBook();
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x00042268 File Offset: 0x00040468
		[Server]
		public void SetRuleBook([NotNull] RuleBook newRuleBook)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkRuleBook::SetRuleBook(RoR2.RuleBook)' called on client");
				return;
			}
			if (this.ruleBook.Equals(newRuleBook))
			{
				return;
			}
			base.SetDirtyBit(1U);
			this.ruleBook.Copy(newRuleBook);
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x000422A4 File Offset: 0x000404A4
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
				writer.Write(this.ruleBook);
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x000422E2 File Offset: 0x000404E2
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleBook(this.ruleBook);
			}
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x04000EBE RID: 3774
		private const uint ruleBookDirtyBit = 1U;
	}
}
