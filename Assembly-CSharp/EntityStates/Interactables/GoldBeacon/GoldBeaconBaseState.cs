using System;
using RoR2;
using RoR2.Hologram;
using UnityEngine;

namespace EntityStates.Interactables.GoldBeacon
{
	// Token: 0x0200013A RID: 314
	public class GoldBeaconBaseState : BaseState
	{
		// Token: 0x06000606 RID: 1542 RVA: 0x0001BC5C File Offset: 0x00019E5C
		protected void SetReady(bool ready)
		{
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				PrintController component = modelTransform.GetComponent<PrintController>();
				component.paused = !ready;
				if (!ready)
				{
					component.age = 0f;
				}
				ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					component2.FindChild("Purchased").gameObject.SetActive(ready);
				}
			}
			PurchaseInteraction component3 = base.GetComponent<PurchaseInteraction>();
			if (component3)
			{
				component3.SetAvailable(!ready);
			}
			HologramProjector component4 = base.GetComponent<HologramProjector>();
			if (component4)
			{
				component4.hologramPivot.gameObject.SetActive(!ready);
			}
		}
	}
}
