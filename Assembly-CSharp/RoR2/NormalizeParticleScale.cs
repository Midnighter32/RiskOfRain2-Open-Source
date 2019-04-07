using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000375 RID: 885
	[RequireComponent(typeof(ParticleSystem))]
	[ExecuteInEditMode]
	public class NormalizeParticleScale : MonoBehaviour
	{
		// Token: 0x06001291 RID: 4753 RVA: 0x0005AFC8 File Offset: 0x000591C8
		public void OnEnable()
		{
			this.UpdateParticleSystem();
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x0005AFD0 File Offset: 0x000591D0
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

		// Token: 0x0400163B RID: 5691
		private ParticleSystem particleSystem;
	}
}
