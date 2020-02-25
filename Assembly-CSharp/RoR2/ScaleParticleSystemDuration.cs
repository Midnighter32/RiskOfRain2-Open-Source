using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200030D RID: 781
	public class ScaleParticleSystemDuration : MonoBehaviour
	{
		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06001258 RID: 4696 RVA: 0x0004F231 File Offset: 0x0004D431
		// (set) Token: 0x06001257 RID: 4695 RVA: 0x0004F219 File Offset: 0x0004D419
		public float newDuration
		{
			get
			{
				return this._newDuration;
			}
			set
			{
				if (this._newDuration != value)
				{
					this._newDuration = value;
					this.UpdateParticleDurations();
				}
			}
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x0004F239 File Offset: 0x0004D439
		private void Start()
		{
			this.UpdateParticleDurations();
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x0004F244 File Offset: 0x0004D444
		private void UpdateParticleDurations()
		{
			float simulationSpeed = this.initialDuration / this._newDuration;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = this.particleSystems[i];
				if (particleSystem)
				{
					particleSystem.main.simulationSpeed = simulationSpeed;
				}
			}
		}

		// Token: 0x0400114C RID: 4428
		public float initialDuration = 1f;

		// Token: 0x0400114D RID: 4429
		private float _newDuration = 1f;

		// Token: 0x0400114E RID: 4430
		public ParticleSystem[] particleSystems;
	}
}
