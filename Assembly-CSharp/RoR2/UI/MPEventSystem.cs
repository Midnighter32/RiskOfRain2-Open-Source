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
	// Token: 0x02000607 RID: 1543
	[RequireComponent(typeof(RewiredStandaloneInputModule))]
	public class MPEventSystem : EventSystem
	{
		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060022B8 RID: 8888 RVA: 0x000A4091 File Offset: 0x000A2291
		// (set) Token: 0x060022B9 RID: 8889 RVA: 0x000A4098 File Offset: 0x000A2298
		public static int activeCount { get; private set; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060022BA RID: 8890 RVA: 0x000A40A0 File Offset: 0x000A22A0
		public bool isHovering
		{
			get
			{
				return base.currentInputModule && ((MPInputModule)base.currentInputModule).isHovering;
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060022BB RID: 8891 RVA: 0x000A40C1 File Offset: 0x000A22C1
		public bool isCursorVisible
		{
			get
			{
				return this.cursorIndicatorController.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x000A40D4 File Offset: 0x000A22D4
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

		// Token: 0x060022BD RID: 8893 RVA: 0x000A4130 File Offset: 0x000A2330
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

		// Token: 0x060022BE RID: 8894 RVA: 0x000A418B File Offset: 0x000A238B
		protected override void Awake()
		{
			base.Awake();
			MPEventSystem.instancesList.Add(this);
			this.cursorIndicatorController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/CursorIndicator"), base.transform).GetComponent<CursorIndicatorController>();
			this.inputMapperHelper = new InputMapperHelper(this);
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x000A41CA File Offset: 0x000A23CA
		private static void OnActiveSceneChanged(Scene scene1, Scene scene2)
		{
			MPEventSystem.RecenterCursors();
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x000A41D4 File Offset: 0x000A23D4
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

		// Token: 0x060022C1 RID: 8897 RVA: 0x000A424C File Offset: 0x000A244C
		protected override void OnDestroy()
		{
			this.player.controllers.RemoveLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
			MPEventSystem.instancesList.Remove(this);
			this.inputMapperHelper.Dispose();
			base.OnDestroy();
		}

		// Token: 0x060022C2 RID: 8898 RVA: 0x000A4288 File Offset: 0x000A2488
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

		// Token: 0x060022C3 RID: 8899 RVA: 0x000A430E File Offset: 0x000A250E
		protected override void OnEnable()
		{
			base.OnEnable();
			MPEventSystem.activeCount++;
		}

		// Token: 0x060022C4 RID: 8900 RVA: 0x000A4322 File Offset: 0x000A2522
		protected override void OnDisable()
		{
			this.SetCursorIndicatorEnabled(false);
			base.OnDisable();
			MPEventSystem.activeCount--;
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060022C5 RID: 8901 RVA: 0x000A433D File Offset: 0x000A253D
		// (set) Token: 0x060022C6 RID: 8902 RVA: 0x000A4345 File Offset: 0x000A2545
		public MPEventSystem.InputSource currentInputSource { get; private set; } = MPEventSystem.InputSource.Gamepad;

		// Token: 0x060022C7 RID: 8903 RVA: 0x000A4350 File Offset: 0x000A2550
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

		// Token: 0x060022C8 RID: 8904 RVA: 0x000A4408 File Offset: 0x000A2608
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

		// Token: 0x060022C9 RID: 8905 RVA: 0x000A4438 File Offset: 0x000A2638
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

		// Token: 0x060022CA RID: 8906 RVA: 0x000A4486 File Offset: 0x000A2686
		public Color GetColor()
		{
			if (MPEventSystem.activeCount <= 1)
			{
				return Color.white;
			}
			return ColorCatalog.GetMultiplayerColor(this.playerSlot);
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x000A44A1 File Offset: 0x000A26A1
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

		// Token: 0x060022CC RID: 8908 RVA: 0x000A44D4 File Offset: 0x000A26D4
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

		// Token: 0x060022CD RID: 8909 RVA: 0x000A4520 File Offset: 0x000A2720
		private static Vector2 RandomOnCircle()
		{
			float value = UnityEngine.Random.value;
			return new Vector2(Mathf.Cos(value * 3.1415927f * 2f), Mathf.Sin(value * 3.1415927f * 2f));
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x000A455C File Offset: 0x000A275C
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

		// Token: 0x060022CF RID: 8911 RVA: 0x000A45D4 File Offset: 0x000A27D4
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

		// Token: 0x060022D0 RID: 8912 RVA: 0x000A4794 File Offset: 0x000A2994
		static MPEventSystem()
		{
			RoR2Application.onUpdate += MPEventSystem.PushCursorsApart;
			SceneManager.activeSceneChanged += MPEventSystem.OnActiveSceneChanged;
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060022D1 RID: 8913 RVA: 0x000A47E6 File Offset: 0x000A29E6
		// (set) Token: 0x060022D2 RID: 8914 RVA: 0x000A47EE File Offset: 0x000A29EE
		public InputMapperHelper inputMapperHelper { get; private set; }

		// Token: 0x040025C5 RID: 9669
		private static readonly List<MPEventSystem> instancesList = new List<MPEventSystem>();

		// Token: 0x040025C6 RID: 9670
		public static ReadOnlyCollection<MPEventSystem> readOnlyInstancesList = new ReadOnlyCollection<MPEventSystem>(MPEventSystem.instancesList);

		// Token: 0x040025C8 RID: 9672
		public int cursorOpenerCount;

		// Token: 0x040025C9 RID: 9673
		public int playerSlot = -1;

		// Token: 0x040025CA RID: 9674
		[NonSerialized]
		public bool allowCursorPush = true;

		// Token: 0x040025CB RID: 9675
		[NonSerialized]
		public bool isCombinedEventSystem;

		// Token: 0x040025CD RID: 9677
		private CursorIndicatorController cursorIndicatorController;

		// Token: 0x040025CE RID: 9678
		[NotNull]
		public Player player;

		// Token: 0x040025CF RID: 9679
		[CanBeNull]
		public LocalUser localUser;

		// Token: 0x040025D0 RID: 9680
		public TooltipProvider currentTooltipProvider;

		// Token: 0x040025D1 RID: 9681
		public TooltipController currentTooltip;

		// Token: 0x040025D2 RID: 9682
		private static MPEventSystem.PushInfo[] pushInfos = Array.Empty<MPEventSystem.PushInfo>();

		// Token: 0x040025D3 RID: 9683
		private const float radius = 24f;

		// Token: 0x040025D4 RID: 9684
		private const float invRadius = 0.041666668f;

		// Token: 0x040025D5 RID: 9685
		private const float radiusSqr = 576f;

		// Token: 0x040025D6 RID: 9686
		private const float pushFactor = 10f;

		// Token: 0x02000608 RID: 1544
		public enum InputSource
		{
			// Token: 0x040025D9 RID: 9689
			Keyboard,
			// Token: 0x040025DA RID: 9690
			Gamepad
		}

		// Token: 0x02000609 RID: 1545
		private struct PushInfo
		{
			// Token: 0x040025DB RID: 9691
			public int index;

			// Token: 0x040025DC RID: 9692
			public Vector2 position;

			// Token: 0x040025DD RID: 9693
			public Vector2 pushVector;
		}
	}
}
