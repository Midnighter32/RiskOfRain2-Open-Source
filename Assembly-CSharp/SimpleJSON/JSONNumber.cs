using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000089 RID: 137
	public class JSONNumber : JSONNode
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002BB RID: 699 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002BD RID: 701 RVA: 0x0000BBEA File Offset: 0x00009DEA
		// (set) Token: 0x060002BE RID: 702 RVA: 0x0000BBF8 File Offset: 0x00009DF8
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

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0000BC16 File Offset: 0x00009E16
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x0000BC1E File Offset: 0x00009E1E
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

		// Token: 0x060002C1 RID: 705 RVA: 0x0000BC27 File Offset: 0x00009E27
		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000BC36 File Offset: 0x00009E36
		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000BC45 File Offset: 0x00009E45
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(4);
			aWriter.Write(this.m_Data);
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000BC5A File Offset: 0x00009E5A
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000BC6C File Offset: 0x00009E6C
		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000BCD4 File Offset: 0x00009ED4
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

		// Token: 0x060002C7 RID: 711 RVA: 0x0000BD28 File Offset: 0x00009F28
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x04000246 RID: 582
		private double m_Data;
	}
}
