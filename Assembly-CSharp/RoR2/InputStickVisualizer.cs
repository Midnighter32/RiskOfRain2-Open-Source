using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000407 RID: 1031
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputStickVisualizer : MonoBehaviour
	{
		// Token: 0x060016F0 RID: 5872 RVA: 0x0006D405 File Offset: 0x0006B605
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x0006D413 File Offset: 0x0006B613
		private Player GetPlayer()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem == null)
			{
				return null;
			}
			return eventSystem.player;
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x0006D42B File Offset: 0x0006B62B
		private CameraRigController GetCameraRigController()
		{
			if (CameraRigController.readOnlyInstancesList.Count <= 0)
			{
				return null;
			}
			return CameraRigController.readOnlyInstancesList[0];
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x0006D448 File Offset: 0x0006B648
		private void SetBarValues(Vector2 vector, Scrollbar scrollbarX, Scrollbar scrollbarY)
		{
			if (scrollbarX)
			{
				scrollbarX.value = Util.Remap(vector.x, -1f, 1f, 0f, 1f);
			}
			if (scrollbarY)
			{
				scrollbarY.value = Util.Remap(vector.y, -1f, 1f, 0f, 1f);
			}
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x0006D4B0 File Offset: 0x0006B6B0
		private void Update()
		{
			Player player = this.GetPlayer();
			CameraRigController cameraRigController = this.GetCameraRigController();
			if (!cameraRigController || player == null)
			{
				return;
			}
			Vector2 vector = new Vector2(player.GetAxis(0), player.GetAxis(1));
			Vector2 vector2 = new Vector2(player.GetAxis(16), player.GetAxis(17));
			this.SetBarValues(vector, this.moveXBar, this.moveYBar);
			this.SetBarValues(vector2, this.aimXBar, this.aimYBar);
			this.SetBarValues(cameraRigController.aimStickPostDualZone, this.aimStickPostDualZoneXBar, this.aimStickPostDualZoneYBar);
			this.SetBarValues(cameraRigController.aimStickPostExponent, this.aimStickPostExponentXBar, this.aimStickPostExponentYBar);
			this.SetBarValues(cameraRigController.aimStickPostSmoothing, this.aimStickPostSmoothingXBar, this.aimStickPostSmoothingYBar);
			this.moveXLabel.text = string.Format("move.x={0:0.0000}", vector.x);
			this.moveYLabel.text = string.Format("move.y={0:0.0000}", vector.y);
			this.aimXLabel.text = string.Format("aim.x={0:0.0000}", vector2.x);
			this.aimYLabel.text = string.Format("aim.y={0:0.0000}", vector2.y);
		}

		// Token: 0x04001A27 RID: 6695
		[Header("Move")]
		public Scrollbar moveXBar;

		// Token: 0x04001A28 RID: 6696
		public Scrollbar moveYBar;

		// Token: 0x04001A29 RID: 6697
		public TextMeshProUGUI moveXLabel;

		// Token: 0x04001A2A RID: 6698
		public TextMeshProUGUI moveYLabel;

		// Token: 0x04001A2B RID: 6699
		[Header("Aim")]
		public Scrollbar aimXBar;

		// Token: 0x04001A2C RID: 6700
		public Scrollbar aimYBar;

		// Token: 0x04001A2D RID: 6701
		public TextMeshProUGUI aimXLabel;

		// Token: 0x04001A2E RID: 6702
		public TextMeshProUGUI aimYLabel;

		// Token: 0x04001A2F RID: 6703
		public Scrollbar aimStickPostSmoothingXBar;

		// Token: 0x04001A30 RID: 6704
		public Scrollbar aimStickPostSmoothingYBar;

		// Token: 0x04001A31 RID: 6705
		public Scrollbar aimStickPostDualZoneXBar;

		// Token: 0x04001A32 RID: 6706
		public Scrollbar aimStickPostDualZoneYBar;

		// Token: 0x04001A33 RID: 6707
		public Scrollbar aimStickPostExponentXBar;

		// Token: 0x04001A34 RID: 6708
		public Scrollbar aimStickPostExponentYBar;

		// Token: 0x04001A35 RID: 6709
		private MPEventSystemLocator eventSystemLocator;
	}
}
