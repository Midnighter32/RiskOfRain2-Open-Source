using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000400 RID: 1024
	public class RuleChoiceDef
	{
		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060018CF RID: 6351 RVA: 0x0006ACA2 File Offset: 0x00068EA2
		public bool isDefaultChoice
		{
			get
			{
				return this.ruleDef.defaultChoiceIndex == this.localIndex;
			}
		}

		// Token: 0x0400172F RID: 5935
		public RuleDef ruleDef;

		// Token: 0x04001730 RID: 5936
		public string spritePath;

		// Token: 0x04001731 RID: 5937
		public string materialPath;

		// Token: 0x04001732 RID: 5938
		public string tooltipNameToken;

		// Token: 0x04001733 RID: 5939
		public Color tooltipNameColor = Color.white;

		// Token: 0x04001734 RID: 5940
		public string tooltipBodyToken;

		// Token: 0x04001735 RID: 5941
		public Color tooltipBodyColor = Color.white;

		// Token: 0x04001736 RID: 5942
		public string localName;

		// Token: 0x04001737 RID: 5943
		public string globalName;

		// Token: 0x04001738 RID: 5944
		public int localIndex;

		// Token: 0x04001739 RID: 5945
		public int globalIndex;

		// Token: 0x0400173A RID: 5946
		public string unlockableName;

		// Token: 0x0400173B RID: 5947
		public bool availableInSinglePlayer = true;

		// Token: 0x0400173C RID: 5948
		public bool availableInMultiPlayer = true;

		// Token: 0x0400173D RID: 5949
		public DifficultyIndex difficultyIndex = DifficultyIndex.Invalid;

		// Token: 0x0400173E RID: 5950
		public ArtifactIndex artifactIndex = ArtifactIndex.None;

		// Token: 0x0400173F RID: 5951
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x04001740 RID: 5952
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x04001741 RID: 5953
		public object extraData;

		// Token: 0x04001742 RID: 5954
		public bool excludeByDefault;
	}
}
