using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025C RID: 604
	public class ApplyTorqueOnStart : MonoBehaviour
	{
		// Token: 0x06000B40 RID: 2880 RVA: 0x00037C10 File Offset: 0x00035E10
		private void Start()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component)
			{
				Vector3 vector = this.localTorque;
				if (this.randomize)
				{
					vector.x = UnityEngine.Random.Range(-vector.x / 2f, vector.x / 2f);
					vector.y = UnityEngine.Random.Range(-vector.y / 2f, vector.y / 2f);
					vector.z = UnityEngine.Random.Range(-vector.z / 2f, vector.z / 2f);
				}
				component.AddRelativeTorque(vector);
			}
		}

		// Token: 0x04000F50 RID: 3920
		public Vector3 localTorque;

		// Token: 0x04000F51 RID: 3921
		public bool randomize;
	}
}
