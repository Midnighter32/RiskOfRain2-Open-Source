using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000304 RID: 772
	public class RTPCController : MonoBehaviour
	{
		// Token: 0x060011A1 RID: 4513 RVA: 0x0004CF00 File Offset: 0x0004B100
		private void Start()
		{
			if (this.akSoundString.Length > 0)
			{
				Util.PlaySound(this.akSoundString, base.gameObject, this.rtpcString, this.rtpcValue);
				return;
			}
			AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValue, base.gameObject);
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x0004CF52 File Offset: 0x0004B152
		private void FixedUpdate()
		{
			if (this.useCurveInstead)
			{
				this.fixedAge += Time.fixedDeltaTime;
				AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValueCurve.Evaluate(this.fixedAge), base.gameObject);
			}
		}

		// Token: 0x040010FD RID: 4349
		public string akSoundString;

		// Token: 0x040010FE RID: 4350
		public string rtpcString;

		// Token: 0x040010FF RID: 4351
		public float rtpcValue;

		// Token: 0x04001100 RID: 4352
		public bool useCurveInstead;

		// Token: 0x04001101 RID: 4353
		public AnimationCurve rtpcValueCurve;

		// Token: 0x04001102 RID: 4354
		private float fixedAge;
	}
}
