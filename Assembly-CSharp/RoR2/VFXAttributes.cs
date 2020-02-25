using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000370 RID: 880
	public class VFXAttributes : MonoBehaviour
	{
		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06001575 RID: 5493 RVA: 0x0005B9DE File Offset: 0x00059BDE
		public static ReadOnlyCollection<VFXAttributes> readonlyVFXList
		{
			get
			{
				return VFXAttributes._readonlyVFXList;
			}
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x0005B9E8 File Offset: 0x00059BE8
		public int GetIntensityScore()
		{
			switch (this.vfxIntensity)
			{
			case VFXAttributes.VFXIntensity.Low:
				return 1;
			case VFXAttributes.VFXIntensity.Medium:
				return 5;
			case VFXAttributes.VFXIntensity.High:
				return 25;
			default:
				return 0;
			}
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x0005BA18 File Offset: 0x00059C18
		public void OnEnable()
		{
			VFXAttributes.vfxList.Add(this);
			VFXBudget.totalCost += this.GetIntensityScore();
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x0005BA36 File Offset: 0x00059C36
		public void OnDisable()
		{
			VFXAttributes.vfxList.Remove(this);
			VFXBudget.totalCost -= this.GetIntensityScore();
		}

		// Token: 0x04001401 RID: 5121
		private static List<VFXAttributes> vfxList = new List<VFXAttributes>();

		// Token: 0x04001402 RID: 5122
		private static ReadOnlyCollection<VFXAttributes> _readonlyVFXList = new ReadOnlyCollection<VFXAttributes>(VFXAttributes.vfxList);

		// Token: 0x04001403 RID: 5123
		[Tooltip("Controls whether or not a VFX appears at all - consider if you would notice if this entire VFX never appeared. Also means it has a networking consequence.")]
		public VFXAttributes.VFXPriority vfxPriority;

		// Token: 0x04001404 RID: 5124
		[Tooltip("Define how expensive a particle system is IF it appears.")]
		public VFXAttributes.VFXIntensity vfxIntensity;

		// Token: 0x04001405 RID: 5125
		public Light[] optionalLights;

		// Token: 0x04001406 RID: 5126
		[Tooltip("Particle systems that may be deactivated without impacting gameplay.")]
		public ParticleSystem[] secondaryParticleSystem;

		// Token: 0x02000371 RID: 881
		public enum VFXPriority
		{
			// Token: 0x04001408 RID: 5128
			Low,
			// Token: 0x04001409 RID: 5129
			Medium,
			// Token: 0x0400140A RID: 5130
			Always
		}

		// Token: 0x02000372 RID: 882
		public enum VFXIntensity
		{
			// Token: 0x0400140C RID: 5132
			Low,
			// Token: 0x0400140D RID: 5133
			Medium,
			// Token: 0x0400140E RID: 5134
			High
		}
	}
}
