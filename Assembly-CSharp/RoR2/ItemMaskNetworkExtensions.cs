using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003B4 RID: 948
	public static class ItemMaskNetworkExtensions
	{
		// Token: 0x060016F4 RID: 5876 RVA: 0x0000D703 File Offset: 0x0000B903
		public static void Write(this NetworkWriter writer, ItemIndex itemIndex)
		{
			writer.WritePackedUInt32((uint)(itemIndex + 1));
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x0000D70E File Offset: 0x0000B90E
		public static ItemIndex ReadItemIndex(this NetworkReader reader)
		{
			return (ItemIndex)(reader.ReadPackedUInt32() - 1U);
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x00063B70 File Offset: 0x00061D70
		[SystemInitializer(new Type[]
		{
			typeof(ItemCatalog)
		})]
		private static void Init()
		{
			ItemMaskNetworkExtensions.itemMaskBitCount = ItemCatalog.itemCount;
			ItemMaskNetworkExtensions.itemMaskByteCount = ItemMaskNetworkExtensions.itemMaskBitCount + 7 >> 3;
			ItemMaskNetworkExtensions.itemMaskByteBuffer = new byte[ItemMaskNetworkExtensions.itemMaskByteCount];
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x00063B9C File Offset: 0x00061D9C
		public static void WriteItemStacks(this NetworkWriter writer, int[] srcItemStacks)
		{
			int num = 0;
			for (int i = 0; i < ItemMaskNetworkExtensions.itemMaskByteCount; i++)
			{
				byte b = 0;
				int num2 = 0;
				while (num2 < 8 && num < ItemMaskNetworkExtensions.itemMaskBitCount)
				{
					if (srcItemStacks[num] > 0)
					{
						b |= (byte)(1 << num2);
					}
					num2++;
					num++;
				}
				ItemMaskNetworkExtensions.itemMaskByteBuffer[i] = b;
			}
			for (int j = 0; j < ItemMaskNetworkExtensions.itemMaskByteCount; j++)
			{
				writer.Write(ItemMaskNetworkExtensions.itemMaskByteBuffer[j]);
			}
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				int num3 = srcItemStacks[(int)itemIndex];
				if (num3 > 0)
				{
					writer.WritePackedUInt32((uint)num3);
				}
				itemIndex++;
			}
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x00063C3C File Offset: 0x00061E3C
		public static void ReadItemStacks(this NetworkReader reader, int[] destItemStacks)
		{
			for (int i = 0; i < ItemMaskNetworkExtensions.itemMaskByteCount; i++)
			{
				ItemMaskNetworkExtensions.itemMaskByteBuffer[i] = reader.ReadByte();
			}
			int num = 0;
			for (int j = 0; j < ItemMaskNetworkExtensions.itemMaskByteCount; j++)
			{
				byte b = ItemMaskNetworkExtensions.itemMaskByteBuffer[j];
				int num2 = 0;
				while (num2 < 8 && num < ItemMaskNetworkExtensions.itemMaskBitCount)
				{
					destItemStacks[num] = (int)(((b & (byte)(1 << num2)) != 0) ? reader.ReadPackedUInt32() : 0U);
					num2++;
					num++;
				}
			}
		}

		// Token: 0x040015E8 RID: 5608
		private static int itemMaskBitCount;

		// Token: 0x040015E9 RID: 5609
		private static int itemMaskByteCount;

		// Token: 0x040015EA RID: 5610
		private static byte[] itemMaskByteBuffer;
	}
}
