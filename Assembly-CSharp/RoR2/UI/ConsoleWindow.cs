using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using RoR2.ConVar;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005C6 RID: 1478
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class ConsoleWindow : MonoBehaviour
	{
		// Token: 0x170002EA RID: 746
		// (get) Token: 0x0600212C RID: 8492 RVA: 0x0009BACC File Offset: 0x00099CCC
		// (set) Token: 0x0600212D RID: 8493 RVA: 0x0009BAD3 File Offset: 0x00099CD3
		public static ConsoleWindow instance { get; private set; }

		// Token: 0x0600212E RID: 8494 RVA: 0x0009BADC File Offset: 0x00099CDC
		public void Start()
		{
			base.GetComponent<MPEventSystemProvider>().eventSystem = MPEventSystemManager.kbmEventSystem;
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 1f;
			}
			this.outputField.textComponent.gameObject.AddComponent<RectTransformDimensionsChangeEvent>().onRectTransformDimensionsChange += this.OnOutputFieldRectTransformDimensionsChange;
		}

		// Token: 0x0600212F RID: 8495 RVA: 0x0009BB46 File Offset: 0x00099D46
		private void OnOutputFieldRectTransformDimensionsChange()
		{
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
			}
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x0009BB84 File Offset: 0x00099D84
		public void OnEnable()
		{
			Console.onLogReceived += this.OnLogReceived;
			Console.onClear += this.OnClear;
			this.RebuildOutput();
			this.inputField.onSubmit.AddListener(new UnityAction<string>(this.Submit));
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldValueChanged));
			ConsoleWindow.instance = this;
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x0009BBF7 File Offset: 0x00099DF7
		public void SubmitInputField()
		{
			this.inputField.onSubmit.Invoke(this.inputField.text);
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x0009BC14 File Offset: 0x00099E14
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

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06002133 RID: 8499 RVA: 0x0009BCAC File Offset: 0x00099EAC
		private bool autoCompleteInUse
		{
			get
			{
				return this.autoCompleteDropdown && this.autoCompleteDropdown.IsExpanded;
			}
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x0009BCC8 File Offset: 0x00099EC8
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

		// Token: 0x06002135 RID: 8501 RVA: 0x0009BDD0 File Offset: 0x00099FD0
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

		// Token: 0x06002136 RID: 8502 RVA: 0x0009C008 File Offset: 0x0009A208
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

		// Token: 0x06002137 RID: 8503 RVA: 0x0009C074 File Offset: 0x0009A274
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

		// Token: 0x06002138 RID: 8504 RVA: 0x0009C0EE File Offset: 0x0009A2EE
		private void OnLogReceived(Console.Log log)
		{
			this.RebuildOutput();
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x0009C0EE File Offset: 0x0009A2EE
		private void OnClear()
		{
			this.RebuildOutput();
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x0009C0F8 File Offset: 0x0009A2F8
		private void RebuildOutput()
		{
			float value = 0f;
			if (this.outputField.verticalScrollbar)
			{
				value = this.outputField.verticalScrollbar.value;
			}
			string[] array = new string[Console.logs.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Console.logs[i].message;
			}
			this.outputField.text = string.Join("\n", array);
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
				this.outputField.verticalScrollbar.value = value;
			}
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x0009C1C4 File Offset: 0x0009A3C4
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

		// Token: 0x0600213C RID: 8508 RVA: 0x0009C27A File Offset: 0x0009A47A
		[RuntimeInitializeOnLoadMethod]
		public static void Init()
		{
			RoR2Application.onUpdate += ConsoleWindow.CheckConsoleKey;
		}

		// Token: 0x040023B8 RID: 9144
		public TMP_InputField inputField;

		// Token: 0x040023B9 RID: 9145
		public TMP_InputField outputField;

		// Token: 0x040023BA RID: 9146
		private TMP_Dropdown autoCompleteDropdown;

		// Token: 0x040023BB RID: 9147
		private Console.AutoComplete autoComplete;

		// Token: 0x040023BC RID: 9148
		private bool preventAutoCompleteUpdate;

		// Token: 0x040023BD RID: 9149
		private bool preventHistoryReset;

		// Token: 0x040023BE RID: 9150
		private int historyIndex = -1;

		// Token: 0x040023BF RID: 9151
		private const string consoleEnabledDefaultValue = "0";

		// Token: 0x040023C0 RID: 9152
		private static BoolConVar cvConsoleEnabled = new BoolConVar("console_enabled", ConVarFlags.None, "0", "Enables/Disables the console.");

		// Token: 0x020005C7 RID: 1479
		private enum InputFieldState
		{
			// Token: 0x040023C2 RID: 9154
			Neutral,
			// Token: 0x040023C3 RID: 9155
			History,
			// Token: 0x040023C4 RID: 9156
			AutoComplete
		}
	}
}
