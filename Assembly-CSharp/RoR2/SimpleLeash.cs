using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000334 RID: 820
	[DefaultExecutionOrder(99999)]
	public class SimpleLeash : MonoBehaviour
	{
		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x0005371E File Offset: 0x0005191E
		// (set) Token: 0x06001384 RID: 4996 RVA: 0x00053726 File Offset: 0x00051926
		public Vector3 leashOrigin { get; set; }

		// Token: 0x06001385 RID: 4997 RVA: 0x0005372F File Offset: 0x0005192F
		private void Awake()
		{
			this.transform = base.transform;
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x00053749 File Offset: 0x00051949
		private void OnEnable()
		{
			this.leashOrigin = this.transform.position;
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x0005375C File Offset: 0x0005195C
		private void LateUpdate()
		{
			this.isNetworkControlled = (this.networkIdentity != null && !Util.HasEffectiveAuthority(this.networkIdentity));
			if (!this.isNetworkControlled)
			{
				this.Simulate(Time.deltaTime);
			}
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x00053790 File Offset: 0x00051990
		private void Simulate(float deltaTime)
		{
			Vector3 position = this.transform.position;
			Vector3 leashOrigin = this.leashOrigin;
			Vector3 a = position - leashOrigin;
			float sqrMagnitude = a.sqrMagnitude;
			if (sqrMagnitude > this.minLeashRadius * this.minLeashRadius)
			{
				float num = Mathf.Sqrt(sqrMagnitude);
				Vector3 a2 = a / num;
				Vector3 target = leashOrigin + a2 * this.minLeashRadius;
				Vector3 vector = position;
				if (num > this.maxLeashRadius)
				{
					vector = leashOrigin + a2 * this.maxLeashRadius;
				}
				vector = Vector3.SmoothDamp(vector, target, ref this.velocity, this.smoothTime, this.maxFollowSpeed, deltaTime);
				if (vector != position)
				{
					this.transform.position = vector;
				}
			}
		}

		// Token: 0x04001249 RID: 4681
		public float minLeashRadius = 1f;

		// Token: 0x0400124A RID: 4682
		public float maxLeashRadius = 20f;

		// Token: 0x0400124B RID: 4683
		public float maxFollowSpeed = 40f;

		// Token: 0x0400124C RID: 4684
		public float smoothTime = 0.15f;

		// Token: 0x0400124D RID: 4685
		private new Transform transform;

		// Token: 0x0400124E RID: 4686
		[CanBeNull]
		private NetworkIdentity networkIdentity;

		// Token: 0x04001250 RID: 4688
		private Vector3 velocity = Vector3.zero;

		// Token: 0x04001251 RID: 4689
		private bool isNetworkControlled;
	}
}
