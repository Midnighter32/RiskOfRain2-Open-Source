using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000634 RID: 1588
	public class SelectableDescriptionUpdater : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x060023A1 RID: 9121 RVA: 0x000A7AD4 File Offset: 0x000A5CD4
		public void OnPointerExit(PointerEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x000A7AD4 File Offset: 0x000A5CD4
		public void OnDeselect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x000A7AE6 File Offset: 0x000A5CE6
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x000A7AE6 File Offset: 0x000A5CE6
		public void OnSelect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x04002698 RID: 9880
		public LanguageTextMeshController languageTextMeshController;

		// Token: 0x04002699 RID: 9881
		public string selectableDescriptionToken;
	}
}
