using System;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200059E RID: 1438
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ChatBox : MonoBehaviour
	{
		// Token: 0x06002224 RID: 8740 RVA: 0x00093C1C File Offset: 0x00091E1C
		private void UpdateFade(float deltaTime)
		{
			this.fadeTimer -= deltaTime;
			if (!this.fadeGroup)
			{
				return;
			}
			float alpha;
			if (this.showInput)
			{
				alpha = 1f;
				this.ResetFadeTimer();
			}
			else if (this.fadeTimer < 0f)
			{
				alpha = 0f;
			}
			else if (this.fadeTimer < 5f)
			{
				alpha = Mathf.Sqrt(Util.Remap(this.fadeTimer, 5f, 0f, 1f, 0f));
			}
			else
			{
				alpha = 1f;
			}
			this.fadeGroup.alpha = alpha;
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x00093CB6 File Offset: 0x00091EB6
		private void ResetFadeTimer()
		{
			this.fadeTimer = 10f;
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06002226 RID: 8742 RVA: 0x00093CC3 File Offset: 0x00091EC3
		private bool showKeybindTipOnStart
		{
			get
			{
				return Chat.readOnlyLog.Count == 0;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06002227 RID: 8743 RVA: 0x00093CD2 File Offset: 0x00091ED2
		// (set) Token: 0x06002228 RID: 8744 RVA: 0x00093CDC File Offset: 0x00091EDC
		private bool showInput
		{
			get
			{
				return this._showInput;
			}
			set
			{
				if (this._showInput != value)
				{
					this._showInput = value;
					if (this.inputField)
					{
						this.inputField.gameObject.SetActive(this._showInput);
					}
					if (this.sendButton)
					{
						this.sendButton.gameObject.SetActive(this._showInput);
					}
					for (int i = 0; i < this.gameplayHiddenGraphics.Length; i++)
					{
						this.gameplayHiddenGraphics[i].enabled = this._showInput;
					}
					if (this._showInput)
					{
						this.FocusInputField();
						return;
					}
					this.UnfocusInputField();
				}
			}
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x00093D80 File Offset: 0x00091F80
		public void SubmitChat()
		{
			string text = this.inputField.text;
			if (text != "")
			{
				this.inputField.text = "";
				ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
				if (readOnlyLocalPlayersList.Count > 0)
				{
					string text2 = text;
					text2 = text2.Replace("\\", "\\\\");
					text2 = text2.Replace("\"", "\\\"");
					Console.instance.SubmitCmd(readOnlyLocalPlayersList[0], "say \"" + text2 + "\"", false);
					Debug.Log("Submitting say cmd.");
				}
			}
			Debug.LogFormat("SubmitChat() submittedText={0}", new object[]
			{
				text
			});
			this.showInput = false;
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnInputFieldEndEdit()
		{
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x00093E30 File Offset: 0x00092030
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.showInput = true;
			this.showInput = false;
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x00093E4C File Offset: 0x0009204C
		private void Start()
		{
			if (this.showKeybindTipOnStart && !RoR2Application.isInSinglePlayer)
			{
				Chat.AddMessage(Language.GetString("CHAT_KEYBIND_TIP"));
			}
			this.BuildChat();
			this.ScrollToBottom();
			this.inputField.resetOnDeActivation = true;
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x00093E84 File Offset: 0x00092084
		private void OnEnable()
		{
			this.BuildChat();
			this.ScrollToBottom();
			base.Invoke("ScrollToBottom", 0f);
			Chat.onChatChanged += this.OnChatChangedHandler;
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x00093EB3 File Offset: 0x000920B3
		private void OnDisable()
		{
			Chat.onChatChanged -= this.OnChatChangedHandler;
		}

		// Token: 0x0600222F RID: 8751 RVA: 0x00093EC6 File Offset: 0x000920C6
		private void OnChatChangedHandler()
		{
			float value = this.messagesText.verticalScrollbar.value;
			this.ResetFadeTimer();
			this.BuildChat();
			this.ScrollToBottom();
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x00093EEB File Offset: 0x000920EB
		public void ScrollToBottom()
		{
			this.messagesText.verticalScrollbar.value = 0f;
			this.messagesText.verticalScrollbar.value = 1f;
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x00093F18 File Offset: 0x00092118
		private void BuildChat()
		{
			ReadOnlyCollection<string> readOnlyLog = Chat.readOnlyLog;
			string[] array = new string[readOnlyLog.Count];
			readOnlyLog.CopyTo(array, 0);
			this.messagesText.text = string.Join("\n", array);
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x00093F54 File Offset: 0x00092154
		private void Update()
		{
			this.UpdateFade(Time.deltaTime);
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			GameObject gameObject = eventSystem ? eventSystem.currentSelectedGameObject : null;
			bool flag = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
			if (!this.showInput && flag && !(ConsoleWindow.instance != null))
			{
				this.showInput = true;
				return;
			}
			if (gameObject == this.inputField.gameObject)
			{
				if (flag)
				{
					if (this.showInput)
					{
						this.SubmitChat();
					}
					else if (!gameObject)
					{
						this.showInput = true;
					}
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					this.showInput = false;
					return;
				}
			}
			else
			{
				this.showInput = false;
			}
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x00094014 File Offset: 0x00092214
		private void FocusInputField()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem)
			{
				eventSystem.SetSelectedGameObject(this.inputField.gameObject);
			}
			this.inputField.ActivateInputField();
			this.inputField.text = "";
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x00094064 File Offset: 0x00092264
		private void UnfocusInputField()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem && eventSystem.currentSelectedGameObject == this.inputField.gameObject)
			{
				eventSystem.SetSelectedGameObject(null);
			}
			this.inputField.DeactivateInputField();
		}

		// Token: 0x04001F92 RID: 8082
		public TMP_InputField messagesText;

		// Token: 0x04001F93 RID: 8083
		public TMP_InputField inputField;

		// Token: 0x04001F94 RID: 8084
		public Button sendButton;

		// Token: 0x04001F95 RID: 8085
		public Graphic[] gameplayHiddenGraphics;

		// Token: 0x04001F96 RID: 8086
		[Tooltip("The canvas group to use to fade this chat box. Leave empty for no fading behavior.")]
		public CanvasGroup fadeGroup;

		// Token: 0x04001F97 RID: 8087
		private const float fadeWait = 5f;

		// Token: 0x04001F98 RID: 8088
		private const float fadeDuration = 5f;

		// Token: 0x04001F99 RID: 8089
		private float fadeTimer;

		// Token: 0x04001F9A RID: 8090
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001F9B RID: 8091
		private bool _showInput;
	}
}
