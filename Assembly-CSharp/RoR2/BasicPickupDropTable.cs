using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000C2 RID: 194
	[CreateAssetMenu(menuName = "RoR2/DropTables/BasicPickupDropTable")]
	public class BasicPickupDropTable : PickupDropTable
	{
		// Token: 0x060003CA RID: 970 RVA: 0x0000EA02 File Offset: 0x0000CC02
		protected override void OnRunStart(Run run)
		{
			this.GenerateWeightedSelection(run);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0000EA0C File Offset: 0x0000CC0C
		private void GenerateWeightedSelection(Run run)
		{
			this.selector.Clear();
			this.selector.AddChoice(run.availableTier1DropList, this.tier1Weight);
			this.selector.AddChoice(run.availableTier2DropList, this.tier2Weight);
			this.selector.AddChoice(run.availableTier3DropList, this.tier3Weight);
			this.selector.AddChoice(run.availableLunarDropList, this.lunarWeight);
			this.selector.AddChoice(run.availableEquipmentDropList, this.equipmentWeight);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0000EA98 File Offset: 0x0000CC98
		public override PickupIndex GenerateDrop(Xoroshiro128Plus rng)
		{
			List<PickupIndex> list = this.selector.Evaluate(rng.nextNormalizedFloat);
			if (list.Count <= 0)
			{
				return PickupIndex.none;
			}
			return rng.NextElementUniform<PickupIndex>(list);
		}

		// Token: 0x04000356 RID: 854
		public float tier1Weight = 0.8f;

		// Token: 0x04000357 RID: 855
		public float tier2Weight = 0.2f;

		// Token: 0x04000358 RID: 856
		public float tier3Weight = 0.01f;

		// Token: 0x04000359 RID: 857
		public float lunarWeight;

		// Token: 0x0400035A RID: 858
		public float equipmentWeight;

		// Token: 0x0400035B RID: 859
		private readonly WeightedSelection<List<PickupIndex>> selector = new WeightedSelection<List<PickupIndex>>(8);
	}
}
