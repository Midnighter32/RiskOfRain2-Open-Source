using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000515 RID: 1301
	[RequireComponent(typeof(EffectComponent))]
	public class ItemTakenOrbEffect : MonoBehaviour
	{
		// Token: 0x06001D49 RID: 7497 RVA: 0x00088840 File Offset: 0x00086A40
		private void Start()
		{
			ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)(base.GetComponent<EffectComponent>().effectData.genericUInt - 1u));
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

		// Token: 0x04001F7E RID: 8062
		public TrailRenderer trailToColor;

		// Token: 0x04001F7F RID: 8063
		public SpriteRenderer iconSpriteRenderer;
	}
}
