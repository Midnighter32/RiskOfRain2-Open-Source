using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A9 RID: 681
	public struct ConCommandArgs
	{
		// Token: 0x06000DCD RID: 3533 RVA: 0x0004400D File Offset: 0x0004220D
		public void CheckArgumentCount(int count)
		{
			ConCommandException.CheckArgumentCount(this.userArgs, count);
		}

		// Token: 0x1700012C RID: 300
		public string this[int i]
		{
			get
			{
				return this.userArgs[i];
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00044029 File Offset: 0x00042229
		public int Count
		{
			get
			{
				return this.userArgs.Count;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x00044036 File Offset: 0x00042236
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

		// Token: 0x06000DD1 RID: 3537 RVA: 0x00044054 File Offset: 0x00042254
		public ulong? TryGetArgUlong(int index)
		{
			ulong value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new ulong?(value);
			}
			return null;
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x00044094 File Offset: 0x00042294
		public ulong GetArgULong(int index)
		{
			ulong? num = this.TryGetArgUlong(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be an unsigned integer.", index));
			}
			return num.Value;
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x000440D0 File Offset: 0x000422D0
		public int? TryGetArgInt(int index)
		{
			int value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new int?(value);
			}
			return null;
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x00044110 File Offset: 0x00042310
		public int GetArgInt(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be an integer.", index));
			}
			return num.Value;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x0004414C File Offset: 0x0004234C
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

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00044190 File Offset: 0x00042390
		public bool GetArgBool(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be a boolean.", index));
			}
			return num.Value > 0;
		}

		// Token: 0x040011D8 RID: 4568
		public List<string> userArgs;

		// Token: 0x040011D9 RID: 4569
		public NetworkUser sender;

		// Token: 0x040011DA RID: 4570
		public string commandName;
	}
}
