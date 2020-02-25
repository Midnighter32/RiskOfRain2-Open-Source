using System;
using UnityEngine;

namespace RoR2.WwiseUtils
{
	// Token: 0x02000495 RID: 1173
	public class SoundbankLoader : MonoBehaviour
	{
		// Token: 0x06001C71 RID: 7281 RVA: 0x000799E8 File Offset: 0x00077BE8
		private void Start()
		{
			for (int i = 0; i < this.soundbankStrings.Length; i++)
			{
				AkBankManager.LoadBank(this.soundbankStrings[i], this.decodeBank, this.saveDecodedBank);
			}
		}

		// Token: 0x0400196F RID: 6511
		public string[] soundbankStrings;

		// Token: 0x04001970 RID: 6512
		public bool decodeBank;

		// Token: 0x04001971 RID: 6513
		public bool saveDecodedBank;
	}
}
