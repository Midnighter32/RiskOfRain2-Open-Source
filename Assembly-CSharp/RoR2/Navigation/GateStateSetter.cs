using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x020004DA RID: 1242
	public class GateStateSetter : MonoBehaviour
	{
		// Token: 0x06001DAB RID: 7595 RVA: 0x0007E9FA File Offset: 0x0007CBFA
		private void OnEnable()
		{
			this.UpdateGates(true);
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x0007EA03 File Offset: 0x0007CC03
		private void OnDisable()
		{
			this.UpdateGates(false);
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0007EA0C File Offset: 0x0007CC0C
		private void UpdateGates(bool enabledState)
		{
			if (!SceneInfo.instance)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.gateToEnableWhenEnabled))
			{
				SceneInfo.instance.SetGateState(this.gateToEnableWhenEnabled, enabledState);
			}
			if (!string.IsNullOrEmpty(this.gateToDisableWhenEnabled))
			{
				SceneInfo.instance.SetGateState(this.gateToDisableWhenEnabled, !enabledState);
			}
		}

		// Token: 0x04001AE2 RID: 6882
		public string gateToEnableWhenEnabled;

		// Token: 0x04001AE3 RID: 6883
		public string gateToDisableWhenEnabled;
	}
}
