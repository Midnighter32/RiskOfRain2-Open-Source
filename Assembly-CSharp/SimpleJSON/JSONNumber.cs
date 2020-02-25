using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200008E RID: 142
	public class JSONNumber : JSONNode
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x0000C692 File Offset: 0x0000A892
		// (set) Token: 0x060002FA RID: 762 RVA: 0x0000C6A0 File Offset: 0x0000A8A0
		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				double data;
				if (double.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002FB RID: 763 RVA: 0x0000C6BE File Offset: 0x0000A8BE
		// (set) Token: 0x060002FC RID: 764 RVA: 0x0000C6C6 File Offset: 0x0000A8C6
		public override double AsDouble
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

		// Token: 0x060002FD RID: 765 RVA: 0x0000C6CF File Offset: 0x0000A8CF
		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000C6DE File Offset: 0x0000A8DE
		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000C6ED File Offset: 0x0000A8ED
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(4);
			aWriter.Write(this.m_Data);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000C702 File Offset: 0x0000A902
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000C714 File Offset: 0x0000A914
		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000C77C File Offset: 0x0000A97C
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JSONNumber jsonnumber = obj as JSONNumber;
			if (jsonnumber != null)
			{
				return this.m_Data == jsonnumber.m_Data;
			}
			return JSONNumber.IsNumeric(obj) && Convert.ToDouble(obj) == this.m_Data;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000C7D0 File Offset: 0x0000A9D0
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x04000251 RID: 593
		private double m_Data;
	}
}
