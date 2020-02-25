using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI
{
	// Token: 0x0200060D RID: 1549
	[RequireComponent(typeof(MPEventSystemProvider))]
	[RequireComponent(typeof(RectTransform))]
	public class PauseScreenController : MonoBehaviour
	{
		// Token: 0x060024AF RID: 9391 RVA: 0x0009FF9B File Offset: 0x0009E19B
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
		}

		// Token: 0x060024B0 RID: 9392 RVA: 0x0009FFAC File Offset: 0x0009E1AC
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

		// Token: 0x060024B1 RID: 9393 RVA: 0x000A0008 File Offset: 0x0009E208
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

		// Token: 0x060024B2 RID: 9394 RVA: 0x000A0055 File Offset: 0x0009E255
		public void OpenSettingsMenu()
		{
			UnityEngine.Object.Destroy(this.submenuObject);
			this.submenuObject = UnityEngine.Object.Instantiate<GameObject>(this.settingsPanelPrefab, this.rootPanel);
			this.mainPanel.gameObject.SetActive(false);
		}

		// Token: 0x060024B3 RID: 9395 RVA: 0x000A008A File Offset: 0x0009E28A
		public void Update()
		{
			if (!this.submenuObject)
			{
				this.mainPanel.gameObject.SetActive(true);
			}
			if (!NetworkManager.singleton.isNetworkActive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04002265 RID: 8805
		public static readonly List<PauseScreenController> instancesList = new List<PauseScreenController>();

		// Token: 0x04002266 RID: 8806
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x04002267 RID: 8807
		[Tooltip("The main panel to which any submenus will be parented.")]
		public RectTransform rootPanel;

		// Token: 0x04002268 RID: 8808
		[Tooltip("The panel which contains the main controls. This will be disabled while in a submenu.")]
		public RectTransform mainPanel;

		// Token: 0x04002269 RID: 8809
		public GameObject settingsPanelPrefab;

		// Token: 0x0400226A RID: 8810
		private GameObject submenuObject;

		// Token: 0x0400226B RID: 8811
		private static float oldTimeScale;

		// Token: 0x0400226C RID: 8812
		private static bool paused = false;
	}
}
