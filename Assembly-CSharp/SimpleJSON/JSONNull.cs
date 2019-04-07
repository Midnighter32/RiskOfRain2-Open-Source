using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200008B RID: 139
	public class JSONNull : JSONNode
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000BDE4 File Offset: 0x00009FE4
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.NullValue;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000BDE7 File Offset: 0x00009FE7
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x00004507 File Offset: 0x00002707
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

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x0000A1ED File Offset: 0x000083ED
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x00004507 File Offset: 0x00002707
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

		// Token: 0x060002DA RID: 730 RVA: 0x0000BDEE File Offset: 0x00009FEE
		public override bool Equals(object obj)
		{
			return this == obj || obj is JSONNull;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000BDFF File Offset: 0x00009FFF
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(5);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000BE08 File Offset: 0x0000A008
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}
	}
}
