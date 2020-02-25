using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Facepunch.Steamworks;
using Rewired;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x020002EF RID: 751
	public class RoR2Application : MonoBehaviour
	{
		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0000AC89 File Offset: 0x00008E89
		public static bool noAudio
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06001128 RID: 4392 RVA: 0x0004BA6F File Offset: 0x00049C6F
		// (set) Token: 0x06001129 RID: 4393 RVA: 0x0004BA77 File Offset: 0x00049C77
		public Client steamworksClient { get; private set; }

		// Token: 0x0600112A RID: 4394 RVA: 0x0004BA80 File Offset: 0x00049C80
		public static string GetBuildId()
		{
			if (RoR2Application.isModded)
			{
				return "MOD";
			}
			return RoR2Application.steamBuildId;
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x0600112B RID: 4395 RVA: 0x0004BA94 File Offset: 0x00049C94
		// (set) Token: 0x0600112C RID: 4396 RVA: 0x0004BA9B File Offset: 0x00049C9B
		public static RoR2Application instance { get; private set; }

		// Token: 0x0600112D RID: 4397 RVA: 0x0004BAA4 File Offset: 0x00049CA4
		private void Awake()
		{
			if (RoR2Application.maxPlayers != 4 || (Application.genuineCheckAvailable && !Application.genuine))
			{
				RoR2Application.isModded = true;
			}
			this.stopwatch.Start();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (RoR2Application.instance)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			RoR2Application.instance = this;
			if (!this.loaded)
			{
				this.OnLoad();
				this.loaded = true;
			}
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0004BB16 File Offset: 0x00049D16
		private void Start()
		{
			if (RoR2Application.instance == this && RoR2Application.onStart != null)
			{
				RoR2Application.onStart();
				RoR2Application.onStart = null;
			}
		}

		// Token: 0x0600112F RID: 4399
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtSetTimerResolution(int desiredResolution, bool setResolution, out int currentResolution);

		// Token: 0x06001130 RID: 4400
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtQueryTimerResolution(out int minimumResolution, out int maximumResolution, out int currentResolution);

		// Token: 0x06001131 RID: 4401 RVA: 0x0004BB3C File Offset: 0x00049D3C
		private void Update()
		{
			if (RoR2Application.waitMsConVar.value >= 0)
			{
				Thread.Sleep(RoR2Application.waitMsConVar.value);
			}
			Cursor.lockState = ((MPEventSystemManager.kbmEventSystem.isCursorVisible || MPEventSystemManager.combinedEventSystem.isCursorVisible) ? CursorLockMode.None : CursorLockMode.Locked);
			Cursor.visible = false;
			Action action = RoR2Application.onUpdate;
			if (action != null)
			{
				action();
			}
			Action action2 = Interlocked.Exchange<Action>(ref RoR2Application.onNextUpdate, null);
			if (action2 != null)
			{
				action2();
			}
			RoR2Application.timeTimers.Update(Time.deltaTime);
			RoR2Application.unscaledTimeTimers.Update(Time.unscaledDeltaTime);
		}

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06001132 RID: 4402 RVA: 0x0004BBD0 File Offset: 0x00049DD0
		// (remove) Token: 0x06001133 RID: 4403 RVA: 0x0004BC04 File Offset: 0x00049E04
		public static event Action onUpdate;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06001134 RID: 4404 RVA: 0x0004BC38 File Offset: 0x00049E38
		// (remove) Token: 0x06001135 RID: 4405 RVA: 0x0004BC6C File Offset: 0x00049E6C
		public static event Action onFixedUpdate;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x06001136 RID: 4406 RVA: 0x0004BCA0 File Offset: 0x00049EA0
		// (remove) Token: 0x06001137 RID: 4407 RVA: 0x0004BCD4 File Offset: 0x00049ED4
		public static event Action onLateUpdate;

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x06001138 RID: 4408 RVA: 0x0004BD08 File Offset: 0x00049F08
		// (remove) Token: 0x06001139 RID: 4409 RVA: 0x0004BD3C File Offset: 0x00049F3C
		public static event Action onNextUpdate;

		// Token: 0x0600113A RID: 4410 RVA: 0x0004BD6F File Offset: 0x00049F6F
		private void FixedUpdate()
		{
			Action action = RoR2Application.onFixedUpdate;
			if (action != null)
			{
				action();
			}
			RoR2Application.fixedTimeTimers.Update(Time.fixedDeltaTime);
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x0004BD90 File Offset: 0x00049F90
		private void LateUpdate()
		{
			Action action = RoR2Application.onLateUpdate;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x0004BDA1 File Offset: 0x00049FA1
		// (set) Token: 0x0600113D RID: 4413 RVA: 0x0004BDA8 File Offset: 0x00049FA8
		public static FileSystem fileSystem { get; private set; }

		// Token: 0x0600113E RID: 4414 RVA: 0x0004BDB0 File Offset: 0x00049FB0
		private void OnLoad()
		{
			RoR2Application.UnitySystemConsoleRedirector.Redirect();
			PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
			RoR2Application.fileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Application.dataPath), true);
			RoR2Application.cloudStorage = RoR2Application.fileSystem;
			Func<bool> func = RoR2Application.loadSteamworksClient;
			if (func == null || !func())
			{
				Application.Quit();
				return;
			}
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Rewired Input Manager"));
			ReInput.ControllerConnectedEvent += RoR2Application.AssignNewController;
			foreach (ControllerType controllerType in new ControllerType[]
			{
				ControllerType.Keyboard,
				ControllerType.Mouse,
				ControllerType.Joystick
			})
			{
				Controller[] controllers = ReInput.controllers.GetControllers(controllerType);
				if (controllers != null)
				{
					for (int j = 0; j < controllers.Length; j++)
					{
						RoR2Application.AssignNewController(controllers[j]);
					}
				}
			}
			this.stateManager.Initialize();
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/MPEventSystemManager"));
			UnityEngine.Object.Instantiate<GameObject>(this.networkManagerPrefab);
			if (!RoR2Application.noAudio)
			{
				if (UnityEngine.Object.FindObjectOfType<AkInitializer>())
				{
					Debug.LogError("Attempting to initialize wwise when AkInitializer already exists! This will cause a crash!");
					return;
				}
				this.wwiseGlobalInstance = UnityEngine.Object.Instantiate<GameObject>(this.wwiseGlobalPrefab);
				UnityEngine.Object.Instantiate<GameObject>(this.audioManagerPrefab);
				this.wwiseGlobalInstance.GetComponent<AkInitializer>();
			}
			GameObject gameObject = new GameObject("Console");
			gameObject.AddComponent<SetDontDestroyOnLoad>();
			gameObject.AddComponent<Console>();
			SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode loadSceneMode)
			{
				Debug.LogFormat("Loaded scene {0} loadSceneMode={1}", new object[]
				{
					scene.name,
					loadSceneMode
				});
			};
			SceneManager.sceneUnloaded += delegate(Scene scene)
			{
				Debug.LogFormat("Unloaded scene {0}", new object[]
				{
					scene.name
				});
			};
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene newScene)
			{
				Debug.LogFormat("Active scene changed from {0} to {1}", new object[]
				{
					oldScene.name,
					newScene.name
				});
			};
			SystemInitializerAttribute.Execute();
			UserProfile.LoadUserProfiles();
			if (RoR2Application.onLoad != null)
			{
				RoR2Application.onLoad();
				RoR2Application.onLoad = null;
			}
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x0004BF84 File Offset: 0x0004A184
		private void OnDestroy()
		{
			if (RoR2Application.instance == this)
			{
				Action action = RoR2Application.unloadSteamworksClient;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x0004BFA2 File Offset: 0x0004A1A2
		private void OnApplicationQuit()
		{
			UserProfile.HandleShutDown();
			if (Console.instance)
			{
				Console.instance.SaveArchiveConVars();
			}
			Action action = RoR2Application.unloadSteamworksClient;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x0004BFCE File Offset: 0x0004A1CE
		public static bool isInSinglePlayer
		{
			get
			{
				return NetworkServer.dontListen && LocalUserManager.readOnlyLocalUsersList.Count == 1;
			}
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x0004BFE8 File Offset: 0x0004A1E8
		private static void AssignJoystickToAvailablePlayer(Controller controller)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (player.name != "PlayerMain" && player.controllers.joystickCount == 0 && !player.controllers.hasKeyboard && !player.controllers.hasMouse)
				{
					player.controllers.AddController(controller, false);
					return;
				}
			}
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0004C060 File Offset: 0x0004A260
		private static void AssignNewController(ControllerStatusChangedEventArgs args)
		{
			RoR2Application.AssignNewController(ReInput.controllers.GetController(args.controllerType, args.controllerId));
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x0004C07D File Offset: 0x0004A27D
		private static void AssignNewController(Controller controller)
		{
			ReInput.players.GetPlayer("PlayerMain").controllers.AddController(controller, false);
			if (controller.type == ControllerType.Joystick)
			{
				RoR2Application.AssignJoystickToAvailablePlayer(controller);
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06001145 RID: 4421 RVA: 0x0004C0A9 File Offset: 0x0004A2A9
		// (set) Token: 0x06001146 RID: 4422 RVA: 0x0004C0B0 File Offset: 0x0004A2B0
		public static bool sessionCheatsEnabled { get; private set; }

		// Token: 0x06001147 RID: 4423 RVA: 0x0004C0B8 File Offset: 0x0004A2B8
		[ConCommand(commandName = "pause", flags = ConVarFlags.None, helpText = "Toggles game pause state.")]
		private static void CCTogglePause(ConCommandArgs args)
		{
			if (RoR2Application.instance.pauseScreenInstance)
			{
				UnityEngine.Object.Destroy(RoR2Application.instance.pauseScreenInstance);
				RoR2Application.instance.pauseScreenInstance = null;
				return;
			}
			if (NetworkManager.singleton.isNetworkActive)
			{
				RoR2Application.instance.pauseScreenInstance = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/PauseScreen"), RoR2Application.instance.transform);
			}
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x0004C120 File Offset: 0x0004A320
		[ConCommand(commandName = "quit", flags = ConVarFlags.None, helpText = "Close the application.")]
		private static void CCQuit(ConCommandArgs args)
		{
			Application.Quit();
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0004C127 File Offset: 0x0004A327
		public static void IncrementActiveWriteCount()
		{
			Interlocked.Increment(ref RoR2Application.activeWriteCount);
			RoR2Application.saveIconAlpha = 2f;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0004C140 File Offset: 0x0004A340
		public static void DecrementActiveWriteCount()
		{
			Interlocked.Decrement(ref RoR2Application.activeWriteCount);
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0004C14D File Offset: 0x0004A34D
		[RuntimeInitializeOnLoadMethod]
		private static void InitSaveIcon()
		{
			UnityEngine.UI.Image saveImage = RoR2Application.instance.mainCanvas.transform.Find("SafeArea/SaveIcon").GetComponent<UnityEngine.UI.Image>();
			RoR2Application.onUpdate += delegate()
			{
				UnityEngine.Color color = saveImage.color;
				if (RoR2Application.activeWriteCount <= 0)
				{
					color.a = (RoR2Application.saveIconAlpha = Mathf.Max(RoR2Application.saveIconAlpha - 4f * Time.unscaledDeltaTime, 0f));
				}
				saveImage.color = color;
			};
		}

		// Token: 0x0400109E RID: 4254
		[HideInInspector]
		[SerializeField]
		private bool loaded;

		// Token: 0x0400109F RID: 4255
		public static readonly string messageForModders = "We don't officially support modding at this time but if you're going to mod the game please change this value to true if you're modding the game. This will disable some things like Prismatic Trials and put players into a separate matchmaking queue from vanilla users to protect their game experience.";

		// Token: 0x040010A0 RID: 4256
		public static bool isModded = false;

		// Token: 0x040010A1 RID: 4257
		public GameObject networkManagerPrefab;

		// Token: 0x040010A2 RID: 4258
		public GameObject wwiseGlobalPrefab;

		// Token: 0x040010A3 RID: 4259
		public GameObject audioManagerPrefab;

		// Token: 0x040010A4 RID: 4260
		public EntityStateManager stateManager;

		// Token: 0x040010A5 RID: 4261
		public PostProcessVolume postProcessSettingsController;

		// Token: 0x040010A6 RID: 4262
		public Canvas mainCanvas;

		// Token: 0x040010A7 RID: 4263
		public Stopwatch stopwatch = new Stopwatch();

		// Token: 0x040010A8 RID: 4264
		public const string gameName = "Risk of Rain 2";

		// Token: 0x040010A9 RID: 4265
		private const uint ror1AppId = 248820U;

		// Token: 0x040010AA RID: 4266
		public const uint ror2AppId = 632360U;

		// Token: 0x040010AB RID: 4267
		private const uint ror2DedicatedServerAppId = 1180760U;

		// Token: 0x040010AC RID: 4268
		public const bool isDedicatedServer = false;

		// Token: 0x040010AD RID: 4269
		public const uint appId = 632360U;

		// Token: 0x040010AF RID: 4271
		public static string steamBuildId = "STEAM_UNINITIALIZED";

		// Token: 0x040010B0 RID: 4272
		public static readonly int hardMaxPlayers = 16;

		// Token: 0x040010B1 RID: 4273
		public static readonly int maxPlayers = 4;

		// Token: 0x040010B2 RID: 4274
		public static readonly int maxLocalPlayers = 4;

		// Token: 0x040010B3 RID: 4275
		private GameObject wwiseGlobalInstance;

		// Token: 0x040010B5 RID: 4277
		private static IntConVar waitMsConVar = new IntConVar("wait_ms", ConVarFlags.None, "-1", "How many milliseconds to sleep between each frame. -1 for no sleeping between frames.");

		// Token: 0x040010B6 RID: 4278
		private GameObject pauseScreenInstance;

		// Token: 0x040010B7 RID: 4279
		public static readonly TimerQueue timeTimers = new TimerQueue();

		// Token: 0x040010B8 RID: 4280
		public static readonly TimerQueue fixedTimeTimers = new TimerQueue();

		// Token: 0x040010B9 RID: 4281
		public static readonly TimerQueue unscaledTimeTimers = new TimerQueue();

		// Token: 0x040010BF RID: 4287
		public static FileSystem cloudStorage;

		// Token: 0x040010C0 RID: 4288
		public static Func<bool> loadSteamworksClient;

		// Token: 0x040010C1 RID: 4289
		public static Action unloadSteamworksClient;

		// Token: 0x040010C2 RID: 4290
		public static Action onLoad;

		// Token: 0x040010C3 RID: 4291
		public static Action onStart;

		// Token: 0x040010C5 RID: 4293
		public static RoR2Application.CheatsConVar cvCheats = new RoR2Application.CheatsConVar("cheats", ConVarFlags.ExecuteOnServer, "0", "Enable cheats. Achievements, unlock progression, and stat tracking will be disabled until the application is restarted.");

		// Token: 0x040010C6 RID: 4294
		private static float oldTimeScale = 1f;

		// Token: 0x040010C7 RID: 4295
		public static Action onPauseStartGlobal;

		// Token: 0x040010C8 RID: 4296
		public static Action onPauseEndGlobal;

		// Token: 0x040010C9 RID: 4297
		private static RoR2Application.TimeScaleConVar cvTimeScale = new RoR2Application.TimeScaleConVar("timescale", ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat | ConVarFlags.Engine, null, "The timescale of the game.");

		// Token: 0x040010CA RID: 4298
		private static RoR2Application.TimeStepConVar cvTimeStep = new RoR2Application.TimeStepConVar("timestep", ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat | ConVarFlags.Engine, null, "The timestep of the game.");

		// Token: 0x040010CB RID: 4299
		public static readonly Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);

		// Token: 0x040010CC RID: 4300
		public static BoolConVar enableDamageNumbers = new BoolConVar("enable_damage_numbers", ConVarFlags.Archive, "1", "Whether or not damage and healing numbers spawn.");

		// Token: 0x040010CD RID: 4301
		private static int activeWriteCount;

		// Token: 0x040010CE RID: 4302
		private static volatile float saveIconAlpha = 0f;

		// Token: 0x020002F0 RID: 752
		private class TimerResolutionConVar : BaseConVar
		{
			// Token: 0x0600114E RID: 4430 RVA: 0x0000972B File Offset: 0x0000792B
			private TimerResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600114F RID: 4431 RVA: 0x0004C2A0 File Offset: 0x0004A4A0
			public override void SetString(string newValue)
			{
				int desiredResolution;
				if (TextSerialization.TryParseInvariant(newValue, out desiredResolution))
				{
					int num;
					RoR2Application.NtSetTimerResolution(desiredResolution, true, out num);
					Debug.LogFormat("{0} set to {1}", new object[]
					{
						this.name,
						num
					});
				}
			}

			// Token: 0x06001150 RID: 4432 RVA: 0x0004C2E4 File Offset: 0x0004A4E4
			public override string GetString()
			{
				int num;
				int num2;
				int value;
				RoR2Application.NtQueryTimerResolution(out num, out num2, out value);
				return TextSerialization.ToStringInvariant(value);
			}

			// Token: 0x040010CF RID: 4303
			private static RoR2Application.TimerResolutionConVar instance = new RoR2Application.TimerResolutionConVar("timer_resolution", ConVarFlags.Engine, null, "The Windows timer resolution.");
		}

		// Token: 0x020002F1 RID: 753
		public class CheatsConVar : BaseConVar
		{
			// Token: 0x06001152 RID: 4434 RVA: 0x0000972B File Offset: 0x0000792B
			public CheatsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x1700021A RID: 538
			// (get) Token: 0x06001153 RID: 4435 RVA: 0x0004C31C File Offset: 0x0004A51C
			// (set) Token: 0x06001154 RID: 4436 RVA: 0x0004C324 File Offset: 0x0004A524
			public bool boolValue
			{
				get
				{
					return this._boolValue;
				}
				private set
				{
					if (this._boolValue)
					{
						RoR2Application.sessionCheatsEnabled = true;
					}
				}
			}

			// Token: 0x06001155 RID: 4437 RVA: 0x0004C334 File Offset: 0x0004A534
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					this.boolValue = (num != 0);
				}
			}

			// Token: 0x06001156 RID: 4438 RVA: 0x0004C355 File Offset: 0x0004A555
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040010D0 RID: 4304
			private bool _boolValue;
		}

		// Token: 0x020002F2 RID: 754
		private class TimeScaleConVar : BaseConVar
		{
			// Token: 0x06001157 RID: 4439 RVA: 0x0000972B File Offset: 0x0000792B
			public TimeScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001158 RID: 4440 RVA: 0x0004C36C File Offset: 0x0004A56C
			public override void SetString(string newValue)
			{
				float timeScale;
				if (TextSerialization.TryParseInvariant(newValue, out timeScale))
				{
					Time.timeScale = timeScale;
				}
			}

			// Token: 0x06001159 RID: 4441 RVA: 0x0004C389 File Offset: 0x0004A589
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Time.timeScale);
			}
		}

		// Token: 0x020002F3 RID: 755
		private class TimeStepConVar : BaseConVar
		{
			// Token: 0x0600115A RID: 4442 RVA: 0x0000972B File Offset: 0x0000792B
			public TimeStepConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600115B RID: 4443 RVA: 0x0004C398 File Offset: 0x0004A598
			public override void SetString(string newValue)
			{
				float fixedDeltaTime;
				if (TextSerialization.TryParseInvariant(newValue, out fixedDeltaTime))
				{
					Time.fixedDeltaTime = fixedDeltaTime;
				}
			}

			// Token: 0x0600115C RID: 4444 RVA: 0x0004C3B5 File Offset: 0x0004A5B5
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Time.fixedDeltaTime);
			}
		}

		// Token: 0x020002F4 RID: 756
		private class SyncPhysicsConVar : BaseConVar
		{
			// Token: 0x0600115D RID: 4445 RVA: 0x0000972B File Offset: 0x0000792B
			private SyncPhysicsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600115E RID: 4446 RVA: 0x0004C3C4 File Offset: 0x0004A5C4
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					bool flag = num != 0;
					if (Physics.autoSyncTransforms != flag)
					{
						Physics.autoSyncTransforms = flag;
					}
				}
			}

			// Token: 0x0600115F RID: 4447 RVA: 0x0004C3EE File Offset: 0x0004A5EE
			public override string GetString()
			{
				if (!Physics.autoSyncTransforms)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040010D1 RID: 4305
			public static RoR2Application.SyncPhysicsConVar instance = new RoR2Application.SyncPhysicsConVar("sync_physics", ConVarFlags.None, "0", "Enable/disables Physics 'autosyncing' between moves.");
		}

		// Token: 0x020002F5 RID: 757
		private class AutoSimulatePhysicsConVar : BaseConVar
		{
			// Token: 0x06001161 RID: 4449 RVA: 0x0000972B File Offset: 0x0000792B
			private AutoSimulatePhysicsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001162 RID: 4450 RVA: 0x0004C420 File Offset: 0x0004A620
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					bool flag = num != 0;
					if (flag != Physics.autoSimulation)
					{
						Physics.autoSimulation = flag;
					}
				}
			}

			// Token: 0x06001163 RID: 4451 RVA: 0x0004C44A File Offset: 0x0004A64A
			public override string GetString()
			{
				if (!Physics.autoSimulation)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040010D2 RID: 4306
			public static RoR2Application.AutoSimulatePhysicsConVar instance = new RoR2Application.AutoSimulatePhysicsConVar("auto_simulate_physics", ConVarFlags.None, "1", "Enable/disables Physics autosimulate.");
		}

		// Token: 0x020002F6 RID: 758
		private static class UnitySystemConsoleRedirector
		{
			// Token: 0x06001165 RID: 4453 RVA: 0x0004C47A File Offset: 0x0004A67A
			public static void Redirect()
			{
				Console.SetOut(new RoR2Application.UnitySystemConsoleRedirector.OutWriter());
				Console.SetError(new RoR2Application.UnitySystemConsoleRedirector.ErrorWriter());
			}

			// Token: 0x020002F7 RID: 759
			private class OutWriter : RoR2Application.UnitySystemConsoleRedirector.UnityTextWriter
			{
				// Token: 0x06001166 RID: 4454 RVA: 0x0004C490 File Offset: 0x0004A690
				public override void WriteBufferToUnity(string str)
				{
					Debug.Log(str);
				}
			}

			// Token: 0x020002F8 RID: 760
			private class ErrorWriter : RoR2Application.UnitySystemConsoleRedirector.UnityTextWriter
			{
				// Token: 0x06001168 RID: 4456 RVA: 0x0004C4A0 File Offset: 0x0004A6A0
				public override void WriteBufferToUnity(string str)
				{
					Debug.LogError(str);
				}
			}

			// Token: 0x020002F9 RID: 761
			private abstract class UnityTextWriter : TextWriter
			{
				// Token: 0x0600116A RID: 4458 RVA: 0x0004C4A8 File Offset: 0x0004A6A8
				public override void Flush()
				{
					this.WriteBufferToUnity(this.buffer.ToString());
					this.buffer.Length = 0;
				}

				// Token: 0x0600116B RID: 4459
				public abstract void WriteBufferToUnity(string str);

				// Token: 0x0600116C RID: 4460 RVA: 0x0004C4C8 File Offset: 0x0004A6C8
				public override void Write(string value)
				{
					this.buffer.Append(value);
					if (value != null)
					{
						int length = value.Length;
						if (length > 0 && value[length - 1] == '\n')
						{
							this.Flush();
						}
					}
				}

				// Token: 0x0600116D RID: 4461 RVA: 0x0004C503 File Offset: 0x0004A703
				public override void Write(char value)
				{
					this.buffer.Append(value);
					if (value == '\n')
					{
						this.Flush();
					}
				}

				// Token: 0x0600116E RID: 4462 RVA: 0x0004C51D File Offset: 0x0004A71D
				public override void Write(char[] value, int index, int count)
				{
					this.Write(new string(value, index, count));
				}

				// Token: 0x1700021B RID: 539
				// (get) Token: 0x0600116F RID: 4463 RVA: 0x0004C52D File Offset: 0x0004A72D
				public override Encoding Encoding
				{
					get
					{
						return Encoding.Default;
					}
				}

				// Token: 0x040010D3 RID: 4307
				private StringBuilder buffer = new StringBuilder();
			}
		}
	}
}
