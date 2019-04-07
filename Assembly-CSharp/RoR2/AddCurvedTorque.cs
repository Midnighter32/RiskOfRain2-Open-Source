using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000249 RID: 585
	public class AddCurvedTorque : MonoBehaviour
	{
		// Token: 0x06000AFE RID: 2814 RVA: 0x00036A2C File Offset: 0x00034C2C
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

		// Token: 0x04000EEA RID: 3818
		public AnimationCurve torqueCurve;

		// Token: 0x04000EEB RID: 3819
		public Vector3 localTorqueVector;

		// Token: 0x04000EEC RID: 3820
		public float lifetime;

		// Token: 0x04000EED RID: 3821
		public Rigidbody[] rigidbodies;

		// Token: 0x04000EEE RID: 3822
		private float stopwatch;
	}
}
