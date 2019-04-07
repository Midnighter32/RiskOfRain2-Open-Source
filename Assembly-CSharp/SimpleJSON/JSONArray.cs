using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000080 RID: 128
	public class JSONArray : JSONNode, IEnumerable
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Array;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsArray
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700004A RID: 74
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					return new JSONLazyCreator(this);
				}
				return this.m_List[aIndex];
			}
			set
			{
				if (value == null)
				{
					value = new JSONNull();
				}
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					this.m_List.Add(value);
					return;
				}
				this.m_List[aIndex] = value;
			}
		}

		// Token: 0x1700004B RID: 75
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				if (value == null)
				{
					value = new JSONNull();
				}
				this.m_List.Add(value);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000AF19 File Offset: 0x00009119
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000AEFB File Offset: 0x000090FB
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = new JSONNull();
			}
			this.m_List.Add(aItem);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000AF26 File Offset: 0x00009126
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				return null;
			}
			JSONNode result = this.m_List[aIndex];
			this.m_List.RemoveAt(aIndex);
			return result;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000AF54 File Offset: 0x00009154
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000271 RID: 625 RVA: 0x0000AF64 File Offset: 0x00009164
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (JSONNode jsonnode in this.m_List)
				{
					yield return jsonnode;
				}
				List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000AF74 File Offset: 0x00009174
		public IEnumerator GetEnumerator()
		{
			foreach (JSONNode jsonnode in this.m_List)
			{
				yield return jsonnode;
			}
			List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000AF84 File Offset: 0x00009184
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000AFD4 File Offset: 0x000091D4
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('[');
			int count = this.m_List.Count;
			if (this.inline)
			{
				aMode = JSONTextMode.Compact;
			}
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					aSB.Append(',');
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				this.m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JSONTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append(']');
		}

		// Token: 0x04000229 RID: 553
		private List<JSONNode> m_List = new List<JSONNode>();

		// Token: 0x0400022A RID: 554
		public bool inline;
	}
}
