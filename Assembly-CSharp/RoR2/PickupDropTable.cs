using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E0 RID: 992
	public abstract class PickupDropTable : ScriptableObject
	{
		// Token: 0x06001810 RID: 6160
		public abstract PickupIndex GenerateDrop(Xoroshiro128Plus rng);

		// Token: 0x06001811 RID: 6161 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnRunStart(Run run)
		{
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x00068C61 File Offset: 0x00066E61
		protected void OnEnable()
		{
			PickupDropTable.instancesList.Add(this);
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x00068C6E File Offset: 0x00066E6E
		protected void OnDisable()
		{
			PickupDropTable.instancesList.Remove(this);
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x00068C7C File Offset: 0x00066E7C
		static PickupDropTable()
		{
			Run.onRunStartGlobal += PickupDropTable.RegenerateAll;
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x00068C9C File Offset: 0x00066E9C
		private static void RegenerateAll(Run run)
		{
			for (int i = 0; i < PickupDropTable.instancesList.Count; i++)
			{
				PickupDropTable.instancesList[i].OnRunStart(run);
			}
		}

		// Token: 0x040016CA RID: 5834
		private static readonly List<PickupDropTable> instancesList = new List<PickupDropTable>();
	}
}
