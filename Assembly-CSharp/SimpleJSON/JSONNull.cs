using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000090 RID: 144
	public class JSONNull : JSONNode
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000310 RID: 784 RVA: 0x0000C88C File Offset: 0x0000AA8C
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.NullValue;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0000C88F File Offset: 0x0000AA8F
		// (set) Token: 0x06000313 RID: 787 RVA: 0x0000409B File Offset: 0x0000229B
		public override string Value
		{
			get
			{
				return "null";
			}
			set
			{
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000314 RID: 788 RVA: 0x0000AC89 File Offset: 0x00008E89
		// (set) Token: 0x06000315 RID: 789 RVA: 0x0000409B File Offset: 0x0000229B
		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000C896 File Offset: 0x0000AA96
		public override bool Equals(object obj)
		{
			return this == obj || obj is JSONNull;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000C8A7 File Offset: 0x0000AAA7
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(5);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}
	}
}
