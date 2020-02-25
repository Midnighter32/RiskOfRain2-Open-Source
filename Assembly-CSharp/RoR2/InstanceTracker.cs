using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AA RID: 938
	public static class InstanceTracker
	{
		// Token: 0x060016BF RID: 5823 RVA: 0x00061B41 File Offset: 0x0005FD41
		public static void Add<T>([NotNull] T instance) where T : MonoBehaviour
		{
			InstanceTracker.TypeData<T>.Add(instance);
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x00061B49 File Offset: 0x0005FD49
		public static void Remove<T>([NotNull] T instance) where T : MonoBehaviour
		{
			InstanceTracker.TypeData<T>.Remove(instance);
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00061B51 File Offset: 0x0005FD51
		[NotNull]
		public static List<T> GetInstancesList<T>() where T : MonoBehaviour
		{
			return InstanceTracker.TypeData<T>.instancesList;
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00061B58 File Offset: 0x0005FD58
		public static T FirstOrNull<T>() where T : MonoBehaviour
		{
			if (InstanceTracker.TypeData<T>.instancesList.Count == 0)
			{
				return default(T);
			}
			return InstanceTracker.TypeData<T>.instancesList[0];
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x00061B88 File Offset: 0x0005FD88
		[NotNull]
		public static IEnumerable<MonoBehaviour> FindInstancesEnumerable([NotNull] Type t)
		{
			IEnumerable<MonoBehaviour> result;
			if (!InstanceTracker.instancesLists.TryGetValue(t, out result))
			{
				return Enumerable.Empty<MonoBehaviour>();
			}
			return result;
		}

		// Token: 0x0400154B RID: 5451
		private static readonly Dictionary<Type, IEnumerable<MonoBehaviour>> instancesLists = new Dictionary<Type, IEnumerable<MonoBehaviour>>();

		// Token: 0x020003AB RID: 939
		private static class TypeData<T> where T : MonoBehaviour
		{
			// Token: 0x060016C5 RID: 5829 RVA: 0x00061BB7 File Offset: 0x0005FDB7
			static TypeData()
			{
				InstanceTracker.instancesLists[typeof(T)] = InstanceTracker.TypeData<T>.instancesList;
			}

			// Token: 0x060016C6 RID: 5830 RVA: 0x00061BDC File Offset: 0x0005FDDC
			public static void Add(T instance)
			{
				InstanceTracker.TypeData<T>.instancesList.Add(instance);
			}

			// Token: 0x060016C7 RID: 5831 RVA: 0x00061BE9 File Offset: 0x0005FDE9
			public static void Remove(T instance)
			{
				InstanceTracker.TypeData<T>.instancesList.Remove(instance);
			}

			// Token: 0x0400154C RID: 5452
			public static readonly List<T> instancesList = new List<T>();
		}
	}
}
