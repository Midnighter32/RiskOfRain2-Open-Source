using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BB RID: 1467
	public class CarouselNavigationController : MonoBehaviour
	{
		// Token: 0x060020D5 RID: 8405 RVA: 0x0009A347 File Offset: 0x00098547
		private void Awake()
		{
			this.buttonAllocator = new UIElementAllocator<MPButton>(this.container, this.buttonPrefab);
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x0009A360 File Offset: 0x00098560
		private void Start()
		{
			if (this.leftButton)
			{
				this.leftButton.onClick.AddListener(new UnityAction(this.NavigatePreviousPage));
			}
			if (this.rightButton)
			{
				this.rightButton.onClick.AddListener(new UnityAction(this.NavigateNextPage));
			}
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x0009A3BF File Offset: 0x000985BF
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x0009A3C7 File Offset: 0x000985C7
		public void SetDisplayData(CarouselNavigationController.DisplayData newDisplayData)
		{
			if (newDisplayData.Equals(this.displayData))
			{
				return;
			}
			this.displayData = newDisplayData;
			this.Rebuild();
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x0009A3E8 File Offset: 0x000985E8
		private void Rebuild()
		{
			this.buttonAllocator.AllocateElements(this.displayData.pageCount);
			int i = 0;
			int count = this.buttonAllocator.elements.Count;
			while (i < count)
			{
				MPButton mpbutton = this.buttonAllocator.elements[i];
				ColorBlock colors = mpbutton.colors;
				colors.colorMultiplier = 1f;
				mpbutton.colors = colors;
				mpbutton.onClick.RemoveAllListeners();
				CarouselNavigationController.DisplayData buttonDisplayData = new CarouselNavigationController.DisplayData(this.displayData.pageCount, i);
				mpbutton.onClick.AddListener(delegate()
				{
					this.SetDisplayData(buttonDisplayData);
					Action<int> action = this.onPageChangeSubmitted;
					if (action == null)
					{
						return;
					}
					action(this.displayData.pageIndex);
				});
				i++;
			}
			if (this.displayData.pageIndex >= 0 && this.displayData.pageIndex < this.displayData.pageCount)
			{
				MPButton mpbutton2 = this.buttonAllocator.elements[this.displayData.pageIndex];
				ColorBlock colors2 = mpbutton2.colors;
				colors2.colorMultiplier = 2f;
				mpbutton2.colors = colors2;
			}
			bool flag = this.displayData.pageCount > 1;
			bool interactable = flag && (this.allowLooping || this.displayData.pageIndex > 0);
			bool interactable2 = flag && (this.allowLooping || this.displayData.pageIndex < this.displayData.pageCount - 1);
			if (this.leftButton)
			{
				this.leftButton.gameObject.SetActive(flag);
				this.leftButton.interactable = interactable;
			}
			if (this.rightButton)
			{
				this.rightButton.gameObject.SetActive(flag);
				this.rightButton.interactable = interactable2;
			}
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x0009A5AC File Offset: 0x000987AC
		public void NavigateNextPage()
		{
			if (this.displayData.pageCount <= 0)
			{
				return;
			}
			int num = this.displayData.pageIndex + 1;
			bool flag = num >= this.displayData.pageCount;
			if (flag)
			{
				if (!this.allowLooping)
				{
					return;
				}
				num = 0;
			}
			Debug.LogFormat("NavigateNextPage() desiredPageIndex={0} overflow={1}", new object[]
			{
				num,
				flag
			});
			this.SetDisplayData(new CarouselNavigationController.DisplayData(this.displayData.pageCount, num));
			Action<int> action = this.onPageChangeSubmitted;
			if (action != null)
			{
				action(num);
			}
			if (flag)
			{
				Action action2 = this.onOverflow;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x0009A658 File Offset: 0x00098858
		public void NavigatePreviousPage()
		{
			if (this.displayData.pageCount <= 0)
			{
				return;
			}
			int num = this.displayData.pageIndex - 1;
			bool flag = num < 0;
			if (flag)
			{
				if (!this.allowLooping)
				{
					return;
				}
				num = this.displayData.pageCount - 1;
			}
			Debug.LogFormat("NavigatePreviousPage() desiredPageIndex={0} underflow={1}", new object[]
			{
				num,
				flag
			});
			this.SetDisplayData(new CarouselNavigationController.DisplayData(this.displayData.pageCount, num));
			Action<int> action = this.onPageChangeSubmitted;
			if (action != null)
			{
				action(num);
			}
			if (flag)
			{
				Action action2 = this.onUnderflow;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x060020DC RID: 8412 RVA: 0x0009A704 File Offset: 0x00098904
		// (remove) Token: 0x060020DD RID: 8413 RVA: 0x0009A73C File Offset: 0x0009893C
		public event Action<int> onPageChangeSubmitted;

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x060020DE RID: 8414 RVA: 0x0009A774 File Offset: 0x00098974
		// (remove) Token: 0x060020DF RID: 8415 RVA: 0x0009A7AC File Offset: 0x000989AC
		public event Action onUnderflow;

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x060020E0 RID: 8416 RVA: 0x0009A7E4 File Offset: 0x000989E4
		// (remove) Token: 0x060020E1 RID: 8417 RVA: 0x0009A81C File Offset: 0x00098A1C
		public event Action onOverflow;

		// Token: 0x0400235F RID: 9055
		public GameObject buttonPrefab;

		// Token: 0x04002360 RID: 9056
		public RectTransform container;

		// Token: 0x04002361 RID: 9057
		public MPButton leftButton;

		// Token: 0x04002362 RID: 9058
		public MPButton rightButton;

		// Token: 0x04002363 RID: 9059
		public bool allowLooping;

		// Token: 0x04002364 RID: 9060
		public UIElementAllocator<MPButton> buttonAllocator;

		// Token: 0x04002365 RID: 9061
		private int currentPageIndex;

		// Token: 0x04002366 RID: 9062
		private CarouselNavigationController.DisplayData displayData = new CarouselNavigationController.DisplayData(0, 0);

		// Token: 0x020005BC RID: 1468
		public struct DisplayData : IEquatable<CarouselNavigationController.DisplayData>
		{
			// Token: 0x060020E3 RID: 8419 RVA: 0x0009A866 File Offset: 0x00098A66
			public DisplayData(int pageCount, int pageIndex)
			{
				this.pageCount = pageCount;
				this.pageIndex = pageIndex;
			}

			// Token: 0x060020E4 RID: 8420 RVA: 0x0009A876 File Offset: 0x00098A76
			public bool Equals(CarouselNavigationController.DisplayData other)
			{
				return this.pageCount == other.pageCount && this.pageIndex == other.pageIndex;
			}

			// Token: 0x060020E5 RID: 8421 RVA: 0x0009A898 File Offset: 0x00098A98
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is CarouselNavigationController.DisplayData)
				{
					CarouselNavigationController.DisplayData other = (CarouselNavigationController.DisplayData)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x060020E6 RID: 8422 RVA: 0x0009A8C4 File Offset: 0x00098AC4
			public override int GetHashCode()
			{
				return this.pageCount * 397 ^ this.pageIndex;
			}

			// Token: 0x0400236A RID: 9066
			public readonly int pageCount;

			// Token: 0x0400236B RID: 9067
			public readonly int pageIndex;
		}
	}
}
