using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047E RID: 1150
	public class RuleChoiceDef
	{
		// Token: 0x04001D04 RID: 7428
		public RuleDef ruleDef;

		// Token: 0x04001D05 RID: 7429
		public string spritePath;

		// Token: 0x04001D06 RID: 7430
		public string materialPath;

		// Token: 0x04001D07 RID: 7431
		public string tooltipNameToken;

		// Token: 0x04001D08 RID: 7432
		public Color tooltipNameColor = Color.white;

		// Token: 0x04001D09 RID: 7433
		public string tooltipBodyToken;

		// Token: 0x04001D0A RID: 7434
		public Color tooltipBodyColor = Color.white;

		// Token: 0x04001D0B RID: 7435
		public string localName;

		// Token: 0x04001D0C RID: 7436
		public string globalName;

		// Token: 0x04001D0D RID: 7437
		public int localIndex;

		// Token: 0x04001D0E RID: 7438
		public int globalIndex;

		// Token: 0x04001D0F RID: 7439
		public string unlockableName;

		// Token: 0x04001D10 RID: 7440
		public bool availableInSinglePlayer = true;

		// Token: 0x04001D11 RID: 7441
		public bool availableInMultiPlayer = true;

		// Token: 0x04001D12 RID: 7442
		public DifficultyIndex difficultyIndex = DifficultyIndex.Invalid;

		// Token: 0x04001D13 RID: 7443
		public ArtifactIndex artifactIndex = ArtifactIndex.None;

		// Token: 0x04001D14 RID: 7444
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x04001D15 RID: 7445
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x04001D16 RID: 7446
		public object extraData;

		// Token: 0x04001D17 RID: 7447
		public bool excludeByDefault;
	}
}
