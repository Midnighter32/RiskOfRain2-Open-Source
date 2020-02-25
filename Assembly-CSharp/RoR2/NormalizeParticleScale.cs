using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A6 RID: 678
	[ExecuteInEditMode]
	[RequireComponent(typeof(ParticleSystem))]
	public class NormalizeParticleScale : MonoBehaviour
	{
		// Token: 0x06000F79 RID: 3961 RVA: 0x00044250 File Offset: 0x00042450
		public void OnEnable()
		{
			this.UpdateParticleSystem();
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x00044258 File Offset: 0x00042458
		private void UpdateParticleSystem()
		{
			if (!this.particleSystem)
			{
				this.particleSystem = base.GetComponent<ParticleSystem>();
			}
			ParticleSystem.MainModule main = this.particleSystem.main;
			ParticleSystem.MinMaxCurve startSize = main.startSize;
			Vector3 lossyScale = base.transform.lossyScale;
			float num = Mathf.Max(new float[]
			{
				lossyScale.x,
				lossyScale.y,
				lossyScale.z
			});
			startSize.constantMin /= num;
			startSize.constantMax /= num;
			main.startSize = startSize;
		}

		// Token: 0x04000EEC RID: 3820
		private ParticleSystem particleSystem;
	}
}
