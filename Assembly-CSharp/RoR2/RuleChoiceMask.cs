using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200047A RID: 1146
	public class RuleChoiceMask : SerializableBitArray
	{
		// Token: 0x0600198D RID: 6541 RVA: 0x00079FE9 File Offset: 0x000781E9
		public RuleChoiceMask() : base(RuleCatalog.choiceCount)
		{
		}

		// Token: 0x1700025F RID: 607
		public bool this[RuleChoiceDef choiceDef]
		{
			get
			{
				return base[choiceDef.globalIndex];
			}
			set
			{
				base[choiceDef.globalIndex] = value;
			}
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x0007A014 File Offset: 0x00078214
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0007A044 File Offset: 0x00078244
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0007A074 File Offset: 0x00078274
		public override bool Equals(object obj)
		{
			RuleChoiceMask ruleChoiceMask = obj as RuleChoiceMask;
			if (ruleChoiceMask != null)
			{
				for (int i = 0; i < this.bytes.Length; i++)
				{
					if (this.bytes[i] != ruleChoiceMask.bytes[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0007A0B4 File Offset: 0x000782B4
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0007A0E4 File Offset: 0x000782E4
		public void Copy([NotNull] RuleChoiceMask src)
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
