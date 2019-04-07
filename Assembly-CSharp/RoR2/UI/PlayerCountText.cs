using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000621 RID: 1569
	public class PlayerCountText : MonoBehaviour
	{
		// Token: 0x06002346 RID: 9030 RVA: 0x000A6430 File Offset: 0x000A4630
		private void Update()
		{
			if (this.targetText)
			{
				this.targetText.text = string.Format("{0}/{1}", NetworkUser.readOnlyInstancesList.Count, NetworkManager.singleton.maxConnections);
			}
		}

		// Token: 0x04002644 RID: 9796
		public Text targetText;
	}
}
