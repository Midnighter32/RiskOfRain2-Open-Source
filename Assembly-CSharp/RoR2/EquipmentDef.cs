using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000132 RID: 306
	public class EquipmentDef
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x0001627D File Offset: 0x0001447D
		// (set) Token: 0x0600057C RID: 1404 RVA: 0x00016285 File Offset: 0x00014485
		public EquipmentIndex equipmentIndex { get; set; } = EquipmentIndex.None;

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600057D RID: 1405 RVA: 0x0001628E File Offset: 0x0001448E
		public Texture pickupIconTexture
		{
			get
			{
				return Resources.Load<Texture>(this.pickupIconPath);
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x0001629B File Offset: 0x0001449B
		public Sprite pickupIconSprite
		{
			get
			{
				return Resources.Load<Sprite>(this.pickupIconPath);
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x000162A8 File Offset: 0x000144A8
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

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x000162C7 File Offset: 0x000144C7
		public GameObject pickupModelPrefab
		{
			get
			{
				return Resources.Load<GameObject>(this.pickupModelPath);
			}
		}

		// Token: 0x040005E0 RID: 1504
		public string name;

		// Token: 0x040005E1 RID: 1505
		public string pickupModelPath = "Prefabs/NullModel";

		// Token: 0x040005E2 RID: 1506
		public float cooldown;

		// Token: 0x040005E3 RID: 1507
		public string nameToken;

		// Token: 0x040005E4 RID: 1508
		public string pickupToken;

		// Token: 0x040005E5 RID: 1509
		public string descriptionToken;

		// Token: 0x040005E6 RID: 1510
		public string loreToken;

		// Token: 0x040005E7 RID: 1511
		public string addressToken;

		// Token: 0x040005E8 RID: 1512
		public string pickupIconPath = "Textures/ItemIcons/texNullIcon";

		// Token: 0x040005E9 RID: 1513
		public string unlockableName = "";

		// Token: 0x040005EA RID: 1514
		public ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.Equipment;

		// Token: 0x040005EB RID: 1515
		public bool canDrop;

		// Token: 0x040005EC RID: 1516
		public bool enigmaCompatible;

		// Token: 0x040005ED RID: 1517
		public bool isLunar;

		// Token: 0x040005EE RID: 1518
		public bool isBoss;

		// Token: 0x040005EF RID: 1519
		public BuffIndex passiveBuff = BuffIndex.None;

		// Token: 0x040005F0 RID: 1520
		public bool appearsInSinglePlayer = true;

		// Token: 0x040005F1 RID: 1521
		public bool appearsInMultiPlayer = true;

		// Token: 0x040005F2 RID: 1522
		public MageElement mageElement;
	}
}
