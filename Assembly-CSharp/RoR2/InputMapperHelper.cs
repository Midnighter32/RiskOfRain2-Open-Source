using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A6 RID: 934
	public class InputMapperHelper : IDisposable
	{
		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x0006131C File Offset: 0x0005F51C
		// (set) Token: 0x060016A6 RID: 5798 RVA: 0x00061324 File Offset: 0x0005F524
		public bool isListening { get; private set; }

		// Token: 0x060016A7 RID: 5799 RVA: 0x0006132D File Offset: 0x0005F52D
		public InputMapperHelper(MPEventSystem eventSystem)
		{
			this.eventSystem = eventSystem;
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00061360 File Offset: 0x0005F560
		private InputMapper AddInputMapper()
		{
			InputMapper inputMapper = new InputMapper();
			inputMapper.ConflictFoundEvent += this.InputMapperOnConflictFoundEvent;
			inputMapper.CanceledEvent += this.InputMapperOnCanceledEvent;
			inputMapper.ErrorEvent += this.InputMapperOnErrorEvent;
			inputMapper.InputMappedEvent += this.InputMapperOnInputMappedEvent;
			inputMapper.StartedEvent += this.InputMapperOnStartedEvent;
			inputMapper.StoppedEvent += this.InputMapperOnStoppedEvent;
			inputMapper.TimedOutEvent += this.InputMapperOnTimedOutEvent;
			inputMapper.options = new InputMapper.Options
			{
				allowAxes = true,
				allowButtons = true,
				allowKeyboardKeysWithModifiers = false,
				allowKeyboardModifierKeyAsPrimary = true,
				checkForConflicts = true,
				checkForConflictsWithAllPlayers = false,
				checkForConflictsWithPlayerIds = Array.Empty<int>(),
				checkForConflictsWithSelf = true,
				checkForConflictsWithSystemPlayer = false,
				defaultActionWhenConflictFound = InputMapper.ConflictResponse.Add,
				holdDurationToMapKeyboardModifierKeyAsPrimary = 0f,
				ignoreMouseXAxis = true,
				ignoreMouseYAxis = true,
				timeout = float.PositiveInfinity
			};
			this.inputMappers.Add(inputMapper);
			return inputMapper;
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00061478 File Offset: 0x0005F678
		private void RemoveInputMapper(InputMapper inputMapper)
		{
			inputMapper.ConflictFoundEvent -= this.InputMapperOnConflictFoundEvent;
			inputMapper.CanceledEvent -= this.InputMapperOnCanceledEvent;
			inputMapper.ErrorEvent -= this.InputMapperOnErrorEvent;
			inputMapper.InputMappedEvent -= this.InputMapperOnInputMappedEvent;
			inputMapper.StartedEvent -= this.InputMapperOnStartedEvent;
			inputMapper.StoppedEvent -= this.InputMapperOnStoppedEvent;
			inputMapper.TimedOutEvent -= this.InputMapperOnTimedOutEvent;
			this.inputMappers.Remove(inputMapper);
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x00061510 File Offset: 0x0005F710
		public void Start(Player player, IList<Controller> controllers, InputAction action, AxisRange axisRange)
		{
			this.Stop();
			this.isListening = true;
			this.currentPlayer = player;
			this.currentAction = action;
			this.currentAxisRange = axisRange;
			this.maps = (from controller in controllers
			select player.controllers.maps.GetFirstMapInCategory(controller, 0) into map
			where map != null
			select map).Distinct<ControllerMap>().ToArray<ControllerMap>();
			Debug.Log(this.maps.Length);
			foreach (ControllerMap controllerMap in this.maps)
			{
				InputMapper.Context mappingContext = new InputMapper.Context
				{
					actionId = action.id,
					controllerMap = controllerMap,
					actionRange = this.currentAxisRange
				};
				this.AddInputMapper().Start(mappingContext);
			}
			this.dialogBox = SimpleDialogBox.Create(this.eventSystem);
			this.timer = this.timeout;
			this.UpdateDialogBoxString();
			RoR2Application.onUpdate += this.Update;
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x0006162C File Offset: 0x0005F82C
		public void Stop()
		{
			if (!this.isListening)
			{
				return;
			}
			this.maps = Array.Empty<ControllerMap>();
			this.currentPlayer = null;
			this.currentAction = null;
			for (int i = this.inputMappers.Count - 1; i >= 0; i--)
			{
				InputMapper inputMapper = this.inputMappers[i];
				inputMapper.Stop();
				this.RemoveInputMapper(inputMapper);
			}
			if (this.dialogBox)
			{
				UnityEngine.Object.Destroy(this.dialogBox.rootObject);
				this.dialogBox = null;
			}
			this.isListening = false;
			RoR2Application.onUpdate -= this.Update;
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x000616CC File Offset: 0x0005F8CC
		private void Update()
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (this.isListening)
			{
				this.timer -= unscaledDeltaTime;
				if (this.timer < 0f)
				{
					this.Stop();
					return;
				}
				if (this.currentPlayer.GetButtonDown(25))
				{
					this.Stop();
					SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(this.eventSystem);
					simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair("OPTION_REBIND_DIALOG_TITLE", Array.Empty<object>());
					simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair("OPTION_REBIND_CANCELLED_DIALOG_DESCRIPTION", Array.Empty<object>());
					simpleDialogBox.AddCancelButton(CommonLanguageTokens.ok, Array.Empty<object>());
					return;
				}
				this.UpdateDialogBoxString();
			}
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00061770 File Offset: 0x0005F970
		private void UpdateDialogBoxString()
		{
			if (this.dialogBox && this.timer >= 0f)
			{
				string @string = Language.GetString(InputCatalog.GetActionNameToken(this.currentAction.name, AxisRange.Full));
				this.dialogBox.headerToken = new SimpleDialogBox.TokenParamsPair
				{
					token = CommonLanguageTokens.optionRebindDialogTitle,
					formatParams = Array.Empty<object>()
				};
				this.dialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair
				{
					token = CommonLanguageTokens.optionRebindDialogDescription,
					formatParams = new object[]
					{
						@string,
						this.timer
					}
				};
			}
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x0006181F File Offset: 0x0005FA1F
		private void InputMapperOnTimedOutEvent(InputMapper.TimedOutEventData timedOutEventData)
		{
			Debug.Log("InputMapperOnTimedOutEvent");
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x0006182B File Offset: 0x0005FA2B
		private void InputMapperOnStoppedEvent(InputMapper.StoppedEventData stoppedEventData)
		{
			Debug.Log("InputMapperOnStoppedEvent");
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x00061837 File Offset: 0x0005FA37
		private void InputMapperOnStartedEvent(InputMapper.StartedEventData startedEventData)
		{
			Debug.Log("InputMapperOnStartedEvent");
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x00061844 File Offset: 0x0005FA44
		private void InputMapperOnInputMappedEvent(InputMapper.InputMappedEventData inputMappedEventData)
		{
			Debug.Log("InputMapperOnInputMappedEvent");
			InputMapperHelper.<>c__DisplayClass23_0 CS$<>8__locals1;
			CS$<>8__locals1.incomingActionElementMap = inputMappedEventData.actionElementMap;
			CS$<>8__locals1.incomingActionId = inputMappedEventData.actionElementMap.actionId;
			CS$<>8__locals1.incomingElementIndex = inputMappedEventData.actionElementMap.elementIndex;
			CS$<>8__locals1.incomingElementType = inputMappedEventData.actionElementMap.elementType;
			CS$<>8__locals1.map = inputMappedEventData.actionElementMap.controllerMap;
			foreach (ControllerMap controllerMap in this.maps)
			{
				if (controllerMap != CS$<>8__locals1.map)
				{
					controllerMap.DeleteElementMapsWithAction(CS$<>8__locals1.incomingActionId);
				}
			}
			while (InputMapperHelper.<InputMapperOnInputMappedEvent>g__DeleteFirstConflictingElementMap|23_1(ref CS$<>8__locals1))
			{
			}
			MPEventSystem mpeventSystem = this.eventSystem;
			if (mpeventSystem != null)
			{
				LocalUser localUser = mpeventSystem.localUser;
				if (localUser != null)
				{
					localUser.userProfile.RequestSave(false);
				}
			}
			Debug.Log("Mapping accepted.");
			this.Stop();
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x0006191A File Offset: 0x0005FB1A
		private void InputMapperOnErrorEvent(InputMapper.ErrorEventData errorEventData)
		{
			Debug.Log("InputMapperOnErrorEvent");
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x00061926 File Offset: 0x0005FB26
		private void InputMapperOnCanceledEvent(InputMapper.CanceledEventData canceledEventData)
		{
			Debug.Log("InputMapperOnCanceledEvent");
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x00061934 File Offset: 0x0005FB34
		private void InputMapperOnConflictFoundEvent(InputMapper.ConflictFoundEventData conflictFoundEventData)
		{
			Debug.Log("InputMapperOnConflictFoundEvent");
			InputMapper.ConflictResponse obj;
			if (conflictFoundEventData.conflicts.Any((ElementAssignmentConflictInfo elementAssignmentConflictInfo) => InputMapperHelper.forbiddenElements.Contains(elementAssignmentConflictInfo.elementIdentifier.name)))
			{
				obj = InputMapper.ConflictResponse.Ignore;
			}
			else
			{
				obj = InputMapper.ConflictResponse.Add;
			}
			conflictFoundEventData.responseCallback(obj);
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x00061989 File Offset: 0x0005FB89
		public void Dispose()
		{
			this.Stop();
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x00061A20 File Offset: 0x0005FC20
		[CompilerGenerated]
		internal static bool <InputMapperOnInputMappedEvent>g__ActionElementMapConflicts|23_0(ActionElementMap actionElementMap, ref InputMapperHelper.<>c__DisplayClass23_0 A_1)
		{
			if (actionElementMap == A_1.incomingActionElementMap)
			{
				return false;
			}
			bool flag = actionElementMap.elementIndex == A_1.incomingElementIndex && actionElementMap.elementType == A_1.incomingElementType;
			bool flag2 = actionElementMap.actionId == A_1.incomingActionId && actionElementMap.axisContribution == A_1.incomingActionElementMap.axisContribution;
			return flag || flag2;
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x00061A80 File Offset: 0x0005FC80
		[CompilerGenerated]
		internal static bool <InputMapperOnInputMappedEvent>g__DeleteFirstConflictingElementMap|23_1(ref InputMapperHelper.<>c__DisplayClass23_0 A_0)
		{
			foreach (ActionElementMap actionElementMap in A_0.map.AllMaps)
			{
				if (InputMapperHelper.<InputMapperOnInputMappedEvent>g__ActionElementMapConflicts|23_0(actionElementMap, ref A_0))
				{
					Debug.LogFormat("Deleting conflicting mapping {0}", new object[]
					{
						actionElementMap
					});
					A_0.map.DeleteElementMap(actionElementMap.id);
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001536 RID: 5430
		private readonly MPEventSystem eventSystem;

		// Token: 0x04001537 RID: 5431
		private readonly List<InputMapper> inputMappers = new List<InputMapper>();

		// Token: 0x04001538 RID: 5432
		private ControllerMap[] maps = Array.Empty<ControllerMap>();

		// Token: 0x04001539 RID: 5433
		private SimpleDialogBox dialogBox;

		// Token: 0x0400153A RID: 5434
		public float timeout = 5f;

		// Token: 0x0400153B RID: 5435
		private float timer;

		// Token: 0x0400153C RID: 5436
		private Player currentPlayer;

		// Token: 0x0400153D RID: 5437
		private InputAction currentAction;

		// Token: 0x0400153E RID: 5438
		private AxisRange currentAxisRange;

		// Token: 0x04001540 RID: 5440
		private Action<InputMapper.ConflictResponse> conflictResponseCallback;

		// Token: 0x04001541 RID: 5441
		private static readonly HashSet<string> forbiddenElements = new HashSet<string>
		{
			"Left Stick X",
			"Left Stick Y",
			"Right Stick X",
			"Right Stick Y",
			"Mouse Horizontal",
			"Mouse Vertical",
			Keyboard.GetKeyName(KeyCode.Escape),
			Keyboard.GetKeyName(KeyCode.KeypadEnter),
			Keyboard.GetKeyName(KeyCode.Return)
		};
	}
}
