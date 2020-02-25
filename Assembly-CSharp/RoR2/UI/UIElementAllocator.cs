using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000651 RID: 1617
	public class UIElementAllocator<T> where T : Component
	{
		// Token: 0x06002606 RID: 9734 RVA: 0x000A5664 File Offset: 0x000A3864
		public UIElementAllocator([NotNull] RectTransform containerTransform, [NotNull] GameObject elementPrefab)
		{
			this.containerTransform = containerTransform;
			this.elementPrefab = elementPrefab;
			this.elementControllerComponentsList = new List<T>();
			this.elements = new ReadOnlyCollection<T>(this.elementControllerComponentsList);
		}

		// Token: 0x06002607 RID: 9735 RVA: 0x000A5698 File Offset: 0x000A3898
		public void AllocateElements(int desiredCount)
		{
			if (desiredCount < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = this.elementControllerComponentsList.Count - 1; i >= desiredCount; i--)
			{
				T t = this.elementControllerComponentsList[i];
				UIElementAllocator<T>.ElementOperationDelegate elementOperationDelegate = this.onDestroyElement;
				if (elementOperationDelegate != null)
				{
					elementOperationDelegate(i, t);
				}
				UnityEngine.Object.Destroy(t.gameObject);
				this.elementControllerComponentsList.RemoveAt(i);
			}
			for (int j = this.elementControllerComponentsList.Count; j < desiredCount; j++)
			{
				T component = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab, this.containerTransform).GetComponent<T>();
				this.elementControllerComponentsList.Add(component);
				component.gameObject.SetActive(true);
				UIElementAllocator<T>.ElementOperationDelegate elementOperationDelegate2 = this.onCreateElement;
				if (elementOperationDelegate2 != null)
				{
					elementOperationDelegate2(j, component);
				}
			}
		}

		// Token: 0x040023C8 RID: 9160
		public readonly RectTransform containerTransform;

		// Token: 0x040023C9 RID: 9161
		public readonly GameObject elementPrefab;

		// Token: 0x040023CA RID: 9162
		[NotNull]
		private List<T> elementControllerComponentsList;

		// Token: 0x040023CB RID: 9163
		[NotNull]
		public readonly ReadOnlyCollection<T> elements;

		// Token: 0x040023CC RID: 9164
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onCreateElement;

		// Token: 0x040023CD RID: 9165
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onDestroyElement;

		// Token: 0x02000652 RID: 1618
		// (Invoke) Token: 0x06002609 RID: 9737
		public delegate void ElementOperationDelegate(int index, T element);
	}
}
