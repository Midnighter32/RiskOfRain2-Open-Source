using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CC RID: 1484
	[RequireComponent(typeof(Image))]
	internal class CurrentDifficultyIconController : MonoBehaviour
	{
		// Token: 0x06002153 RID: 8531 RVA: 0x0009C834 File Offset: 0x0009AA34
		private void Start()
		{
			if (Run.instance)
			{
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty);
				base.GetComponent<Image>().sprite = difficultyDef.GetIconSprite();
			}
		}
	}
}
