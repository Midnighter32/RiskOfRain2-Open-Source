using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000203 RID: 515
	public class FlickerLight : MonoBehaviour
	{
		// Token: 0x06000AFF RID: 2815 RVA: 0x00030C70 File Offset: 0x0002EE70
		private void Start()
		{
			this.initialLightIntensity = this.light.intensity;
			this.randomPhase = UnityEngine.Random.Range(0f, 6.2831855f);
			for (int i = 0; i < this.sinWaves.Length; i++)
			{
				Wave[] array = this.sinWaves;
				int num = i;
				array[num].cycleOffset = array[num].cycleOffset + this.randomPhase;
			}
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00030CD4 File Offset: 0x0002EED4
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			float num = this.initialLightIntensity;
			for (int i = 0; i < this.sinWaves.Length; i++)
			{
				num *= 1f + this.sinWaves[i].Evaluate(this.stopwatch);
			}
			this.light.intensity = num;
		}

		// Token: 0x04000B6B RID: 2923
		public Light light;

		// Token: 0x04000B6C RID: 2924
		public Wave[] sinWaves;

		// Token: 0x04000B6D RID: 2925
		private float initialLightIntensity;

		// Token: 0x04000B6E RID: 2926
		private float stopwatch;

		// Token: 0x04000B6F RID: 2927
		private float randomPhase;
	}
}
