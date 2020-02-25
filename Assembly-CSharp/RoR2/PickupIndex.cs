using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E1 RID: 993
	[Serializable]
	public struct PickupIndex : IEquatable<PickupIndex>
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06001817 RID: 6167 RVA: 0x00068CCF File Offset: 0x00066ECF
		public bool isValid
		{
			get
			{
				return (ulong)this.value < (ulong)((long)PickupCatalog.pickupCount);
			}
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x00068CE0 File Offset: 0x00066EE0
		public PickupIndex(int value)
		{
			this.value = ((value < 0) ? -1 : value);
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x00068CF0 File Offset: 0x00066EF0
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public PickupIndex(ItemIndex itemIndex)
		{
			this.value = PickupCatalog.FindPickupIndex(itemIndex).value;
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x00068D03 File Offset: 0x00066F03
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public PickupIndex(EquipmentIndex equipmentIndex)
		{
			this.value = PickupCatalog.FindPickupIndex(equipmentIndex).value;
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x0600181B RID: 6171 RVA: 0x00068D16 File Offset: 0x00066F16
		private PickupDef pickupDef
		{
			get
			{
				return PickupCatalog.GetPickupDef(this);
			}
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x00068D23 File Offset: 0x00066F23
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public GameObject GetHiddenPickupDisplayPrefab()
		{
			return PickupCatalog.GetHiddenPickupDisplayPrefab();
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x00068D2A File Offset: 0x00066F2A
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public GameObject GetPickupDisplayPrefab()
		{
			PickupDef pickupDef = this.pickupDef;
			if (pickupDef == null)
			{
				return null;
			}
			return pickupDef.displayPrefab;
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x00068D3D File Offset: 0x00066F3D
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public GameObject GetPickupDropletDisplayPrefab()
		{
			PickupDef pickupDef = this.pickupDef;
			if (pickupDef == null)
			{
				return null;
			}
			return pickupDef.dropletDisplayPrefab;
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x00068D50 File Offset: 0x00066F50
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public Color GetPickupColor()
		{
			PickupDef pickupDef = this.pickupDef;
			if (pickupDef == null)
			{
				return Color.black;
			}
			return pickupDef.baseColor;
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x00068D67 File Offset: 0x00066F67
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public string GetPickupNameToken()
		{
			PickupDef pickupDef = this.pickupDef;
			return ((pickupDef != null) ? pickupDef.nameToken : null) ?? "???";
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x00068D84 File Offset: 0x00066F84
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public string GetUnlockableName()
		{
			PickupDef pickupDef = this.pickupDef;
			return ((pickupDef != null) ? pickupDef.unlockableName : null) ?? "";
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x00068DA1 File Offset: 0x00066FA1
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public bool IsLunar()
		{
			PickupDef pickupDef = this.pickupDef;
			return pickupDef != null && pickupDef.isLunar;
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x00068DB4 File Offset: 0x00066FB4
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public bool IsBoss()
		{
			PickupDef pickupDef = this.pickupDef;
			return pickupDef != null && pickupDef.isBoss;
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x00068DC7 File Offset: 0x00066FC7
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public string GetInteractContextToken()
		{
			PickupDef pickupDef = this.pickupDef;
			return ((pickupDef != null) ? pickupDef.interactContextToken : null) ?? "";
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06001825 RID: 6181 RVA: 0x00068DE4 File Offset: 0x00066FE4
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public ItemIndex itemIndex
		{
			get
			{
				PickupDef pickupDef = this.pickupDef;
				if (pickupDef == null)
				{
					return ItemIndex.None;
				}
				return pickupDef.itemIndex;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06001826 RID: 6182 RVA: 0x00068DF7 File Offset: 0x00066FF7
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public EquipmentIndex equipmentIndex
		{
			get
			{
				PickupDef pickupDef = this.pickupDef;
				if (pickupDef == null)
				{
					return EquipmentIndex.None;
				}
				return pickupDef.equipmentIndex;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06001827 RID: 6183 RVA: 0x00068E0A File Offset: 0x0006700A
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public uint coinValue
		{
			get
			{
				PickupDef pickupDef = this.pickupDef;
				if (pickupDef == null)
				{
					return 0U;
				}
				return pickupDef.coinValue;
			}
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x00068E1D File Offset: 0x0006701D
		public static bool operator ==(PickupIndex a, PickupIndex b)
		{
			return a.value == b.value;
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x00068E2D File Offset: 0x0006702D
		public static bool operator !=(PickupIndex a, PickupIndex b)
		{
			return a.value != b.value;
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x00068E40 File Offset: 0x00067040
		public static bool operator <(PickupIndex a, PickupIndex b)
		{
			return a.value < b.value;
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x00068E50 File Offset: 0x00067050
		public static bool operator >(PickupIndex a, PickupIndex b)
		{
			return a.value > b.value;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x00068E60 File Offset: 0x00067060
		public static bool operator <=(PickupIndex a, PickupIndex b)
		{
			return a.value >= b.value;
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x00068E73 File Offset: 0x00067073
		public static bool operator >=(PickupIndex a, PickupIndex b)
		{
			return a.value <= b.value;
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x00068E86 File Offset: 0x00067086
		public static PickupIndex operator ++(PickupIndex a)
		{
			return new PickupIndex(a.value + 1);
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x00068E95 File Offset: 0x00067095
		public static PickupIndex operator --(PickupIndex a)
		{
			return new PickupIndex(a.value - 1);
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x00068EA4 File Offset: 0x000670A4
		public override bool Equals(object obj)
		{
			return obj is PickupIndex && this == (PickupIndex)obj;
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x00068E1D File Offset: 0x0006701D
		public bool Equals(PickupIndex other)
		{
			return this.value == other.value;
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x00068EC4 File Offset: 0x000670C4
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x00068EDF File Offset: 0x000670DF
		public static void WriteToNetworkWriter(NetworkWriter writer, PickupIndex value)
		{
			writer.WritePackedIndex32(value.value);
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x00068EED File Offset: 0x000670ED
		public static PickupIndex ReadFromNetworkReader(NetworkReader reader)
		{
			return new PickupIndex(reader.ReadPackedIndex32());
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00068EFA File Offset: 0x000670FA
		public override string ToString()
		{
			PickupDef pickupDef = this.pickupDef;
			return ((pickupDef != null) ? pickupDef.internalName : null) ?? string.Format("BadPickupIndex{0}", this.value);
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x00068F27 File Offset: 0x00067127
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public static PickupIndex Find(string name)
		{
			return PickupCatalog.FindPickupIndex(name);
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x00068F30 File Offset: 0x00067130
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public static PickupIndex.Enumerator GetEnumerator()
		{
			return default(PickupIndex.Enumerator);
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06001838 RID: 6200 RVA: 0x00068F48 File Offset: 0x00067148
		[Obsolete("PickupIndex methods are deprecated. Use PickupCatalog instead.")]
		public static GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator> allPickups
		{
			get
			{
				return default(GenericStaticEnumerable<PickupIndex, PickupIndex.Enumerator>);
			}
		}

		// Token: 0x040016CB RID: 5835
		public static readonly PickupIndex none = new PickupIndex(-1);

		// Token: 0x040016CC RID: 5836
		[SerializeField]
		public readonly int value;

		// Token: 0x020003E2 RID: 994
		public struct Enumerator : IEnumerator<PickupIndex>, IEnumerator, IDisposable
		{
			// Token: 0x0600183A RID: 6202 RVA: 0x00068F6B File Offset: 0x0006716B
			public bool MoveNext()
			{
				this.position = ++this.position;
				return this.position.value < PickupCatalog.pickupCount;
			}

			// Token: 0x0600183B RID: 6203 RVA: 0x00068F90 File Offset: 0x00067190
			public void Reset()
			{
				this.position = PickupIndex.none;
			}

			// Token: 0x170002D4 RID: 724
			// (get) Token: 0x0600183C RID: 6204 RVA: 0x00068F9D File Offset: 0x0006719D
			public PickupIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x170002D5 RID: 725
			// (get) Token: 0x0600183D RID: 6205 RVA: 0x00068FA5 File Offset: 0x000671A5
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x0600183E RID: 6206 RVA: 0x0000409B File Offset: 0x0000229B
			void IDisposable.Dispose()
			{
			}

			// Token: 0x040016CD RID: 5837
			private PickupIndex position;
		}
	}
}
