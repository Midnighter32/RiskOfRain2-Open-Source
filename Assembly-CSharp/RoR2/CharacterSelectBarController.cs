using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035D RID: 861
	public class CharacterSelectBarController : MonoBehaviour
	{
		// Token: 0x060014EC RID: 5356 RVA: 0x00059608 File Offset: 0x00057808
		private bool ShouldDisplaySurvivor(SurvivorDef survivorDef)
		{
			return survivorDef != null;
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x00059610 File Offset: 0x00057810
		private void Build()
		{
			List<SurvivorIndex> list = new List<SurvivorIndex>();
			for (int i = 0; i < SurvivorCatalog.idealSurvivorOrder.Length; i++)
			{
				SurvivorIndex survivorIndex = SurvivorCatalog.idealSurvivorOrder[i];
				SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
				if (this.ShouldDisplaySurvivor(survivorDef))
				{
					list.Add(survivorIndex);
				}
			}
			this.survivorIconControllers.AllocateElements(list.Count);
			ReadOnlyCollection<SurvivorIconController> elements = this.survivorIconControllers.elements;
			for (int j = 0; j < list.Count; j++)
			{
				elements[j].survivorIndex = list[j];
			}
			for (int k = list.Count; k < SurvivorCatalog.survivorMaxCount; k++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.WIPButtonPrefab, this.iconContainer).gameObject.SetActive(true);
			}
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x000596D2 File Offset: 0x000578D2
		private void Awake()
		{
			this.survivorIconControllers = new UIElementAllocator<SurvivorIconController>(this.iconContainer, this.choiceButtonPrefab);
			this.Build();
		}

		// Token: 0x0400138C RID: 5004
		public GameObject choiceButtonPrefab;

		// Token: 0x0400138D RID: 5005
		public GameObject WIPButtonPrefab;

		// Token: 0x0400138E RID: 5006
		public RectTransform iconContainer;

		// Token: 0x0400138F RID: 5007
		private UIElementAllocator<SurvivorIconController> survivorIconControllers;
	}
}
