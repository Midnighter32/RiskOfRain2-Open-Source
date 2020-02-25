using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000622 RID: 1570
	public class SelectableDescriptionUpdater : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x0600251E RID: 9502 RVA: 0x000A1E68 File Offset: 0x000A0068
		public void OnPointerExit(PointerEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x000A1E68 File Offset: 0x000A0068
		public void OnDeselect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = "";
		}

		// Token: 0x06002520 RID: 9504 RVA: 0x000A1E7A File Offset: 0x000A007A
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x000A1E7A File Offset: 0x000A007A
		public void OnSelect(BaseEventData eventData)
		{
			this.languageTextMeshController.token = this.selectableDescriptionToken;
		}

		// Token: 0x040022DF RID: 8927
		public LanguageTextMeshController languageTextMeshController;

		// Token: 0x040022E0 RID: 8928
		public string selectableDescriptionToken;
	}
}
