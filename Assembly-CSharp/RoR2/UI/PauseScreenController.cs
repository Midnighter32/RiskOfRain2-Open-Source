using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI
{
	// Token: 0x0200061E RID: 1566
	[RequireComponent(typeof(MPEventSystemProvider))]
	[RequireComponent(typeof(RectTransform))]
	public class PauseScreenController : MonoBehaviour
	{
		// Token: 0x06002339 RID: 9017 RVA: 0x000A5CCF File Offset: 0x000A3ECF
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x000A5CE0 File Offset: 0x000A3EE0
		private void OnEnable()
		{
			if (PauseScreenController.instancesList.Count == 0)
			{
				PauseScreenController.paused = NetworkServer.dontListen;
				if (PauseScreenController.paused)
				{
					if (RoR2Application.onPauseStartGlobal != null)
					{
						RoR2Application.onPauseStartGlobal();
					}
					PauseScreenController.oldTimeScale = Time.timeScale;
					Time.timeScale = 0f;
				}
			}
			PauseScreenController.instancesList.Add(this);
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x000A5D3C File Offset: 0x000A3F3C
		private void OnDisable()
		{
			PauseScreenController.instancesList.Remove(this);
			if (PauseScreenController.instancesList.Count == 0 && PauseScreenController.paused)
			{
				Time.timeScale = PauseScreenController.oldTimeScale;
				PauseScreenController.paused = false;
				if (RoR2Application.onPauseEndGlobal != null)
				{
					RoR2Application.onPauseEndGlobal();
				}
			}
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x000A5D89 File Offset: 0x000A3F89
		public void OpenSettingsMenu()
		{
			UnityEngine.Object.Destroy(this.submenuObject);
			this.submenuObject = UnityEngine.Object.Instantiate<GameObject>(this.settingsPanelPrefab, this.rootPanel);
			this.mainPanel.gameObject.SetActive(false);
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x000A5DBE File Offset: 0x000A3FBE
		public void Update()
		{
			if (!this.submenuObject)
			{
				this.mainPanel.gameObject.SetActive(true);
			}
		}

		// Token: 0x0400261E RID: 9758
		public static readonly List<PauseScreenController> instancesList = new List<PauseScreenController>();

		// Token: 0x0400261F RID: 9759
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x04002620 RID: 9760
		[Tooltip("The main panel to which any submenus will be parented.")]
		public RectTransform rootPanel;

		// Token: 0x04002621 RID: 9761
		[Tooltip("The panel which contains the main controls. This will be disabled while in a submenu.")]
		public RectTransform mainPanel;

		// Token: 0x04002622 RID: 9762
		public GameObject settingsPanelPrefab;

		// Token: 0x04002623 RID: 9763
		private GameObject submenuObject;

		// Token: 0x04002624 RID: 9764
		private static float oldTimeScale;

		// Token: 0x04002625 RID: 9765
		private static bool paused = false;
	}
}
