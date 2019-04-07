using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200046F RID: 1135
	public class ProjectIssueChecker
	{
		// Token: 0x06001955 RID: 6485 RVA: 0x0007938A File Offset: 0x0007758A
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

		// Token: 0x06001956 RID: 6486 RVA: 0x00079394 File Offset: 0x00077594
		private ProjectIssueChecker()
		{
			this.assetCheckMethods = new Dictionary<Type, List<MethodInfo>>();
			this.allChecks = new List<MethodInfo>();
			this.enabledChecks = new Dictionary<MethodInfo, bool>();
			Assembly assembly = typeof(RoR2Application).Assembly;
			ProjectIssueChecker.<>c__DisplayClass7_0 CS$<>8__locals1;
			CS$<>8__locals1.types = assembly.GetTypes();
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
							this.<.ctor>g__AddMethodForTypeDescending|7_1(assetType, methodInfo, ref CS$<>8__locals1);
							this.allChecks.Add(methodInfo);
							this.enabledChecks.Add(methodInfo, true);
						}
					}
				}
			}
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x000794C8 File Offset: 0x000776C8
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

		// Token: 0x06001958 RID: 6488 RVA: 0x00079578 File Offset: 0x00077778
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

		// Token: 0x06001959 RID: 6489 RVA: 0x000795C0 File Offset: 0x000777C0
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

		// Token: 0x0600195A RID: 6490 RVA: 0x00079608 File Offset: 0x00077808
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

		// Token: 0x0600195B RID: 6491 RVA: 0x00079654 File Offset: 0x00077854
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

		// Token: 0x0600195C RID: 6492 RVA: 0x000796A0 File Offset: 0x000778A0
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

		// Token: 0x04001CD0 RID: 7376
		private Dictionary<Type, List<MethodInfo>> assetCheckMethods;

		// Token: 0x04001CD1 RID: 7377
		private List<MethodInfo> allChecks;

		// Token: 0x04001CD2 RID: 7378
		private Dictionary<MethodInfo, bool> enabledChecks;

		// Token: 0x04001CD3 RID: 7379
		private bool checkScenes = true;

		// Token: 0x04001CD4 RID: 7380
		private List<string> scenesToCheck = new List<string>();

		// Token: 0x04001CD5 RID: 7381
		private string currentAssetPath = "";

		// Token: 0x04001CD6 RID: 7382
		private readonly Stack<UnityEngine.Object> assetStack = new Stack<UnityEngine.Object>();

		// Token: 0x04001CD7 RID: 7383
		private UnityEngine.Object currentAsset;

		// Token: 0x04001CD8 RID: 7384
		private List<ProjectIssueChecker.LogMessage> log = new List<ProjectIssueChecker.LogMessage>();

		// Token: 0x04001CD9 RID: 7385
		private string currentSceneName = "";

		// Token: 0x02000470 RID: 1136
		// (Invoke) Token: 0x06001960 RID: 6496
		private delegate void ObjectCheckDelegate(ProjectIssueChecker issueChecker, UnityEngine.Object obj);

		// Token: 0x02000471 RID: 1137
		private struct LogMessage
		{
			// Token: 0x04001CDA RID: 7386
			public bool error;

			// Token: 0x04001CDB RID: 7387
			public string message;

			// Token: 0x04001CDC RID: 7388
			public UnityEngine.Object context;

			// Token: 0x04001CDD RID: 7389
			public string assetPath;
		}
	}
}
