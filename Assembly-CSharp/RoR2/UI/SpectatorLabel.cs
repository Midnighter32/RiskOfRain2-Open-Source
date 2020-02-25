using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000634 RID: 1588
	public class SpectatorLabel : MonoBehaviour
	{
		// Token: 0x0600256F RID: 9583 RVA: 0x000A2E9B File Offset: 0x000A109B
		private void Awake()
		{
			this.labelRoot.SetActive(false);
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x000A2EA9 File Offset: 0x000A10A9
		private void Update()
		{
			this.UpdateLabel();
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x000A2EB4 File Offset: 0x000A10B4
		private void UpdateLabel()
		{
			CameraRigController cameraRigController = this.hud.cameraRigController;
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			if (cameraRigController)
			{
				gameObject = cameraRigController.target;
				gameObject2 = cameraRigController.localUserViewer.cachedBodyObject;
			}
			if (gameObject == gameObject2 || !(gameObject != null))
			{
				this.labelRoot.SetActive(false);
				this.cachedTarget = null;
				return;
			}
			this.labelRoot.SetActive(true);
			if (this.cachedTarget == gameObject)
			{
				return;
			}
			string text = gameObject ? Util.GetBestBodyName(gameObject) : "";
			this.label.SetText(Language.GetStringFormatted("SPECTATING_NAME_FORMAT", new object[]
			{
				text
			}));
			this.cachedTarget = gameObject;
		}

		// Token: 0x04002328 RID: 9000
		public HUD hud;

		// Token: 0x04002329 RID: 9001
		public HGTextMeshProUGUI label;

		// Token: 0x0400232A RID: 9002
		public GameObject labelRoot;

		// Token: 0x0400232B RID: 9003
		private GameObject cachedTarget;

		// Token: 0x0400232C RID: 9004
		private HudElement hudElement;
	}
}
