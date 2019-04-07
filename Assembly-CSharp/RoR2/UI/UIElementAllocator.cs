using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200065C RID: 1628
	public class UIElementAllocator<T> where T : Component
	{
		// Token: 0x06002462 RID: 9314 RVA: 0x000AAB28 File Offset: 0x000A8D28
		public UIElementAllocator([NotNull] RectTransform containerTransform, [NotNull] GameObject elementPrefab)
		{
			this.containerTransform = containerTransform;
			this.elementPrefab = elementPrefab;
			this.elementControllerComponentsList = new List<T>();
			this.elements = new ReadOnlyCollection<T>(this.elementControllerComponentsList);
		}

		// Token: 0x06002463 RID: 9315 RVA: 0x000AAB5C File Offset: 0x000A8D5C
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

		// Token: 0x04002761 RID: 10081
		public readonly RectTransform containerTransform;

		// Token: 0x04002762 RID: 10082
		public readonly GameObject elementPrefab;

		// Token: 0x04002763 RID: 10083
		[NotNull]
		private List<T> elementControllerComponentsList;

		// Token: 0x04002764 RID: 10084
		[NotNull]
		public readonly ReadOnlyCollection<T> elements;

		// Token: 0x04002765 RID: 10085
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onCreateElement;

		// Token: 0x04002766 RID: 10086
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onDestroyElement;

		// Token: 0x0200065D RID: 1629
		// (Invoke) Token: 0x06002465 RID: 9317
		public delegate void ElementOperationDelegate(int index, T element);
	}
}
