using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000373 RID: 883
	public static class VFXBudget
	{
		// Token: 0x0600157B RID: 5499 RVA: 0x0005BA70 File Offset: 0x00059C70
		public static bool CanAffordSpawn(GameObject prefab)
		{
			return VFXBudget.CanAffordSpawn(prefab.GetComponent<VFXAttributes>());
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x0005BA80 File Offset: 0x00059C80
		public static bool CanAffordSpawn(VFXAttributes vfxAttributes)
		{
			if (vfxAttributes == null)
			{
				return true;
			}
			int intensityScore = vfxAttributes.GetIntensityScore();
			int num = VFXBudget.totalCost + intensityScore + VFXBudget.particleCostBias.value;
			switch (vfxAttributes.vfxPriority)
			{
			case VFXAttributes.VFXPriority.Low:
				return Mathf.Pow((float)VFXBudget.lowPriorityCostThreshold.value / (float)num, VFXBudget.chanceFailurePower) > UnityEngine.Random.value;
			case VFXAttributes.VFXPriority.Medium:
				return Mathf.Pow((float)VFXBudget.mediumPriorityCostThreshold.value / (float)num, VFXBudget.chanceFailurePower) > UnityEngine.Random.value;
			case VFXAttributes.VFXPriority.Always:
				return true;
			default:
				return true;
			}
		}

		// Token: 0x0400140F RID: 5135
		public static int totalCost = 0;

		// Token: 0x04001410 RID: 5136
		private static IntConVar lowPriorityCostThreshold = new IntConVar("vfxbudget_low_priority_cost_threshold", ConVarFlags.None, "50", "");

		// Token: 0x04001411 RID: 5137
		private static IntConVar mediumPriorityCostThreshold = new IntConVar("vfxbudget_medium_priority_cost_threshold", ConVarFlags.None, "200", "");

		// Token: 0x04001412 RID: 5138
		private static IntConVar particleCostBias = new IntConVar("vfxbudget_particle_cost_bias", ConVarFlags.Archive, "0", "");

		// Token: 0x04001413 RID: 5139
		private static float chanceFailurePower = 1f;
	}
}
