using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000397 RID: 919
	public class PreGameShakeController : MonoBehaviour
	{
		// Token: 0x06001363 RID: 4963 RVA: 0x0005EB07 File Offset: 0x0005CD07
		private void ResetTimer()
		{
			this.timer = UnityEngine.Random.Range(this.minInterval, this.maxInterval);
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x0005EB20 File Offset: 0x0005CD20
		private void DoShake()
		{
			this.shakeEmitter.StartShake();
			Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
			foreach (Rigidbody rigidbody in this.physicsBodies)
			{
				if (rigidbody)
				{
					Vector3 force = onUnitSphere * ((0.75f + UnityEngine.Random.value * 0.25f) * this.physicsForce);
					float y = rigidbody.GetComponent<Collider>().bounds.min.y;
					Vector3 centerOfMass = rigidbody.centerOfMass;
					centerOfMass.y = y;
					rigidbody.AddForceAtPosition(force, centerOfMass);
				}
			}
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x0005EBB5 File Offset: 0x0005CDB5
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x0005EBBD File Offset: 0x0005CDBD
		private void Update()
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.ResetTimer();
				this.DoShake();
			}
		}

		// Token: 0x04001700 RID: 5888
		public ShakeEmitter shakeEmitter;

		// Token: 0x04001701 RID: 5889
		public float minInterval = 0.5f;

		// Token: 0x04001702 RID: 5890
		public float maxInterval = 7f;

		// Token: 0x04001703 RID: 5891
		public Rigidbody[] physicsBodies;

		// Token: 0x04001704 RID: 5892
		public float physicsForce;

		// Token: 0x04001705 RID: 5893
		private float timer;
	}
}
