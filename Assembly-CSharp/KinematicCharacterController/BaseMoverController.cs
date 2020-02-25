using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200090C RID: 2316
	public abstract class BaseMoverController : MonoBehaviour
	{
		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x060033D1 RID: 13265 RVA: 0x000E0CCE File Offset: 0x000DEECE
		// (set) Token: 0x060033D2 RID: 13266 RVA: 0x000E0CD6 File Offset: 0x000DEED6
		public PhysicsMover Mover { get; private set; }

		// Token: 0x060033D3 RID: 13267 RVA: 0x000E0CDF File Offset: 0x000DEEDF
		public void SetupMover(PhysicsMover mover)
		{
			this.Mover = mover;
		}

		// Token: 0x060033D4 RID: 13268
		public abstract void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime);
	}
}
