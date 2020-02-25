using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001BA RID: 442
	public class Console : MonoBehaviour
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600097B RID: 2427 RVA: 0x00029293 File Offset: 0x00027493
		// (set) Token: 0x0600097C RID: 2428 RVA: 0x0002929A File Offset: 0x0002749A
		public static Console instance { get; private set; }

		// Token: 0x0600097D RID: 2429 RVA: 0x000292A2 File Offset: 0x000274A2
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RegisterLogHandler()
		{
			Application.logMessageReceived += Console.HandleLog;
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x000292B8 File Offset: 0x000274B8
		private static void HandleLog(string message, string stackTrace, LogType logType)
		{
			if (logType == LogType.Error)
			{
				message = string.Format(CultureInfo.InvariantCulture, "<color=#FF0000>{0}</color>", message);
			}
			else if (logType == LogType.Warning)
			{
				message = string.Format(CultureInfo.InvariantCulture, "<color=#FFFF00>{0}</color>", message);
			}
			Console.Log log = new Console.Log
			{
				message = message,
				stackTrace = stackTrace,
				logType = logType
			};
			Console.logs.Add(log);
			if (Console.maxMessages.value > 0)
			{
				while (Console.logs.Count > Console.maxMessages.value)
				{
					Console.logs.RemoveAt(0);
				}
			}
			if (Console.onLogReceived != null)
			{
				Console.onLogReceived(log);
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x0600097F RID: 2431 RVA: 0x00029364 File Offset: 0x00027564
		// (remove) Token: 0x06000980 RID: 2432 RVA: 0x00029398 File Offset: 0x00027598
		public static event Console.LogReceivedDelegate onLogReceived;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06000981 RID: 2433 RVA: 0x000293CC File Offset: 0x000275CC
		// (remove) Token: 0x06000982 RID: 2434 RVA: 0x00029400 File Offset: 0x00027600
		public static event Action onClear;

		// Token: 0x06000983 RID: 2435 RVA: 0x00029434 File Offset: 0x00027634
		private string GetVstrValue(NetworkUser user, string identifier)
		{
			string result;
			if (!(user == null))
			{
				result = "";
				return result;
			}
			if (this.vstrs.TryGetValue(identifier, out result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0002946C File Offset: 0x0002766C
		private void InitConVars()
		{
			this.allConVars = new Dictionary<string, BaseConVar>();
			this.archiveConVars = new List<BaseConVar>();
			foreach (Type type in typeof(BaseConVar).Assembly.GetTypes())
			{
				foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (fieldInfo.FieldType.IsSubclassOf(typeof(BaseConVar)))
					{
						if (fieldInfo.IsStatic)
						{
							BaseConVar conVar = (BaseConVar)fieldInfo.GetValue(null);
							this.RegisterConVarInternal(conVar);
						}
						else if (type.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
						{
							Debug.LogErrorFormat("ConVar defined as {0}.{1} could not be registered. ConVars must be static fields.", new object[]
							{
								type.Name,
								fieldInfo.Name
							});
						}
					}
				}
				foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo.GetCustomAttribute<ConVarProviderAttribute>() != null)
					{
						if (methodInfo.ReturnType != typeof(IEnumerable<BaseConVar>) || methodInfo.GetParameters().Length != 0)
						{
							Debug.LogErrorFormat("ConVar provider {0}.{1} does not match the signature \"static IEnumerable<ConVar.BaseConVar>()\".", new object[]
							{
								type.Name,
								methodInfo.Name
							});
						}
						else if (!methodInfo.IsStatic)
						{
							Debug.LogErrorFormat("ConVar provider {0}.{1} could not be invoked. Methods marked with the ConVarProvider attribute must be static.", new object[]
							{
								type.Name,
								methodInfo.Name
							});
						}
						else
						{
							foreach (BaseConVar conVar2 in ((IEnumerable<BaseConVar>)methodInfo.Invoke(null, Array.Empty<object>())))
							{
								this.RegisterConVarInternal(conVar2);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in this.allConVars)
			{
				BaseConVar value = keyValuePair.Value;
				if ((value.flags & ConVarFlags.Engine) != ConVarFlags.None)
				{
					value.defaultValue = value.GetString();
				}
				else if (value.defaultValue != null)
				{
					value.AttemptSetString(value.defaultValue);
				}
			}
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x000296BC File Offset: 0x000278BC
		private void RegisterConVarInternal(BaseConVar conVar)
		{
			if (conVar == null)
			{
				Debug.LogWarning("Attempted to register null ConVar");
				return;
			}
			this.allConVars[conVar.name] = conVar;
			if ((conVar.flags & ConVarFlags.Archive) != ConVarFlags.None)
			{
				this.archiveConVars.Add(conVar);
			}
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x000296F4 File Offset: 0x000278F4
		public BaseConVar FindConVar(string name)
		{
			BaseConVar result;
			if (this.allConVars.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x00029714 File Offset: 0x00027914
		public void SubmitCmd(NetworkUser sender, string cmd, bool recordSubmit = false)
		{
			if (recordSubmit)
			{
				Console.Log log = new Console.Log
				{
					message = string.Format(CultureInfo.InvariantCulture, "<color=#C0C0C0>] {0}</color>", cmd),
					stackTrace = "",
					logType = LogType.Log
				};
				Console.logs.Add(log);
				if (Console.onLogReceived != null)
				{
					Console.onLogReceived(log);
				}
				Console.userCmdHistory.Add(cmd);
			}
			Queue<string> tokens = new Console.Lexer(cmd).GetTokens();
			List<string> list = new List<string>();
			bool flag = false;
			while (tokens.Count != 0)
			{
				string text = tokens.Dequeue();
				if (text == ";")
				{
					flag = false;
					if (list.Count > 0)
					{
						string concommandName = list[0].ToLower(CultureInfo.InvariantCulture);
						list.RemoveAt(0);
						this.RunCmd(sender, concommandName, list);
						list.Clear();
					}
				}
				else
				{
					if (flag)
					{
						text = this.GetVstrValue(sender, text);
						flag = false;
					}
					if (text == "vstr")
					{
						flag = true;
					}
					else
					{
						list.Add(text);
					}
				}
			}
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00029817 File Offset: 0x00027A17
		private void ForwardCmdToServer(ConCommandArgs args)
		{
			if (!args.sender)
			{
				return;
			}
			args.sender.CallCmdSendConsoleCommand(args.commandName, args.userArgs.ToArray());
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x00029843 File Offset: 0x00027A43
		public void RunClientCmd(NetworkUser sender, string concommandName, string[] args)
		{
			this.RunCmd(sender, concommandName, new List<string>(args));
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x00029854 File Offset: 0x00027A54
		private void RunCmd(NetworkUser sender, string concommandName, List<string> userArgs)
		{
			bool flag = sender != null && !sender.isLocalPlayer;
			Console.ConCommand conCommand = null;
			BaseConVar baseConVar = null;
			ConVarFlags flags;
			if (this.concommandCatalog.TryGetValue(concommandName, out conCommand))
			{
				flags = conCommand.flags;
			}
			else
			{
				baseConVar = this.FindConVar(concommandName);
				if (baseConVar == null)
				{
					Debug.LogFormat("\"{0}\" is not a recognized ConCommand or ConVar.", new object[]
					{
						concommandName
					});
					return;
				}
				flags = baseConVar.flags;
			}
			bool flag2 = (flags & ConVarFlags.ExecuteOnServer) > ConVarFlags.None;
			if (!NetworkServer.active && flag2)
			{
				this.ForwardCmdToServer(new ConCommandArgs
				{
					sender = sender,
					commandName = concommandName,
					userArgs = userArgs
				});
				return;
			}
			if (flag && (flags & ConVarFlags.SenderMustBeServer) != ConVarFlags.None)
			{
				Debug.LogFormat("Blocked server-only command {0} from remote user {1}.", new object[]
				{
					concommandName,
					sender.userName
				});
				return;
			}
			if (flag && !flag2)
			{
				Debug.LogFormat("Blocked non-transmittable command {0} from remote user {1}.", new object[]
				{
					concommandName,
					sender.userName
				});
				return;
			}
			if ((flags & ConVarFlags.Cheat) != ConVarFlags.None && !RoR2Application.cvCheats.boolValue)
			{
				Debug.LogFormat("Command \"{0}\" cannot be used while cheats are disabled.", new object[]
				{
					concommandName
				});
				return;
			}
			if (conCommand != null)
			{
				try
				{
					conCommand.action(new ConCommandArgs
					{
						sender = sender,
						commandName = concommandName,
						userArgs = userArgs
					});
				}
				catch (ConCommandException ex)
				{
					Debug.LogFormat("Command \"{0}\" failed: {1}", new object[]
					{
						concommandName,
						ex.Message
					});
				}
				return;
			}
			if (baseConVar == null)
			{
				return;
			}
			if (userArgs.Count > 0)
			{
				baseConVar.AttemptSetString(userArgs[0]);
				return;
			}
			Debug.LogFormat("\"{0}\" = \"{1}\"\n{2}", new object[]
			{
				concommandName,
				baseConVar.GetString(),
				baseConVar.helpText
			});
		}

		// Token: 0x0600098B RID: 2443
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		// Token: 0x0600098C RID: 2444
		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		// Token: 0x0600098D RID: 2445
		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int processId);

		// Token: 0x0600098E RID: 2446
		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		// Token: 0x0600098F RID: 2447
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x06000990 RID: 2448 RVA: 0x00029A10 File Offset: 0x00027C10
		private static string ReadInputStream()
		{
			if (Console.stdInQueue.Count > 0)
			{
				return Console.stdInQueue.Dequeue();
			}
			return null;
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x00029A2C File Offset: 0x00027C2C
		private static void ThreadedInputQueue()
		{
			string item;
			while (Console.systemConsoleType != Console.SystemConsoleType.None && (item = Console.ReadLine()) != null)
			{
				Console.stdInQueue.Enqueue(item);
			}
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x00029A58 File Offset: 0x00027C58
		private static void SetupSystemConsole()
		{
			bool flag = false;
			bool flag2 = false;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i] == "-console")
				{
					flag = true;
				}
				if (commandLineArgs[i] == "-console_detach")
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				Console.systemConsoleType = Console.SystemConsoleType.Attach;
				if (flag2)
				{
					Console.systemConsoleType = Console.SystemConsoleType.Alloc;
				}
			}
			switch (Console.systemConsoleType)
			{
			case Console.SystemConsoleType.Attach:
				Console.AttachConsole(-1);
				break;
			case Console.SystemConsoleType.Alloc:
				Console.AllocConsole();
				break;
			}
			if (Console.systemConsoleType != Console.SystemConsoleType.None)
			{
				Console.SetIn(new StreamReader(Console.OpenStandardInput()));
				Console.stdInReaderThread = new Thread(new ThreadStart(Console.ThreadedInputQueue));
				Console.stdInReaderThread.Start();
			}
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x00029B14 File Offset: 0x00027D14
		private void Awake()
		{
			Console.instance = this;
			Console.SetupSystemConsole();
			this.InitConVars();
			foreach (SearchableAttribute searchableAttribute in SearchableAttribute.GetInstances<ConCommandAttribute>())
			{
				ConCommandAttribute conCommandAttribute = (ConCommandAttribute)searchableAttribute;
				this.concommandCatalog[conCommandAttribute.commandName.ToLower(CultureInfo.InvariantCulture)] = new Console.ConCommand
				{
					flags = conCommandAttribute.flags,
					action = (Console.ConCommandDelegate)Delegate.CreateDelegate(typeof(Console.ConCommandDelegate), conCommandAttribute.target as MethodInfo),
					helpText = conCommandAttribute.helpText
				};
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				Debug.LogFormat("arg[{0}]=\"{1}\"", new object[]
				{
					i,
					commandLineArgs[i]
				});
			}
			MPEventSystemManager.availability.CallWhenAvailable(new Action(this.LoadStartupConfigs));
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x00029C1C File Offset: 0x00027E1C
		private void LoadStartupConfigs()
		{
			this.SubmitCmd(null, "exec config", false);
			this.SubmitCmd(null, "exec autoexec", false);
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x00029C38 File Offset: 0x00027E38
		private void Update()
		{
			string cmd;
			while ((cmd = Console.ReadInputStream()) != null)
			{
				this.SubmitCmd(null, cmd, true);
			}
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x00029C5C File Offset: 0x00027E5C
		private void OnDestroy()
		{
			if (Console.stdInReaderThread != null)
			{
				Console.stdInReaderThread = null;
			}
			if (Console.systemConsoleType != Console.SystemConsoleType.None)
			{
				Console.systemConsoleType = Console.SystemConsoleType.None;
				IntPtr consoleWindow = Console.GetConsoleWindow();
				if (consoleWindow != IntPtr.Zero)
				{
					Console.PostMessage(consoleWindow, 256U, 13, 0);
				}
				if (Console.stdInReaderThread != null)
				{
					Console.stdInReaderThread.Join();
					Console.stdInReaderThread = null;
				}
				Console.SetIn(null);
				Console.SetOut(null);
				Console.SetError(null);
				Console.FreeConsole();
			}
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x00029CD4 File Offset: 0x00027ED4
		private static string LoadConfig(string fileName)
		{
			string text = Console.sharedStringBuilder.Clear().Append("/Config/").Append(fileName).Append(".cfg").ToString();
			try
			{
				using (Stream stream = RoR2Application.fileSystem.OpenFile(text, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					if (stream != null)
					{
						using (TextReader textReader = new StreamReader(stream))
						{
							return textReader.ReadToEnd();
						}
					}
				}
			}
			catch (IOException ex)
			{
				Debug.LogFormat("Could not load config {0}: {1}", new object[]
				{
					text,
					ex.Message
				});
			}
			return null;
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00029D98 File Offset: 0x00027F98
		public void SaveArchiveConVars()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (TextWriter textWriter = new StreamWriter(memoryStream, Encoding.UTF8))
				{
					for (int i = 0; i < this.archiveConVars.Count; i++)
					{
						BaseConVar baseConVar = this.archiveConVars[i];
						textWriter.Write(baseConVar.name);
						textWriter.Write(" ");
						textWriter.Write(baseConVar.GetString());
						textWriter.Write(";\r\n");
					}
					textWriter.Write("echo \"Loaded archived convars.\";");
					textWriter.Flush();
					RoR2Application.fileSystem.CreateDirectory("/Config/");
					try
					{
						using (Stream stream = RoR2Application.fileSystem.OpenFile("/Config/config.cfg", FileMode.Create, FileAccess.Write, FileShare.None))
						{
							if (stream != null)
							{
								stream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
								stream.Close();
							}
						}
					}
					catch (IOException ex)
					{
						Debug.LogFormat("Failed to write archived convars: {0}", new object[]
						{
							ex.Message
						});
					}
				}
			}
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00029EDC File Offset: 0x000280DC
		[ConCommand(commandName = "set_vstr", flags = ConVarFlags.None, helpText = "Sets the specified vstr to the specified value.")]
		private static void CCSetVstr(ConCommandArgs args)
		{
			args.CheckArgumentCount(2);
			Console.instance.vstrs.Add(args[0], args[1]);
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00029F08 File Offset: 0x00028108
		[ConCommand(commandName = "exec", flags = ConVarFlags.None, helpText = "Executes a named config from the \"Config/\" folder.")]
		private static void CCExec(ConCommandArgs args)
		{
			if (args.Count > 0)
			{
				string text = Console.LoadConfig(args[0]);
				if (text != null)
				{
					Console.instance.SubmitCmd(args.sender, text, false);
				}
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00029F42 File Offset: 0x00028142
		[ConCommand(commandName = "echo", flags = ConVarFlags.None, helpText = "Echoes the given text to the console.")]
		private static void CCEcho(ConCommandArgs args)
		{
			if (args.Count > 0)
			{
				Debug.Log(args[0]);
				return;
			}
			Console.ShowHelpText(args.commandName);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00029F68 File Offset: 0x00028168
		[ConCommand(commandName = "cvarlist", flags = ConVarFlags.None, helpText = "Print all available convars and concommands.")]
		private static void CCCvarList(ConCommandArgs args)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in Console.instance.allConVars)
			{
				list.Add(keyValuePair.Key);
			}
			foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in Console.instance.concommandCatalog)
			{
				list.Add(keyValuePair2.Key);
			}
			list.Sort();
			Debug.Log(string.Join("\n", list.ToArray()));
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x0002A034 File Offset: 0x00028234
		[ConCommand(commandName = "help", flags = ConVarFlags.None, helpText = "Show help text for the named convar or concommand.")]
		private static void CCHelp(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				Console.instance.SubmitCmd(args.sender, "find \"*\"", false);
				return;
			}
			Console.ShowHelpText(args[0]);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x0002A064 File Offset: 0x00028264
		[ConCommand(commandName = "find", flags = ConVarFlags.None, helpText = "Find all concommands and convars with the specified substring.")]
		private static void CCFind(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				Console.ShowHelpText("find");
				return;
			}
			string text = args[0].ToLower(CultureInfo.InvariantCulture);
			bool flag = text == "*";
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in Console.instance.allConVars)
			{
				if (flag || keyValuePair.Key.ToLower(CultureInfo.InvariantCulture).Contains(text) || keyValuePair.Value.helpText.ToLower(CultureInfo.InvariantCulture).Contains(text))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in Console.instance.concommandCatalog)
			{
				if (flag || keyValuePair2.Key.ToLower(CultureInfo.InvariantCulture).Contains(text) || keyValuePair2.Value.helpText.ToLower(CultureInfo.InvariantCulture).Contains(text))
				{
					list.Add(keyValuePair2.Key);
				}
			}
			list.Sort();
			string[] array = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = Console.GetHelpText(list[i]);
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x0002A208 File Offset: 0x00028408
		[ConCommand(commandName = "clear", flags = ConVarFlags.None, helpText = "Clears the console output.")]
		private static void CCClear(ConCommandArgs args)
		{
			Console.logs.Clear();
			Action action = Console.onClear;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x0002A224 File Offset: 0x00028424
		private static string GetHelpText(string commandName)
		{
			Console.ConCommand conCommand;
			if (Console.instance.concommandCatalog.TryGetValue(commandName, out conCommand))
			{
				return string.Format(CultureInfo.InvariantCulture, "<color=#FF7F7F>\"{0}\"</color>\n- {1}", commandName, conCommand.helpText);
			}
			BaseConVar baseConVar = Console.instance.FindConVar(commandName);
			if (baseConVar != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "<color=#FF7F7F>\"{0}\" = \"{1}\"</color>\n - {2}", commandName, baseConVar.GetString(), baseConVar.helpText);
			}
			return "";
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x0002A28D File Offset: 0x0002848D
		public static void ShowHelpText(string commandName)
		{
			Debug.Log(Console.GetHelpText(commandName));
		}

		// Token: 0x040009C5 RID: 2501
		public static List<Console.Log> logs = new List<Console.Log>();

		// Token: 0x040009C8 RID: 2504
		private Dictionary<string, string> vstrs = new Dictionary<string, string>();

		// Token: 0x040009C9 RID: 2505
		private Dictionary<string, Console.ConCommand> concommandCatalog = new Dictionary<string, Console.ConCommand>();

		// Token: 0x040009CA RID: 2506
		private Dictionary<string, BaseConVar> allConVars;

		// Token: 0x040009CB RID: 2507
		private List<BaseConVar> archiveConVars;

		// Token: 0x040009CC RID: 2508
		public static List<string> userCmdHistory = new List<string>();

		// Token: 0x040009CD RID: 2509
		private const int VK_RETURN = 13;

		// Token: 0x040009CE RID: 2510
		private const int WM_KEYDOWN = 256;

		// Token: 0x040009CF RID: 2511
		private static byte[] inputStreamBuffer = new byte[256];

		// Token: 0x040009D0 RID: 2512
		private static Queue<string> stdInQueue = new Queue<string>();

		// Token: 0x040009D1 RID: 2513
		private static Thread stdInReaderThread = null;

		// Token: 0x040009D2 RID: 2514
		private static Console.SystemConsoleType systemConsoleType = Console.SystemConsoleType.None;

		// Token: 0x040009D3 RID: 2515
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x040009D4 RID: 2516
		private const string configFolder = "/Config/";

		// Token: 0x040009D5 RID: 2517
		private const string archiveConVarsPath = "/Config/config.cfg";

		// Token: 0x040009D6 RID: 2518
		private static IntConVar maxMessages = new IntConVar("max_messages", ConVarFlags.Archive, "25", "Maximum number of messages that can be held in the console log.");

		// Token: 0x020001BB RID: 443
		public struct Log
		{
			// Token: 0x040009D7 RID: 2519
			public string message;

			// Token: 0x040009D8 RID: 2520
			public string stackTrace;

			// Token: 0x040009D9 RID: 2521
			public LogType logType;
		}

		// Token: 0x020001BC RID: 444
		// (Invoke) Token: 0x060009A5 RID: 2469
		public delegate void LogReceivedDelegate(Console.Log log);

		// Token: 0x020001BD RID: 445
		private class Lexer
		{
			// Token: 0x060009A8 RID: 2472 RVA: 0x0002A322 File Offset: 0x00028522
			public Lexer(string srcString)
			{
				this.srcString = srcString;
				this.readIndex = 0;
			}

			// Token: 0x060009A9 RID: 2473 RVA: 0x0002A343 File Offset: 0x00028543
			private static bool IsIgnorableCharacter(char character)
			{
				return !Console.Lexer.IsSeparatorCharacter(character) && !Console.Lexer.IsQuoteCharacter(character) && !Console.Lexer.IsIdentifierCharacter(character) && character != '/';
			}

			// Token: 0x060009AA RID: 2474 RVA: 0x0002A367 File Offset: 0x00028567
			private static bool IsSeparatorCharacter(char character)
			{
				return character == ';' || character == '\n';
			}

			// Token: 0x060009AB RID: 2475 RVA: 0x0002A375 File Offset: 0x00028575
			private static bool IsQuoteCharacter(char character)
			{
				return character == '\'' || character == '"';
			}

			// Token: 0x060009AC RID: 2476 RVA: 0x0002A383 File Offset: 0x00028583
			private static bool IsIdentifierCharacter(char character)
			{
				return char.IsLetterOrDigit(character) || character == '_' || character == '.' || character == '-' || character == ':';
			}

			// Token: 0x060009AD RID: 2477 RVA: 0x0002A3A4 File Offset: 0x000285A4
			private bool TrimComment()
			{
				if (this.readIndex >= this.srcString.Length)
				{
					return false;
				}
				if (this.srcString[this.readIndex] == '/')
				{
					if (this.readIndex + 1 < this.srcString.Length)
					{
						char c = this.srcString[this.readIndex + 1];
						if (c == '/')
						{
							while (this.readIndex < this.srcString.Length)
							{
								if (this.srcString[this.readIndex] == '\n')
								{
									this.readIndex++;
									return true;
								}
								this.readIndex++;
							}
							return true;
						}
						if (c == '*')
						{
							while (this.readIndex < this.srcString.Length - 1)
							{
								if (this.srcString[this.readIndex] == '*' && this.srcString[this.readIndex + 1] == '/')
								{
									this.readIndex += 2;
									return true;
								}
								this.readIndex++;
							}
							return true;
						}
					}
					this.readIndex++;
				}
				return false;
			}

			// Token: 0x060009AE RID: 2478 RVA: 0x0002A4D0 File Offset: 0x000286D0
			private void TrimWhitespace()
			{
				while (this.readIndex < this.srcString.Length && Console.Lexer.IsIgnorableCharacter(this.srcString[this.readIndex]))
				{
					this.readIndex++;
				}
			}

			// Token: 0x060009AF RID: 2479 RVA: 0x0002A50D File Offset: 0x0002870D
			private void TrimUnused()
			{
				do
				{
					this.TrimWhitespace();
				}
				while (this.TrimComment());
			}

			// Token: 0x060009B0 RID: 2480 RVA: 0x0002A520 File Offset: 0x00028720
			private static int UnescapeNext(string srcString, int startPos, out char result)
			{
				result = '\\';
				int num = startPos + 1;
				if (num < srcString.Length)
				{
					char c = srcString[num];
					if (c <= '\'')
					{
						if (c != '"' && c != '\'')
						{
							return 1;
						}
					}
					else if (c != '\\')
					{
						if (c != 'n')
						{
							return 1;
						}
						result = '\n';
						return 2;
					}
					result = c;
					return 2;
				}
				return 1;
			}

			// Token: 0x060009B1 RID: 2481 RVA: 0x0002A574 File Offset: 0x00028774
			public string NextToken()
			{
				this.TrimUnused();
				if (this.readIndex == this.srcString.Length)
				{
					return null;
				}
				Console.Lexer.TokenType tokenType = Console.Lexer.TokenType.Identifier;
				char c = this.srcString[this.readIndex];
				char c2 = '\0';
				if (Console.Lexer.IsQuoteCharacter(c))
				{
					tokenType = Console.Lexer.TokenType.NestedString;
					c2 = c;
					this.readIndex++;
				}
				else if (Console.Lexer.IsSeparatorCharacter(c))
				{
					this.readIndex++;
					return ";";
				}
				while (this.readIndex < this.srcString.Length)
				{
					char c3 = this.srcString[this.readIndex];
					if (tokenType == Console.Lexer.TokenType.Identifier)
					{
						if (!Console.Lexer.IsIdentifierCharacter(c3))
						{
							break;
						}
					}
					else if (tokenType == Console.Lexer.TokenType.NestedString)
					{
						if (c3 == '\\')
						{
							this.readIndex += Console.Lexer.UnescapeNext(this.srcString, this.readIndex, out c3) - 1;
						}
						else if (c3 == c2)
						{
							this.readIndex++;
							break;
						}
					}
					this.stringBuilder.Append(c3);
					this.readIndex++;
				}
				string result = this.stringBuilder.ToString();
				this.stringBuilder.Length = 0;
				return result;
			}

			// Token: 0x060009B2 RID: 2482 RVA: 0x0002A698 File Offset: 0x00028898
			public Queue<string> GetTokens()
			{
				Queue<string> queue = new Queue<string>();
				for (string item = this.NextToken(); item != null; item = this.NextToken())
				{
					queue.Enqueue(item);
				}
				queue.Enqueue(";");
				return queue;
			}

			// Token: 0x040009DA RID: 2522
			private string srcString;

			// Token: 0x040009DB RID: 2523
			private int readIndex;

			// Token: 0x040009DC RID: 2524
			private StringBuilder stringBuilder = new StringBuilder();

			// Token: 0x020001BE RID: 446
			private enum TokenType
			{
				// Token: 0x040009DE RID: 2526
				Identifier,
				// Token: 0x040009DF RID: 2527
				NestedString
			}
		}

		// Token: 0x020001BF RID: 447
		private class Substring
		{
			// Token: 0x17000140 RID: 320
			// (get) Token: 0x060009B3 RID: 2483 RVA: 0x0002A6D1 File Offset: 0x000288D1
			public int endIndex
			{
				get
				{
					return this.startIndex + this.length;
				}
			}

			// Token: 0x17000141 RID: 321
			// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0002A6E0 File Offset: 0x000288E0
			public string str
			{
				get
				{
					return this.srcString.Substring(this.startIndex, this.length);
				}
			}

			// Token: 0x17000142 RID: 322
			// (get) Token: 0x060009B5 RID: 2485 RVA: 0x0002A6F9 File Offset: 0x000288F9
			public Console.Substring nextToken
			{
				get
				{
					return new Console.Substring
					{
						srcString = this.srcString,
						startIndex = this.startIndex + this.length,
						length = 0
					};
				}
			}

			// Token: 0x040009E0 RID: 2528
			public string srcString;

			// Token: 0x040009E1 RID: 2529
			public int startIndex;

			// Token: 0x040009E2 RID: 2530
			public int length;
		}

		// Token: 0x020001C0 RID: 448
		private class ConCommand
		{
			// Token: 0x040009E3 RID: 2531
			public ConVarFlags flags;

			// Token: 0x040009E4 RID: 2532
			public Console.ConCommandDelegate action;

			// Token: 0x040009E5 RID: 2533
			public string helpText;
		}

		// Token: 0x020001C1 RID: 449
		// (Invoke) Token: 0x060009B9 RID: 2489
		public delegate void ConCommandDelegate(ConCommandArgs args);

		// Token: 0x020001C2 RID: 450
		private enum SystemConsoleType
		{
			// Token: 0x040009E7 RID: 2535
			None,
			// Token: 0x040009E8 RID: 2536
			Attach,
			// Token: 0x040009E9 RID: 2537
			Alloc
		}

		// Token: 0x020001C3 RID: 451
		public class AutoComplete
		{
			// Token: 0x060009BC RID: 2492 RVA: 0x0002A728 File Offset: 0x00028928
			public AutoComplete(Console console)
			{
				HashSet<string> hashSet = new HashSet<string>();
				for (int i = 0; i < Console.userCmdHistory.Count; i++)
				{
					hashSet.Add(Console.userCmdHistory[i]);
				}
				foreach (KeyValuePair<string, BaseConVar> keyValuePair in console.allConVars)
				{
					hashSet.Add(keyValuePair.Key);
				}
				foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in console.concommandCatalog)
				{
					hashSet.Add(keyValuePair2.Key);
				}
				foreach (string item in hashSet)
				{
					this.searchableStrings.Add(item);
				}
				this.searchableStrings.Sort();
			}

			// Token: 0x060009BD RID: 2493 RVA: 0x0002A868 File Offset: 0x00028A68
			public bool SetSearchString(string newSearchString)
			{
				newSearchString = newSearchString.ToLower(CultureInfo.InvariantCulture);
				if (newSearchString == this.searchString)
				{
					return false;
				}
				this.searchString = newSearchString;
				List<Console.AutoComplete.MatchInfo> list = new List<Console.AutoComplete.MatchInfo>();
				for (int i = 0; i < this.searchableStrings.Count; i++)
				{
					string text = this.searchableStrings[i];
					int num = Math.Min(text.Length, this.searchString.Length);
					int num2 = 0;
					while (num2 < num && char.ToLower(text[num2]) == this.searchString[num2])
					{
						num2++;
					}
					if (num2 > 1)
					{
						list.Add(new Console.AutoComplete.MatchInfo
						{
							str = text,
							similarity = num2
						});
					}
				}
				list.Sort(delegate(Console.AutoComplete.MatchInfo a, Console.AutoComplete.MatchInfo b)
				{
					if (a.similarity == b.similarity)
					{
						return string.CompareOrdinal(a.str, b.str);
					}
					if (a.similarity <= b.similarity)
					{
						return 1;
					}
					return -1;
				});
				this.resultsList = new List<string>();
				for (int j = 0; j < list.Count; j++)
				{
					this.resultsList.Add(list[j].str);
				}
				return true;
			}

			// Token: 0x040009EA RID: 2538
			private List<string> searchableStrings = new List<string>();

			// Token: 0x040009EB RID: 2539
			private string searchString;

			// Token: 0x040009EC RID: 2540
			public List<string> resultsList = new List<string>();

			// Token: 0x020001C4 RID: 452
			private struct MatchInfo
			{
				// Token: 0x040009ED RID: 2541
				public string str;

				// Token: 0x040009EE RID: 2542
				public int similarity;
			}
		}
	}
}
