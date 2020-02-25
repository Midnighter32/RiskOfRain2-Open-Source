using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004CF RID: 1231
	[RequireComponent(typeof(EffectComponent))]
	public class ItemTakenOrbEffect : MonoBehaviour
	{
		// Token: 0x06001D7A RID: 7546 RVA: 0x0007DAD8 File Offset: 0x0007BCD8
		private void Start()
		{
			ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)(base.GetComponent<EffectComponent>().effectData.genericUInt - 1U));
			ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.Error;
			Sprite sprite = null;
			if (itemDef != null)
			{
				colorIndex = itemDef.colorIndex;
				sprite = itemDef.pickupIconSprite;
			}
			this.trailToColor.endColor = (this.trailToColor.startColor = ColorCatalog.GetColor(colorIndex));
			this.iconSpriteRenderer.sprite = sprite;
		}

		// Token: 0x04001A91 RID: 6801
		public TrailRenderer trailToColor;

		// Token: 0x04001A92 RID: 6802
		public SpriteRenderer iconSpriteRenderer;
	}
}
