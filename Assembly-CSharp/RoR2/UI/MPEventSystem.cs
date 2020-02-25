using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RoR2.UI
{
	// Token: 0x020005F6 RID: 1526
	[RequireComponent(typeof(RewiredStandaloneInputModule))]
	public class MPEventSystem : EventSystem
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06002428 RID: 9256 RVA: 0x0009E201 File Offset: 0x0009C401
		// (set) Token: 0x06002429 RID: 9257 RVA: 0x0009E208 File Offset: 0x0009C408
		public static int activeCount { get; private set; }

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x0600242A RID: 9258 RVA: 0x0009E210 File Offset: 0x0009C410
		public bool isHovering
		{
			get
			{
				return base.currentInputModule && ((MPInputModule)base.currentInputModule).isHovering;
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x0600242B RID: 9259 RVA: 0x0009E231 File Offset: 0x0009C431
		public bool isCursorVisible
		{
			get
			{
				return this.cursorIndicatorController.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x0009E244 File Offset: 0x0009C444
		public static MPEventSystem FindByPlayer(Player player)
		{
			foreach (MPEventSystem mpeventSystem in MPEventSystem.instancesList)
			{
				if (mpeventSystem.player == player)
				{
					return mpeventSystem;
				}
			}
			return null;
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x0009E2A0 File Offset: 0x0009C4A0
		protected override void Update()
		{
			EventSystem current = EventSystem.current;
			EventSystem.current = this;
			base.Update();
			EventSystem.current = current;
			if (this.player.GetButtonDown(25) && (PauseScreenController.instancesList.Count == 0 || SimpleDialogBox.instancesList.Count == 0))
			{
				Console.instance.SubmitCmd(null, "pause", false);
			}
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x0009E2FB File Offset: 0x0009C4FB
		protected override void Awake()
		{
			base.Awake();
			MPEventSystem.instancesList.Add(this);
			this.cursorIndicatorController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/CursorIndicator"), base.transform).GetComponent<CursorIndicatorController>();
			this.inputMapperHelper = new InputMapperHelper(this);
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x0009E33A File Offset: 0x0009C53A
		private static void OnActiveSceneChanged(Scene scene1, Scene scene2)
		{
			MPEventSystem.RecenterCursors();
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x0009E344 File Offset: 0x0009C544
		private static void RecenterCursors()
		{
			foreach (MPEventSystem mpeventSystem in MPEventSystem.instancesList)
			{
				if (mpeventSystem.currentInputSource == MPEventSystem.InputSource.Gamepad && mpeventSystem.currentInputModule)
				{
					((MPInput)mpeventSystem.currentInputModule.input).CenterCursor();
				}
			}
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x0009E3BC File Offset: 0x0009C5BC
		protected override void OnDestroy()
		{
			this.player.controllers.RemoveLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
			MPEventSystem.instancesList.Remove(this);
			this.inputMapperHelper.Dispose();
			base.OnDestroy();
		}

		// Token: 0x06002432 RID: 9266 RVA: 0x0009E3F8 File Offset: 0x0009C5F8
		protected override void Start()
		{
			base.Start();
			this.SetCursorIndicatorEnabled(false);
			if (base.currentInputModule && base.currentInputModule.input)
			{
				((MPInput)base.currentInputModule.input).CenterCursor();
			}
			this.player.controllers.AddLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
			this.OnLastActiveControllerChanged(this.player, this.player.controllers.GetLastActiveController());
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x0009E47E File Offset: 0x0009C67E
		protected override void OnEnable()
		{
			base.OnEnable();
			MPEventSystem.activeCount++;
		}

		// Token: 0x06002434 RID: 9268 RVA: 0x0009E492 File Offset: 0x0009C692
		protected override void OnDisable()
		{
			this.SetCursorIndicatorEnabled(false);
			base.OnDisable();
			MPEventSystem.activeCount--;
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06002435 RID: 9269 RVA: 0x0009E4AD File Offset: 0x0009C6AD
		// (set) Token: 0x06002436 RID: 9270 RVA: 0x0009E4B5 File Offset: 0x0009C6B5
		public MPEventSystem.InputSource currentInputSource { get; private set; } = MPEventSystem.InputSource.Gamepad;

		// Token: 0x06002437 RID: 9271 RVA: 0x0009E4C0 File Offset: 0x0009C6C0
		protected void LateUpdate()
		{
			bool flag = this.cursorOpenerCount > 0 && base.currentInputModule && base.currentInputModule.input;
			this.SetCursorIndicatorEnabled(flag);
			MPInputModule mpinputModule = base.currentInputModule as MPInputModule;
			if (flag)
			{
				CursorIndicatorController.CursorSet cursorSet = this.cursorIndicatorController.noneCursorSet;
				MPEventSystem.InputSource currentInputSource = this.currentInputSource;
				if (currentInputSource != MPEventSystem.InputSource.Keyboard)
				{
					if (currentInputSource == MPEventSystem.InputSource.Gamepad)
					{
						cursorSet = this.cursorIndicatorController.gamepadCursorSet;
					}
				}
				else
				{
					cursorSet = this.cursorIndicatorController.mouseCursorSet;
				}
				this.cursorIndicatorController.SetCursor(cursorSet, this.isHovering ? CursorIndicatorController.CursorImage.Hover : CursorIndicatorController.CursorImage.Pointer, this.GetColor());
				this.cursorIndicatorController.SetPosition(mpinputModule.input.mousePosition);
			}
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x0009E578 File Offset: 0x0009C778
		private void OnLastActiveControllerChanged(Player player, Controller controller)
		{
			if (controller == null)
			{
				return;
			}
			ControllerType type = controller.type;
			if (type <= ControllerType.Mouse)
			{
				this.currentInputSource = MPEventSystem.InputSource.Keyboard;
				return;
			}
			if (type != ControllerType.Joystick)
			{
				return;
			}
			this.currentInputSource = MPEventSystem.InputSource.Gamepad;
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x0009E5A8 File Offset: 0x0009C7A8
		private void SetCursorIndicatorEnabled(bool cursorIndicatorEnabled)
		{
			if (this.cursorIndicatorController.gameObject.activeSelf != cursorIndicatorEnabled)
			{
				this.cursorIndicatorController.gameObject.SetActive(cursorIndicatorEnabled);
				if (cursorIndicatorEnabled)
				{
					((MPInput)((MPInputModule)base.currentInputModule).input).CenterCursor();
				}
			}
		}

		// Token: 0x0600243A RID: 9274 RVA: 0x0009E5F6 File Offset: 0x0009C7F6
		public Color GetColor()
		{
			if (MPEventSystem.activeCount <= 1)
			{
				return Color.white;
			}
			return ColorCatalog.GetMultiplayerColor(this.playerSlot);
		}

		// Token: 0x0600243B RID: 9275 RVA: 0x0009E611 File Offset: 0x0009C811
		public bool GetCursorPosition(out Vector2 position)
		{
			if (base.currentInputModule)
			{
				position = base.currentInputModule.input.mousePosition;
				return true;
			}
			position = Vector2.zero;
			return false;
		}

		// Token: 0x0600243C RID: 9276 RVA: 0x0009E644 File Offset: 0x0009C844
		public Rect GetScreenRect()
		{
			LocalUser localUser = this.localUser;
			CameraRigController cameraRigController = (localUser != null) ? localUser.cameraRigController : null;
			if (!cameraRigController)
			{
				return new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
			}
			return cameraRigController.viewport;
		}

		// Token: 0x0600243D RID: 9277 RVA: 0x0009E690 File Offset: 0x0009C890
		private static Vector2 RandomOnCircle()
		{
			float value = UnityEngine.Random.value;
			return new Vector2(Mathf.Cos(value * 3.1415927f * 2f), Mathf.Sin(value * 3.1415927f * 2f));
		}

		// Token: 0x0600243E RID: 9278 RVA: 0x0009E6CC File Offset: 0x0009C8CC
		private static Vector2 CalculateCursorPushVector(Vector2 positionA, Vector2 positionB)
		{
			Vector2 vector = positionA - positionB;
			if (vector == Vector2.zero)
			{
				vector = MPEventSystem.RandomOnCircle();
			}
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude >= 576f)
			{
				return Vector2.zero;
			}
			float num = Mathf.Sqrt(sqrMagnitude);
			float num2 = num * 0.041666668f;
			float d = 1f - num2;
			return vector / num * d * 10f * 0.5f;
		}

		// Token: 0x0600243F RID: 9279 RVA: 0x0009E744 File Offset: 0x0009C944
		private static void PushCursorsApart()
		{
			if (MPEventSystem.activeCount <= 1)
			{
				return;
			}
			int count = MPEventSystem.instancesList.Count;
			if (MPEventSystem.pushInfos.Length < MPEventSystem.activeCount)
			{
				MPEventSystem.pushInfos = new MPEventSystem.PushInfo[MPEventSystem.activeCount];
			}
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (MPEventSystem.instancesList[i].enabled)
				{
					Vector2 position;
					MPEventSystem.instancesList[i].GetCursorPosition(out position);
					MPEventSystem.pushInfos[num++] = new MPEventSystem.PushInfo
					{
						index = i,
						position = position
					};
				}
			}
			for (int j = 0; j < MPEventSystem.activeCount; j++)
			{
				MPEventSystem.PushInfo pushInfo = MPEventSystem.pushInfos[j];
				for (int k = j + 1; k < MPEventSystem.activeCount; k++)
				{
					MPEventSystem.PushInfo pushInfo2 = MPEventSystem.pushInfos[k];
					Vector2 b = MPEventSystem.CalculateCursorPushVector(pushInfo.position, pushInfo2.position);
					pushInfo.pushVector += b;
					pushInfo2.pushVector -= b;
					MPEventSystem.pushInfos[k] = pushInfo2;
				}
				MPEventSystem.pushInfos[j] = pushInfo;
			}
			for (int l = 0; l < MPEventSystem.activeCount; l++)
			{
				MPEventSystem.PushInfo pushInfo3 = MPEventSystem.pushInfos[l];
				MPEventSystem mpeventSystem = MPEventSystem.instancesList[pushInfo3.index];
				if (mpeventSystem.allowCursorPush && mpeventSystem.currentInputModule)
				{
					((MPInput)mpeventSystem.currentInputModule.input).internalMousePosition += pushInfo3.pushVector;
				}
			}
		}

		// Token: 0x06002440 RID: 9280 RVA: 0x0009E904 File Offset: 0x0009CB04
		static MPEventSystem()
		{
			RoR2Application.onUpdate += MPEventSystem.PushCursorsApart;
			SceneManager.activeSceneChanged += MPEventSystem.OnActiveSceneChanged;
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06002441 RID: 9281 RVA: 0x0009E956 File Offset: 0x0009CB56
		// (set) Token: 0x06002442 RID: 9282 RVA: 0x0009E95E File Offset: 0x0009CB5E
		public InputMapperHelper inputMapperHelper { get; private set; }

		// Token: 0x04002209 RID: 8713
		private static readonly List<MPEventSystem> instancesList = new List<MPEventSystem>();

		// Token: 0x0400220A RID: 8714
		public static ReadOnlyCollection<MPEventSystem> readOnlyInstancesList = new ReadOnlyCollection<MPEventSystem>(MPEventSystem.instancesList);

		// Token: 0x0400220C RID: 8716
		public int cursorOpenerCount;

		// Token: 0x0400220D RID: 8717
		public int playerSlot = -1;

		// Token: 0x0400220E RID: 8718
		[NonSerialized]
		public bool allowCursorPush = true;

		// Token: 0x0400220F RID: 8719
		[NonSerialized]
		public bool isCombinedEventSystem;

		// Token: 0x04002211 RID: 8721
		private CursorIndicatorController cursorIndicatorController;

		// Token: 0x04002212 RID: 8722
		[NotNull]
		public Player player;

		// Token: 0x04002213 RID: 8723
		[CanBeNull]
		public LocalUser localUser;

		// Token: 0x04002214 RID: 8724
		public TooltipProvider currentTooltipProvider;

		// Token: 0x04002215 RID: 8725
		public TooltipController currentTooltip;

		// Token: 0x04002216 RID: 8726
		private static MPEventSystem.PushInfo[] pushInfos = Array.Empty<MPEventSystem.PushInfo>();

		// Token: 0x04002217 RID: 8727
		private const float radius = 24f;

		// Token: 0x04002218 RID: 8728
		private const float invRadius = 0.041666668f;

		// Token: 0x04002219 RID: 8729
		private const float radiusSqr = 576f;

		// Token: 0x0400221A RID: 8730
		private const float pushFactor = 10f;

		// Token: 0x020005F7 RID: 1527
		public enum InputSource
		{
			// Token: 0x0400221D RID: 8733
			Keyboard,
			// Token: 0x0400221E RID: 8734
			Gamepad
		}

		// Token: 0x020005F8 RID: 1528
		private struct PushInfo
		{
			// Token: 0x0400221F RID: 8735
			public int index;

			// Token: 0x04002220 RID: 8736
			public Vector2 position;

			// Token: 0x04002221 RID: 8737
			public Vector2 pushVector;
		}
	}
}
