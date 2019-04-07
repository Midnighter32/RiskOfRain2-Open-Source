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
using RoR2.Networking;
using SteamAPIValidator;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x020003AF RID: 943
	public class RoR2Application : MonoBehaviour
	{
		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x00061DBF File Offset: 0x0005FFBF
		// (set) Token: 0x060013F7 RID: 5111 RVA: 0x00061DC7 File Offset: 0x0005FFC7
		public Client steamworksClient { get; private set; }

		// Token: 0x060013F8 RID: 5112 RVA: 0x00061DD0 File Offset: 0x0005FFD0
		public static string GetBuildId()
		{
			if (RoR2Application.isModded)
			{
				return "MOD";
			}
			return RoR2Application.steamBuildId;
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060013F9 RID: 5113 RVA: 0x00061DE4 File Offset: 0x0005FFE4
		// (set) Token: 0x060013FA RID: 5114 RVA: 0x00061DEB File Offset: 0x0005FFEB
		public static RoR2Application instance { get; private set; }

		// Token: 0x060013FB RID: 5115 RVA: 0x00061DF4 File Offset: 0x0005FFF4
		private void Awake()
		{
			if (RoR2Application.maxPlayers != 4)
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

		// Token: 0x060013FC RID: 5116 RVA: 0x00061E58 File Offset: 0x00060058
		private void Start()
		{
			if (RoR2Application.instance == this && RoR2Application.onStart != null)
			{
				RoR2Application.onStart();
				RoR2Application.onStart = null;
			}
		}

		// Token: 0x060013FD RID: 5117
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtSetTimerResolution(int desiredResolution, bool setResolution, out int currentResolution);

		// Token: 0x060013FE RID: 5118
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtQueryTimerResolution(out int minimumResolution, out int maximumResolution, out int currentResolution);

		// Token: 0x060013FF RID: 5119 RVA: 0x00061E80 File Offset: 0x00060080
		private void Update()
		{
			if (RoR2Application.waitMsConVar.value >= 0)
			{
				Thread.Sleep(RoR2Application.waitMsConVar.value);
			}
			if (this.steamworksClient != null)
			{
				this.steamworksClient.Update();
			}
			Cursor.lockState = ((MPEventSystemManager.kbmEventSystem.isCursorVisible || MPEventSystemManager.combinedEventSystem.isCursorVisible) ? CursorLockMode.None : CursorLockMode.Locked);
			Cursor.visible = false;
			if (RoR2Application.onUpdate != null)
			{
				RoR2Application.onUpdate();
			}
			Action action = Interlocked.Exchange<Action>(ref RoR2Application.onNextUpdate, null);
			if (action != null)
			{
				action();
			}
			RoR2Application.timeTimers.Update(Time.deltaTime);
			RoR2Application.unscaledTimeTimers.Update(Time.unscaledDeltaTime);
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001400 RID: 5120 RVA: 0x00061F28 File Offset: 0x00060128
		// (remove) Token: 0x06001401 RID: 5121 RVA: 0x00061F5C File Offset: 0x0006015C
		public static event Action onUpdate;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001402 RID: 5122 RVA: 0x00061F90 File Offset: 0x00060190
		// (remove) Token: 0x06001403 RID: 5123 RVA: 0x00061FC4 File Offset: 0x000601C4
		public static event Action onFixedUpdate;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06001404 RID: 5124 RVA: 0x00061FF8 File Offset: 0x000601F8
		// (remove) Token: 0x06001405 RID: 5125 RVA: 0x0006202C File Offset: 0x0006022C
		public static event Action onLateUpdate;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06001406 RID: 5126 RVA: 0x00062060 File Offset: 0x00060260
		// (remove) Token: 0x06001407 RID: 5127 RVA: 0x00062094 File Offset: 0x00060294
		public static event Action onNextUpdate;

		// Token: 0x06001408 RID: 5128 RVA: 0x000620C7 File Offset: 0x000602C7
		private void FixedUpdate()
		{
			Action action = RoR2Application.onFixedUpdate;
			if (action != null)
			{
				action();
			}
			RoR2Application.fixedTimeTimers.Update(Time.fixedDeltaTime);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x000620E8 File Offset: 0x000602E8
		private void LateUpdate()
		{
			Action action = RoR2Application.onLateUpdate;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x0600140A RID: 5130 RVA: 0x000620F9 File Offset: 0x000602F9
		// (set) Token: 0x0600140B RID: 5131 RVA: 0x00062100 File Offset: 0x00060300
		public static FileSystem fileSystem { get; private set; }

		// Token: 0x0600140C RID: 5132 RVA: 0x00062108 File Offset: 0x00060308
		private void OnLoad()
		{
			RoR2Application.UnitySystemConsoleRedirector.Redirect();
			if (File.Exists("steam_appid.txt"))
			{
				try
				{
					File.Delete("steam_appid.txt");
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
				}
				if (File.Exists("steam_appid.txt"))
				{
					Debug.Log("Cannot delete steam_appid.txt. Quitting...");
					Application.Quit();
					return;
				}
			}
			Config.ForUnity(Application.platform.ToString());
			this.steamworksClient = new Client(632360u);
			if (Client.RestartIfNecessary(632360u) || !this.steamworksClient.IsValid || !SteamApiValidator.IsValidSteamApiDll())
			{
				Debug.Log("Unable to initialize Facepunch.Steamworks.");
				Application.Quit();
				return;
			}
			if (!this.steamworksClient.App.IsSubscribed(632360u))
			{
				Debug.Log("Steam user not subscribed to app. Quitting...");
				Application.Quit();
				return;
			}
			RoR2Application.steamBuildId = TextSerialization.ToStringInvariant(this.steamworksClient.BuildId);
			this.steamworksAuthTicket = this.steamworksClient.Auth.GetAuthSessionTicket();
			SteamworksEventManager.Init(this.steamworksClient);
			this.steamworksAvailability.MakeAvailable();
			PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
			RoR2Application.fileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Application.dataPath), true);
			RoR2Application.cloudStorage = RoR2Application.fileSystem;
			RoR2Application.cloudStorage = new SteamworksRemoteStorageFileSystem();
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
			if (UnityEngine.Object.FindObjectOfType<AkInitializer>())
			{
				Debug.LogError("Attempting to initialize wwise when AkInitializer already exists! This will cause a crash!");
				return;
			}
			this.wwiseGlobalInstance = UnityEngine.Object.Instantiate<GameObject>(this.wwiseGlobalPrefab);
			UnityEngine.Object.Instantiate<GameObject>(this.audioManagerPrefab);
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

		// Token: 0x0600140D RID: 5133 RVA: 0x000623CC File Offset: 0x000605CC
		private void ShutdownSteamworks()
		{
			if (this.steamworksClient != null)
			{
				if (GameNetworkManager.singleton)
				{
					GameNetworkManager.singleton.ForceCloseAllConnections();
				}
				Debug.Log("Shutting down Steamworks...");
				this.steamworksClient.Lobby.Leave();
				if (Server.Instance != null)
				{
					Server.Instance.Dispose();
				}
				this.steamworksClient.Update();
				this.steamworksClient.Dispose();
				this.steamworksClient = null;
				Debug.Log("Shut down Steamworks.");
			}
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00062449 File Offset: 0x00060649
		private void OnDestroy()
		{
			this.ShutdownSteamworks();
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00062451 File Offset: 0x00060651
		private void OnApplicationQuit()
		{
			UserProfile.HandleShutDown();
			if (Console.instance)
			{
				Console.instance.SaveArchiveConVars();
			}
			this.ShutdownSteamworks();
			bool isEditor = Application.isEditor;
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06001410 RID: 5136 RVA: 0x0006247A File Offset: 0x0006067A
		public static bool isInSinglePlayer
		{
			get
			{
				return NetworkServer.dontListen && LocalUserManager.readOnlyLocalUsersList.Count == 1;
			}
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00062494 File Offset: 0x00060694
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

		// Token: 0x06001412 RID: 5138 RVA: 0x0006250C File Offset: 0x0006070C
		private static void AssignNewController(ControllerStatusChangedEventArgs args)
		{
			RoR2Application.AssignNewController(ReInput.controllers.GetController(args.controllerType, args.controllerId));
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00062529 File Offset: 0x00060729
		private static void AssignNewController(Controller controller)
		{
			ReInput.players.GetPlayer("PlayerMain").controllers.AddController(controller, false);
			if (controller.type == ControllerType.Joystick)
			{
				RoR2Application.AssignJoystickToAvailablePlayer(controller);
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06001414 RID: 5140 RVA: 0x00062555 File Offset: 0x00060755
		// (set) Token: 0x06001415 RID: 5141 RVA: 0x0006255C File Offset: 0x0006075C
		public static bool sessionCheatsEnabled { get; private set; }

		// Token: 0x06001416 RID: 5142 RVA: 0x00062564 File Offset: 0x00060764
		[ConCommand(commandName = "pause", flags = ConVarFlags.None, helpText = "Toggles game pause state.")]
		private static void CCTogglePause(ConCommandArgs args)
		{
			if (RoR2Application.instance.pauseScreenInstance)
			{
				UnityEngine.Object.Destroy(RoR2Application.instance.pauseScreenInstance);
				RoR2Application.instance.pauseScreenInstance = null;
				return;
			}
			if (Run.instance)
			{
				RoR2Application.instance.pauseScreenInstance = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/PauseScreen"));
			}
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x000625C2 File Offset: 0x000607C2
		[ConCommand(commandName = "quit", flags = ConVarFlags.None, helpText = "Close the application.")]
		private static void CCQuit(ConCommandArgs args)
		{
			Application.Quit();
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x000625C9 File Offset: 0x000607C9
		public static void IncrementActiveWriteCount()
		{
			Interlocked.Increment(ref RoR2Application.activeWriteCount);
			RoR2Application.saveIconAlpha = 2f;
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x000625E2 File Offset: 0x000607E2
		public static void DecrementActiveWriteCount()
		{
			Interlocked.Decrement(ref RoR2Application.activeWriteCount);
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x000625EF File Offset: 0x000607EF
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

		// Token: 0x040017C1 RID: 6081
		[HideInInspector]
		[SerializeField]
		private bool loaded;

		// Token: 0x040017C2 RID: 6082
		public static readonly string messageForModders = "We don't officially support modding at this time but if you're going to mod the game please change this value to true if you're modding the game. This will disable some things like Prismatic Trials and put players into a separate matchmaking queue from vanilla users to protect their game experience.";

		// Token: 0x040017C3 RID: 6083
		public static bool isModded = false;

		// Token: 0x040017C4 RID: 6084
		public GameObject networkManagerPrefab;

		// Token: 0x040017C5 RID: 6085
		public GameObject wwiseGlobalPrefab;

		// Token: 0x040017C6 RID: 6086
		public GameObject audioManagerPrefab;

		// Token: 0x040017C7 RID: 6087
		public EntityStateManager stateManager;

		// Token: 0x040017C8 RID: 6088
		public PostProcessVolume postProcessSettingsController;

		// Token: 0x040017C9 RID: 6089
		public Canvas mainCanvas;

		// Token: 0x040017CA RID: 6090
		public Stopwatch stopwatch = new Stopwatch();

		// Token: 0x040017CB RID: 6091
		public const string gameName = "Risk of Rain 2";

		// Token: 0x040017CC RID: 6092
		private const uint ror2AppId = 632360u;

		// Token: 0x040017CD RID: 6093
		public const uint appId = 632360u;

		// Token: 0x040017CF RID: 6095
		public Auth.Ticket steamworksAuthTicket;

		// Token: 0x040017D0 RID: 6096
		public ResourceAvailability steamworksAvailability;

		// Token: 0x040017D1 RID: 6097
		private static string steamBuildId = "STEAM_UNINITIALIZED";

		// Token: 0x040017D2 RID: 6098
		public static readonly int hardMaxPlayers = 16;

		// Token: 0x040017D3 RID: 6099
		public static readonly int maxPlayers = 4;

		// Token: 0x040017D4 RID: 6100
		public static readonly int maxLocalPlayers = 4;

		// Token: 0x040017D5 RID: 6101
		private GameObject wwiseGlobalInstance;

		// Token: 0x040017D7 RID: 6103
		private static IntConVar waitMsConVar = new IntConVar("wait_ms", ConVarFlags.None, "-1", "How many milliseconds to sleep between each frame. -1 for no sleeping between frames.");

		// Token: 0x040017D8 RID: 6104
		private GameObject pauseScreenInstance;

		// Token: 0x040017D9 RID: 6105
		public static readonly TimerQueue timeTimers = new TimerQueue();

		// Token: 0x040017DA RID: 6106
		public static readonly TimerQueue fixedTimeTimers = new TimerQueue();

		// Token: 0x040017DB RID: 6107
		public static readonly TimerQueue unscaledTimeTimers = new TimerQueue();

		// Token: 0x040017E1 RID: 6113
		public static FileSystem cloudStorage;

		// Token: 0x040017E2 RID: 6114
		public static Action onLoad;

		// Token: 0x040017E3 RID: 6115
		public static Action onStart;

		// Token: 0x040017E4 RID: 6116
		private const bool isRewiredFixedYet = true;

		// Token: 0x040017E6 RID: 6118
		public static RoR2Application.CheatsConVar cvCheats = new RoR2Application.CheatsConVar("cheats", ConVarFlags.ExecuteOnServer, "0", "Enable cheats. Achievements, unlock progression, and stat tracking will be disabled until the application is restarted.");

		// Token: 0x040017E7 RID: 6119
		private static float oldTimeScale = 1f;

		// Token: 0x040017E8 RID: 6120
		public static Action onPauseStartGlobal;

		// Token: 0x040017E9 RID: 6121
		public static Action onPauseEndGlobal;

		// Token: 0x040017EA RID: 6122
		private static RoR2Application.TimeScaleConVar cvTimeScale = new RoR2Application.TimeScaleConVar("timescale", ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat | ConVarFlags.Engine, null, "The timescale of the game.");

		// Token: 0x040017EB RID: 6123
		private static RoR2Application.TimeStepConVar cvTimeStep = new RoR2Application.TimeStepConVar("timestep", ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat | ConVarFlags.Engine, null, "The timestep of the game.");

		// Token: 0x040017EC RID: 6124
		public static readonly Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);

		// Token: 0x040017ED RID: 6125
		public static BoolConVar enableDamageNumbers = new BoolConVar("enable_damage_numbers", ConVarFlags.None, "1", "Whether or not damage and healing numbers spawn.");

		// Token: 0x040017EE RID: 6126
		private static int activeWriteCount;

		// Token: 0x040017EF RID: 6127
		private static volatile float saveIconAlpha = 0f;

		// Token: 0x020003B0 RID: 944
		private class TimerResolutionConVar : BaseConVar
		{
			// Token: 0x0600141D RID: 5149 RVA: 0x00037E38 File Offset: 0x00036038
			private TimerResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600141E RID: 5150 RVA: 0x00062744 File Offset: 0x00060944
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

			// Token: 0x0600141F RID: 5151 RVA: 0x00062788 File Offset: 0x00060988
			public override string GetString()
			{
				int num;
				int num2;
				int value;
				RoR2Application.NtQueryTimerResolution(out num, out num2, out value);
				return TextSerialization.ToStringInvariant(value);
			}

			// Token: 0x040017F0 RID: 6128
			private static RoR2Application.TimerResolutionConVar instance = new RoR2Application.TimerResolutionConVar("timer_resolution", ConVarFlags.Engine, null, "The Windows timer resolution.");
		}

		// Token: 0x020003B1 RID: 945
		public class CheatsConVar : BaseConVar
		{
			// Token: 0x06001421 RID: 5153 RVA: 0x00037E38 File Offset: 0x00036038
			public CheatsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x170001C4 RID: 452
			// (get) Token: 0x06001422 RID: 5154 RVA: 0x000627C0 File Offset: 0x000609C0
			// (set) Token: 0x06001423 RID: 5155 RVA: 0x000627C8 File Offset: 0x000609C8
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

			// Token: 0x06001424 RID: 5156 RVA: 0x000627D8 File Offset: 0x000609D8
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					this.boolValue = (num != 0);
				}
			}

			// Token: 0x06001425 RID: 5157 RVA: 0x000627F9 File Offset: 0x000609F9
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040017F1 RID: 6129
			private bool _boolValue;
		}

		// Token: 0x020003B2 RID: 946
		private class TimeScaleConVar : BaseConVar
		{
			// Token: 0x06001426 RID: 5158 RVA: 0x00037E38 File Offset: 0x00036038
			public TimeScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001427 RID: 5159 RVA: 0x00062810 File Offset: 0x00060A10
			public override void SetString(string newValue)
			{
				float timeScale;
				if (TextSerialization.TryParseInvariant(newValue, out timeScale))
				{
					Time.timeScale = timeScale;
				}
			}

			// Token: 0x06001428 RID: 5160 RVA: 0x0006282D File Offset: 0x00060A2D
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Time.timeScale);
			}
		}

		// Token: 0x020003B3 RID: 947
		private class TimeStepConVar : BaseConVar
		{
			// Token: 0x06001429 RID: 5161 RVA: 0x00037E38 File Offset: 0x00036038
			public TimeStepConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600142A RID: 5162 RVA: 0x0006283C File Offset: 0x00060A3C
			public override void SetString(string newValue)
			{
				float fixedDeltaTime;
				if (TextSerialization.TryParseInvariant(newValue, out fixedDeltaTime))
				{
					Time.fixedDeltaTime = fixedDeltaTime;
				}
			}

			// Token: 0x0600142B RID: 5163 RVA: 0x00062859 File Offset: 0x00060A59
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Time.fixedDeltaTime);
			}
		}

		// Token: 0x020003B4 RID: 948
		private class SyncPhysicsConVar : BaseConVar
		{
			// Token: 0x0600142C RID: 5164 RVA: 0x00037E38 File Offset: 0x00036038
			private SyncPhysicsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600142D RID: 5165 RVA: 0x00062868 File Offset: 0x00060A68
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

			// Token: 0x0600142E RID: 5166 RVA: 0x00062892 File Offset: 0x00060A92
			public override string GetString()
			{
				if (!Physics.autoSyncTransforms)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040017F2 RID: 6130
			public static RoR2Application.SyncPhysicsConVar instance = new RoR2Application.SyncPhysicsConVar("sync_physics", ConVarFlags.None, "0", "Enable/disables Physics 'autosyncing' between moves.");
		}

		// Token: 0x020003B5 RID: 949
		private class AutoSimulatePhysicsConVar : BaseConVar
		{
			// Token: 0x06001430 RID: 5168 RVA: 0x00037E38 File Offset: 0x00036038
			private AutoSimulatePhysicsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001431 RID: 5169 RVA: 0x000628C4 File Offset: 0x00060AC4
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

			// Token: 0x06001432 RID: 5170 RVA: 0x000628EE File Offset: 0x00060AEE
			public override string GetString()
			{
				if (!Physics.autoSimulation)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040017F3 RID: 6131
			public static RoR2Application.AutoSimulatePhysicsConVar instance = new RoR2Application.AutoSimulatePhysicsConVar("auto_simulate_physics", ConVarFlags.None, "1", "Enable/disables Physics autosimulate.");
		}

		// Token: 0x020003B6 RID: 950
		private static class UnitySystemConsoleRedirector
		{
			// Token: 0x06001434 RID: 5172 RVA: 0x0006291E File Offset: 0x00060B1E
			public static void Redirect()
			{
				Console.SetOut(new RoR2Application.UnitySystemConsoleRedirector.OutWriter());
				Console.SetError(new RoR2Application.UnitySystemConsoleRedirector.ErrorWriter());
			}

			// Token: 0x020003B7 RID: 951
			private class OutWriter : RoR2Application.UnitySystemConsoleRedirector.UnityTextWriter
			{
				// Token: 0x06001435 RID: 5173 RVA: 0x00062934 File Offset: 0x00060B34
				public override void WriteBufferToUnity(string str)
				{
					Debug.Log(str);
				}
			}

			// Token: 0x020003B8 RID: 952
			private class ErrorWriter : RoR2Application.UnitySystemConsoleRedirector.UnityTextWriter
			{
				// Token: 0x06001437 RID: 5175 RVA: 0x00062944 File Offset: 0x00060B44
				public override void WriteBufferToUnity(string str)
				{
					Debug.LogError(str);
				}
			}

			// Token: 0x020003B9 RID: 953
			private abstract class UnityTextWriter : TextWriter
			{
				// Token: 0x06001439 RID: 5177 RVA: 0x0006294C File Offset: 0x00060B4C
				public override void Flush()
				{
					this.WriteBufferToUnity(this.buffer.ToString());
					this.buffer.Length = 0;
				}

				// Token: 0x0600143A RID: 5178
				public abstract void WriteBufferToUnity(string str);

				// Token: 0x0600143B RID: 5179 RVA: 0x0006296C File Offset: 0x00060B6C
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

				// Token: 0x0600143C RID: 5180 RVA: 0x000629A7 File Offset: 0x00060BA7
				public override void Write(char value)
				{
					this.buffer.Append(value);
					if (value == '\n')
					{
						this.Flush();
					}
				}

				// Token: 0x0600143D RID: 5181 RVA: 0x000629C1 File Offset: 0x00060BC1
				public override void Write(char[] value, int index, int count)
				{
					this.Write(new string(value, index, count));
				}

				// Token: 0x170001C5 RID: 453
				// (get) Token: 0x0600143E RID: 5182 RVA: 0x000629D1 File Offset: 0x00060BD1
				public override Encoding Encoding
				{
					get
					{
						return Encoding.Default;
					}
				}

				// Token: 0x040017F4 RID: 6132
				private StringBuilder buffer = new StringBuilder();
			}
		}
	}
}
