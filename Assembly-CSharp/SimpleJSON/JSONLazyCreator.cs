using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000091 RID: 145
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000C8C6 File Offset: 0x0000AAC6
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.None;
			}
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000C8C9 File Offset: 0x0000AAC9
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000C8DF File Offset: 0x0000AADF
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000C8F5 File Offset: 0x0000AAF5
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

		// Token: 0x17000075 RID: 117
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

		// Token: 0x17000076 RID: 118
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

		// Token: 0x06000323 RID: 803 RVA: 0x0000C978 File Offset: 0x0000AB78
		public override void Add(JSONNode aItem)
		{
			this.Set(new JSONArray
			{
				aItem
			});
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000C99C File Offset: 0x0000AB9C
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

		// Token: 0x06000325 RID: 805 RVA: 0x0000C9BE File Offset: 0x0000ABBE
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000C9C9 File Offset: 0x0000ABC9
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000C9BE File Offset: 0x0000ABBE
		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000C9D8 File Offset: 0x0000ABD8
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0000C9FC File Offset: 0x0000ABFC
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

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000CA18 File Offset: 0x0000AC18
		// (set) Token: 0x0600032C RID: 812 RVA: 0x0000CA40 File Offset: 0x0000AC40
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

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0000CA5C File Offset: 0x0000AC5C
		// (set) Token: 0x0600032E RID: 814 RVA: 0x0000CA88 File Offset: 0x0000AC88
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

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0000CAA4 File Offset: 0x0000ACA4
		// (set) Token: 0x06000330 RID: 816 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
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

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000331 RID: 817 RVA: 0x0000CADC File Offset: 0x0000ACDC
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000332 RID: 818 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		public override JSONObject AsObject
		{
			get
			{
				JSONObject jsonobject = new JSONObject();
				this.Set(jsonobject);
				return jsonobject;
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		// Token: 0x04000253 RID: 595
		private JSONNode m_Node;

		// Token: 0x04000254 RID: 596
		private string m_Key;
	}
}
