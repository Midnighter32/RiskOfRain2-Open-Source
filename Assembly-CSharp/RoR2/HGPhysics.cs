using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000437 RID: 1079
	public static class HGPhysics
	{
		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06001806 RID: 6150 RVA: 0x00072C65 File Offset: 0x00070E65
		// (set) Token: 0x06001807 RID: 6151 RVA: 0x00072C6C File Offset: 0x00070E6C
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

		// Token: 0x06001808 RID: 6152 RVA: 0x00072C9B File Offset: 0x00070E9B
		public static int OverlapBoxNonAllocShared(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapBoxNonAlloc(center, halfExtents, HGPhysics.sharedCollidersBuffer, orientation, layerMask, queryTriggerInteraction);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x00072CB3 File Offset: 0x00070EB3
		public static int OverlapSphereNonAllocShared(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapSphereNonAlloc(position, radius, HGPhysics.sharedCollidersBuffer, layerMask, queryTriggerInteraction);
		}

		// Token: 0x04001B64 RID: 7012
		public static readonly Collider[] sharedCollidersBuffer = new Collider[65536];

		// Token: 0x04001B65 RID: 7013
		private static int _sharedCollidersBufferEntriesCount = 0;
	}
}
