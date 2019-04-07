using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000465 RID: 1125
	public class PathFollower
	{
		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06001913 RID: 6419 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool isOnJumpLink
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06001914 RID: 6420 RVA: 0x000786DC File Offset: 0x000768DC
		public bool nextWaypointNeedsJump
		{
			get
			{
				return this.waypoints.Count > 0 && this.currentWaypoint < this.waypoints.Count && this.waypoints[this.currentWaypoint].minJumpHeight > 0f;
			}
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x00078729 File Offset: 0x00076929
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x0007874C File Offset: 0x0007694C
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

		// Token: 0x06001917 RID: 6423 RVA: 0x00078794 File Offset: 0x00076994
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

		// Token: 0x06001918 RID: 6424 RVA: 0x00078850 File Offset: 0x00076A50
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

		// Token: 0x06001919 RID: 6425 RVA: 0x000788E8 File Offset: 0x00076AE8
		private void Reset()
		{
			this.nodeGraph = null;
			this.nextNode = NodeGraph.NodeIndex.invalid;
			this.previousNode = NodeGraph.NodeIndex.invalid;
			this.waypoints.Clear();
			this.currentWaypoint = 0;
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x0007891C File Offset: 0x00076B1C
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

		// Token: 0x0600191B RID: 6427 RVA: 0x000789D8 File Offset: 0x00076BD8
		private bool GetNextNodePosition(out Vector3 nextPosition)
		{
			if (this.nodeGraph != null && this.nextNode != NodeGraph.NodeIndex.invalid && this.nodeGraph.GetNodePosition(this.nextNode, out nextPosition))
			{
				return true;
			}
			nextPosition = Vector3.zero;
			return false;
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x00078A28 File Offset: 0x00076C28
		public Vector3 GetNextPosition()
		{
			Vector3 result;
			if (this.GetNextNodePosition(out result))
			{
				return result;
			}
			return this.targetPosition;
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x00078A48 File Offset: 0x00076C48
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

		// Token: 0x04001C9E RID: 7326
		private Vector3 currentPosition;

		// Token: 0x04001C9F RID: 7327
		public Vector3 targetPosition;

		// Token: 0x04001CA0 RID: 7328
		private const float waypointPassDistance = 2f;

		// Token: 0x04001CA1 RID: 7329
		private const float waypointPassYTolerance = 2f;

		// Token: 0x04001CA2 RID: 7330
		private NodeGraph nodeGraph;

		// Token: 0x04001CA3 RID: 7331
		private List<Path.Waypoint> waypoints = new List<Path.Waypoint>();

		// Token: 0x04001CA4 RID: 7332
		private int currentWaypoint;

		// Token: 0x04001CA5 RID: 7333
		private NodeGraph.NodeIndex previousNode = NodeGraph.NodeIndex.invalid;

		// Token: 0x04001CA6 RID: 7334
		private NodeGraph.NodeIndex nextNode = NodeGraph.NodeIndex.invalid;
	}
}
