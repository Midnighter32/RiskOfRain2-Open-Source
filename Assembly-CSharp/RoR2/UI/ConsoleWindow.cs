using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Rewired;
using RoR2.ConVar;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005A4 RID: 1444
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class ConsoleWindow : MonoBehaviour
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x0600225B RID: 8795 RVA: 0x00094884 File Offset: 0x00092A84
		// (set) Token: 0x0600225C RID: 8796 RVA: 0x0009488B File Offset: 0x00092A8B
		public static ConsoleWindow instance { get; private set; }

		// Token: 0x0600225D RID: 8797 RVA: 0x00094894 File Offset: 0x00092A94
		public void Start()
		{
			base.GetComponent<MPEventSystemProvider>().eventSystem = MPEventSystemManager.kbmEventSystem;
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 1f;
			}
			this.outputField.textComponent.gameObject.AddComponent<RectTransformDimensionsChangeEvent>().onRectTransformDimensionsChange += this.OnOutputFieldRectTransformDimensionsChange;
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x000948FE File Offset: 0x00092AFE
		private void OnOutputFieldRectTransformDimensionsChange()
		{
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
			}
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x0009493C File Offset: 0x00092B3C
		public void OnEnable()
		{
			Console.onLogReceived += this.OnLogReceived;
			Console.onClear += this.OnClear;
			this.RebuildOutput();
			this.inputField.onSubmit.AddListener(new UnityAction<string>(this.Submit));
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldValueChanged));
			ConsoleWindow.instance = this;
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x000949AF File Offset: 0x00092BAF
		public void SubmitInputField()
		{
			this.inputField.onSubmit.Invoke(this.inputField.text);
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x000949CC File Offset: 0x00092BCC
		public void Submit(string text)
		{
			if (this.inputField.text == "")
			{
				return;
			}
			if (this.autoCompleteDropdown)
			{
				this.autoCompleteDropdown.Hide();
			}
			this.inputField.text = "";
			ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
			NetworkUser sender = null;
			if (readOnlyLocalPlayersList.Count > 0)
			{
				sender = readOnlyLocalPlayersList[0];
			}
			Console.instance.SubmitCmd(sender, text, true);
			if (this.inputField && this.inputField.IsActive())
			{
				this.inputField.ActivateInputField();
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06002262 RID: 8802 RVA: 0x00094A64 File Offset: 0x00092C64
		private bool autoCompleteInUse
		{
			get
			{
				return this.autoCompleteDropdown && this.autoCompleteDropdown.IsExpanded;
			}
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x00094A80 File Offset: 0x00092C80
		private void OnInputFieldValueChanged(string text)
		{
			if (!this.preventHistoryReset)
			{
				this.historyIndex = -1;
			}
			if (!this.preventAutoCompleteUpdate)
			{
				if (text.Length > 0 != (this.autoComplete != null))
				{
					if (this.autoComplete != null)
					{
						UnityEngine.Object.Destroy(this.autoCompleteDropdown.gameObject);
						this.autoComplete = null;
					}
					else
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ConsoleAutoCompleteDropdown"), this.inputField.transform);
						this.autoCompleteDropdown = gameObject.GetComponent<TMP_Dropdown>();
						this.autoComplete = new Console.AutoComplete(Console.instance);
						this.autoCompleteDropdown.onValueChanged.AddListener(new UnityAction<int>(this.ApplyAutoComplete));
					}
				}
				if (this.autoComplete != null && this.autoComplete.SetSearchString(text))
				{
					List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
					List<string> resultsList = this.autoComplete.resultsList;
					for (int i = 0; i < resultsList.Count; i++)
					{
						list.Add(new TMP_Dropdown.OptionData(resultsList[i]));
					}
					this.autoCompleteDropdown.options = list;
				}
			}
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x00094B88 File Offset: 0x00092D88
		public void Update()
		{
			EventSystem eventSystem = MPEventSystemManager.FindEventSystem(ReInput.players.GetPlayer(0));
			if (eventSystem && eventSystem.currentSelectedGameObject == this.inputField.gameObject)
			{
				ConsoleWindow.InputFieldState inputFieldState = ConsoleWindow.InputFieldState.Neutral;
				if (this.autoCompleteDropdown && this.autoCompleteInUse)
				{
					inputFieldState = ConsoleWindow.InputFieldState.AutoComplete;
				}
				else if (this.historyIndex != -1)
				{
					inputFieldState = ConsoleWindow.InputFieldState.History;
				}
				bool keyDown = Input.GetKeyDown(KeyCode.UpArrow);
				bool keyDown2 = Input.GetKeyDown(KeyCode.DownArrow);
				switch (inputFieldState)
				{
				case ConsoleWindow.InputFieldState.Neutral:
					if (keyDown)
					{
						if (Console.userCmdHistory.Count > 0)
						{
							this.historyIndex = Console.userCmdHistory.Count - 1;
							this.preventHistoryReset = true;
							this.inputField.text = Console.userCmdHistory[this.historyIndex];
							this.inputField.MoveToEndOfLine(false, false);
							this.preventHistoryReset = false;
						}
					}
					else if (keyDown2 && this.autoCompleteDropdown)
					{
						this.autoCompleteDropdown.Show();
						this.autoCompleteDropdown.value = 0;
						this.autoCompleteDropdown.onValueChanged.Invoke(this.autoCompleteDropdown.value);
					}
					break;
				case ConsoleWindow.InputFieldState.History:
				{
					int num = 0;
					if (keyDown)
					{
						num--;
					}
					if (keyDown2)
					{
						num++;
					}
					if (num != 0)
					{
						this.historyIndex += num;
						if (this.historyIndex < 0)
						{
							this.historyIndex = 0;
						}
						if (this.historyIndex >= Console.userCmdHistory.Count)
						{
							this.historyIndex = -1;
						}
						else
						{
							this.preventHistoryReset = true;
							this.inputField.text = Console.userCmdHistory[this.historyIndex];
							this.inputField.MoveToEndOfLine(false, false);
							this.preventHistoryReset = false;
						}
					}
					break;
				}
				case ConsoleWindow.InputFieldState.AutoComplete:
					if (keyDown2)
					{
						TMP_Dropdown tmp_Dropdown = this.autoCompleteDropdown;
						int value = tmp_Dropdown.value + 1;
						tmp_Dropdown.value = value;
					}
					if (keyDown)
					{
						if (this.autoCompleteDropdown.value > 0)
						{
							TMP_Dropdown tmp_Dropdown2 = this.autoCompleteDropdown;
							int value = tmp_Dropdown2.value - 1;
							tmp_Dropdown2.value = value;
						}
						else
						{
							this.autoCompleteDropdown.Hide();
						}
					}
					break;
				}
				eventSystem.SetSelectedGameObject(this.inputField.gameObject);
			}
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x00094DC0 File Offset: 0x00092FC0
		private void ApplyAutoComplete(int optionIndex)
		{
			if (this.autoCompleteDropdown && this.autoCompleteDropdown.options.Count > optionIndex)
			{
				this.preventAutoCompleteUpdate = true;
				this.inputField.text = this.autoCompleteDropdown.options[optionIndex].text;
				this.inputField.MoveToEndOfLine(false, false);
				this.preventAutoCompleteUpdate = false;
			}
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x00094E2C File Offset: 0x0009302C
		public void OnDisable()
		{
			Console.onLogReceived -= this.OnLogReceived;
			Console.onClear -= this.OnClear;
			this.inputField.onSubmit.RemoveListener(new UnityAction<string>(this.Submit));
			this.inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.OnInputFieldValueChanged));
			if (ConsoleWindow.instance == this)
			{
				ConsoleWindow.instance = null;
			}
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x00094EA6 File Offset: 0x000930A6
		private void OnLogReceived(Console.Log log)
		{
			this.RebuildOutput();
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x00094EA6 File Offset: 0x000930A6
		private void OnClear()
		{
			this.RebuildOutput();
		}

		// Token: 0x06002269 RID: 8809 RVA: 0x00094EB0 File Offset: 0x000930B0
		private void RebuildOutput()
		{
			float value = 0f;
			if (this.outputField.verticalScrollbar)
			{
				value = this.outputField.verticalScrollbar.value;
			}
			string[] array = new string[Console.logs.Count];
			this.stringBuilder.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				this.stringBuilder.AppendLine(Console.logs[i].message);
			}
			this.outputField.text = this.stringBuilder.ToString();
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
				this.outputField.verticalScrollbar.value = value;
			}
		}

		// Token: 0x0600226A RID: 8810 RVA: 0x00094F90 File Offset: 0x00093190
		private static void CheckConsoleKey()
		{
			bool keyDown = Input.GetKeyDown(KeyCode.BackQuote);
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && keyDown)
			{
				ConsoleWindow.cvConsoleEnabled.SetBool(!ConsoleWindow.cvConsoleEnabled.value);
			}
			if (ConsoleWindow.cvConsoleEnabled.value && keyDown)
			{
				if (ConsoleWindow.instance)
				{
					UnityEngine.Object.Destroy(ConsoleWindow.instance.gameObject);
				}
				else
				{
					UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ConsoleWindow")).GetComponent<ConsoleWindow>().inputField.ActivateInputField();
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape) && ConsoleWindow.instance)
			{
				UnityEngine.Object.Destroy(ConsoleWindow.instance.gameObject);
			}
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x00095046 File Offset: 0x00093246
		[RuntimeInitializeOnLoadMethod]
		public static void Init()
		{
			RoR2Application.onUpdate += ConsoleWindow.CheckConsoleKey;
		}

		// Token: 0x04001FBF RID: 8127
		public TMP_InputField inputField;

		// Token: 0x04001FC0 RID: 8128
		public TMP_InputField outputField;

		// Token: 0x04001FC1 RID: 8129
		private TMP_Dropdown autoCompleteDropdown;

		// Token: 0x04001FC2 RID: 8130
		private Console.AutoComplete autoComplete;

		// Token: 0x04001FC3 RID: 8131
		private bool preventAutoCompleteUpdate;

		// Token: 0x04001FC4 RID: 8132
		private bool preventHistoryReset;

		// Token: 0x04001FC5 RID: 8133
		private int historyIndex = -1;

		// Token: 0x04001FC6 RID: 8134
		private readonly StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04001FC7 RID: 8135
		private const string consoleEnabledDefaultValue = "0";

		// Token: 0x04001FC8 RID: 8136
		private static BoolConVar cvConsoleEnabled = new BoolConVar("console_enabled", ConVarFlags.None, "0", "Enables/Disables the console.");

		// Token: 0x020005A5 RID: 1445
		private enum InputFieldState
		{
			// Token: 0x04001FCA RID: 8138
			Neutral,
			// Token: 0x04001FCB RID: 8139
			History,
			// Token: 0x04001FCC RID: 8140
			AutoComplete
		}
	}
}
