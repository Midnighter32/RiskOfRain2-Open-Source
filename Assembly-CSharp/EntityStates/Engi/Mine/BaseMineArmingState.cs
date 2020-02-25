using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.Mine
{
	// Token: 0x02000877 RID: 2167
	public class BaseMineArmingState : BaseState
	{
		// Token: 0x060030D4 RID: 12500 RVA: 0x000D23AC File Offset: 0x000D05AC
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlayScaledSound(this.onEnterSfx, base.gameObject, this.onEnterSfxPlaybackRate);
			if (!string.IsNullOrEmpty(this.pathToChildToEnable))
			{
				this.enabledChild = base.transform.Find(this.pathToChildToEnable);
				if (this.enabledChild)
				{
					this.enabledChild.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x000D2419 File Offset: 0x000D0619
		public override void OnExit()
		{
			if (this.enabledChild)
			{
				this.enabledChild.gameObject.SetActive(false);
			}
			base.OnExit();
		}

		// Token: 0x04002F12 RID: 12050
		[SerializeField]
		public float damageScale;

		// Token: 0x04002F13 RID: 12051
		[SerializeField]
		public float forceScale;

		// Token: 0x04002F14 RID: 12052
		[SerializeField]
		public float blastRadiusScale;

		// Token: 0x04002F15 RID: 12053
		[SerializeField]
		public float triggerRadius;

		// Token: 0x04002F16 RID: 12054
		[SerializeField]
		public string onEnterSfx;

		// Token: 0x04002F17 RID: 12055
		[SerializeField]
		public float onEnterSfxPlaybackRate;

		// Token: 0x04002F18 RID: 12056
		[SerializeField]
		public string pathToChildToEnable;

		// Token: 0x04002F19 RID: 12057
		private Transform enabledChild;
	}
}
