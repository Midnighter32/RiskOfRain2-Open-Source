using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C2 RID: 962
	public class RunConsoleStringOnEnable : MonoBehaviour
	{
		// Token: 0x060014EA RID: 5354 RVA: 0x00064A45 File Offset: 0x00062C45
		private void OnEnable()
		{
			Console.instance.SubmitCmd(null, this.consoleString, false);
		}

		// Token: 0x04001843 RID: 6211
		public string consoleString;
	}
}
