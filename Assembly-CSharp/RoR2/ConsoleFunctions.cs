using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000405 RID: 1029
	public class ConsoleFunctions : MonoBehaviour
	{
		// Token: 0x060016EA RID: 5866 RVA: 0x0006D341 File Offset: 0x0006B541
		public void SubmitCmd(string cmd)
		{
			Console.instance.SubmitCmd(null, cmd, false);
		}
	}
}
