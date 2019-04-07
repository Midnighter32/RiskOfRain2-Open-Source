using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x02000340 RID: 832
	public interface ITeleportHandler : IEventSystemHandler
	{
		// Token: 0x06001138 RID: 4408
		void OnTeleport(Vector3 oldPosition, Vector3 newPosition);
	}
}
