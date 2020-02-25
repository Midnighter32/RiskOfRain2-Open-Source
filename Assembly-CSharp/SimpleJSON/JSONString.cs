using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200008D RID: 141
	public class JSONString : JSONNode
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002EE RID: 750 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.String;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsString
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x0000C5D6 File Offset: 0x0000A7D6
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x0000C5DE File Offset: 0x0000A7DE
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

		// Token: 0x060002F2 RID: 754 RVA: 0x0000C5E7 File Offset: 0x0000A7E7
		public JSONString(string aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0000C5F6 File Offset: 0x0000A7F6
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(3);
			aWriter.Write(this.m_Data);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000C60B File Offset: 0x0000A80B
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('"').Append(JSONNode.Escape(this.m_Data)).Append('"');
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000C630 File Offset: 0x0000A830
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

		// Token: 0x060002F6 RID: 758 RVA: 0x0000C682 File Offset: 0x0000A882
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x04000250 RID: 592
		private string m_Data;
	}
}
