using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000443 RID: 1091
	public class InputMapperHelper : IDisposable
	{
		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06001843 RID: 6211 RVA: 0x00073860 File Offset: 0x00071A60
		// (set) Token: 0x06001844 RID: 6212 RVA: 0x00073868 File Offset: 0x00071A68
		public bool isListening { get; private set; }

		// Token: 0x06001845 RID: 6213 RVA: 0x00073871 File Offset: 0x00071A71
		public InputMapperHelper(MPEventSystem eventSystem)
		{
			this.eventSystem = eventSystem;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x000738A4 File Offset: 0x00071AA4
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

		// Token: 0x06001847 RID: 6215 RVA: 0x000739BC File Offset: 0x00071BBC
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

		// Token: 0x06001848 RID: 6216 RVA: 0x00073A54 File Offset: 0x00071C54
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

		// Token: 0x06001849 RID: 6217 RVA: 0x00073B70 File Offset: 0x00071D70
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

		// Token: 0x0600184A RID: 6218 RVA: 0x00073C10 File Offset: 0x00071E10
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

		// Token: 0x0600184B RID: 6219 RVA: 0x00073CB4 File Offset: 0x00071EB4
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

		// Token: 0x0600184C RID: 6220 RVA: 0x00073D63 File Offset: 0x00071F63
		private void InputMapperOnTimedOutEvent(InputMapper.TimedOutEventData timedOutEventData)
		{
			Debug.Log("InputMapperOnTimedOutEvent");
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x00073D6F File Offset: 0x00071F6F
		private void InputMapperOnStoppedEvent(InputMapper.StoppedEventData stoppedEventData)
		{
			Debug.Log("InputMapperOnStoppedEvent");
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x00073D7B File Offset: 0x00071F7B
		private void InputMapperOnStartedEvent(InputMapper.StartedEventData startedEventData)
		{
			Debug.Log("InputMapperOnStartedEvent");
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x00073D88 File Offset: 0x00071F88
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

		// Token: 0x06001850 RID: 6224 RVA: 0x00073E5E File Offset: 0x0007205E
		private void InputMapperOnErrorEvent(InputMapper.ErrorEventData errorEventData)
		{
			Debug.Log("InputMapperOnErrorEvent");
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x00073E6A File Offset: 0x0007206A
		private void InputMapperOnCanceledEvent(InputMapper.CanceledEventData canceledEventData)
		{
			Debug.Log("InputMapperOnCanceledEvent");
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x00073E78 File Offset: 0x00072078
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

		// Token: 0x06001853 RID: 6227 RVA: 0x00073ECD File Offset: 0x000720CD
		public void Dispose()
		{
			this.Stop();
		}

		// Token: 0x04001B89 RID: 7049
		private readonly MPEventSystem eventSystem;

		// Token: 0x04001B8A RID: 7050
		private readonly List<InputMapper> inputMappers = new List<InputMapper>();

		// Token: 0x04001B8B RID: 7051
		private ControllerMap[] maps = Array.Empty<ControllerMap>();

		// Token: 0x04001B8C RID: 7052
		private SimpleDialogBox dialogBox;

		// Token: 0x04001B8D RID: 7053
		public float timeout = 5f;

		// Token: 0x04001B8E RID: 7054
		private float timer;

		// Token: 0x04001B8F RID: 7055
		private Player currentPlayer;

		// Token: 0x04001B90 RID: 7056
		private InputAction currentAction;

		// Token: 0x04001B91 RID: 7057
		private AxisRange currentAxisRange;

		// Token: 0x04001B93 RID: 7059
		private Action<InputMapper.ConflictResponse> conflictResponseCallback;

		// Token: 0x04001B94 RID: 7060
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
