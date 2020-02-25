using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x020004E1 RID: 1249
	[RequireComponent(typeof(MapNode))]
	public class MapNodeLink : MonoBehaviour
	{
		// Token: 0x06001DD1 RID: 7633 RVA: 0x0007FACC File Offset: 0x0007DCCC
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

		// Token: 0x06001DD2 RID: 7634 RVA: 0x0007FB30 File Offset: 0x0007DD30
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

		// Token: 0x04001AFD RID: 6909
		public MapNode other;

		// Token: 0x04001AFE RID: 6910
		public float minJumpHeight;

		// Token: 0x04001AFF RID: 6911
		[Tooltip("The gate name associated with this link. If the named gate is closed, this link will not be used in pathfinding.")]
		public string gateName = "";

		// Token: 0x04001B00 RID: 6912
		public GameObject[] objectsToEnableDuringTest;

		// Token: 0x04001B01 RID: 6913
		public GameObject[] objectsToDisableDuringTest;
	}
}
