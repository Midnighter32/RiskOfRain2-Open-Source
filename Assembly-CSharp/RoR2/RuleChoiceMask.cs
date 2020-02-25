using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FC RID: 1020
	public class RuleChoiceMask : SerializableBitArray
	{
		// Token: 0x060018AD RID: 6317 RVA: 0x0006A365 File Offset: 0x00068565
		public RuleChoiceMask() : base(RuleCatalog.choiceCount)
		{
		}

		// Token: 0x170002E1 RID: 737
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

		// Token: 0x060018B0 RID: 6320 RVA: 0x0006A390 File Offset: 0x00068590
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0006A3C0 File Offset: 0x000685C0
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0006A3F0 File Offset: 0x000685F0
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

		// Token: 0x060018B3 RID: 6323 RVA: 0x0006A430 File Offset: 0x00068630
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0006A460 File Offset: 0x00068660
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
