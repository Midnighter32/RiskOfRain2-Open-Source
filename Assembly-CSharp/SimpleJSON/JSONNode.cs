using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200007C RID: 124
	public abstract class JSONNode
	{
		// Token: 0x1700002E RID: 46
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

		// Token: 0x1700002F RID: 47
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

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600020D RID: 525 RVA: 0x0000A1E6 File Offset: 0x000083E6
		// (set) Token: 0x0600020E RID: 526 RVA: 0x00004507 File Offset: 0x00002707
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

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000A1F0 File Offset: 0x000083F0
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000A1E3 File Offset: 0x000083E3
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000A1E3 File Offset: 0x000083E3
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000A1FE File Offset: 0x000083FE
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000A201 File Offset: 0x00008401
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000A20A File Offset: 0x0000840A
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600021D RID: 541 RVA: 0x0000A21A File Offset: 0x0000841A
		public virtual IEnumerable<string> Keys
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000A224 File Offset: 0x00008424
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000A248 File Offset: 0x00008448
		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		// Token: 0x06000220 RID: 544
		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000221 RID: 545
		public abstract JSONNodeType Tag { get; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0000A26C File Offset: 0x0000846C
		// (set) Token: 0x06000223 RID: 547 RVA: 0x0000A29D File Offset: 0x0000849D
		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(this.Value, out result))
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000A2AC File Offset: 0x000084AC
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0000A2B5 File Offset: 0x000084B5
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000226 RID: 550 RVA: 0x0000A2BF File Offset: 0x000084BF
		// (set) Token: 0x06000227 RID: 551 RVA: 0x0000A2B5 File Offset: 0x000084B5
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000A2C8 File Offset: 0x000084C8
		// (set) Token: 0x06000229 RID: 553 RVA: 0x0000A2F6 File Offset: 0x000084F6
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600022A RID: 554 RVA: 0x0000A30D File Offset: 0x0000850D
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600022B RID: 555 RVA: 0x0000A315 File Offset: 0x00008515
		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000A31D File Offset: 0x0000851D
		public static implicit operator JSONNode(string s)
		{
			return new JSONString(s);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000A325 File Offset: 0x00008525
		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000A338 File Offset: 0x00008538
		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000A340 File Offset: 0x00008540
		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000A35B File Offset: 0x0000855B
		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000A364 File Offset: 0x00008564
		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000A35B File Offset: 0x0000855B
		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000A37B File Offset: 0x0000857B
		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000A38E File Offset: 0x0000858E
		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000A396 File Offset: 0x00008596
		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000A3AC File Offset: 0x000085AC
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

		// Token: 0x06000237 RID: 567 RVA: 0x0000A3FB File Offset: 0x000085FB
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000A407 File Offset: 0x00008607
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000A40D File Offset: 0x0000860D
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000A418 File Offset: 0x00008618
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

		// Token: 0x0600023B RID: 571 RVA: 0x0000A550 File Offset: 0x00008750
		private static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}
			string a = token.ToLower();
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

		// Token: 0x0600023C RID: 572 RVA: 0x0000A5E4 File Offset: 0x000087E4
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

		// Token: 0x0600023D RID: 573 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000A950 File Offset: 0x00008B50
		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000A96B File Offset: 0x00008B6B
		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000A96B File Offset: 0x00008B6B
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000A96B File Offset: 0x00008B6B
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000A978 File Offset: 0x00008B78
		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000A9C8 File Offset: 0x00008BC8
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

		// Token: 0x06000244 RID: 580 RVA: 0x0000AA14 File Offset: 0x00008C14
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

		// Token: 0x06000245 RID: 581 RVA: 0x0000A96B File Offset: 0x00008B6B
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000A96B File Offset: 0x00008B6B
		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000A96B File Offset: 0x00008B6B
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000AAF4 File Offset: 0x00008CF4
		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000AB2C File Offset: 0x00008D2C
		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000AB64 File Offset: 0x00008D64
		public static JSONNode LoadFromBase64(string aBase64)
		{
			return JSONNode.LoadFromStream(new MemoryStream(Convert.FromBase64String(aBase64))
			{
				Position = 0L
			});
		}

		// Token: 0x0400021C RID: 540
		internal static StringBuilder m_EscapeBuilder = new StringBuilder();
	}
}
