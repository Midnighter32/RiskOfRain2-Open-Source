using System;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C2 RID: 1474
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ChatBox : MonoBehaviour
	{
		// Token: 0x060020FA RID: 8442 RVA: 0x0009B068 File Offset: 0x00099268
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

		// Token: 0x060020FB RID: 8443 RVA: 0x0009B102 File Offset: 0x00099302
		private void ResetFadeTimer()
		{
			this.fadeTimer = 10f;
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060020FC RID: 8444 RVA: 0x0009B10F File Offset: 0x0009930F
		private bool showKeybindTipOnStart
		{
			get
			{
				return Chat.readOnlyLog.Count == 0;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060020FD RID: 8445 RVA: 0x0009B11E File Offset: 0x0009931E
		// (set) Token: 0x060020FE RID: 8446 RVA: 0x0009B128 File Offset: 0x00099328
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

		// Token: 0x060020FF RID: 8447 RVA: 0x0009B1CC File Offset: 0x000993CC
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

		// Token: 0x06002100 RID: 8448 RVA: 0x00004507 File Offset: 0x00002707
		public void OnInputFieldEndEdit()
		{
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x0009B27C File Offset: 0x0009947C
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.showInput = true;
			this.showInput = false;
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x0009B298 File Offset: 0x00099498
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

		// Token: 0x06002103 RID: 8451 RVA: 0x0009B2D0 File Offset: 0x000994D0
		private void OnEnable()
		{
			this.BuildChat();
			this.ScrollToBottom();
			base.Invoke("ScrollToBottom", 0f);
			Chat.onChatChanged += this.OnChatChangedHandler;
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x0009B2FF File Offset: 0x000994FF
		private void OnDisable()
		{
			Chat.onChatChanged -= this.OnChatChangedHandler;
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x0009B312 File Offset: 0x00099512
		private void OnChatChangedHandler()
		{
			float value = this.messagesText.verticalScrollbar.value;
			this.ResetFadeTimer();
			this.BuildChat();
			this.ScrollToBottom();
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x0009B337 File Offset: 0x00099537
		public void ScrollToBottom()
		{
			this.messagesText.verticalScrollbar.value = 0f;
			this.messagesText.verticalScrollbar.value = 1f;
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x0009B364 File Offset: 0x00099564
		private void BuildChat()
		{
			ReadOnlyCollection<string> readOnlyLog = Chat.readOnlyLog;
			string[] array = new string[readOnlyLog.Count];
			readOnlyLog.CopyTo(array, 0);
			this.messagesText.text = string.Join("\n", array);
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x0009B3A0 File Offset: 0x000995A0
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

		// Token: 0x06002109 RID: 8457 RVA: 0x0009B460 File Offset: 0x00099660
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

		// Token: 0x0600210A RID: 8458 RVA: 0x0009B4B0 File Offset: 0x000996B0
		private void UnfocusInputField()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem && eventSystem.currentSelectedGameObject == this.inputField.gameObject)
			{
				eventSystem.SetSelectedGameObject(null);
			}
			this.inputField.DeactivateInputField();
		}

		// Token: 0x04002395 RID: 9109
		public TMP_InputField messagesText;

		// Token: 0x04002396 RID: 9110
		public TMP_InputField inputField;

		// Token: 0x04002397 RID: 9111
		public Button sendButton;

		// Token: 0x04002398 RID: 9112
		public Graphic[] gameplayHiddenGraphics;

		// Token: 0x04002399 RID: 9113
		[Tooltip("The canvas group to use to fade this chat box. Leave empty for no fading behavior.")]
		public CanvasGroup fadeGroup;

		// Token: 0x0400239A RID: 9114
		private const float fadeWait = 5f;

		// Token: 0x0400239B RID: 9115
		private const float fadeDuration = 5f;

		// Token: 0x0400239C RID: 9116
		private float fadeTimer;

		// Token: 0x0400239D RID: 9117
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400239E RID: 9118
		private bool _showInput;
	}
}
