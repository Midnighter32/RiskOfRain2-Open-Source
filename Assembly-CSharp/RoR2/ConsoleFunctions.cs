using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035E RID: 862
	public class ConsoleFunctions : MonoBehaviour
	{
		// Token: 0x060014F0 RID: 5360 RVA: 0x000596F1 File Offset: 0x000578F1
		public void SubmitCmd(string cmd)
		{
			Console.instance.SubmitCmd(null, cmd, false);
		}
	}
}
