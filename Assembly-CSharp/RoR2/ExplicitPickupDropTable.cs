using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000136 RID: 310
	[CreateAssetMenu(menuName = "RoR2/DropTables/ExplicitPickupDropTable")]
	public class ExplicitPickupDropTable : PickupDropTable
	{
		// Token: 0x06000594 RID: 1428 RVA: 0x00017272 File Offset: 0x00015472
		protected override void OnRunStart(Run run)
		{
			this.GenerateWeightedSelection();
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0001727C File Offset: 0x0001547C
		private void GenerateWeightedSelection()
		{
			this.weightedSelection.Clear();
			foreach (ExplicitPickupDropTable.Entry entry in this.entries)
			{
				this.weightedSelection.AddChoice(PickupCatalog.FindPickupIndex(entry.pickupName), entry.pickupWeight);
			}
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x000172CD File Offset: 0x000154CD
		public override PickupIndex GenerateDrop(Xoroshiro128Plus rng)
		{
			return this.weightedSelection.Evaluate(rng.nextNormalizedFloat);
		}

		// Token: 0x040005FD RID: 1533
		public ExplicitPickupDropTable.Entry[] entries = Array.Empty<ExplicitPickupDropTable.Entry>();

		// Token: 0x040005FE RID: 1534
		private readonly WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>(8);

		// Token: 0x02000137 RID: 311
		[Serializable]
		public struct Entry
		{
			// Token: 0x040005FF RID: 1535
			public string pickupName;

			// Token: 0x04000600 RID: 1536
			public float pickupWeight;
		}
	}
}
