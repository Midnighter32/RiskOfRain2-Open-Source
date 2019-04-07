using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200038B RID: 907
	public class PortalStatueBehavior : MonoBehaviour
	{
		// Token: 0x0600130C RID: 4876 RVA: 0x0005D55C File Offset: 0x0005B75C
		public void GrantPortalEntry()
		{
			PortalStatueBehavior.PortalType portalType = this.portalType;
			if (portalType != PortalStatueBehavior.PortalType.Shop)
			{
				if (portalType == PortalStatueBehavior.PortalType.Goldshores)
				{
					if (TeleporterInteraction.instance)
					{
						TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal = true;
					}
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
					{
						origin = base.transform.position,
						rotation = Quaternion.identity,
						scale = 1f,
						color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Money)
					}, true);
				}
			}
			else
			{
				if (TeleporterInteraction.instance)
				{
					TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal = true;
				}
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
				{
					origin = base.transform.position,
					rotation = Quaternion.identity,
					scale = 1f,
					color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem)
				}, true);
			}
			foreach (PortalStatueBehavior portalStatueBehavior in UnityEngine.Object.FindObjectsOfType<PortalStatueBehavior>())
			{
				if (portalStatueBehavior.portalType == this.portalType)
				{
					PurchaseInteraction component = portalStatueBehavior.GetComponent<PurchaseInteraction>();
					if (component)
					{
						component.Networkavailable = false;
					}
				}
			}
		}

		// Token: 0x040016D0 RID: 5840
		public PortalStatueBehavior.PortalType portalType;

		// Token: 0x0200038C RID: 908
		public enum PortalType
		{
			// Token: 0x040016D2 RID: 5842
			Shop,
			// Token: 0x040016D3 RID: 5843
			Goldshores,
			// Token: 0x040016D4 RID: 5844
			Count
		}
	}
}
