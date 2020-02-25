using System;

namespace RoR2
{
	// Token: 0x0200039F RID: 927
	public class IndexAllocator
	{
		// Token: 0x06001674 RID: 5748 RVA: 0x000609AD File Offset: 0x0005EBAD
		public IndexAllocator()
		{
			this.ranges = new IndexAllocator.Range[16];
			this.ranges[0] = new IndexAllocator.Range(0, int.MaxValue);
			this.rangeCount = 1;
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x000609E0 File Offset: 0x0005EBE0
		public int RequestIndex()
		{
			int result = this.ranges[0].TakeIndex();
			if (this.ranges[0].empty)
			{
				this.RemoveAt(0);
			}
			return result;
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x00060A0D File Offset: 0x0005EC0D
		private void RemoveAt(int i)
		{
			HGArrayUtilities.ArrayRemoveAt<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, 1);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x00060A22 File Offset: 0x0005EC22
		private void InsertAt(int i, IndexAllocator.Range range)
		{
			HGArrayUtilities.ArrayInsert<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, ref range);
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00060A38 File Offset: 0x0005EC38
		public void FreeIndex(int index)
		{
			if (index < this.ranges[0].startIndex)
			{
				if (this.ranges[0].TryExtending(index))
				{
					return;
				}
				this.InsertAt(0, new IndexAllocator.Range(index, index + 1));
				return;
			}
			else
			{
				if (this.ranges[this.rangeCount - 1].endIndex > index)
				{
					int i = 1;
					while (i < this.rangeCount)
					{
						int endIndex = this.ranges[i - 1].endIndex;
						int startIndex = this.ranges[i].startIndex;
						if (endIndex <= index && index < startIndex)
						{
							bool flag = index == endIndex;
							bool flag2 = index == startIndex - 1;
							if (flag ^ flag2)
							{
								if (flag)
								{
									IndexAllocator.Range[] array = this.ranges;
									int num = i - 1;
									array[num].endIndex = array[num].endIndex + 1;
									return;
								}
								IndexAllocator.Range[] array2 = this.ranges;
								int num2 = i;
								array2[num2].startIndex = array2[num2].startIndex - 1;
								return;
							}
							else
							{
								if (flag)
								{
									this.ranges[i - 1].endIndex = this.ranges[i].endIndex;
									this.RemoveAt(i);
									return;
								}
								this.InsertAt(i, new IndexAllocator.Range(index, index + 1));
								return;
							}
						}
						else
						{
							i++;
						}
					}
					return;
				}
				if (this.ranges[this.rangeCount - 1].TryExtending(index))
				{
					return;
				}
				this.InsertAt(this.rangeCount, new IndexAllocator.Range(index, index + 1));
				return;
			}
		}

		// Token: 0x04001525 RID: 5413
		private IndexAllocator.Range[] ranges;

		// Token: 0x04001526 RID: 5414
		private int rangeCount;

		// Token: 0x020003A0 RID: 928
		private struct Range
		{
			// Token: 0x06001679 RID: 5753 RVA: 0x00060B9D File Offset: 0x0005ED9D
			public Range(int startIndex, int endIndex)
			{
				this.startIndex = startIndex;
				this.endIndex = endIndex;
			}

			// Token: 0x0600167A RID: 5754 RVA: 0x00060BB0 File Offset: 0x0005EDB0
			public int TakeIndex()
			{
				int num = this.startIndex;
				this.startIndex = num + 1;
				return num;
			}

			// Token: 0x0600167B RID: 5755 RVA: 0x00060BCE File Offset: 0x0005EDCE
			public bool TryExtending(int index)
			{
				if (index == this.startIndex - 1)
				{
					this.startIndex--;
					return true;
				}
				if (index == this.endIndex)
				{
					this.endIndex++;
					return true;
				}
				return false;
			}

			// Token: 0x1700029B RID: 667
			// (get) Token: 0x0600167C RID: 5756 RVA: 0x00060C05 File Offset: 0x0005EE05
			public bool empty
			{
				get
				{
					return this.startIndex == this.endIndex;
				}
			}

			// Token: 0x1700029C RID: 668
			// (get) Token: 0x0600167D RID: 5757 RVA: 0x00060C15 File Offset: 0x0005EE15
			public int size
			{
				get
				{
					return this.endIndex - this.startIndex;
				}
			}

			// Token: 0x04001527 RID: 5415
			public int startIndex;

			// Token: 0x04001528 RID: 5416
			public int endIndex;
		}
	}
}
