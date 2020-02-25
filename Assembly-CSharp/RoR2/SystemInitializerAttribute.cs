using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000453 RID: 1107
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[MeansImplicitUse]
	public class SystemInitializerAttribute : Attribute
	{
		// Token: 0x06001AE9 RID: 6889 RVA: 0x0007233E File Offset: 0x0007053E
		public SystemInitializerAttribute(params Type[] dependencies)
		{
			if (dependencies != null)
			{
				this.dependencies = dependencies;
			}
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x0007235C File Offset: 0x0007055C
		public static void Execute()
		{
			Queue<SystemInitializerAttribute> queue = new Queue<SystemInitializerAttribute>();
			foreach (Type type in typeof(SystemInitializerAttribute).Assembly.GetTypes())
			{
				foreach (MethodInfo element in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					SystemInitializerAttribute customAttribute = element.GetCustomAttribute<SystemInitializerAttribute>();
					if (customAttribute != null)
					{
						queue.Enqueue(customAttribute);
						customAttribute.methodInfo = element;
						customAttribute.associatedType = type;
					}
				}
			}
			SystemInitializerAttribute.<>c__DisplayClass4_0 CS$<>8__locals1;
			CS$<>8__locals1.initializedTypes = new HashSet<Type>();
			int num = 0;
			while (queue.Count > 0)
			{
				SystemInitializerAttribute systemInitializerAttribute = queue.Dequeue();
				if (!SystemInitializerAttribute.<Execute>g__InitializerDependenciesMet|4_0(systemInitializerAttribute, ref CS$<>8__locals1))
				{
					queue.Enqueue(systemInitializerAttribute);
					num++;
					if (num >= queue.Count)
					{
						Debug.LogFormat("SystemInitializerAttribute infinite loop detected. currentMethod={0}", new object[]
						{
							systemInitializerAttribute.associatedType.FullName + systemInitializerAttribute.methodInfo.Name
						});
						return;
					}
				}
				else
				{
					Debug.Log("Initializing system: " + systemInitializerAttribute.associatedType.Name);
					systemInitializerAttribute.methodInfo.Invoke(null, Array.Empty<object>());
					CS$<>8__locals1.initializedTypes.Add(systemInitializerAttribute.associatedType);
					num = 0;
				}
			}
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x000724A4 File Offset: 0x000706A4
		[CompilerGenerated]
		internal static bool <Execute>g__InitializerDependenciesMet|4_0(SystemInitializerAttribute initializerAttribute, ref SystemInitializerAttribute.<>c__DisplayClass4_0 A_1)
		{
			foreach (Type item in initializerAttribute.dependencies)
			{
				if (!A_1.initializedTypes.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400186C RID: 6252
		public Type[] dependencies = Array.Empty<Type>();

		// Token: 0x0400186D RID: 6253
		private MethodInfo methodInfo;

		// Token: 0x0400186E RID: 6254
		private Type associatedType;
	}
}
