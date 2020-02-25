using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000595 RID: 1429
	public class CarouselNavigationController : MonoBehaviour
	{
		// Token: 0x060021F7 RID: 8695 RVA: 0x00092B0B File Offset: 0x00090D0B
		private void Awake()
		{
			this.buttonAllocator = new UIElementAllocator<MPButton>(this.container, this.buttonPrefab);
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x00092B24 File Offset: 0x00090D24
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

		// Token: 0x060021F9 RID: 8697 RVA: 0x00092B83 File Offset: 0x00090D83
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x00092B8B File Offset: 0x00090D8B
		public void SetDisplayData(CarouselNavigationController.DisplayData newDisplayData)
		{
			if (newDisplayData.Equals(this.displayData))
			{
				return;
			}
			this.displayData = newDisplayData;
			this.Rebuild();
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x00092BAC File Offset: 0x00090DAC
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

		// Token: 0x060021FC RID: 8700 RVA: 0x00092D70 File Offset: 0x00090F70
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

		// Token: 0x060021FD RID: 8701 RVA: 0x00092E1C File Offset: 0x0009101C
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

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x060021FE RID: 8702 RVA: 0x00092EC8 File Offset: 0x000910C8
		// (remove) Token: 0x060021FF RID: 8703 RVA: 0x00092F00 File Offset: 0x00091100
		public event Action<int> onPageChangeSubmitted;

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x06002200 RID: 8704 RVA: 0x00092F38 File Offset: 0x00091138
		// (remove) Token: 0x06002201 RID: 8705 RVA: 0x00092F70 File Offset: 0x00091170
		public event Action onUnderflow;

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06002202 RID: 8706 RVA: 0x00092FA8 File Offset: 0x000911A8
		// (remove) Token: 0x06002203 RID: 8707 RVA: 0x00092FE0 File Offset: 0x000911E0
		public event Action onOverflow;

		// Token: 0x04001F55 RID: 8021
		public GameObject buttonPrefab;

		// Token: 0x04001F56 RID: 8022
		public RectTransform container;

		// Token: 0x04001F57 RID: 8023
		public MPButton leftButton;

		// Token: 0x04001F58 RID: 8024
		public MPButton rightButton;

		// Token: 0x04001F59 RID: 8025
		public bool allowLooping;

		// Token: 0x04001F5A RID: 8026
		public UIElementAllocator<MPButton> buttonAllocator;

		// Token: 0x04001F5B RID: 8027
		private int currentPageIndex;

		// Token: 0x04001F5C RID: 8028
		private CarouselNavigationController.DisplayData displayData = new CarouselNavigationController.DisplayData(0, 0);

		// Token: 0x02000596 RID: 1430
		public struct DisplayData : IEquatable<CarouselNavigationController.DisplayData>
		{
			// Token: 0x06002205 RID: 8709 RVA: 0x0009302A File Offset: 0x0009122A
			public DisplayData(int pageCount, int pageIndex)
			{
				this.pageCount = pageCount;
				this.pageIndex = pageIndex;
			}

			// Token: 0x06002206 RID: 8710 RVA: 0x0009303A File Offset: 0x0009123A
			public bool Equals(CarouselNavigationController.DisplayData other)
			{
				return this.pageCount == other.pageCount && this.pageIndex == other.pageIndex;
			}

			// Token: 0x06002207 RID: 8711 RVA: 0x0009305C File Offset: 0x0009125C
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

			// Token: 0x06002208 RID: 8712 RVA: 0x00093088 File Offset: 0x00091288
			public override int GetHashCode()
			{
				return this.pageCount * 397 ^ this.pageIndex;
			}

			// Token: 0x04001F60 RID: 8032
			public readonly int pageCount;

			// Token: 0x04001F61 RID: 8033
			public readonly int pageIndex;
		}
	}
}
