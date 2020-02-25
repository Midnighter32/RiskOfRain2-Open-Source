using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FB RID: 1019
	public class RuleMask : SerializableBitArray
	{
		// Token: 0x060018A7 RID: 6311 RVA: 0x0006A256 File Offset: 0x00068456
		public RuleMask() : base(RuleCatalog.ruleCount)
		{
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0006A264 File Offset: 0x00068464
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0006A294 File Offset: 0x00068494
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x0006A2C4 File Offset: 0x000684C4
		public override bool Equals(object obj)
		{
			RuleMask ruleMask = obj as RuleMask;
			if (ruleMask != null)
			{
				for (int i = 0; i < this.bytes.Length; i++)
				{
					if (this.bytes[i] != ruleMask.bytes[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0006A304 File Offset: 0x00068504
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0006A334 File Offset: 0x00068534
		public void Copy([NotNull] RuleMask src)
		{
			byte[] bytes = src.bytes;
			byte[] bytes2 = this.bytes;
			int i = 0;
			int num = bytes2.Length;
			while (i < num)
			{
				bytes2[i] = bytes[i];
				i++;
			}
		}
	}
}
