using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200030B RID: 779
	public class RunConsoleStringOnEnable : MonoBehaviour
	{
		// Token: 0x06001253 RID: 4691 RVA: 0x0004F199 File Offset: 0x0004D399
		private void OnEnable()
		{
			Console.instance.SubmitCmd(null, this.consoleString, false);
		}

		// Token: 0x04001148 RID: 4424
		public string consoleString;
	}
}
