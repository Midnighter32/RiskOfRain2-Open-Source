using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000404 RID: 1028
	public class CharacterSelectBarController : MonoBehaviour
	{
		// Token: 0x060016E6 RID: 5862 RVA: 0x0006D258 File Offset: 0x0006B458
		private bool ShouldDisplaySurvivor(SurvivorDef survivorDef)
		{
			return survivorDef != null;
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x0006D260 File Offset: 0x0006B460
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

		// Token: 0x060016E8 RID: 5864 RVA: 0x0006D322 File Offset: 0x0006B522
		private void Awake()
		{
			this.survivorIconControllers = new UIElementAllocator<SurvivorIconController>(this.iconContainer, this.choiceButtonPrefab);
			this.Build();
		}

		// Token: 0x04001A1C RID: 6684
		public GameObject choiceButtonPrefab;

		// Token: 0x04001A1D RID: 6685
		public GameObject WIPButtonPrefab;

		// Token: 0x04001A1E RID: 6686
		public RectTransform iconContainer;

		// Token: 0x04001A1F RID: 6687
		private UIElementAllocator<SurvivorIconController> survivorIconControllers;
	}
}
