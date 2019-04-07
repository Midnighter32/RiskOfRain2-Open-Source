using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Facepunch.Steamworks;

namespace RoR2
{
	// Token: 0x020002AA RID: 682
	[Serializable]
	public class ConCommandException : Exception
	{
		// Token: 0x06000DD7 RID: 3543 RVA: 0x000441CE File Offset: 0x000423CE
		public ConCommandException()
		{
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x000441D6 File Offset: 0x000423D6
		public ConCommandException(string message) : base(message)
		{
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x000441DF File Offset: 0x000423DF
		public ConCommandException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x000441E9 File Offset: 0x000423E9
		protected ConCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x000441F3 File Offset: 0x000423F3
		public static void CheckSteamworks()
		{
			if (Client.Instance == null)
			{
				throw new ConCommandException("Steamworks not available.");
			}
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x00044207 File Offset: 0x00042407
		public static void CheckArgumentCount(List<string> args, int requiredArgCount)
		{
			if (args.Count < requiredArgCount)
			{
				throw new ConCommandException(string.Format("{0} argument(s) required, {1} argument(s) provided.", requiredArgCount, args.Count));
			}
		}
	}
}
