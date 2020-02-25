using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200058B RID: 1419
	public class AssignStageToken : MonoBehaviour
	{
		// Token: 0x060021C7 RID: 8647 RVA: 0x000921C2 File Offset: 0x000903C2
		private void Start()
		{
			this.titleText.text = Language.GetString(SceneInfo.instance.sceneDef.nameToken);
			this.subtitleText.text = Language.GetString(SceneInfo.instance.sceneDef.subtitleToken);
		}

		// Token: 0x04001F26 RID: 7974
		public TextMeshProUGUI titleText;

		// Token: 0x04001F27 RID: 7975
		public TextMeshProUGUI subtitleText;
	}
}
