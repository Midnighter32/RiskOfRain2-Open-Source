using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000466 RID: 1126
	[Serializable]
	public struct PickupIndex : IEquatable<PickupIndex>
	{
		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600191F RID: 6431 RVA: 0x00078B31 File Offset: 0x00076D31
		public bool isValid
		{
			get
			{
				return this.value < 106;
			}
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x00078B3D File Offset: 0x00076D3D
		private PickupIndex(int value)
		{
			this.value = ((value < 0) ? -1 : value);
		}

		// Token: 0x06001921 RID: 6433 RVA: 0x00078B3D File Offset: 0x00076D3D
		public PickupIndex(ItemIndex itemIndex)
		{
			this.value = (int)((itemIndex < ItemIndex.Syringe) ? ItemIndex.None : itemIndex);
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x00078B4D File Offset: 0x00076D4D
		public PickupIndex(EquipmentIndex equipmentIndex)
		{
			this.value = (int)((equipmentIndex < EquipmentIndex.CommandMissile) ? EquipmentIndex.None : (78 + equipmentIndex));
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x00078B60 File Offset: 0x00076D60
		public GameObject GetHiddenPickupDisplayPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x00078B6C File Offset: 0x00076D6C
		public GameObject GetPickupDisplayPrefab()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return Resources.Load<GameObject>(ItemCatalog.GetItemDef((ItemIndex)this.value).pickupModelPath);
				}
				if (this.value < 105)
				{
					return Resources.Load<GameObject>(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).pickupModelPath);
				}
				if (this.value < 106)
				{
					return Resources.Load<GameObject>("Prefabs/PickupModels/PickupLunarCoin");
				}
			}
			return null;
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x00078BDC File Offset: 0x00076DDC
		public GameObject GetPickupDropletDisplayPrefab()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)this.value);
					string path = null;
					switch (itemDef.tier)
					{
					case ItemTier.Tier1:
						path = "Prefabs/ItemPickups/Tier1Orb";
						break;
					case ItemTier.Tier2:
						path = "Prefabs/ItemPickups/Tier2Orb";
						break;
					case ItemTier.Tier3:
						path = "Prefabs/ItemPickups/Tier3Orb";
						break;
					case ItemTier.Lunar:
						path = "Prefabs/ItemPickups/LunarOrb";
						break;
					}
					if (!string.IsNullOrEmpty(path))
					{
						return Resources.Load<GameObject>(path);
					}
					return null;
				}
				else
				{
					if (this.value < 105)
					{
						return Resources.Load<GameObject>("Prefabs/ItemPickups/EquipmentOrb");
					}
					if (this.value < 106)
					{
						return Resources.Load<GameObject>("Prefabs/ItemPickups/LunarOrb");
					}
				}
			}
			return null;
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x00078C84 File Offset: 0x00076E84
		public Color GetPickupColor()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ColorCatalog.GetColor(ItemCatalog.GetItemDef((ItemIndex)this.value).colorIndex);
				}
				if (this.value < 105)
				{
					return ColorCatalog.GetColor(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).colorIndex);
				}
				if (this.value < 106)
				{
					return ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
				}
			}
			return Color.black;
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x00078D04 File Offset: 0x00076F04
		public Color GetPickupColorDark()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ColorCatalog.GetColor(ItemCatalog.GetItemDef((ItemIndex)this.value).darkColorIndex);
				}
				if (this.value < 105)
				{
					return ColorCatalog.GetColor(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).colorIndex);
				}
				if (this.value < 106)
				{
					return ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
				}
			}
			return Color.black;
		}

		// Token: 0x06001928 RID: 6440 RVA: 0x00078D84 File Offset: 0x00076F84
		public string GetPickupNameToken()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).nameToken;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).nameToken;
				}
				if (this.value < 106)
				{
					return "PICKUP_LUNAR_COIN";
				}
			}
			return "???";
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x00078DE8 File Offset: 0x00076FE8
		public string GetUnlockableName()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).unlockableName;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).unlockableName;
				}
			}
			return "";
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x00078E3C File Offset: 0x0007703C
		public bool IsLunar()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).tier == ItemTier.Lunar;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).isLunar;
				}
			}
			return false;
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x00078E90 File Offset: 0x00077090
		public bool IsBoss()
		{
			if (this.value >= 0)
			{
				if (this.value < 78)
				{
					return ItemCatalog.GetItemDef((ItemIndex)this.value).tier == ItemTier.Boss;
				}
				if (this.value < 105)
				{
					return EquipmentCatalog.GetEquipmentDef((EquipmentIndex)(this.value - 78)).isBoss;
				}
			}
			return false;
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x0600192C RID: 6444 RVA: 0x00078EE3 File Offset: 0x000770E3
		public ItemIndex itemIndex
		{
			get
			{
				if (this.value < 0 || this.value >= 78)
				{
					return ItemIndex.None;
				}
				return (ItemIndex)this.value;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x0600192D RID: 6445 RVA: 0x00078F00 File Offset: 0x00077100
		public EquipmentIndex equipmentIndex
		{
			get
			{
				if (this.value < 78 || this.value >= 105)
				{
					return EquipmentIndex.None;
				}
				return (EquipmentIndex)(this.value - 78);
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600192E RID: 6446 RVA: 0x00078F21 File Offset: 0x00077121
		public int coinIndex
		{
			get
			{
				if (this.value < 105 || this.value >= 106)
				{
					return -1;
				}
				return this.value - 105;
			}
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x00078F42 File Offset: 0x00077142
		public static bool operator ==(PickupIndex a, PickupIndex b)
		{
			return a.value == b.value;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x00078F52 File Offset: 0x00077152
		public static bool operator !=(PickupIndex a, PickupIndex b)
		{
			return a.value != b.value;
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x00078F65 File Offset: 0x00077165
		public static bool operator <(PickupIndex a, PickupIndex b)
		{
			return a.value < b.value;
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x00078F75 File Offset: 0x00077175
		public static bool operator >(PickupIndex a, PickupIndex b)
		{
			return a.value > b.value;
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x00078F85 File Offset: 0x00077185
		public static bool operator <=(PickupIndex a, PickupIndex b)
		{
			return a.value >= b.value;
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x00078F98 File Offset: 0x00077198
		public static bool operator >=(PickupIndex a, PickupIndex b)
		{
			return a.value <= b.value;
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x00078FAB File Offset: 0x000771AB
		public static PickupIndex operator ++(PickupIndex a)
		{
			return new PickupIndex(a.value + 1);
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x00078FBA File Offset: 0x000771BA
		public static PickupIndex operator --(PickupIndex a)
		{
			return new PickupIndex(a.value - 1);
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x00078FC9 File Offset: 0x000771C9
		public override bool Equals(object obj)
		{
			return obj is PickupIndex && this == (PickupIndex)obj;
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x00078F42 File Offset: 0x00077142
		public bool Equals(PickupIndex other)
		{
			return this.value == other.value;
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x00078FE8 File Offset: 0x000771E8
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x00079003 File Offset: 0x00077203
		public static void WriteToNetworkWriter(NetworkWriter writer, PickupIndex value)
		{
			writer.Write((byte)(value.value + 1));
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x00079014 File Offset: 0x00077214
		public static PickupIndex ReadFromNetworkReader(NetworkReader reader)
		{
			return new PickupIndex((int)(reader.ReadByte() - 1));
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00079024 File Offset: 0x00077224
		static PickupIndex()
		{
			PickupIndex.allPickupNames[0] = "None";
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				PickupIndex.allPickupNames[(int)(1 + itemIndex)] = "ItemIndex." + itemIndex.ToString();
			}
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				PickupIndex.allPickupNames[(int)(79 + equipmentIndex)] = "EquipmentIndex." + equipmentIndex.ToString();
			}
			for (int i = 105; i < 106; i++)
			{
				PickupIndex.allPickupNames[1 + i] = "LunarCoin.Coin" + (i - 105);
			}
			PickupIndex.stringToPickupIndexTable = new Dictionary<string, PickupIndex>();
			for (int j = 0; j < PickupIndex.allPickupNames.Length; j++)
			{
				PickupIndex.stringToPickupIndexTable.Add(PickupIndex.allPickupNames[j], new PickupIndex(j - 1));
			}
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00079140 File Offset: 0x00077340
		public override string ToString()
		{
			int num = this.value + 1;
			if (num > -1 && num < PickupIndex.allPickupNames.Length)
			{
				return PickupIndex.allPickupNames[num];
			}
			return string.Format("BadPickupIndex{0}", this.value);
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x00079184 File Offset: 0x00077384
		public static PickupIndex Find(string name)
		{
			PickupIndex result;
			if (PickupIndex.stringToPickupIndexTable.TryGetValue(name, out result))
			{
				return result;
			}
			return PickupIndex.none;
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x000791A8 File Offset: 0x000773A8
		public static PickupIndex.Enumerator GetEnumerator()
		{
			return default(PickupIndex.Enumerator);
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001940 RID: 6464 RVA: 0x000791C0 File Offset: 0x000773C0
		public static GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator> allPickups
		{
			get
			{
				return default(GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator>);
			}
		}

		// Token: 0x04001CA7 RID: 7335
		public static readonly PickupIndex none = new PickupIndex(-1);

		// Token: 0x04001CA8 RID: 7336
		[SerializeField]
		public readonly int value;

		// Token: 0x04001CA9 RID: 7337
		private const int pickupsStart = -1;

		// Token: 0x04001CAA RID: 7338
		private const int itemStart = 0;

		// Token: 0x04001CAB RID: 7339
		private const int itemEnd = 78;

		// Token: 0x04001CAC RID: 7340
		private const int equipmentStart = 78;

		// Token: 0x04001CAD RID: 7341
		private const int equipmentEnd = 105;

		// Token: 0x04001CAE RID: 7342
		private const int coinsStart = 105;

		// Token: 0x04001CAF RID: 7343
		private const int coinsCount = 1;

		// Token: 0x04001CB0 RID: 7344
		private const int coinsEnd = 106;

		// Token: 0x04001CB1 RID: 7345
		public static readonly PickupIndex lunarCoin1 = new PickupIndex(105);

		// Token: 0x04001CB2 RID: 7346
		private const int pickupsEnd = 106;

		// Token: 0x04001CB3 RID: 7347
		public static readonly PickupIndex first = new PickupIndex(0);

		// Token: 0x04001CB4 RID: 7348
		public static readonly PickupIndex last = new PickupIndex(105);

		// Token: 0x04001CB5 RID: 7349
		public static readonly PickupIndex end = new PickupIndex(106);

		// Token: 0x04001CB6 RID: 7350
		public const int count = 106;

		// Token: 0x04001CB7 RID: 7351
		public static readonly string[] allPickupNames = new string[107];

		// Token: 0x04001CB8 RID: 7352
		private static readonly Dictionary<string, PickupIndex> stringToPickupIndexTable;

		// Token: 0x02000467 RID: 1127
		public struct Enumerator : IEnumerator<PickupIndex>, IEnumerator, IDisposable
		{
			// Token: 0x06001941 RID: 6465 RVA: 0x000791D6 File Offset: 0x000773D6
			public bool MoveNext()
			{
				this.position = ++this.position;
				return this.position < PickupIndex.end;
			}

			// Token: 0x06001942 RID: 6466 RVA: 0x000791F9 File Offset: 0x000773F9
			public void Reset()
			{
				this.position = PickupIndex.none;
			}

			// Token: 0x17000258 RID: 600
			// (get) Token: 0x06001943 RID: 6467 RVA: 0x00079206 File Offset: 0x00077406
			public PickupIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x17000259 RID: 601
			// (get) Token: 0x06001944 RID: 6468 RVA: 0x0007920E File Offset: 0x0007740E
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x06001945 RID: 6469 RVA: 0x00004507 File Offset: 0x00002707
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04001CB9 RID: 7353
			private PickupIndex position;
		}
	}
}
