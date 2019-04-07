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
	// Token: 0x020002AB RID: 683
	public class Console : MonoBehaviour
	{
		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x00044233 File Offset: 0x00042433
		// (set) Token: 0x06000DDE RID: 3550 RVA: 0x0004423A File Offset: 0x0004243A
		public static Console instance { get; private set; }

		// Token: 0x06000DDF RID: 3551 RVA: 0x00044242 File Offset: 0x00042442
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RegisterLogHandler()
		{
			Application.logMessageReceived += Console.HandleLog;
		}

		// Token: 0x06000DE0 RID: 3552 RVA: 0x00044258 File Offset: 0x00042458
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

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000DE1 RID: 3553 RVA: 0x00044304 File Offset: 0x00042504
		// (remove) Token: 0x06000DE2 RID: 3554 RVA: 0x00044338 File Offset: 0x00042538
		public static event Console.LogReceivedDelegate onLogReceived;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000DE3 RID: 3555 RVA: 0x0004436C File Offset: 0x0004256C
		// (remove) Token: 0x06000DE4 RID: 3556 RVA: 0x000443A0 File Offset: 0x000425A0
		public static event Action onClear;

		// Token: 0x06000DE5 RID: 3557 RVA: 0x000443D4 File Offset: 0x000425D4
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

		// Token: 0x06000DE6 RID: 3558 RVA: 0x0004440C File Offset: 0x0004260C
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
					value.SetString(value.defaultValue);
				}
			}
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x0004465C File Offset: 0x0004285C
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

		// Token: 0x06000DE8 RID: 3560 RVA: 0x00044694 File Offset: 0x00042894
		public BaseConVar FindConVar(string name)
		{
			BaseConVar result;
			if (this.allConVars.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x000446B4 File Offset: 0x000428B4
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
						string concommandName = list[0].ToLower();
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

		// Token: 0x06000DEA RID: 3562 RVA: 0x000447B4 File Offset: 0x000429B4
		private void ForwardCmdToServer(ConCommandArgs args)
		{
			if (!args.sender)
			{
				return;
			}
			Console.sendCmdBuilder.Append(args.commandName);
			Console.sendCmdBuilder.Append(" ");
			foreach (string value in args.userArgs)
			{
				Console.sendCmdBuilder.Append("\"");
				Console.sendCmdBuilder.Append(value);
				Console.sendCmdBuilder.Append("\"");
			}
			string cmd = Console.sendCmdBuilder.ToString();
			Console.sendCmdBuilder.Length = 0;
			args.sender.CallCmdSendConsoleCommand(cmd);
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x00044880 File Offset: 0x00042A80
		private void RunCmd(NetworkUser sender, string concommandName, List<string> userArgs)
		{
			bool active = NetworkServer.active;
			Console.ConCommand conCommand;
			if (this.concommandCatalog.TryGetValue(concommandName, out conCommand))
			{
				if (!active && (conCommand.flags & ConVarFlags.ExecuteOnServer) > ConVarFlags.None)
				{
					this.ForwardCmdToServer(new ConCommandArgs
					{
						sender = sender,
						commandName = concommandName,
						userArgs = userArgs
					});
					return;
				}
				if (NetworkServer.active && sender && !sender.isLocalPlayer && (conCommand.flags & ConVarFlags.SenderMustBeServer) != ConVarFlags.None)
				{
					return;
				}
				if ((conCommand.flags & ConVarFlags.Cheat) != ConVarFlags.None && !RoR2Application.cvCheats.boolValue)
				{
					Debug.LogFormat("Command \"{0}\" cannot be used while cheats are disabled.", new object[]
					{
						concommandName
					});
					return;
				}
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
			else
			{
				BaseConVar baseConVar = this.FindConVar(concommandName);
				if (baseConVar == null)
				{
					Debug.LogFormat("\"{0}\" is not a recognized ConCommand or ConVar.", new object[]
					{
						concommandName
					});
					return;
				}
				if (!active && (baseConVar.flags & ConVarFlags.ExecuteOnServer) > ConVarFlags.None)
				{
					this.ForwardCmdToServer(new ConCommandArgs
					{
						sender = sender,
						commandName = concommandName,
						userArgs = userArgs
					});
					return;
				}
				if (NetworkServer.active && sender && !sender.isLocalPlayer && (baseConVar.flags & ConVarFlags.SenderMustBeServer) != ConVarFlags.None)
				{
					return;
				}
				if (userArgs.Count <= 0)
				{
					Debug.LogFormat("\"{0}\" = \"{1}\"\n{2}", new object[]
					{
						concommandName,
						baseConVar.GetString(),
						baseConVar.helpText
					});
					return;
				}
				if ((baseConVar.flags & ConVarFlags.Cheat) != ConVarFlags.None && !RoR2Application.cvCheats.boolValue)
				{
					Debug.LogFormat("Command \"{0}\" cannot be changed while cheats are disabled.", new object[]
					{
						concommandName
					});
					return;
				}
				baseConVar.SetString(userArgs[0]);
				return;
			}
		}

		// Token: 0x06000DEC RID: 3564
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		// Token: 0x06000DED RID: 3565
		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		// Token: 0x06000DEE RID: 3566
		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int processId);

		// Token: 0x06000DEF RID: 3567
		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		// Token: 0x06000DF0 RID: 3568
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x06000DF1 RID: 3569 RVA: 0x00044A80 File Offset: 0x00042C80
		private static string ReadInputStream()
		{
			if (Console.stdInQueue.Count > 0)
			{
				return Console.stdInQueue.Dequeue();
			}
			return null;
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x00044A9C File Offset: 0x00042C9C
		private static void ThreadedInputQueue()
		{
			string item;
			while (Console.systemConsoleType != Console.SystemConsoleType.None && (item = Console.ReadLine()) != null)
			{
				Console.stdInQueue.Enqueue(item);
			}
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x00044AC8 File Offset: 0x00042CC8
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

		// Token: 0x06000DF4 RID: 3572 RVA: 0x00044B84 File Offset: 0x00042D84
		private void Awake()
		{
			Console.instance = this;
			Console.SetupSystemConsole();
			this.InitConVars();
			Type[] types = base.GetType().Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(false);
					for (int k = 0; k < customAttributes.Length; k++)
					{
						ConCommandAttribute conCommandAttribute = ((Attribute)customAttributes[k]) as ConCommandAttribute;
						if (conCommandAttribute != null)
						{
							this.concommandCatalog[conCommandAttribute.commandName.ToLower()] = new Console.ConCommand
							{
								flags = conCommandAttribute.flags,
								action = (Console.ConCommandDelegate)Delegate.CreateDelegate(typeof(Console.ConCommandDelegate), methodInfo),
								helpText = conCommandAttribute.helpText
							};
						}
					}
				}
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int l = 0; l < commandLineArgs.Length; l++)
			{
				Debug.LogFormat("arg[{0}]=\"{1}\"", new object[]
				{
					l,
					commandLineArgs[l]
				});
			}
			MPEventSystemManager.availability.CallWhenAvailable(new Action(this.LoadStartupConfigs));
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x00044CC3 File Offset: 0x00042EC3
		private void LoadStartupConfigs()
		{
			this.SubmitCmd(null, "exec config", false);
			this.SubmitCmd(null, "exec autoexec", false);
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x00044CE0 File Offset: 0x00042EE0
		private void Update()
		{
			string cmd;
			while ((cmd = Console.ReadInputStream()) != null)
			{
				this.SubmitCmd(null, cmd, true);
			}
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x00044D04 File Offset: 0x00042F04
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
					Console.PostMessage(consoleWindow, 256u, 13, 0);
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

		// Token: 0x06000DF8 RID: 3576 RVA: 0x00044D7C File Offset: 0x00042F7C
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

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00044E40 File Offset: 0x00043040
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

		// Token: 0x06000DFA RID: 3578 RVA: 0x00044F84 File Offset: 0x00043184
		[ConCommand(commandName = "set_vstr", flags = ConVarFlags.None, helpText = "Sets the specified vstr to the specified value.")]
		private static void CCSetVstr(ConCommandArgs args)
		{
			args.CheckArgumentCount(2);
			Console.instance.vstrs.Add(args[0], args[1]);
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00044FB0 File Offset: 0x000431B0
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

		// Token: 0x06000DFC RID: 3580 RVA: 0x00044FEA File Offset: 0x000431EA
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

		// Token: 0x06000DFD RID: 3581 RVA: 0x00045010 File Offset: 0x00043210
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

		// Token: 0x06000DFE RID: 3582 RVA: 0x000450DC File Offset: 0x000432DC
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

		// Token: 0x06000DFF RID: 3583 RVA: 0x0004510C File Offset: 0x0004330C
		[ConCommand(commandName = "find", flags = ConVarFlags.None, helpText = "Find all concommands and convars with the specified substring.")]
		private static void CCFind(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				Console.ShowHelpText("find");
				return;
			}
			string text = args[0].ToLower();
			bool flag = text == "*";
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in Console.instance.allConVars)
			{
				if (flag || keyValuePair.Key.ToLower().Contains(text) || keyValuePair.Value.helpText.ToLower().Contains(text))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in Console.instance.concommandCatalog)
			{
				if (flag || keyValuePair2.Key.ToLower().Contains(text) || keyValuePair2.Value.helpText.ToLower().Contains(text))
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

		// Token: 0x06000E00 RID: 3584 RVA: 0x00045298 File Offset: 0x00043498
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

		// Token: 0x06000E01 RID: 3585 RVA: 0x000452B4 File Offset: 0x000434B4
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

		// Token: 0x06000E02 RID: 3586 RVA: 0x0004531D File Offset: 0x0004351D
		public static void ShowHelpText(string commandName)
		{
			Debug.Log(Console.GetHelpText(commandName));
		}

		// Token: 0x040011DC RID: 4572
		public static List<Console.Log> logs = new List<Console.Log>();

		// Token: 0x040011DF RID: 4575
		private Dictionary<string, string> vstrs = new Dictionary<string, string>();

		// Token: 0x040011E0 RID: 4576
		private Dictionary<string, Console.ConCommand> concommandCatalog = new Dictionary<string, Console.ConCommand>();

		// Token: 0x040011E1 RID: 4577
		private Dictionary<string, BaseConVar> allConVars;

		// Token: 0x040011E2 RID: 4578
		private List<BaseConVar> archiveConVars;

		// Token: 0x040011E3 RID: 4579
		public static List<string> userCmdHistory = new List<string>();

		// Token: 0x040011E4 RID: 4580
		private static StringBuilder sendCmdBuilder = new StringBuilder();

		// Token: 0x040011E5 RID: 4581
		private const int VK_RETURN = 13;

		// Token: 0x040011E6 RID: 4582
		private const int WM_KEYDOWN = 256;

		// Token: 0x040011E7 RID: 4583
		private static byte[] inputStreamBuffer = new byte[256];

		// Token: 0x040011E8 RID: 4584
		private static Queue<string> stdInQueue = new Queue<string>();

		// Token: 0x040011E9 RID: 4585
		private static Thread stdInReaderThread = null;

		// Token: 0x040011EA RID: 4586
		private static Console.SystemConsoleType systemConsoleType = Console.SystemConsoleType.None;

		// Token: 0x040011EB RID: 4587
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x040011EC RID: 4588
		private const string configFolder = "/Config/";

		// Token: 0x040011ED RID: 4589
		private const string archiveConVarsPath = "/Config/config.cfg";

		// Token: 0x040011EE RID: 4590
		private static IntConVar maxMessages = new IntConVar("max_messages", ConVarFlags.Archive, "25", "Maximum number of messages that can be held in the console log.");

		// Token: 0x020002AC RID: 684
		public struct Log
		{
			// Token: 0x040011EF RID: 4591
			public string message;

			// Token: 0x040011F0 RID: 4592
			public string stackTrace;

			// Token: 0x040011F1 RID: 4593
			public LogType logType;
		}

		// Token: 0x020002AD RID: 685
		// (Invoke) Token: 0x06000E06 RID: 3590
		public delegate void LogReceivedDelegate(Console.Log log);

		// Token: 0x020002AE RID: 686
		private class Lexer
		{
			// Token: 0x06000E09 RID: 3593 RVA: 0x000453BC File Offset: 0x000435BC
			public Lexer(string srcString)
			{
				this.srcString = srcString;
				this.readIndex = 0;
			}

			// Token: 0x06000E0A RID: 3594 RVA: 0x000453DD File Offset: 0x000435DD
			private static bool IsIgnorableCharacter(char character)
			{
				return !Console.Lexer.IsSeparatorCharacter(character) && !Console.Lexer.IsQuoteCharacter(character) && !Console.Lexer.IsIdentifierCharacter(character) && character != '/';
			}

			// Token: 0x06000E0B RID: 3595 RVA: 0x00045401 File Offset: 0x00043601
			private static bool IsSeparatorCharacter(char character)
			{
				return character == ';' || character == '\n';
			}

			// Token: 0x06000E0C RID: 3596 RVA: 0x0004540F File Offset: 0x0004360F
			private static bool IsQuoteCharacter(char character)
			{
				return character == '\'' || character == '"';
			}

			// Token: 0x06000E0D RID: 3597 RVA: 0x0004541D File Offset: 0x0004361D
			private static bool IsIdentifierCharacter(char character)
			{
				return char.IsLetterOrDigit(character) || character == '_' || character == '.' || character == '-';
			}

			// Token: 0x06000E0E RID: 3598 RVA: 0x00045438 File Offset: 0x00043638
			private bool TrimComment()
			{
				if (this.readIndex >= this.srcString.Length)
				{
					return false;
				}
				if (this.srcString[this.readIndex] == '/' && this.readIndex + 1 < this.srcString.Length)
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
				return false;
			}

			// Token: 0x06000E0F RID: 3599 RVA: 0x00045556 File Offset: 0x00043756
			private void TrimWhitespace()
			{
				while (this.readIndex < this.srcString.Length && Console.Lexer.IsIgnorableCharacter(this.srcString[this.readIndex]))
				{
					this.readIndex++;
				}
			}

			// Token: 0x06000E10 RID: 3600 RVA: 0x00045593 File Offset: 0x00043793
			private void TrimUnused()
			{
				do
				{
					this.TrimWhitespace();
				}
				while (this.TrimComment());
			}

			// Token: 0x06000E11 RID: 3601 RVA: 0x000455A4 File Offset: 0x000437A4
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

			// Token: 0x06000E12 RID: 3602 RVA: 0x000455F8 File Offset: 0x000437F8
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

			// Token: 0x06000E13 RID: 3603 RVA: 0x0004571C File Offset: 0x0004391C
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

			// Token: 0x040011F2 RID: 4594
			private string srcString;

			// Token: 0x040011F3 RID: 4595
			private int readIndex;

			// Token: 0x040011F4 RID: 4596
			private StringBuilder stringBuilder = new StringBuilder();

			// Token: 0x020002AF RID: 687
			private enum TokenType
			{
				// Token: 0x040011F6 RID: 4598
				Identifier,
				// Token: 0x040011F7 RID: 4599
				NestedString
			}
		}

		// Token: 0x020002B0 RID: 688
		private class Substring
		{
			// Token: 0x17000130 RID: 304
			// (get) Token: 0x06000E14 RID: 3604 RVA: 0x00045755 File Offset: 0x00043955
			public int endIndex
			{
				get
				{
					return this.startIndex + this.length;
				}
			}

			// Token: 0x17000131 RID: 305
			// (get) Token: 0x06000E15 RID: 3605 RVA: 0x00045764 File Offset: 0x00043964
			public string str
			{
				get
				{
					return this.srcString.Substring(this.startIndex, this.length);
				}
			}

			// Token: 0x17000132 RID: 306
			// (get) Token: 0x06000E16 RID: 3606 RVA: 0x0004577D File Offset: 0x0004397D
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

			// Token: 0x040011F8 RID: 4600
			public string srcString;

			// Token: 0x040011F9 RID: 4601
			public int startIndex;

			// Token: 0x040011FA RID: 4602
			public int length;
		}

		// Token: 0x020002B1 RID: 689
		private class ConCommand
		{
			// Token: 0x040011FB RID: 4603
			public ConVarFlags flags;

			// Token: 0x040011FC RID: 4604
			public Console.ConCommandDelegate action;

			// Token: 0x040011FD RID: 4605
			public string helpText;
		}

		// Token: 0x020002B2 RID: 690
		// (Invoke) Token: 0x06000E1A RID: 3610
		public delegate void ConCommandDelegate(ConCommandArgs args);

		// Token: 0x020002B3 RID: 691
		private enum SystemConsoleType
		{
			// Token: 0x040011FF RID: 4607
			None,
			// Token: 0x04001200 RID: 4608
			Attach,
			// Token: 0x04001201 RID: 4609
			Alloc
		}

		// Token: 0x020002B4 RID: 692
		public class AutoComplete
		{
			// Token: 0x06000E1D RID: 3613 RVA: 0x000457AC File Offset: 0x000439AC
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

			// Token: 0x06000E1E RID: 3614 RVA: 0x000458EC File Offset: 0x00043AEC
			public bool SetSearchString(string newSearchString)
			{
				newSearchString = newSearchString.ToLower();
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

			// Token: 0x04001202 RID: 4610
			private List<string> searchableStrings = new List<string>();

			// Token: 0x04001203 RID: 4611
			private string searchString;

			// Token: 0x04001204 RID: 4612
			public List<string> resultsList = new List<string>();

			// Token: 0x020002B5 RID: 693
			private struct MatchInfo
			{
				// Token: 0x04001205 RID: 4613
				public string str;

				// Token: 0x04001206 RID: 4614
				public int similarity;
			}
		}
	}
}
