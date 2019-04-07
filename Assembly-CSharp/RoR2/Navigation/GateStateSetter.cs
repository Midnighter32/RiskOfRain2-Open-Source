using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000520 RID: 1312
	public class GateStateSetter : MonoBehaviour
	{
		// Token: 0x06001D7A RID: 7546 RVA: 0x000896C3 File Offset: 0x000878C3
		private void OnEnable()
		{
			this.UpdateGates(true);
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x000896CC File Offset: 0x000878CC
		private void OnDisable()
		{
			this.UpdateGates(false);
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x000896D8 File Offset: 0x000878D8
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

		// Token: 0x04001FC9 RID: 8137
		public string gateToEnableWhenEnabled;

		// Token: 0x04001FCA RID: 8138
		public string gateToDisableWhenEnabled;
	}
}
