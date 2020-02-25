using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x020004D9 RID: 1241
	public class DisableWithGate : MonoBehaviour
	{
		// Token: 0x06001DA9 RID: 7593 RVA: 0x0007E9C3 File Offset: 0x0007CBC3
		private void Awake()
		{
			if (SceneInfo.instance && SceneInfo.instance.groundNodes.IsGateOpen(this.gateToMatch) == this.invert)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04001AE0 RID: 6880
		public string gateToMatch;

		// Token: 0x04001AE1 RID: 6881
		public bool invert;
	}
}
