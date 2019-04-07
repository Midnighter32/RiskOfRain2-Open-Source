using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000414 RID: 1044
	public class VFXAttributes : MonoBehaviour
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x0600173E RID: 5950 RVA: 0x0006E70E File Offset: 0x0006C90E
		public static ReadOnlyCollection<VFXAttributes> readonlyVFXList
		{
			get
			{
				return VFXAttributes._readonlyVFXList;
			}
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0006E718 File Offset: 0x0006C918
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

		// Token: 0x06001740 RID: 5952 RVA: 0x0006E748 File Offset: 0x0006C948
		public void OnEnable()
		{
			VFXAttributes.vfxList.Add(this);
			VFXBudget.totalCost += this.GetIntensityScore();
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0006E766 File Offset: 0x0006C966
		public void OnDisable()
		{
			VFXAttributes.vfxList.Remove(this);
			VFXBudget.totalCost -= this.GetIntensityScore();
		}

		// Token: 0x04001A6B RID: 6763
		private static List<VFXAttributes> vfxList = new List<VFXAttributes>();

		// Token: 0x04001A6C RID: 6764
		private static ReadOnlyCollection<VFXAttributes> _readonlyVFXList = new ReadOnlyCollection<VFXAttributes>(VFXAttributes.vfxList);

		// Token: 0x04001A6D RID: 6765
		[Tooltip("Controls whether or not a VFX appears at all - consider if you would notice if this entire VFX never appeared. Also means it has a networking consequence.")]
		public VFXAttributes.VFXPriority vfxPriority;

		// Token: 0x04001A6E RID: 6766
		[Tooltip("Define how expensive a particle system is IF it appears.")]
		public VFXAttributes.VFXIntensity vfxIntensity;

		// Token: 0x04001A6F RID: 6767
		public Light[] optionalLights;

		// Token: 0x04001A70 RID: 6768
		[Tooltip("Particle systems that may be deactivated without impacting gameplay.")]
		public ParticleSystem[] secondaryParticleSystem;

		// Token: 0x02000415 RID: 1045
		public enum VFXPriority
		{
			// Token: 0x04001A72 RID: 6770
			Low,
			// Token: 0x04001A73 RID: 6771
			Medium,
			// Token: 0x04001A74 RID: 6772
			Always
		}

		// Token: 0x02000416 RID: 1046
		public enum VFXIntensity
		{
			// Token: 0x04001A76 RID: 6774
			Low,
			// Token: 0x04001A77 RID: 6775
			Medium,
			// Token: 0x04001A78 RID: 6776
			High
		}
	}
}
