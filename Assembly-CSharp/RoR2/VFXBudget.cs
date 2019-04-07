using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000417 RID: 1047
	public static class VFXBudget
	{
		// Token: 0x06001744 RID: 5956 RVA: 0x0006E7A0 File Offset: 0x0006C9A0
		public static bool CanAffordSpawn(GameObject prefab)
		{
			VFXAttributes component = prefab.GetComponent<VFXAttributes>();
			if (component)
			{
				int intensityScore = component.GetIntensityScore();
				int num = VFXBudget.totalCost + intensityScore + VFXBudget.particleCostBias.value;
				switch (component.vfxPriority)
				{
				case VFXAttributes.VFXPriority.Low:
					return Mathf.Pow((float)VFXBudget.lowPriorityCostThreshold.value / (float)num, VFXBudget.chanceFailurePower) > UnityEngine.Random.value;
				case VFXAttributes.VFXPriority.Medium:
					return Mathf.Pow((float)VFXBudget.mediumPriorityCostThreshold.value / (float)num, VFXBudget.chanceFailurePower) > UnityEngine.Random.value;
				case VFXAttributes.VFXPriority.Always:
					return true;
				}
			}
			return true;
		}

		// Token: 0x04001A79 RID: 6777
		public static int totalCost = 0;

		// Token: 0x04001A7A RID: 6778
		private static IntConVar lowPriorityCostThreshold = new IntConVar("vfxbudget_low_priority_cost_threshold", ConVarFlags.None, "50", "");

		// Token: 0x04001A7B RID: 6779
		private static IntConVar mediumPriorityCostThreshold = new IntConVar("vfxbudget_medium_priority_cost_threshold", ConVarFlags.None, "200", "");

		// Token: 0x04001A7C RID: 6780
		private static IntConVar particleCostBias = new IntConVar("vfxbudget_particle_cost_bias", ConVarFlags.None, "0", "");

		// Token: 0x04001A7D RID: 6781
		private static float chanceFailurePower = 1f;
	}
}
