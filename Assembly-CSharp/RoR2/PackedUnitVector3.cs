using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D9 RID: 985
	[Serializable]
	public struct PackedUnitVector3
	{
		// Token: 0x060017DF RID: 6111 RVA: 0x00067E90 File Offset: 0x00066090
		static PackedUnitVector3()
		{
			for (int i = 0; i < PackedUnitVector3.uvAdjustment.Length; i++)
			{
				int num = i >> 7;
				int num2 = i & 127;
				if (num + num2 >= 127)
				{
					num = 127 - num;
					num2 = 127 - num2;
				}
				Vector3 vector = new Vector3((float)num, (float)num2, (float)(126 - num - num2));
				PackedUnitVector3.uvAdjustment[i] = 1f / vector.magnitude;
			}
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x00067EFE File Offset: 0x000660FE
		public PackedUnitVector3(ushort value)
		{
			this.value = value;
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x00067F08 File Offset: 0x00066108
		public PackedUnitVector3(Vector3 src)
		{
			this.value = 0;
			if (src.x < 0f)
			{
				this.value |= 32768;
				src.x = -src.x;
			}
			if (src.y < 0f)
			{
				this.value |= 16384;
				src.y = -src.y;
			}
			if (src.z < 0f)
			{
				this.value |= 8192;
				src.z = -src.z;
			}
			float num = 126f / (src.x + src.y + src.z);
			int num2 = (int)(src.x * num);
			int num3 = (int)(src.y * num);
			if (num2 >= 64)
			{
				num2 = 127 - num2;
				num3 = 127 - num3;
			}
			this.value |= (ushort)(num2 << 7);
			this.value |= (ushort)num3;
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x00068008 File Offset: 0x00066208
		public Vector3 Unpack()
		{
			int num = (this.value & 8064) >> 7;
			int num2 = (int)(this.value & 127);
			if (num + num2 >= 127)
			{
				num = 127 - num;
				num2 = 127 - num2;
			}
			float num3 = PackedUnitVector3.uvAdjustment[(int)(this.value & 8191)];
			Vector3 vector = new Vector3(num3 * (float)num, num3 * (float)num2, num3 * (float)(126 - num - num2));
			if ((this.value & 32768) != 0)
			{
				vector.x = -vector.x;
			}
			if ((this.value & 16384) != 0)
			{
				vector.y = -vector.y;
			}
			if ((this.value & 8192) != 0)
			{
				vector.z = -vector.z;
			}
			return vector;
		}

		// Token: 0x04001696 RID: 5782
		[SerializeField]
		public ushort value;

		// Token: 0x04001697 RID: 5783
		private static readonly float[] uvAdjustment = new float[8192];

		// Token: 0x04001698 RID: 5784
		private const ushort signMask = 57344;

		// Token: 0x04001699 RID: 5785
		private const ushort invSignMask = 8191;

		// Token: 0x0400169A RID: 5786
		private const ushort xSignMask = 32768;

		// Token: 0x0400169B RID: 5787
		private const ushort ySignMask = 16384;

		// Token: 0x0400169C RID: 5788
		private const ushort zSignMask = 8192;

		// Token: 0x0400169D RID: 5789
		private const ushort topMask = 8064;

		// Token: 0x0400169E RID: 5790
		private const ushort bottomMask = 127;
	}
}
