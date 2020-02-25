using System;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x020000DB RID: 219
	public class CatalogModHelper<TEntry>
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000443 RID: 1091 RVA: 0x00011768 File Offset: 0x0000F968
		// (remove) Token: 0x06000444 RID: 1092 RVA: 0x000117A0 File Offset: 0x0000F9A0
		public event Action<List<TEntry>> getAdditionalEntries;

		// Token: 0x06000445 RID: 1093 RVA: 0x000117D5 File Offset: 0x0000F9D5
		public CatalogModHelper(Action<int, TEntry> registrationDelegate, Func<TEntry, string> nameGetter)
		{
			this.registrationDelegate = registrationDelegate;
			this.nameGetter = nameGetter;
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x000117EC File Offset: 0x0000F9EC
		public void CollectAndRegisterAdditionalEntries(ref TEntry[] entries)
		{
			int num = entries.Length;
			List<TEntry> list = new List<TEntry>();
			Action<List<TEntry>> action = this.getAdditionalEntries;
			if (action != null)
			{
				action(list);
			}
			list.Sort((TEntry a, TEntry b) => StringComparer.Ordinal.Compare(this.nameGetter(a), this.nameGetter(b)));
			Array.Resize<TEntry>(ref entries, entries.Length + list.Count);
			int i = num;
			int num2 = num + list.Count;
			while (i < num2)
			{
				this.registrationDelegate(i, list[i]);
				i++;
			}
		}

		// Token: 0x04000415 RID: 1045
		private readonly Action<int, TEntry> registrationDelegate;

		// Token: 0x04000416 RID: 1046
		private readonly Func<TEntry, string> nameGetter;
	}
}
