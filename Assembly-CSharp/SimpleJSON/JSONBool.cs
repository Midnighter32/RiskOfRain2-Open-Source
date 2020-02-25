using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200008F RID: 143
	public class JSONBool : JSONNode
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000306 RID: 774 RVA: 0x0000C7E0 File Offset: 0x0000A9E0
		// (set) Token: 0x06000307 RID: 775 RVA: 0x0000C7F0 File Offset: 0x0000A9F0
		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				bool data;
				if (bool.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000308 RID: 776 RVA: 0x0000C80E File Offset: 0x0000AA0E
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0000C816 File Offset: 0x0000AA16
		public override bool AsBool
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

		// Token: 0x0600030A RID: 778 RVA: 0x0000C81F File Offset: 0x0000AA1F
		public JSONBool(bool aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000C6DE File Offset: 0x0000A8DE
		public JSONBool(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0000C82E File Offset: 0x0000AA2E
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(6);
			aWriter.Write(this.m_Data);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000C843 File Offset: 0x0000AA43
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data ? "true" : "false");
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000C860 File Offset: 0x0000AA60
		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && this.m_Data == (bool)obj;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000C87F File Offset: 0x0000AA7F
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x04000252 RID: 594
		private bool m_Data;
	}
}
