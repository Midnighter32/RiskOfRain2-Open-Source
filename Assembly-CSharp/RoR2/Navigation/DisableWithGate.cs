using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200051F RID: 1311
	public class DisableWithGate : MonoBehaviour
	{
		// Token: 0x06001D78 RID: 7544 RVA: 0x0008968C File Offset: 0x0008788C
		private void Awake()
		{
			if (SceneInfo.instance && SceneInfo.instance.groundNodes.IsGateOpen(this.gateToMatch) == this.invert)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04001FC7 RID: 8135
		public string gateToMatch;

		// Token: 0x04001FC8 RID: 8136
		public bool invert;
	}
}
