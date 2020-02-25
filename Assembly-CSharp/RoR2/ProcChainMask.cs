using System;
using System.Text;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E7 RID: 999
	[Serializable]
	public struct ProcChainMask : IEquatable<ProcChainMask>
	{
		// Token: 0x06001845 RID: 6213 RVA: 0x00069066 File Offset: 0x00067266
		public void AddProc(ProcType procType)
		{
			this.mask |= (ushort)(1 << (int)procType);
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0006907D File Offset: 0x0006727D
		public void RemoveProc(ProcType procType)
		{
			this.mask &= (ushort)(~(ushort)(1 << (int)procType));
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x00069095 File Offset: 0x00067295
		public bool HasProc(ProcType procType)
		{
			return ((int)this.mask & 1 << (int)procType) != 0;
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x0000B933 File Offset: 0x00009B33
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000690A7 File Offset: 0x000672A7
		public bool Equals(ProcChainMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x000690B7 File Offset: 0x000672B7
		public override bool Equals(object obj)
		{
			return obj != null && obj is ProcChainMask && this.Equals((ProcChainMask)obj);
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x000690D4 File Offset: 0x000672D4
		public override int GetHashCode()
		{
			return this.mask.GetHashCode();
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x000690E1 File Offset: 0x000672E1
		public override string ToString()
		{
			this.AppendToStringBuilder(ProcChainMask.sharedStringBuilder);
			string result = ProcChainMask.sharedStringBuilder.ToString();
			ProcChainMask.sharedStringBuilder.Clear();
			return result;
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x00069104 File Offset: 0x00067304
		public void AppendToStringBuilder(StringBuilder stringBuilder)
		{
			stringBuilder.Append("(");
			bool flag = false;
			for (ProcType procType = ProcType.Behemoth; procType < ProcType.Count; procType++)
			{
				if (this.HasProc(procType))
				{
					if (flag)
					{
						stringBuilder.Append("|");
					}
					stringBuilder.Append(procType.ToString());
					flag = true;
				}
			}
			stringBuilder.Append(")");
		}

		// Token: 0x040016E3 RID: 5859
		[SerializeField]
		public ushort mask;

		// Token: 0x040016E4 RID: 5860
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
