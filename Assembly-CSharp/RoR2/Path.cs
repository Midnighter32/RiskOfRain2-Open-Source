using System;
using System.Collections.Generic;
using RoR2.Navigation;

namespace RoR2
{
	// Token: 0x020003DB RID: 987
	public class Path : IDisposable
	{
		// Token: 0x060017E3 RID: 6115 RVA: 0x000680BF File Offset: 0x000662BF
		private static Path.Waypoint[] GetWaypointsBuffer()
		{
			if (Path.waypointsBufferPool.Count == 0)
			{
				return new Path.Waypoint[64];
			}
			return Path.waypointsBufferPool.Pop();
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x000680DF File Offset: 0x000662DF
		private static void FreeWaypointsBuffer(Path.Waypoint[] buffer)
		{
			if (buffer.Length != 64)
			{
				return;
			}
			Path.waypointsBufferPool.Push(buffer);
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000680F4 File Offset: 0x000662F4
		public Path(NodeGraph nodeGraph)
		{
			this.nodeGraph = nodeGraph;
			this.waypointsBuffer = Path.GetWaypointsBuffer();
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x0006811C File Offset: 0x0006631C
		public void Dispose()
		{
			Path.FreeWaypointsBuffer(this.waypointsBuffer);
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x060017E7 RID: 6119 RVA: 0x00068129 File Offset: 0x00066329
		// (set) Token: 0x060017E8 RID: 6120 RVA: 0x00068131 File Offset: 0x00066331
		public NodeGraph nodeGraph { get; private set; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x060017E9 RID: 6121 RVA: 0x0006813A File Offset: 0x0006633A
		// (set) Token: 0x060017EA RID: 6122 RVA: 0x00068142 File Offset: 0x00066342
		public PathStatus status { get; set; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x060017EB RID: 6123 RVA: 0x0006814B File Offset: 0x0006634B
		// (set) Token: 0x060017EC RID: 6124 RVA: 0x00068153 File Offset: 0x00066353
		public int waypointsCount { get; private set; }

		// Token: 0x170002C8 RID: 712
		public Path.Waypoint this[int i]
		{
			get
			{
				return this.waypointsBuffer[this.firstWaypointIndex + i];
			}
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x00068174 File Offset: 0x00066374
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

		// Token: 0x060017EF RID: 6127 RVA: 0x0006821C File Offset: 0x0006641C
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

		// Token: 0x060017F0 RID: 6128 RVA: 0x0006827B File Offset: 0x0006647B
		public void Clear()
		{
			this.status = PathStatus.Invalid;
			this.waypointsCount = 0;
			this.firstWaypointIndex = this.waypointsBuffer.Length;
		}

		// Token: 0x040016A3 RID: 5795
		private static readonly Stack<Path.Waypoint[]> waypointsBufferPool = new Stack<Path.Waypoint[]>();

		// Token: 0x040016A4 RID: 5796
		private const int pooledBufferSize = 64;

		// Token: 0x040016A7 RID: 5799
		private Path.Waypoint[] waypointsBuffer;

		// Token: 0x040016A9 RID: 5801
		private int firstWaypointIndex;

		// Token: 0x020003DC RID: 988
		public struct Waypoint
		{
			// Token: 0x040016AA RID: 5802
			public NodeGraph.NodeIndex nodeIndex;

			// Token: 0x040016AB RID: 5803
			public float minJumpHeight;
		}
	}
}
