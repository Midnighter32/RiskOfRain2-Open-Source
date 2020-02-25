using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x0200011D RID: 285
	public struct DegreeSlices : IEnumerable<float>, IEnumerable
	{
		// Token: 0x06000528 RID: 1320 RVA: 0x00014E79 File Offset: 0x00013079
		public DegreeSlices(int sliceCount, float sliceOffset)
		{
			this.sliceCount = sliceCount;
			this.sliceOffset = sliceOffset;
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x00014E89 File Offset: 0x00013089
		public IEnumerator<float> GetEnumerator()
		{
			return new DegreeSlices.Enumerator(this.sliceCount, this.sliceOffset);
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x00014EA1 File Offset: 0x000130A1
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0400055B RID: 1371
		public readonly int sliceCount;

		// Token: 0x0400055C RID: 1372
		public readonly float sliceOffset;

		// Token: 0x0200011E RID: 286
		private struct Enumerator : IEnumerator<float>, IEnumerator, IDisposable
		{
			// Token: 0x0600052B RID: 1323 RVA: 0x00014EA9 File Offset: 0x000130A9
			public Enumerator(int sliceCount, float sliceOffset)
			{
				this.sliceSize = 360f / (float)sliceCount;
				this.offset = sliceOffset * this.sliceSize;
				this.i = -1;
				this.iEnd = sliceCount;
			}

			// Token: 0x1700009F RID: 159
			// (get) Token: 0x0600052C RID: 1324 RVA: 0x00014ED5 File Offset: 0x000130D5
			public float Current
			{
				get
				{
					return (float)this.i * this.sliceSize + this.offset;
				}
			}

			// Token: 0x170000A0 RID: 160
			// (get) Token: 0x0600052D RID: 1325 RVA: 0x00014EEC File Offset: 0x000130EC
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x0600052E RID: 1326 RVA: 0x0000409B File Offset: 0x0000229B
			public void Dispose()
			{
			}

			// Token: 0x0600052F RID: 1327 RVA: 0x00014EF9 File Offset: 0x000130F9
			public bool MoveNext()
			{
				this.i++;
				return this.i < this.iEnd;
			}

			// Token: 0x06000530 RID: 1328 RVA: 0x00014F17 File Offset: 0x00013117
			public void Reset()
			{
				this.i = -1;
			}

			// Token: 0x0400055D RID: 1373
			public readonly float sliceSize;

			// Token: 0x0400055E RID: 1374
			public readonly float offset;

			// Token: 0x0400055F RID: 1375
			public int i;

			// Token: 0x04000560 RID: 1376
			public int iEnd;
		}
	}
}
