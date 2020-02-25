using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000081 RID: 129
	public abstract class JSONNode
	{
		// Token: 0x17000034 RID: 52
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000035 RID: 53
		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000249 RID: 585 RVA: 0x0000AC82 File Offset: 0x00008E82
		// (set) Token: 0x0600024A RID: 586 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600024C RID: 588 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600024F RID: 591 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000250 RID: 592 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000251 RID: 593 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000AC8C File Offset: 0x00008E8C
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000AC7F File Offset: 0x00008E7F
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000AC7F File Offset: 0x00008E7F
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000AC9A File Offset: 0x00008E9A
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000257 RID: 599 RVA: 0x0000AC9D File Offset: 0x00008E9D
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000ACA6 File Offset: 0x00008EA6
		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode jsonnode in this.Children)
				{
					foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
					{
						yield return jsonnode2;
					}
					IEnumerator<JSONNode> enumerator2 = null;
				}
				IEnumerator<JSONNode> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000ACB6 File Offset: 0x00008EB6
		public virtual IEnumerable<string> Keys
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000ACC0 File Offset: 0x00008EC0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000ACE4 File Offset: 0x00008EE4
		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		// Token: 0x0600025C RID: 604
		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600025D RID: 605
		public abstract JSONNodeType Tag { get; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000AD08 File Offset: 0x00008F08
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0000AD40 File Offset: 0x00008F40
		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (!double.TryParse(this.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0000AD4F File Offset: 0x00008F4F
		// (set) Token: 0x06000261 RID: 609 RVA: 0x0000AD58 File Offset: 0x00008F58
		public virtual int AsInt
		{
			get
			{
				return (int)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0000AD62 File Offset: 0x00008F62
		// (set) Token: 0x06000263 RID: 611 RVA: 0x0000AD58 File Offset: 0x00008F58
		public virtual float AsFloat
		{
			get
			{
				return (float)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0000AD6C File Offset: 0x00008F6C
		// (set) Token: 0x06000265 RID: 613 RVA: 0x0000AD9A File Offset: 0x00008F9A
		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(this.Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = (value ? "true" : "false");
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0000ADB1 File Offset: 0x00008FB1
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0000ADB9 File Offset: 0x00008FB9
		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000ADC1 File Offset: 0x00008FC1
		public static implicit operator JSONNode(string s)
		{
			return new JSONString(s);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000ADC9 File Offset: 0x00008FC9
		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000ADDC File Offset: 0x00008FDC
		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000ADE4 File Offset: 0x00008FE4
		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000ADFF File Offset: 0x00008FFF
		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000AE08 File Offset: 0x00009008
		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000ADFF File Offset: 0x00008FFF
		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000AE1F File Offset: 0x0000901F
		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000AE32 File Offset: 0x00009032
		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000AE3A File Offset: 0x0000903A
		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000AE50 File Offset: 0x00009050
		public static bool operator ==(JSONNode a, object b)
		{
			if (a == b)
			{
				return true;
			}
			bool flag = a is JSONNull || a == null || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
			return (flag && flag2) || a.Equals(b);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000AE9F File Offset: 0x0000909F
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000AEAB File Offset: 0x000090AB
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000AEB1 File Offset: 0x000090B1
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000AEBC File Offset: 0x000090BC
		internal static string Escape(string aText)
		{
			JSONNode.m_EscapeBuilder.Length = 0;
			if (JSONNode.m_EscapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				JSONNode.m_EscapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			int i = 0;
			while (i < aText.Length)
			{
				char c = aText[i];
				switch (c)
				{
				case '\b':
					JSONNode.m_EscapeBuilder.Append("\\b");
					break;
				case '\t':
					JSONNode.m_EscapeBuilder.Append("\\t");
					break;
				case '\n':
					JSONNode.m_EscapeBuilder.Append("\\n");
					break;
				case '\v':
					goto IL_FA;
				case '\f':
					JSONNode.m_EscapeBuilder.Append("\\f");
					break;
				case '\r':
					JSONNode.m_EscapeBuilder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_FA;
						}
						JSONNode.m_EscapeBuilder.Append("\\\\");
					}
					else
					{
						JSONNode.m_EscapeBuilder.Append("\\\"");
					}
					break;
				}
				IL_106:
				i++;
				continue;
				IL_FA:
				JSONNode.m_EscapeBuilder.Append(c);
				goto IL_106;
			}
			string result = JSONNode.m_EscapeBuilder.ToString();
			JSONNode.m_EscapeBuilder.Length = 0;
			return result;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000AFF4 File Offset: 0x000091F4
		private static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}
			string a = token.ToLower(CultureInfo.InvariantCulture);
			if (a == "false" || a == "true")
			{
				ctx.Add(tokenName, a == "true");
				return;
			}
			if (a == "null")
			{
				ctx.Add(tokenName, null);
				return;
			}
			double n;
			if (double.TryParse(token, out n))
			{
				ctx.Add(tokenName, n);
				return;
			}
			ctx.Add(tokenName, token);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000B08C File Offset: 0x0000928C
		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			bool flag = false;
			bool flag2 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				if (c <= ',')
				{
					if (c <= ' ')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
						case '\r':
							goto IL_33E;
						case '\v':
						case '\f':
							goto IL_330;
						default:
							if (c != ' ')
							{
								goto IL_330;
							}
							break;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
					}
					else if (c != '"')
					{
						if (c != ',')
						{
							goto IL_330;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
						else
						{
							if (stringBuilder.Length > 0 || flag2)
							{
								JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							}
							text = "";
							stringBuilder.Length = 0;
							flag2 = false;
						}
					}
					else
					{
						flag = !flag;
						flag2 = (flag2 || flag);
					}
				}
				else
				{
					if (c <= ']')
					{
						if (c != ':')
						{
							switch (c)
							{
							case '[':
								if (flag)
								{
									stringBuilder.Append(aJSON[i]);
									goto IL_33E;
								}
								stack.Push(new JSONArray());
								if (jsonnode != null)
								{
									jsonnode.Add(text, stack.Peek());
								}
								text = "";
								stringBuilder.Length = 0;
								jsonnode = stack.Peek();
								goto IL_33E;
							case '\\':
								i++;
								if (flag)
								{
									char c2 = aJSON[i];
									if (c2 <= 'f')
									{
										if (c2 == 'b')
										{
											stringBuilder.Append('\b');
											goto IL_33E;
										}
										if (c2 == 'f')
										{
											stringBuilder.Append('\f');
											goto IL_33E;
										}
									}
									else
									{
										if (c2 == 'n')
										{
											stringBuilder.Append('\n');
											goto IL_33E;
										}
										switch (c2)
										{
										case 'r':
											stringBuilder.Append('\r');
											goto IL_33E;
										case 't':
											stringBuilder.Append('\t');
											goto IL_33E;
										case 'u':
										{
											string s = aJSON.Substring(i + 1, 4);
											stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
											i += 4;
											goto IL_33E;
										}
										}
									}
									stringBuilder.Append(c2);
									goto IL_33E;
								}
								goto IL_33E;
							case ']':
								break;
							default:
								goto IL_330;
							}
						}
						else
						{
							if (flag)
							{
								stringBuilder.Append(aJSON[i]);
								goto IL_33E;
							}
							text = stringBuilder.ToString();
							stringBuilder.Length = 0;
							flag2 = false;
							goto IL_33E;
						}
					}
					else if (c != '{')
					{
						if (c != '}')
						{
							goto IL_330;
						}
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
							goto IL_33E;
						}
						stack.Push(new JSONObject());
						if (jsonnode != null)
						{
							jsonnode.Add(text, stack.Peek());
						}
						text = "";
						stringBuilder.Length = 0;
						jsonnode = stack.Peek();
						goto IL_33E;
					}
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (stringBuilder.Length > 0 || flag2)
						{
							JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							flag2 = false;
						}
						text = "";
						stringBuilder.Length = 0;
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
				}
				IL_33E:
				i++;
				continue;
				IL_330:
				stringBuilder.Append(aJSON[i]);
				goto IL_33E;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000B3F8 File Offset: 0x000095F8
		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000B413 File Offset: 0x00009613
		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000B413 File Offset: 0x00009613
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000B413 File Offset: 0x00009613
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000B420 File Offset: 0x00009620
		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000B470 File Offset: 0x00009670
		public string SaveToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000B4BC File Offset: 0x000096BC
		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONNodeType jsonnodeType = (JSONNodeType)aReader.ReadByte();
			switch (jsonnodeType)
			{
			case JSONNodeType.Array:
			{
				int num = aReader.ReadInt32();
				JSONArray jsonarray = new JSONArray();
				for (int i = 0; i < num; i++)
				{
					jsonarray.Add(JSONNode.Deserialize(aReader));
				}
				return jsonarray;
			}
			case JSONNodeType.Object:
			{
				int num2 = aReader.ReadInt32();
				JSONObject jsonobject = new JSONObject();
				for (int j = 0; j < num2; j++)
				{
					string aKey = aReader.ReadString();
					JSONNode aItem = JSONNode.Deserialize(aReader);
					jsonobject.Add(aKey, aItem);
				}
				return jsonobject;
			}
			case JSONNodeType.String:
				return new JSONString(aReader.ReadString());
			case JSONNodeType.Number:
				return new JSONNumber(aReader.ReadDouble());
			case JSONNodeType.NullValue:
				return new JSONNull();
			case JSONNodeType.Boolean:
				return new JSONBool(aReader.ReadBoolean());
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jsonnodeType);
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000B413 File Offset: 0x00009613
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000B413 File Offset: 0x00009613
		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000B413 File Offset: 0x00009613
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000B59C File Offset: 0x0000979C
		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000B5D4 File Offset: 0x000097D4
		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000B60C File Offset: 0x0000980C
		public static JSONNode LoadFromBase64(string aBase64)
		{
			return JSONNode.LoadFromStream(new MemoryStream(Convert.FromBase64String(aBase64))
			{
				Position = 0L
			});
		}

		// Token: 0x04000227 RID: 551
		internal static StringBuilder m_EscapeBuilder = new StringBuilder();
	}
}
