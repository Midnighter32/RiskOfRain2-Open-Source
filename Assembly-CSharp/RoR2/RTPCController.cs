using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003BC RID: 956
	public class RTPCController : MonoBehaviour
	{
		// Token: 0x06001447 RID: 5191 RVA: 0x00062ABC File Offset: 0x00060CBC
		private void Start()
		{
			if (this.akSoundString.Length > 0)
			{
				Util.PlaySound(this.akSoundString, base.gameObject, this.rtpcString, this.rtpcValue);
				return;
			}
			AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValue, base.gameObject);
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x00062B0E File Offset: 0x00060D0E
		private void FixedUpdate()
		{
			if (this.useCurveInstead)
			{
				this.fixedAge += Time.fixedDeltaTime;
				AkSoundEngine.SetRTPCValue(this.rtpcString, this.rtpcValueCurve.Evaluate(this.fixedAge), base.gameObject);
			}
		}

		// Token: 0x040017FA RID: 6138
		public string akSoundString;

		// Token: 0x040017FB RID: 6139
		public string rtpcString;

		// Token: 0x040017FC RID: 6140
		public float rtpcValue;

		// Token: 0x040017FD RID: 6141
		public bool useCurveInstead;

		// Token: 0x040017FE RID: 6142
		public AnimationCurve rtpcValueCurve;

		// Token: 0x040017FF RID: 6143
		private float fixedAge;
	}
}
