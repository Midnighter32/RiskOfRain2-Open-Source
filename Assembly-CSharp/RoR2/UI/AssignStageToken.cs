using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B1 RID: 1457
	public class AssignStageToken : MonoBehaviour
	{
		// Token: 0x060020A5 RID: 8357 RVA: 0x000999E6 File Offset: 0x00097BE6
		private void Start()
		{
			this.titleText.text = Language.GetString(SceneInfo.instance.sceneDef.nameToken);
			this.subtitleText.text = Language.GetString(SceneInfo.instance.sceneDef.subtitleToken);
		}

		// Token: 0x04002330 RID: 9008
		public TextMeshProUGUI titleText;

		// Token: 0x04002331 RID: 9009
		public TextMeshProUGUI subtitleText;
	}
}
