using System;
using System.Collections.Generic;
using RoR2.Navigation;

namespace RoR2
{
	// Token: 0x02000463 RID: 1123
	public class Path : IDisposable
	{
		// Token: 0x06001904 RID: 6404 RVA: 0x000784F4 File Offset: 0x000766F4
		private static Path.Waypoint[] GetWaypointsBuffer()
		{
			if (Path.waypointsBufferPool.Count == 0)
			{
				return new Path.Waypoint[64];
			}
			return Path.waypointsBufferPool.Pop();
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x00078514 File Offset: 0x00076714
		private static void FreeWaypointsBuffer(Path.Waypoint[] buffer)
		{
			if (buffer.Length != 64)
			{
				return;
			}
			Path.waypointsBufferPool.Push(buffer);
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x00078529 File Offset: 0x00076729
		public Path(NodeGraph nodeGraph)
		{
			this.nodeGraph = nodeGraph;
			this.waypointsBuffer = Path.GetWaypointsBuffer();
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x00078551 File Offset: 0x00076751
		public void Dispose()
		{
			Path.FreeWaypointsBuffer(this.waypointsBuffer);
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06001908 RID: 6408 RVA: 0x0007855E File Offset: 0x0007675E
		// (set) Token: 0x06001909 RID: 6409 RVA: 0x00078566 File Offset: 0x00076766
		public NodeGraph nodeGraph { get; private set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600190A RID: 6410 RVA: 0x0007856F File Offset: 0x0007676F
		// (set) Token: 0x0600190B RID: 6411 RVA: 0x00078577 File Offset: 0x00076777
		public PathStatus status { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600190C RID: 6412 RVA: 0x00078580 File Offset: 0x00076780
		// (set) Token: 0x0600190D RID: 6413 RVA: 0x00078588 File Offset: 0x00076788
		public int waypointsCount { get; private set; }

		// Token: 0x17000250 RID: 592
		public Path.Waypoint this[int i]
		{
			get
			{
				return this.waypointsBuffer[this.firstWaypointIndex + i];
			}
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x000785A8 File Offset: 0x000767A8
		public void PushWaypointToFront(NodeGraph.NodeIndex nodeIndex, float minJumpHeight)
		{
			if (this.waypointsCount + 1 >= this.waypointsBuffer.Length)
			{
				Path.Waypoint[] array = this.waypointsBuffer;
				this.waypointsBuffer = new Path.Waypoint[this.waypointsCount + 32];
				Array.Copy(array, 0, this.waypointsBuffer, this.waypointsBuffer.Length - array.Length, array.Length);
				Path.FreeWaypointsBuffer(array);
			}
			int num = this.waypointsBuffer.Length;
			int num2 = this.waypointsCount + 1;
			this.waypointsCount = num2;
			this.firstWaypointIndex = num - num2;
			this.waypointsBuffer[this.firstWaypointIndex] = new Path.Waypoint
			{
				nodeIndex = nodeIndex,
				minJumpHeight = minJumpHeight
			};
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00078650 File Offset: 0x00076850
		public void WriteWaypointsToList(List<Path.Waypoint> waypointsList)
		{
			if (waypointsList.Capacity < waypointsList.Count + this.waypointsCount)
			{
				waypointsList.Capacity = waypointsList.Count + this.waypointsCount;
			}
			for (int i = this.firstWaypointIndex; i < this.waypointsBuffer.Length; i++)
			{
				waypointsList.Add(this.waypointsBuffer[i]);
			}
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x000786AF File Offset: 0x000768AF
		public void Clear()
		{
			this.status = PathStatus.Invalid;
			this.waypointsCount = 0;
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x04001C95 RID: 7317
		private static readonly Stack<Path.Waypoint[]> waypointsBufferPool = new Stack<Path.Waypoint[]>();

		// Token: 0x04001C96 RID: 7318
		private const int pooledBufferSize = 64;

		// Token: 0x04001C99 RID: 7321
		private Path.Waypoint[] waypointsBuffer;

		// Token: 0x04001C9B RID: 7323
		private int firstWaypointIndex;

		// Token: 0x02000464 RID: 1124
		public struct Waypoint
		{
			// Token: 0x04001C9C RID: 7324
			public NodeGraph.NodeIndex nodeIndex;

			// Token: 0x04001C9D RID: 7325
			public float minJumpHeight;
		}
	}
}
