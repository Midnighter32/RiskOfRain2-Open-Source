using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000479 RID: 1145
	public class RuleMask : SerializableBitArray
	{
		// Token: 0x06001987 RID: 6535 RVA: 0x00079EDB File Offset: 0x000780DB
		public RuleMask() : base(RuleCatalog.ruleCount)
		{
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x00079EE8 File Offset: 0x000780E8
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x00079F18 File Offset: 0x00078118
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x00079F48 File Offset: 0x00078148
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

		// Token: 0x0600198B RID: 6539 RVA: 0x00079F88 File Offset: 0x00078188
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x00079FB8 File Offset: 0x000781B8
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
