using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039A RID: 922
	public static class HGPhysics
	{
		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06001667 RID: 5735 RVA: 0x0006070B File Offset: 0x0005E90B
		// (set) Token: 0x06001668 RID: 5736 RVA: 0x00060714 File Offset: 0x0005E914
		public static int sharedCollidersBufferEntriesCount
		{
			get
			{
				return HGPhysics._sharedCollidersBufferEntriesCount;
			}
			private set
			{
				int num = HGPhysics.sharedCollidersBufferEntriesCount - value;
				if (num > 0)
				{
					Array.Clear(HGPhysics.sharedCollidersBuffer, HGPhysics.sharedCollidersBufferEntriesCount, num);
				}
				HGPhysics._sharedCollidersBufferEntriesCount = value;
			}
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00060743 File Offset: 0x0005E943
		public static int OverlapBoxNonAllocShared(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapBoxNonAlloc(center, halfExtents, HGPhysics.sharedCollidersBuffer, orientation, layerMask, queryTriggerInteraction);
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0006075B File Offset: 0x0005E95B
		public static int OverlapSphereNonAllocShared(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapSphereNonAlloc(position, radius, HGPhysics.sharedCollidersBuffer, layerMask, queryTriggerInteraction);
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x00060771 File Offset: 0x0005E971
		public static float CalculateDistance(float initialVelocity, float acceleration, float time)
		{
			return initialVelocity * time + 0.5f * acceleration * time * time;
		}

		// Token: 0x04001511 RID: 5393
		public static readonly Collider[] sharedCollidersBuffer = new Collider[65536];

		// Token: 0x04001512 RID: 5394
		private static int _sharedCollidersBufferEntriesCount = 0;
	}
}
