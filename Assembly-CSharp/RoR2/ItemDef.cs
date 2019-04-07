using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044A RID: 1098
	public class ItemDef
	{
		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06001862 RID: 6242 RVA: 0x000741AC File Offset: 0x000723AC
		public bool inDroppableTier
		{
			get
			{
				return this.tier != ItemTier.NoTier;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x000741BA File Offset: 0x000723BA
		public Texture pickupIconTexture
		{
			get
			{
				return Resources.Load<Texture>(this.pickupIconPath);
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06001864 RID: 6244 RVA: 0x000741C8 File Offset: 0x000723C8
		public Texture bgIconTexture
		{
			get
			{
				switch (this.tier)
				{
				case ItemTier.Tier1:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texTier1BGIcon");
				case ItemTier.Tier2:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texTier2BGIcon");
				case ItemTier.Tier3:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texTier3BGIcon");
				case ItemTier.Lunar:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texLunarBGIcon");
				case ItemTier.Boss:
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texBossBGIcon");
				default:
					return null;
				}
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06001865 RID: 6245 RVA: 0x00074230 File Offset: 0x00072430
		public Sprite pickupIconSprite
		{
			get
			{
				return Resources.Load<Sprite>(this.pickupIconPath);
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06001866 RID: 6246 RVA: 0x00074240 File Offset: 0x00072440
		public ColorCatalog.ColorIndex colorIndex
		{
			get
			{
				switch (this.tier)
				{
				case ItemTier.Tier1:
					return ColorCatalog.ColorIndex.Tier1Item;
				case ItemTier.Tier2:
					return ColorCatalog.ColorIndex.Tier2Item;
				case ItemTier.Tier3:
					return ColorCatalog.ColorIndex.Tier3Item;
				case ItemTier.Lunar:
					return ColorCatalog.ColorIndex.LunarItem;
				case ItemTier.Boss:
					return ColorCatalog.ColorIndex.BossItem;
				default:
					return ColorCatalog.ColorIndex.None;
				}
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06001867 RID: 6247 RVA: 0x0007427C File Offset: 0x0007247C
		public ColorCatalog.ColorIndex darkColorIndex
		{
			get
			{
				switch (this.tier)
				{
				case ItemTier.Tier1:
					return ColorCatalog.ColorIndex.Tier1ItemDark;
				case ItemTier.Tier2:
					return ColorCatalog.ColorIndex.Tier2ItemDark;
				case ItemTier.Tier3:
					return ColorCatalog.ColorIndex.Tier3ItemDark;
				case ItemTier.Lunar:
					return ColorCatalog.ColorIndex.LunarItemDark;
				case ItemTier.Boss:
					return ColorCatalog.ColorIndex.BossItem;
				default:
					return ColorCatalog.ColorIndex.None;
				}
			}
		}

		// Token: 0x04001BFA RID: 7162
		public ItemIndex itemIndex;

		// Token: 0x04001BFB RID: 7163
		public ItemTier tier;

		// Token: 0x04001BFC RID: 7164
		public string pickupModelPath;

		// Token: 0x04001BFD RID: 7165
		public string nameToken;

		// Token: 0x04001BFE RID: 7166
		public string pickupToken;

		// Token: 0x04001BFF RID: 7167
		public string descriptionToken;

		// Token: 0x04001C00 RID: 7168
		public string loreToken;

		// Token: 0x04001C01 RID: 7169
		public string addressToken;

		// Token: 0x04001C02 RID: 7170
		public string pickupIconPath;

		// Token: 0x04001C03 RID: 7171
		public string unlockableName = "";

		// Token: 0x04001C04 RID: 7172
		public bool hidden;

		// Token: 0x04001C05 RID: 7173
		public bool canRemove = true;

		// Token: 0x04001C06 RID: 7174
		public MageElement mageElement;
	}
}
