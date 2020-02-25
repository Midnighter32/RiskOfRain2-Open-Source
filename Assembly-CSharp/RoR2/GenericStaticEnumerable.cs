using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x02000391 RID: 913
	public struct GenericStaticEnumerable<T, TEnumerator> : IEnumerable<T>, IEnumerable where TEnumerator : struct, IEnumerator<T>
	{
		// Token: 0x06001634 RID: 5684 RVA: 0x0005F88F File Offset: 0x0005DA8F
		static GenericStaticEnumerable()
		{
			GenericStaticEnumerable<T, TEnumerator>.defaultValue.Reset();
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0005F8AC File Offset: 0x0005DAAC
		public IEnumerator<T> GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x0005F8AC File Offset: 0x0005DAAC
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x040014E6 RID: 5350
		private static readonly TEnumerator defaultValue = default(TEnumerator);
	}
}
