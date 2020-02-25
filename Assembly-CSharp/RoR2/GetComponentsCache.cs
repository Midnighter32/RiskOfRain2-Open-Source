using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000392 RID: 914
	public static class GetComponentsCache<T>
	{
		// Token: 0x06001637 RID: 5687 RVA: 0x0005F8B8 File Offset: 0x0005DAB8
		public static void ReturnBuffer(List<T> buffer)
		{
			buffer.Clear();
			GetComponentsCache<T>.buffers.Push(buffer);
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x0005F8CB File Offset: 0x0005DACB
		private static List<T> RequestBuffer()
		{
			if (GetComponentsCache<T>.buffers.Count == 0)
			{
				return new List<T>();
			}
			return GetComponentsCache<T>.buffers.Pop();
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x0005F8EC File Offset: 0x0005DAEC
		public static List<T> GetGameObjectComponents(GameObject gameObject)
		{
			List<T> list = GetComponentsCache<T>.RequestBuffer();
			gameObject.GetComponents<T>(list);
			return list;
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x0005F908 File Offset: 0x0005DB08
		public static List<T> GetGameObjectComponentsInChildren(GameObject gameObject, bool includeInactive = false)
		{
			List<T> list = GetComponentsCache<T>.RequestBuffer();
			gameObject.GetComponentsInChildren<T>(includeInactive, list);
			return list;
		}

		// Token: 0x040014E7 RID: 5351
		private static readonly Stack<List<T>> buffers = new Stack<List<T>>();
	}
}
