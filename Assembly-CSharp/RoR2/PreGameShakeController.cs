using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D4 RID: 724
	public class PreGameShakeController : MonoBehaviour
	{
		// Token: 0x06001081 RID: 4225 RVA: 0x000485CF File Offset: 0x000467CF
		private void ResetTimer()
		{
			this.timer = UnityEngine.Random.Range(this.minInterval, this.maxInterval);
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x000485E8 File Offset: 0x000467E8
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

		// Token: 0x06001083 RID: 4227 RVA: 0x0004867D File Offset: 0x0004687D
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x00048685 File Offset: 0x00046885
		private void Update()
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.ResetTimer();
				this.DoShake();
			}
		}

		// Token: 0x04000FD0 RID: 4048
		public ShakeEmitter shakeEmitter;

		// Token: 0x04000FD1 RID: 4049
		public float minInterval = 0.5f;

		// Token: 0x04000FD2 RID: 4050
		public float maxInterval = 7f;

		// Token: 0x04000FD3 RID: 4051
		public Rigidbody[] physicsBodies;

		// Token: 0x04000FD4 RID: 4052
		public float physicsForce;

		// Token: 0x04000FD5 RID: 4053
		private float timer;
	}
}
