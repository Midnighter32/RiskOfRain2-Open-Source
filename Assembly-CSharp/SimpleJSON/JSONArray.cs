using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000085 RID: 133
	public class JSONArray : JSONNode, IEnumerable
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000B933 File Offset: 0x00009B33
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Array;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsArray
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000050 RID: 80
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

		// Token: 0x17000051 RID: 81
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000B9C1 File Offset: 0x00009BC1
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000B9A3 File Offset: 0x00009BA3
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = new JSONNull();
			}
			this.m_List.Add(aItem);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000B9CE File Offset: 0x00009BCE
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

		// Token: 0x060002AC RID: 684 RVA: 0x0000B9FC File Offset: 0x00009BFC
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000BA0C File Offset: 0x00009C0C
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

		// Token: 0x060002AE RID: 686 RVA: 0x0000BA1C File Offset: 0x00009C1C
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

		// Token: 0x060002AF RID: 687 RVA: 0x0000BA2C File Offset: 0x00009C2C
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000BA7C File Offset: 0x00009C7C
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

		// Token: 0x04000234 RID: 564
		private List<JSONNode> m_List = new List<JSONNode>();

		// Token: 0x04000235 RID: 565
		public bool inline;
	}
}
