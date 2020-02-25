using System;

namespace RoR2
{
	// Token: 0x020000CE RID: 206
	public static class ConCommandArgExtensions
	{
		// Token: 0x06000400 RID: 1024 RVA: 0x0000FE40 File Offset: 0x0000E040
		public static int GetBodyIndex(this ConCommandArgs args, int index)
		{
			if (index < args.userArgs.Count)
			{
				int num = BodyCatalog.FindBodyIndexCaseInsensitive(args[index]);
				if (num != -1)
				{
					return num;
				}
			}
			throw new ConCommandException(string.Format("Argument {0} is not a valid body name.", index));
		}
	}
}
