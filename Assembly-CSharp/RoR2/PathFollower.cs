using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003DD RID: 989
	public class PathFollower
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x060017F2 RID: 6130 RVA: 0x000682A5 File Offset: 0x000664A5
		// (set) Token: 0x060017F3 RID: 6131 RVA: 0x000682AD File Offset: 0x000664AD
		public NodeGraph nodeGraph { get; private set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool isOnJumpLink
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x060017F5 RID: 6133 RVA: 0x000682B8 File Offset: 0x000664B8
		public bool nextWaypointNeedsJump
		{
			get
			{
				return this.waypoints.Count > 0 && this.currentWaypoint < this.waypoints.Count && this.waypoints[this.currentWaypoint].minJumpHeight > 0f;
			}
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x00068305 File Offset: 0x00066505
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x00068328 File Offset: 0x00066528
		public float CalculateJumpVelocityNeededToReachNextWaypoint(float moveSpeed)
		{
			if (!this.nextWaypointNeedsJump)
			{
				return 0f;
			}
			Vector3 vector = this.currentPosition;
			Vector3 nextPosition = this.GetNextPosition();
			return Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(moveSpeed, PathFollower.DistanceXZ(vector, nextPosition)), nextPosition.y - vector.y);
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x00068370 File Offset: 0x00066570
		public void UpdatePosition(Vector3 newPosition)
		{
			this.currentPosition = newPosition;
			Vector3 a;
			if (this.GetNextNodePosition(out a))
			{
				Vector3 vector = a - this.currentPosition;
				Vector3 vector2 = vector;
				vector2.y = 0f;
				float num = 2f;
				if (this.waypoints.Count > this.currentWaypoint + 1 && this.waypoints[this.currentWaypoint + 1].minJumpHeight > 0f)
				{
					num = 0.5f;
				}
				if (num * num >= vector2.sqrMagnitude && Mathf.Abs(vector.y) <= 2f)
				{
					this.SetWaypoint(this.currentWaypoint + 1);
				}
			}
			this.nextNode != NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x0006842C File Offset: 0x0006662C
		private void SetWaypoint(int newWaypoint)
		{
			this.currentWaypoint = Math.Min(newWaypoint, this.waypoints.Count);
			if (this.currentWaypoint == this.waypoints.Count)
			{
				this.nextNode = NodeGraph.NodeIndex.invalid;
				this.previousNode = NodeGraph.NodeIndex.invalid;
				return;
			}
			this.nextNode = this.waypoints[this.currentWaypoint].nodeIndex;
			this.previousNode = ((this.currentWaypoint > 0) ? this.waypoints[this.currentWaypoint - 1].nodeIndex : NodeGraph.NodeIndex.invalid);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000684C4 File Offset: 0x000666C4
		private void Reset()
		{
			this.nodeGraph = null;
			this.nextNode = NodeGraph.NodeIndex.invalid;
			this.previousNode = NodeGraph.NodeIndex.invalid;
			this.waypoints.Clear();
			this.currentWaypoint = 0;
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x000684F8 File Offset: 0x000666F8
		public void SetPath(Path newPath)
		{
			if (this.nodeGraph != newPath.nodeGraph)
			{
				this.Reset();
				this.nodeGraph = newPath.nodeGraph;
			}
			this.waypoints.Clear();
			newPath.WriteWaypointsToList(this.waypoints);
			this.currentWaypoint = 0;
			for (int i = 1; i < this.waypoints.Count; i++)
			{
				if (this.waypoints[i].nodeIndex == this.nextNode && this.waypoints[i - 1].nodeIndex == this.previousNode)
				{
					this.currentWaypoint = i;
					break;
				}
			}
			this.SetWaypoint(this.currentWaypoint);
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x000685B4 File Offset: 0x000667B4
		private bool GetNextNodePosition(out Vector3 nextPosition)
		{
			if (this.nodeGraph != null && this.nextNode != NodeGraph.NodeIndex.invalid && this.nodeGraph.GetNodePosition(this.nextNode, out nextPosition))
			{
				return true;
			}
			nextPosition = Vector3.zero;
			return false;
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x00068604 File Offset: 0x00066804
		public Vector3 GetNextPosition()
		{
			Vector3 result;
			if (this.GetNextNodePosition(out result))
			{
				return result;
			}
			return this.targetPosition;
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x00068624 File Offset: 0x00066824
		public void DebugDrawPath(Color color, float duration)
		{
			for (int i = 1; i < this.waypoints.Count; i++)
			{
				Vector3 start;
				this.nodeGraph.GetNodePosition(this.waypoints[i].nodeIndex, out start);
				Vector3 end;
				this.nodeGraph.GetNodePosition(this.waypoints[i - 1].nodeIndex, out end);
				Debug.DrawLine(start, end, color, duration);
			}
			for (int j = 1; j < this.waypoints.Count; j++)
			{
				Vector3 a;
				this.nodeGraph.GetNodePosition(this.waypoints[j].nodeIndex, out a);
				Debug.DrawLine(a + Vector3.up, a - Vector3.up, color, duration);
			}
		}

		// Token: 0x040016AC RID: 5804
		private Vector3 currentPosition;

		// Token: 0x040016AD RID: 5805
		public Vector3 targetPosition;

		// Token: 0x040016AE RID: 5806
		private const float waypointPassDistance = 2f;

		// Token: 0x040016AF RID: 5807
		private const float waypointPassYTolerance = 2f;

		// Token: 0x040016B1 RID: 5809
		private List<Path.Waypoint> waypoints = new List<Path.Waypoint>();

		// Token: 0x040016B2 RID: 5810
		private int currentWaypoint;

		// Token: 0x040016B3 RID: 5811
		private NodeGraph.NodeIndex previousNode = NodeGraph.NodeIndex.invalid;

		// Token: 0x040016B4 RID: 5812
		private NodeGraph.NodeIndex nextNode = NodeGraph.NodeIndex.invalid;
	}
}
