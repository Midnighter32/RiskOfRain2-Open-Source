using System;
using UnityEngine;

// Token: 0x02000075 RID: 117
public static class SingletonHelper
{
	// Token: 0x060001E0 RID: 480 RVA: 0x00009D68 File Offset: 0x00007F68
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

	// Token: 0x060001E1 RID: 481 RVA: 0x00009DB7 File Offset: 0x00007FB7
	public static void Unassign<T>(ref T field, T instance) where T : UnityEngine.Object
	{
		if (field == instance)
		{
			field = default(T);
		}
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00009DD8 File Offset: 0x00007FD8
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

	// Token: 0x060001E3 RID: 483 RVA: 0x00009E14 File Offset: 0x00008014
	public static T Unassign<T>(T existingInstance, T instance) where T : UnityEngine.Object
	{
		if (instance == existingInstance)
		{
			return default(T);
		}
		return existingInstance;
	}
}
