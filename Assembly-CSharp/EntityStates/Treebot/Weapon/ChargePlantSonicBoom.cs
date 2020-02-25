using System;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x0200074F RID: 1871
	public class ChargePlantSonicBoom : ChargeSonicBoom
	{
		// Token: 0x06002B57 RID: 11095 RVA: 0x000B69E2 File Offset: 0x000B4BE2
		protected override EntityState GetNextState()
		{
			return new FirePlantSonicBoom();
		}
	}
}
