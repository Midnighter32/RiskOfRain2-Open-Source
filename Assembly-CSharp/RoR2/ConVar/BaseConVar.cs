using System;
using UnityEngine;

namespace RoR2.ConVar
{
	// Token: 0x02000677 RID: 1655
	public abstract class BaseConVar
	{
		// Token: 0x060026D7 RID: 9943 RVA: 0x000A9850 File Offset: 0x000A7A50
		protected BaseConVar(string name, ConVarFlags flags, string defaultValue, string helpText)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.flags = flags;
			this.defaultValue = defaultValue;
			if (helpText == null)
			{
				throw new ArgumentNullException("helpText");
			}
			this.helpText = helpText;
		}

		// Token: 0x060026D8 RID: 9944 RVA: 0x000A98A0 File Offset: 0x000A7AA0
		public void AttemptSetString(string newValue)
		{
			try
			{
				this.SetString(newValue);
			}
			catch (ConCommandException ex)
			{
				Debug.LogFormat("Could not set value of ConVar \"{0}\": {1}", new object[]
				{
					ex.Message
				});
			}
		}

		// Token: 0x060026D9 RID: 9945
		public abstract void SetString(string newValue);

		// Token: 0x060026DA RID: 9946
		public abstract string GetString();

		// Token: 0x040024B5 RID: 9397
		public string name;

		// Token: 0x040024B6 RID: 9398
		public ConVarFlags flags;

		// Token: 0x040024B7 RID: 9399
		public string defaultValue;

		// Token: 0x040024B8 RID: 9400
		public string helpText;
	}
}
