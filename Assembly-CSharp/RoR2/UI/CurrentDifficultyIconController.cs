using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AB RID: 1451
	[RequireComponent(typeof(Image))]
	internal class CurrentDifficultyIconController : MonoBehaviour
	{
		// Token: 0x06002283 RID: 8835 RVA: 0x0009578C File Offset: 0x0009398C
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
