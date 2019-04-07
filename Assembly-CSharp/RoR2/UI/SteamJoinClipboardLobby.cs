using System;
using System.Globalization;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000643 RID: 1603
	public class SteamJoinClipboardLobby : MonoBehaviour
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060023DC RID: 9180 RVA: 0x000A875D File Offset: 0x000A695D
		// (set) Token: 0x060023DD RID: 9181 RVA: 0x000A8765 File Offset: 0x000A6965
		public bool validClipboardLobbyID { get; private set; }

		// Token: 0x060023DE RID: 9182 RVA: 0x000A876E File Offset: 0x000A696E
		private void OnEnable()
		{
			SingletonHelper.Assign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x000A877B File Offset: 0x000A697B
		private void OnDisable()
		{
			SingletonHelper.Unassign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x000A8788 File Offset: 0x000A6988
		static SteamJoinClipboardLobby()
		{
			SteamworksLobbyManager.onLobbyJoined += SteamJoinClipboardLobby.OnLobbyJoined;
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x000A879C File Offset: 0x000A699C
		private static void OnLobbyJoined(bool success)
		{
			if (SteamJoinClipboardLobby.instance && SteamJoinClipboardLobby.instance.resultTextComponent)
			{
				SteamJoinClipboardLobby.instance.resultTextTimer = 4f;
				SteamJoinClipboardLobby.instance.resultTextComponent.text = Language.GetString(success ? "STEAM_JOIN_LOBBY_CLIPBOARD_SUCCESS" : "STEAM_JOIN_LOBBY_CLIPBOARD_FAIL");
			}
		}

		// Token: 0x060023E2 RID: 9186 RVA: 0x000A87F8 File Offset: 0x000A69F8
		private void FixedUpdate()
		{
			Client client = Client.Instance;
			this.validClipboardLobbyID = false;
			if (client != null)
			{
				string systemCopyBuffer = GUIUtility.systemCopyBuffer;
				this.validClipboardLobbyID = (CSteamID.TryParse(systemCopyBuffer, out this.clipboardLobbyID) && this.clipboardLobbyID.isLobby && this.clipboardLobbyID.value != client.Lobby.CurrentLobby);
			}
			this.buttonText.text = string.Format(Language.GetString("STEAM_JOIN_LOBBY_ON_CLIPBOARD"), Array.Empty<object>());
			if (this.resultTextTimer > 0f)
			{
				this.resultTextTimer -= Time.fixedDeltaTime;
				this.resultTextComponent.enabled = true;
				return;
			}
			this.resultTextComponent.enabled = false;
		}

		// Token: 0x060023E3 RID: 9187 RVA: 0x000A88B1 File Offset: 0x000A6AB1
		public void TryToJoinClipboardLobby()
		{
			Console.instance.SubmitCmd(null, string.Format(CultureInfo.InvariantCulture, "steam_lobby_join {0}", this.clipboardLobbyID.ToString()), true);
		}

		// Token: 0x040026CE RID: 9934
		public TextMeshProUGUI buttonText;

		// Token: 0x040026CF RID: 9935
		public TextMeshProUGUI resultTextComponent;

		// Token: 0x040026D0 RID: 9936
		public MPButton mpButton;

		// Token: 0x040026D1 RID: 9937
		private CSteamID clipboardLobbyID;

		// Token: 0x040026D3 RID: 9939
		private const float resultTextDuration = 4f;

		// Token: 0x040026D4 RID: 9940
		protected float resultTextTimer;

		// Token: 0x040026D5 RID: 9941
		private static SteamJoinClipboardLobby instance;
	}
}
