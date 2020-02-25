using System;

namespace RoR2
{
	// Token: 0x02000397 RID: 919
	public class HGHeap<T> where T : struct
	{
		// Token: 0x0600165E RID: 5726 RVA: 0x000605A9 File Offset: 0x0005E7A9
		public HGHeap(uint initialSize)
		{
			this.heap = new HGHeap<T>.Element[initialSize];
			this.allocator = new IndexAllocator();
			this.allocator.RequestIndex();
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x000605D4 File Offset: 0x0005E7D4
		public bool PtrIsValid(in HGHeap<T>.Ptr ptr)
		{
			return ptr.targetAddress != 0 && this.heap[ptr.targetAddress].cookie == ptr.targetCookie;
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x00060600 File Offset: 0x0005E800
		public HGHeap<T>.Ptr Alloc()
		{
			int num = this.allocator.RequestIndex();
			if (this.heap.Length <= num)
			{
				Array.Resize<HGHeap<T>.Element>(ref this.heap, this.heap.Length * 2);
			}
			return new HGHeap<T>.Ptr(num, this.heap[num].cookie);
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x00060650 File Offset: 0x0005E850
		public void Free(in HGHeap<T>.Ptr ptr)
		{
			if (!this.PtrIsValid(ptr))
			{
				return;
			}
			HGHeap<T>.Element[] array = this.heap;
			int targetAddress = ptr.targetAddress;
			array[targetAddress].value = default(T);
			array[targetAddress].cookie = array[targetAddress].cookie + 1U;
			this.allocator.FreeIndex(ptr.targetAddress);
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x0006069F File Offset: 0x0005E89F
		public void SetValue(in HGHeap<T>.Ptr ptr, in T value)
		{
			this.heap[ptr.targetAddress].value = value;
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x000606BD File Offset: 0x0005E8BD
		public T GetValue(in HGHeap<T>.Ptr ptr)
		{
			return this.heap[ptr.targetAddress].value;
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x000606D5 File Offset: 0x0005E8D5
		public ref T GetRef(HGHeap<T>.Ptr ptr)
		{
			return ref this.heap[ptr.targetAddress].value;
		}

		// Token: 0x0400150A RID: 5386
		private HGHeap<T>.Element[] heap;

		// Token: 0x0400150B RID: 5387
		private readonly IndexAllocator allocator;

		// Token: 0x0400150C RID: 5388
		public static readonly HGHeap<T>.Ptr nullPtr = new HGHeap<T>.Ptr(0, 0U);

		// Token: 0x02000398 RID: 920
		private struct Element
		{
			// Token: 0x0400150D RID: 5389
			public uint cookie;

			// Token: 0x0400150E RID: 5390
			public T value;
		}

		// Token: 0x02000399 RID: 921
		public struct Ptr
		{
			// Token: 0x06001666 RID: 5734 RVA: 0x000606FB File Offset: 0x0005E8FB
			public Ptr(int targetAddress, uint targetCookie)
			{
				this.targetAddress = targetAddress;
				this.targetCookie = targetCookie;
			}

			// Token: 0x0400150F RID: 5391
			public readonly int targetAddress;

			// Token: 0x04001510 RID: 5392
			public readonly uint targetCookie;
		}
	}
}
