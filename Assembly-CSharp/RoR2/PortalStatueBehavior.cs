using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002C3 RID: 707
	public class PortalStatueBehavior : MonoBehaviour
	{
		// Token: 0x06001019 RID: 4121 RVA: 0x00046CC8 File Offset: 0x00044EC8
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
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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
				EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x04000F8D RID: 3981
		public PortalStatueBehavior.PortalType portalType;

		// Token: 0x020002C4 RID: 708
		public enum PortalType
		{
			// Token: 0x04000F8F RID: 3983
			Shop,
			// Token: 0x04000F90 RID: 3984
			Goldshores,
			// Token: 0x04000F91 RID: 3985
			Count
		}
	}
}
