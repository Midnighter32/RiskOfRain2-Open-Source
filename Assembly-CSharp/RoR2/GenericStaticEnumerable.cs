using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x02000432 RID: 1074
	public struct GenericStaticEnumerable<T, TEnumerator> : IEnumerable<T>, IEnumerable where TEnumerator : struct, IEnumerator<T>
	{
		// Token: 0x060017E2 RID: 6114 RVA: 0x00071FD8 File Offset: 0x000701D8
		static GenericStaticEnumerable()
		{
			GenericStaticEnumerable<T, TEnumerator>.defaultValue.Reset();
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x00071FF5 File Offset: 0x000701F5
		public IEnumerator<T> GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x00071FF5 File Offset: 0x000701F5
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x04001B41 RID: 6977
		private static readonly TEnumerator defaultValue = default(TEnumerator);
	}
}
