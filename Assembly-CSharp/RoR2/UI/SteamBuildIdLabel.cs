using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000642 RID: 1602
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SteamBuildIdLabel : MonoBehaviour
	{
		// Token: 0x060023DA RID: 9178 RVA: 0x000A86FC File Offset: 0x000A68FC
		private void Start()
		{
			if (Client.Instance != null)
			{
				string text = "Steam Build ID " + RoR2Application.GetBuildId();
				string betaName = Client.Instance.BetaName;
				if (!string.IsNullOrEmpty(betaName))
				{
					text = text + "[" + betaName + "]";
				}
				base.GetComponent<TextMeshProUGUI>().text = text;
				return;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
