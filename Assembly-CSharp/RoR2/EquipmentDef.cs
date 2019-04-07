using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000244 RID: 580
	public class EquipmentDef
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x00035B80 File Offset: 0x00033D80
		public Texture pickupIconTexture
		{
			get
			{
				return Resources.Load<Texture>(this.pickupIconPath);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x00035B8D File Offset: 0x00033D8D
		public Sprite pickupIconSprite
		{
			get
			{
				return Resources.Load<Sprite>(this.pickupIconPath);
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x00035B9A File Offset: 0x00033D9A
		public Texture bgIconTexture
		{
			get
			{
				if (this.isLunar)
				{
					return Resources.Load<Texture>("Textures/ItemIcons/BG/texLunarBGIcon");
				}
				return Resources.Load<Texture>("Textures/ItemIcons/BG/texEquipmentBGIcon");
			}
		}

		// Token: 0x04000ECC RID: 3788
		public EquipmentIndex equipmentIndex;

		// Token: 0x04000ECD RID: 3789
		public string pickupModelPath = "Prefabs/NullModel";

		// Token: 0x04000ECE RID: 3790
		public float cooldown;

		// Token: 0x04000ECF RID: 3791
		public string nameToken;

		// Token: 0x04000ED0 RID: 3792
		public string pickupToken;

		// Token: 0x04000ED1 RID: 3793
		public string descriptionToken;

		// Token: 0x04000ED2 RID: 3794
		public string loreToken;

		// Token: 0x04000ED3 RID: 3795
		public string addressToken;

		// Token: 0x04000ED4 RID: 3796
		public string pickupIconPath = "Textures/ItemIcons/texNullIcon";

		// Token: 0x04000ED5 RID: 3797
		public string unlockableName = "";

		// Token: 0x04000ED6 RID: 3798
		public ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.Equipment;

		// Token: 0x04000ED7 RID: 3799
		public bool canDrop;

		// Token: 0x04000ED8 RID: 3800
		public bool enigmaCompatible;

		// Token: 0x04000ED9 RID: 3801
		public bool isLunar;

		// Token: 0x04000EDA RID: 3802
		public bool isBoss;

		// Token: 0x04000EDB RID: 3803
		public BuffIndex passiveBuff = BuffIndex.None;

		// Token: 0x04000EDC RID: 3804
		public bool appearsInSinglePlayer = true;

		// Token: 0x04000EDD RID: 3805
		public bool appearsInMultiPlayer = true;

		// Token: 0x04000EDE RID: 3806
		public MageElement mageElement;
	}
}
