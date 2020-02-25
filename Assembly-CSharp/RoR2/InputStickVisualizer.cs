using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000360 RID: 864
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputStickVisualizer : MonoBehaviour
	{
		// Token: 0x060014F6 RID: 5366 RVA: 0x000597B5 File Offset: 0x000579B5
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x000597C3 File Offset: 0x000579C3
		private Player GetPlayer()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem == null)
			{
				return null;
			}
			return eventSystem.player;
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x000597DB File Offset: 0x000579DB
		private CameraRigController GetCameraRigController()
		{
			if (CameraRigController.readOnlyInstancesList.Count <= 0)
			{
				return null;
			}
			return CameraRigController.readOnlyInstancesList[0];
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x000597F8 File Offset: 0x000579F8
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

		// Token: 0x060014FA RID: 5370 RVA: 0x00059860 File Offset: 0x00057A60
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

		// Token: 0x04001397 RID: 5015
		[Header("Move")]
		public Scrollbar moveXBar;

		// Token: 0x04001398 RID: 5016
		public Scrollbar moveYBar;

		// Token: 0x04001399 RID: 5017
		public TextMeshProUGUI moveXLabel;

		// Token: 0x0400139A RID: 5018
		public TextMeshProUGUI moveYLabel;

		// Token: 0x0400139B RID: 5019
		[Header("Aim")]
		public Scrollbar aimXBar;

		// Token: 0x0400139C RID: 5020
		public Scrollbar aimYBar;

		// Token: 0x0400139D RID: 5021
		public TextMeshProUGUI aimXLabel;

		// Token: 0x0400139E RID: 5022
		public TextMeshProUGUI aimYLabel;

		// Token: 0x0400139F RID: 5023
		public Scrollbar aimStickPostSmoothingXBar;

		// Token: 0x040013A0 RID: 5024
		public Scrollbar aimStickPostSmoothingYBar;

		// Token: 0x040013A1 RID: 5025
		public Scrollbar aimStickPostDualZoneXBar;

		// Token: 0x040013A2 RID: 5026
		public Scrollbar aimStickPostDualZoneYBar;

		// Token: 0x040013A3 RID: 5027
		public Scrollbar aimStickPostExponentXBar;

		// Token: 0x040013A4 RID: 5028
		public Scrollbar aimStickPostExponentYBar;

		// Token: 0x040013A5 RID: 5029
		private MPEventSystemLocator eventSystemLocator;
	}
}
