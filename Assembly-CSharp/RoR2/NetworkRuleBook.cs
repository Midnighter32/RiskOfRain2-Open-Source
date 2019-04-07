using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036C RID: 876
	public class NetworkRuleBook : NetworkBehaviour
	{
		// Token: 0x0600120B RID: 4619 RVA: 0x000591CF File Offset: 0x000573CF
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
			base.SetDirtyBit(1u);
			this.ruleBook.Copy(newRuleBook);
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x00059208 File Offset: 0x00057408
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
				writer.Write(this.ruleBook);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x00059246 File Offset: 0x00057446
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleBook(this.ruleBook);
			}
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x0400160F RID: 5647
		public readonly RuleBook ruleBook = new RuleBook();

		// Token: 0x04001610 RID: 5648
		private const uint ruleBookDirtyBit = 1u;
	}
}
