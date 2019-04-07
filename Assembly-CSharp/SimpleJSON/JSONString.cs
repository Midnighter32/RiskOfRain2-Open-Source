using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000088 RID: 136
	public class JSONString : JSONNode
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.String;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsString
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000BB2E File Offset: 0x00009D2E
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x0000BB36 File Offset: 0x00009D36
		public override string Value
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000BB3F File Offset: 0x00009D3F
		public JSONString(string aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000BB4E File Offset: 0x00009D4E
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(3);
			aWriter.Write(this.m_Data);
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000BB63 File Offset: 0x00009D63
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('"').Append(JSONNode.Escape(this.m_Data)).Append('"');
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000BB88 File Offset: 0x00009D88
		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			string text = obj as string;
			if (text != null)
			{
				return this.m_Data == text;
			}
			JSONString jsonstring = obj as JSONString;
			return jsonstring != null && this.m_Data == jsonstring.m_Data;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000BBDA File Offset: 0x00009DDA
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x04000245 RID: 581
		private string m_Data;
	}
}
