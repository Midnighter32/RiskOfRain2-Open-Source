using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x02000264 RID: 612
	public interface ITeleportHandler : IEventSystemHandler
	{
		// Token: 0x06000D90 RID: 3472
		void OnTeleport(Vector3 oldPosition, Vector3 newPosition);
	}
}
