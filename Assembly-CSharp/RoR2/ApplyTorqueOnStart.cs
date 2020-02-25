using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200014D RID: 333
	public class ApplyTorqueOnStart : MonoBehaviour
	{
		// Token: 0x060005E9 RID: 1513 RVA: 0x00018784 File Offset: 0x00016984
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

		// Token: 0x0400066D RID: 1645
		public Vector3 localTorque;

		// Token: 0x0400066E RID: 1646
		public bool randomize;
	}
}
