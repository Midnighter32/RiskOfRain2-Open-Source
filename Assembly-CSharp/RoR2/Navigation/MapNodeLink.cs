using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000527 RID: 1319
	[RequireComponent(typeof(MapNode))]
	public class MapNodeLink : MonoBehaviour
	{
		// Token: 0x06001D9E RID: 7582 RVA: 0x0008A790 File Offset: 0x00088990
		private void OnValidate()
		{
			if (this.other == this)
			{
				Debug.LogWarning("Map node link cannot link a node to itself.");
				this.other = null;
			}
			if (this.other && this.other.GetComponentInParent<MapNodeGroup>() != base.GetComponentInParent<MapNodeGroup>())
			{
				Debug.LogWarning("Map node link cannot link to a node in a separate node group.");
				this.other = null;
			}
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x0008A7F4 File Offset: 0x000889F4
		private void OnDrawGizmos()
		{
			if (this.other)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = this.other.transform.position;
				Vector3 vector = (position + position2) * 0.5f;
				Color yellow = Color.yellow;
				yellow.a = 0.5f;
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(position, vector);
				Gizmos.color = yellow;
				Gizmos.DrawLine(vector, position2);
			}
		}

		// Token: 0x04001FE4 RID: 8164
		public MapNode other;

		// Token: 0x04001FE5 RID: 8165
		public float minJumpHeight;

		// Token: 0x04001FE6 RID: 8166
		[Tooltip("The gate name associated with this link. If the named gate is closed, this link will not be used in pathfinding.")]
		public string gateName = "";

		// Token: 0x04001FE7 RID: 8167
		public GameObject[] objectsToEnableDuringTest;

		// Token: 0x04001FE8 RID: 8168
		public GameObject[] objectsToDisableDuringTest;
	}
}
