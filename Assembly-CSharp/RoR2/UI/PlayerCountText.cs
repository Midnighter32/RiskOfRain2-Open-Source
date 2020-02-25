using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000610 RID: 1552
	public class PlayerCountText : MonoBehaviour
	{
		// Token: 0x060024C5 RID: 9413 RVA: 0x000A0810 File Offset: 0x0009EA10
		private void Update()
		{
			if (this.targetText)
			{
				this.targetText.text = string.Format("{0}/{1}", NetworkUser.readOnlyInstancesList.Count, NetworkManager.singleton.maxConnections);
			}
		}

		// Token: 0x0400228D RID: 8845
		public Text targetText;
	}
}
