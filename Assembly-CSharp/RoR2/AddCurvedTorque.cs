using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200013A RID: 314
	public class AddCurvedTorque : MonoBehaviour
	{
		// Token: 0x060005A6 RID: 1446 RVA: 0x00017558 File Offset: 0x00015758
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			float d = this.torqueCurve.Evaluate(this.stopwatch / this.lifetime);
			Rigidbody[] array = this.rigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddRelativeTorque(this.localTorqueVector * d);
			}
		}

		// Token: 0x04000607 RID: 1543
		public AnimationCurve torqueCurve;

		// Token: 0x04000608 RID: 1544
		public Vector3 localTorqueVector;

		// Token: 0x04000609 RID: 1545
		public float lifetime;

		// Token: 0x0400060A RID: 1546
		public Rigidbody[] rigidbodies;

		// Token: 0x0400060B RID: 1547
		private float stopwatch;
	}
}
