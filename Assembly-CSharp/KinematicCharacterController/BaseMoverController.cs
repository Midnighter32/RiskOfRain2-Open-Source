using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006C1 RID: 1729
	public abstract class BaseMoverController : MonoBehaviour
	{
		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06002687 RID: 9863 RVA: 0x000B1886 File Offset: 0x000AFA86
		// (set) Token: 0x06002688 RID: 9864 RVA: 0x000B188E File Offset: 0x000AFA8E
		public PhysicsMover Mover { get; private set; }

		// Token: 0x06002689 RID: 9865 RVA: 0x000B1897 File Offset: 0x000AFA97
		public void SetupMover(PhysicsMover mover)
		{
			this.Mover = mover;
		}

		// Token: 0x0600268A RID: 9866
		public abstract void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime);
	}
}
