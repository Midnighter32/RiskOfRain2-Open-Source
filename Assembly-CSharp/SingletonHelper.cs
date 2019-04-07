using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public static class SingletonHelper
{
	// Token: 0x060001B2 RID: 434 RVA: 0x0000945C File Offset: 0x0000765C
	public static void Assign<T>(ref T field, T instance) where T : UnityEngine.Object
	{
		if (!field)
		{
			field = instance;
			return;
		}
		Debug.LogErrorFormat(instance, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
		{
			typeof(T).Name
		});
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x000094AB File Offset: 0x000076AB
	public static void Unassign<T>(ref T field, T instance) where T : UnityEngine.Object
	{
		if (field == instance)
		{
			field = default(T);
		}
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x000094CC File Offset: 0x000076CC
	public static T Assign<T>(T existingInstance, T instance) where T : UnityEngine.Object
	{
		if (!existingInstance)
		{
			return instance;
		}
		Debug.LogErrorFormat(instance, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
		{
			typeof(T).Name
		});
		return existingInstance;
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00009508 File Offset: 0x00007708
	public static T Unassign<T>(T existingInstance, T instance) where T : UnityEngine.Object
	{
		if (instance == existingInstance)
		{
			return default(T);
		}
		return existingInstance;
	}
}
