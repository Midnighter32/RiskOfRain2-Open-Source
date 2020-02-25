using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Facepunch.Steamworks;

namespace RoR2
{
	// Token: 0x020001B9 RID: 441
	[Serializable]
	public class ConCommandException : Exception
	{
		// Token: 0x06000975 RID: 2421 RVA: 0x0002922E File Offset: 0x0002742E
		public ConCommandException()
		{
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x00029236 File Offset: 0x00027436
		public ConCommandException(string message) : base(message)
		{
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0002923F File Offset: 0x0002743F
		public ConCommandException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x00029249 File Offset: 0x00027449
		protected ConCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x00029253 File Offset: 0x00027453
		public static void CheckSteamworks()
		{
			if (Client.Instance == null)
			{
				throw new ConCommandException("Steamworks not available.");
			}
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x00029267 File Offset: 0x00027467
		public static void CheckArgumentCount(List<string> args, int requiredArgCount)
		{
			if (args.Count < requiredArgCount)
			{
				throw new ConCommandException(string.Format("{0} argument(s) required, {1} argument(s) provided.", requiredArgCount, args.Count));
			}
		}
	}
}
