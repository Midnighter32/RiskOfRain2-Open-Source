using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200046B RID: 1131
	[Serializable]
	public struct ProcChainMask : IEquatable<ProcChainMask>
	{
		// Token: 0x0600194B RID: 6475 RVA: 0x000792CE File Offset: 0x000774CE
		public void AddProc(ProcType procType)
		{
			this.mask |= (ushort)(1 << (int)procType);
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x000792E5 File Offset: 0x000774E5
		public void RemoveProc(ProcType procType)
		{
			this.mask &= (ushort)(~(ushort)(1 << (int)procType));
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000792FD File Offset: 0x000774FD
		public bool HasProc(ProcType procType)
		{
			return ((int)this.mask & 1 << (int)procType) != 0;
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0000AE8B File Offset: 0x0000908B
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x0007930F File Offset: 0x0007750F
		public bool Equals(ProcChainMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x0007931F File Offset: 0x0007751F
		public override bool Equals(object obj)
		{
			return obj != null && obj is ProcChainMask && this.Equals((ProcChainMask)obj);
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x0007933C File Offset: 0x0007753C
		public override int GetHashCode()
		{
			return this.mask.GetHashCode();
		}

		// Token: 0x04001CCE RID: 7374
		[SerializeField]
		public ushort mask;
	}
}
