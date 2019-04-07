using System;

namespace RoR2
{
	// Token: 0x0200043C RID: 1084
	public class IndexAllocator
	{
		// Token: 0x06001812 RID: 6162 RVA: 0x00072EF1 File Offset: 0x000710F1
		public IndexAllocator()
		{
			this.ranges = new IndexAllocator.Range[16];
			this.ranges[0] = new IndexAllocator.Range(0, int.MaxValue);
			this.rangeCount = 1;
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x00072F24 File Offset: 0x00071124
		public int RequestIndex()
		{
			int result = this.ranges[0].TakeIndex();
			if (this.ranges[0].empty)
			{
				this.RemoveAt(0);
			}
			return result;
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x00072F51 File Offset: 0x00071151
		private void RemoveAt(int i)
		{
			HGArrayUtilities.ArrayRemoveAt<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, 1);
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x00072F66 File Offset: 0x00071166
		private void InsertAt(int i, IndexAllocator.Range range)
		{
			HGArrayUtilities.ArrayInsert<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, ref range);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x00072F7C File Offset: 0x0007117C
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

		// Token: 0x04001B78 RID: 7032
		private IndexAllocator.Range[] ranges;

		// Token: 0x04001B79 RID: 7033
		private int rangeCount;

		// Token: 0x0200043D RID: 1085
		private struct Range
		{
			// Token: 0x06001817 RID: 6167 RVA: 0x000730E1 File Offset: 0x000712E1
			public Range(int startIndex, int endIndex)
			{
				this.startIndex = startIndex;
				this.endIndex = endIndex;
			}

			// Token: 0x06001818 RID: 6168 RVA: 0x000730F4 File Offset: 0x000712F4
			public int TakeIndex()
			{
				int num = this.startIndex;
				this.startIndex = num + 1;
				return num;
			}

			// Token: 0x06001819 RID: 6169 RVA: 0x00073112 File Offset: 0x00071312
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

			// Token: 0x1700022C RID: 556
			// (get) Token: 0x0600181A RID: 6170 RVA: 0x00073149 File Offset: 0x00071349
			public bool empty
			{
				get
				{
					return this.startIndex == this.endIndex;
				}
			}

			// Token: 0x1700022D RID: 557
			// (get) Token: 0x0600181B RID: 6171 RVA: 0x00073159 File Offset: 0x00071359
			public int size
			{
				get
				{
					return this.endIndex - this.startIndex;
				}
			}

			// Token: 0x04001B7A RID: 7034
			public int startIndex;

			// Token: 0x04001B7B RID: 7035
			public int endIndex;
		}
	}
}
