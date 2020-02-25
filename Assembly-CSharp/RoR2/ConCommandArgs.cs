using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001B8 RID: 440
	public struct ConCommandArgs
	{
		// Token: 0x06000960 RID: 2400 RVA: 0x00028E33 File Offset: 0x00027033
		public void CheckArgumentCount(int count)
		{
			ConCommandException.CheckArgumentCount(this.userArgs, count);
		}

		// Token: 0x1700013A RID: 314
		public string this[int i]
		{
			get
			{
				return this.userArgs[i];
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x00028E4F File Offset: 0x0002704F
		public int Count
		{
			get
			{
				return this.userArgs.Count;
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x00028E5C File Offset: 0x0002705C
		public GameObject senderMasterObject
		{
			get
			{
				if (!this.sender)
				{
					return null;
				}
				return this.sender.masterObject;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x00028E78 File Offset: 0x00027078
		public CharacterMaster senderMaster
		{
			get
			{
				if (!this.sender)
				{
					return null;
				}
				return this.sender.master;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000965 RID: 2405 RVA: 0x00028E94 File Offset: 0x00027094
		public CharacterBody senderBody
		{
			get
			{
				if (!this.sender)
				{
					return null;
				}
				return this.sender.GetCurrentBody();
			}
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x00028EB0 File Offset: 0x000270B0
		public string TryGetArgString(int index)
		{
			if (index < this.userArgs.Count)
			{
				return this.userArgs[index];
			}
			return null;
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00028ECE File Offset: 0x000270CE
		public string GetArgString(int index)
		{
			string text = this.TryGetArgString(index);
			if (text == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be a string.", index));
			}
			return text;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x00028EF0 File Offset: 0x000270F0
		public ulong? TryGetArgUlong(int index)
		{
			ulong value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new ulong?(value);
			}
			return null;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00028F30 File Offset: 0x00027130
		public ulong GetArgULong(int index)
		{
			ulong? num = this.TryGetArgUlong(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be an unsigned integer.", index));
			}
			return num.Value;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x00028F6C File Offset: 0x0002716C
		public int? TryGetArgInt(int index)
		{
			int value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new int?(value);
			}
			return null;
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x00028FAC File Offset: 0x000271AC
		public int GetArgInt(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be an integer.", index));
			}
			return num.Value;
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00028FE8 File Offset: 0x000271E8
		public bool? TryGetArgBool(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num != null)
			{
				int? num2 = num;
				int num3 = 0;
				return new bool?(num2.GetValueOrDefault() > num3 & num2 != null);
			}
			return null;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0002902C File Offset: 0x0002722C
		public bool GetArgBool(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be a boolean.", index));
			}
			return num.Value > 0;
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0002906C File Offset: 0x0002726C
		public float? TryGetArgFloat(int index)
		{
			float value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new float?(value);
			}
			return null;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x000290AC File Offset: 0x000272AC
		public float GetArgFloat(int index)
		{
			float? num = this.TryGetArgFloat(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be a number.", index));
			}
			return num.Value;
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x000290E8 File Offset: 0x000272E8
		public double? TryGetArgDouble(int index)
		{
			double value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new double?(value);
			}
			return null;
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x00029128 File Offset: 0x00027328
		public double GetArgDouble(int index)
		{
			double? num = this.TryGetArgDouble(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be a number.", index));
			}
			return num.Value;
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x00029164 File Offset: 0x00027364
		public T? TryGetArgEnum<T>(int index) where T : struct
		{
			T value;
			if (index < this.userArgs.Count && Enum.TryParse<T>(this.userArgs[index], true, out value))
			{
				return new T?(value);
			}
			return null;
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x000291A8 File Offset: 0x000273A8
		public T GetArgEnum<T>(int index) where T : struct
		{
			T? t = this.TryGetArgEnum<T>(index);
			if (t == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be one of the values of {1}.", index, typeof(T).Name));
			}
			return t.Value;
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x000291F2 File Offset: 0x000273F2
		public LocalUser GetSenderLocalUser()
		{
			if (this.sender && this.sender.localUser != null)
			{
				return this.sender.localUser;
			}
			throw new ConCommandException(string.Format("Command requires a local user that is not available.", Array.Empty<object>()));
		}

		// Token: 0x040009C1 RID: 2497
		public List<string> userArgs;

		// Token: 0x040009C2 RID: 2498
		public NetworkUser sender;

		// Token: 0x040009C3 RID: 2499
		public string commandName;
	}
}
