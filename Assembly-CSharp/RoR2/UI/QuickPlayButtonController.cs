using System;
using RoR2.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000626 RID: 1574
	public class QuickPlayButtonController : UIBehaviour
	{
		// Token: 0x06002354 RID: 9044 RVA: 0x000A65D1 File Offset: 0x000A47D1
		protected new void Start()
		{
			base.Start();
			this.Update();
		}

		// Token: 0x06002355 RID: 9045 RVA: 0x000A65E0 File Offset: 0x000A47E0
		protected void Update()
		{
			bool running = SteamLobbyFinder.running;
			this.quickplayStateText.text = SteamLobbyFinder.GetResolvedStateString();
			if (running)
			{
				this.spinnerRectTransform.gameObject.SetActive(true);
				this.quickplayStateText.gameObject.SetActive(true);
				Vector3 localEulerAngles = this.spinnerRectTransform.localEulerAngles;
				localEulerAngles.z += Time.deltaTime * 360f;
				this.spinnerRectTransform.localEulerAngles = localEulerAngles;
				this.labelController.token = this.stopToken;
				return;
			}
			this.spinnerRectTransform.gameObject.SetActive(false);
			this.quickplayStateText.gameObject.SetActive(false);
			this.labelController.token = this.startToken;
		}

		// Token: 0x06002356 RID: 9046 RVA: 0x000A6699 File Offset: 0x000A4899
		public void ToggleQuickplay()
		{
			Console.instance.SubmitCmd(null, SteamLobbyFinder.running ? "steam_quickplay_stop" : "steam_quickplay_start", false);
		}

		// Token: 0x04002652 RID: 9810
		public LanguageTextMeshController labelController;

		// Token: 0x04002653 RID: 9811
		public string startToken;

		// Token: 0x04002654 RID: 9812
		public string stopToken;

		// Token: 0x04002655 RID: 9813
		public RectTransform spinnerRectTransform;

		// Token: 0x04002656 RID: 9814
		public TextMeshProUGUI quickplayStateText;
	}
}
