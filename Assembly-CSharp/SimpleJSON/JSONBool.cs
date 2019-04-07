using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200008A RID: 138
	public class JSONBool : JSONNode
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000BD35 File Offset: 0x00009F35
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000BD38 File Offset: 0x00009F38
		// (set) Token: 0x060002CB RID: 715 RVA: 0x0000BD48 File Offset: 0x00009F48
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

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000BD66 File Offset: 0x00009F66
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000BD6E File Offset: 0x00009F6E
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

		// Token: 0x060002CE RID: 718 RVA: 0x0000BD77 File Offset: 0x00009F77
		public JSONBool(bool aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000BC36 File Offset: 0x00009E36
		public JSONBool(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000BD86 File Offset: 0x00009F86
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(6);
			aWriter.Write(this.m_Data);
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000BD9B File Offset: 0x00009F9B
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data ? "true" : "false");
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0000BDB8 File Offset: 0x00009FB8
		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && this.m_Data == (bool)obj;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000BDD7 File Offset: 0x00009FD7
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x04000247 RID: 583
		private bool m_Data;
	}
}
