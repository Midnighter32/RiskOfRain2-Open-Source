using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002C0 RID: 704
	[ExecuteAlways]
	public class DamageNumberManager : MonoBehaviour
	{
		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x0004674F File Offset: 0x0004494F
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x00046756 File Offset: 0x00044956
		public static DamageNumberManager instance { get; private set; }

		// Token: 0x06000E4D RID: 3661 RVA: 0x0004675E File Offset: 0x0004495E
		private void OnEnable()
		{
			DamageNumberManager.instance = SingletonHelper.Assign<DamageNumberManager>(DamageNumberManager.instance, this);
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x00046770 File Offset: 0x00044970
		private void OnDisable()
		{
			DamageNumberManager.instance = SingletonHelper.Unassign<DamageNumberManager>(DamageNumberManager.instance, this);
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00046782 File Offset: 0x00044982
		private void Start()
		{
			this.ps = base.GetComponent<ParticleSystem>();
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x00046790 File Offset: 0x00044990
		public void SpawnDamageNumber(float amount, Vector3 position, bool crit, TeamIndex teamIndex, DamageColorIndex damageColorIndex)
		{
			Color a = DamageColor.FindColor(damageColorIndex);
			Color white = Color.white;
			if (teamIndex == TeamIndex.Monster)
			{
				white = new Color(0.5568628f, 0.29411766f, 0.6039216f);
			}
			this.ps.Emit(new ParticleSystem.EmitParams
			{
				position = position,
				startColor = a * white,
				applyShapeToPosition = true
			}, 1);
			this.ps.GetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
			this.customData[this.customData.Count - 1] = new Vector4(1f, 0f, amount, crit ? 1f : 0f);
			this.ps.SetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
		}

		// Token: 0x0400123B RID: 4667
		public float damageValueMin;

		// Token: 0x0400123C RID: 4668
		public float damageValueMax;

		// Token: 0x0400123D RID: 4669
		private List<Vector4> customData = new List<Vector4>();

		// Token: 0x0400123E RID: 4670
		private ParticleSystem ps;
	}
}
