using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003B0 RID: 944
	public class ItemDef
	{
		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060016CD RID: 5837 RVA: 0x00061D25 File Offset: 0x0005FF25
		// (set) Token: 0x060016CE RID: 5838 RVA: 0x00061D2D File Offset: 0x0005FF2D
		public ItemIndex itemIndex { get; set; } = ItemIndex.None;

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060016CF RID: 5839 RVA: 0x00061D36 File Offset: 0x0005FF36
		public bool inDroppableTier
		{
			get
			{
				return this.tier != ItemTier.NoTier;
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060016D0 RID: 5840 RVA: 0x00061D44 File Offset: 0x0005FF44
		public Texture pickupIconTexture
		{
			get
			{
				return Resources.Load<Texture>(this.pickupIconPath);
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060016D1 RID: 5841 RVA: 0x00061D54 File Offset: 0x0005FF54
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

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060016D2 RID: 5842 RVA: 0x00061DBC File Offset: 0x0005FFBC
		public Sprite pickupIconSprite
		{
			get
			{
				return Resources.Load<Sprite>(this.pickupIconPath);
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x00061DC9 File Offset: 0x0005FFC9
		public bool ContainsTag(ItemTag tag)
		{
			return tag == ItemTag.Any || Array.IndexOf<ItemTag>(this.tags, tag) != -1;
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x00061DE2 File Offset: 0x0005FFE2
		public bool DoesNotContainTag(ItemTag tag)
		{
			return Array.IndexOf<ItemTag>(this.tags, tag) == -1;
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060016D5 RID: 5845 RVA: 0x00061DF4 File Offset: 0x0005FFF4
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
					return ColorCatalog.ColorIndex.Unaffordable;
				}
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x060016D6 RID: 5846 RVA: 0x00061E34 File Offset: 0x00060034
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
					return ColorCatalog.ColorIndex.BossItemDark;
				default:
					return ColorCatalog.ColorIndex.Unaffordable;
				}
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060016D7 RID: 5847 RVA: 0x00061E75 File Offset: 0x00060075
		public GameObject pickupModelPrefab
		{
			get
			{
				return Resources.Load<GameObject>(this.pickupModelPath);
			}
		}

		// Token: 0x040015CA RID: 5578
		public string name;

		// Token: 0x040015CB RID: 5579
		public ItemTier tier;

		// Token: 0x040015CC RID: 5580
		public string pickupModelPath;

		// Token: 0x040015CD RID: 5581
		public string nameToken;

		// Token: 0x040015CE RID: 5582
		public string pickupToken;

		// Token: 0x040015CF RID: 5583
		public string descriptionToken;

		// Token: 0x040015D0 RID: 5584
		public string loreToken;

		// Token: 0x040015D1 RID: 5585
		public string pickupIconPath;

		// Token: 0x040015D2 RID: 5586
		public string unlockableName = "";

		// Token: 0x040015D3 RID: 5587
		public bool hidden;

		// Token: 0x040015D4 RID: 5588
		public bool canRemove = true;

		// Token: 0x040015D5 RID: 5589
		public ItemTag[] tags = Array.Empty<ItemTag>();
	}
}
