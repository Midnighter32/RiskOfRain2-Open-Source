using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200008C RID: 140
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000BE1E File Offset: 0x0000A01E
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.None;
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000BE21 File Offset: 0x0000A021
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000BE37 File Offset: 0x0000A037
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000BE4D File Offset: 0x0000A04D
		private void Set(JSONNode aVal)
		{
			if (this.m_Key == null)
			{
				this.m_Node.Add(aVal);
			}
			else
			{
				this.m_Node.Add(this.m_Key, aVal);
			}
			this.m_Node = null;
		}

		// Token: 0x1700006F RID: 111
		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				this.Set(new JSONArray
				{
					value
				});
			}
		}

		// Token: 0x17000070 RID: 112
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				this.Set(new JSONObject
				{
					{
						aKey,
						value
					}
				});
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000BED0 File Offset: 0x0000A0D0
		public override void Add(JSONNode aItem)
		{
			this.Set(new JSONArray
			{
				aItem
			});
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000BEF4 File Offset: 0x0000A0F4
		public override void Add(string aKey, JSONNode aItem)
		{
			this.Set(new JSONObject
			{
				{
					aKey,
					aItem
				}
			});
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000BF16 File Offset: 0x0000A116
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000BF21 File Offset: 0x0000A121
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000BF16 File Offset: 0x0000A116
		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0000BF30 File Offset: 0x0000A130
		// (set) Token: 0x060002EE RID: 750 RVA: 0x0000BF54 File Offset: 0x0000A154
		public override int AsInt
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber((double)value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000BF70 File Offset: 0x0000A170
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x0000BF98 File Offset: 0x0000A198
		public override float AsFloat
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0f;
			}
			set
			{
				JSONNumber aVal = new JSONNumber((double)value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000BFB4 File Offset: 0x0000A1B4
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x0000BFE0 File Offset: 0x0000A1E0
		public override double AsDouble
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0.0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber(value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000BFFC File Offset: 0x0000A1FC
		// (set) Token: 0x060002F4 RID: 756 RVA: 0x0000C018 File Offset: 0x0000A218
		public override bool AsBool
		{
			get
			{
				JSONBool aVal = new JSONBool(false);
				this.Set(aVal);
				return false;
			}
			set
			{
				JSONBool aVal = new JSONBool(value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x0000C034 File Offset: 0x0000A234
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0000C050 File Offset: 0x0000A250
		public override JSONObject AsObject
		{
			get
			{
				JSONObject jsonobject = new JSONObject();
				this.Set(jsonobject);
				return jsonobject;
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000BE08 File Offset: 0x0000A008
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		// Token: 0x04000248 RID: 584
		private JSONNode m_Node;

		// Token: 0x04000249 RID: 585
		private string m_Key;
	}
}
