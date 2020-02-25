using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EE RID: 1006
	public class ProjectIssueChecker
	{
		// Token: 0x06001870 RID: 6256 RVA: 0x00069525 File Offset: 0x00067725
		private static IEnumerable<Assembly> GetAssemblies()
		{
			List<string> list = new List<string>();
			Stack<Assembly> stack = new Stack<Assembly>();
			stack.Push(Assembly.GetEntryAssembly());
			do
			{
				Assembly asm = stack.Pop();
				yield return asm;
				foreach (AssemblyName assemblyName in asm.GetReferencedAssemblies())
				{
					if (!list.Contains(assemblyName.FullName))
					{
						stack.Push(Assembly.Load(assemblyName));
						list.Add(assemblyName.FullName);
					}
				}
				asm = null;
			}
			while (stack.Count > 0);
			yield break;
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x00069530 File Offset: 0x00067730
		private ProjectIssueChecker()
		{
			this.assetCheckMethods = new Dictionary<Type, List<MethodInfo>>();
			this.allChecks = new List<MethodInfo>();
			this.enabledChecks = new Dictionary<MethodInfo, bool>();
			Assembly[] source = new Assembly[]
			{
				typeof(GameObject).Assembly,
				typeof(Canvas).Assembly,
				typeof(RoR2Application).Assembly,
				typeof(TMP_Text).Assembly
			};
			ProjectIssueChecker.<>c__DisplayClass6_0 CS$<>8__locals1;
			CS$<>8__locals1.types = source.SelectMany((Assembly a) => a.GetTypes()).ToArray<Type>();
			Type[] types = CS$<>8__locals1.types;
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					foreach (object obj in methodInfo.GetCustomAttributes(true))
					{
						if (obj is AssetCheckAttribute)
						{
							Type assetType = ((AssetCheckAttribute)obj).assetType;
							this.<.ctor>g__AddMethodForTypeDescending|6_1(assetType, methodInfo, ref CS$<>8__locals1);
							this.allChecks.Add(methodInfo);
							this.enabledChecks.Add(methodInfo, true);
						}
					}
				}
			}
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x000696C8 File Offset: 0x000678C8
		private string GetCurrentAssetFullPath()
		{
			GameObject gameObject = null;
			string arg = "";
			if (this.currentAsset is GameObject)
			{
				gameObject = (GameObject)this.currentAsset;
			}
			else if (this.currentAsset is Component)
			{
				gameObject = ((Component)this.currentAsset).gameObject;
			}
			string arg2 = this.currentAsset ? this.currentAsset.name : "NULL ASSET";
			if (gameObject)
			{
				arg2 = Util.GetGameObjectHierarchyName(gameObject);
			}
			string arg3 = this.currentAsset ? this.currentAsset.GetType().Name : "VOID";
			return string.Format("{0}:{1}({2})", arg, arg2, arg3);
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x00069778 File Offset: 0x00067978
		public void Log(string message, UnityEngine.Object context = null)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = false,
				message = message,
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x000697C0 File Offset: 0x000679C0
		public void LogError(string message, UnityEngine.Object context = null)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = true,
				message = message,
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x00069808 File Offset: 0x00067A08
		public void LogFormat(UnityEngine.Object context, string format, params object[] args)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = false,
				message = string.Format(format, args),
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x00069854 File Offset: 0x00067A54
		public void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = true,
				message = string.Format(format, args),
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x000698A0 File Offset: 0x00067AA0
		private void FlushLog()
		{
			bool flag = false;
			for (int i = 0; i < this.log.Count; i++)
			{
				if (this.log[i].error)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				foreach (ProjectIssueChecker.LogMessage logMessage in this.log)
				{
					if (logMessage.error)
					{
						Debug.LogErrorFormat(logMessage.context, "[\"{0}\"] {1}", new object[]
						{
							logMessage.assetPath,
							logMessage.message
						});
					}
					else
					{
						Debug.LogFormat(logMessage.context, "[\"{0}\"] {1}", new object[]
						{
							logMessage.assetPath,
							logMessage.message
						});
					}
				}
			}
			this.log.Clear();
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x00069988 File Offset: 0x00067B88
		[CompilerGenerated]
		private void <.ctor>g__AddMethodForType|6_0(Type t, MethodInfo methodInfo)
		{
			List<MethodInfo> list = null;
			this.assetCheckMethods.TryGetValue(t, out list);
			if (list == null)
			{
				list = new List<MethodInfo>();
				this.assetCheckMethods[t] = list;
			}
			list.Add(methodInfo);
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x000699C4 File Offset: 0x00067BC4
		[CompilerGenerated]
		private void <.ctor>g__AddMethodForTypeDescending|6_1(Type t, MethodInfo methodInfo, ref ProjectIssueChecker.<>c__DisplayClass6_0 A_3)
		{
			foreach (Type t2 in A_3.types.Where(new Func<Type, bool>(t.IsAssignableFrom)))
			{
				this.<.ctor>g__AddMethodForType|6_0(t2, methodInfo);
			}
		}

		// Token: 0x040016EE RID: 5870
		private Dictionary<Type, List<MethodInfo>> assetCheckMethods;

		// Token: 0x040016EF RID: 5871
		private List<MethodInfo> allChecks;

		// Token: 0x040016F0 RID: 5872
		private Dictionary<MethodInfo, bool> enabledChecks;

		// Token: 0x040016F1 RID: 5873
		private bool checkScenes = true;

		// Token: 0x040016F2 RID: 5874
		private List<string> scenesToCheck = new List<string>();

		// Token: 0x040016F3 RID: 5875
		private string currentAssetPath = "";

		// Token: 0x040016F4 RID: 5876
		private readonly Stack<UnityEngine.Object> assetStack = new Stack<UnityEngine.Object>();

		// Token: 0x040016F5 RID: 5877
		private UnityEngine.Object currentAsset;

		// Token: 0x040016F6 RID: 5878
		private List<ProjectIssueChecker.LogMessage> log = new List<ProjectIssueChecker.LogMessage>();

		// Token: 0x040016F7 RID: 5879
		private string currentSceneName = "";

		// Token: 0x020003EF RID: 1007
		private struct LogMessage
		{
			// Token: 0x040016F8 RID: 5880
			public bool error;

			// Token: 0x040016F9 RID: 5881
			public string message;

			// Token: 0x040016FA RID: 5882
			public UnityEngine.Object context;

			// Token: 0x040016FB RID: 5883
			public string assetPath;
		}
	}
}
