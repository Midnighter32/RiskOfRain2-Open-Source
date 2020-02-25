using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000335 RID: 821
	[DefaultExecutionOrder(99999)]
	public class SimpleRotateToDirection : MonoBehaviour
	{
		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600138A RID: 5002 RVA: 0x0005388D File Offset: 0x00051A8D
		// (set) Token: 0x0600138B RID: 5003 RVA: 0x00053895 File Offset: 0x00051A95
		public Quaternion targetRotation { get; set; }

		// Token: 0x0600138C RID: 5004 RVA: 0x0005389E File Offset: 0x00051A9E
		private void Awake()
		{
			this.transform = base.transform;
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x000538B8 File Offset: 0x00051AB8
		private void OnEnable()
		{
			this.targetRotation = this.transform.rotation;
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x000538CB File Offset: 0x00051ACB
		private void LateUpdate()
		{
			this.isNetworkControlled = (this.networkIdentity != null && !Util.HasEffectiveAuthority(this.networkIdentity));
			if (!this.isNetworkControlled)
			{
				this.Simulate(Time.deltaTime);
			}
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x000538FF File Offset: 0x00051AFF
		private void Simulate(float deltaTime)
		{
			this.transform.rotation = Util.SmoothDampQuaternion(this.transform.rotation, this.targetRotation, ref this.velocity, this.smoothTime, this.maxRotationSpeed, deltaTime);
		}

		// Token: 0x04001252 RID: 4690
		public float smoothTime;

		// Token: 0x04001253 RID: 4691
		public float maxRotationSpeed;

		// Token: 0x04001254 RID: 4692
		private new Transform transform;

		// Token: 0x04001255 RID: 4693
		[CanBeNull]
		private NetworkIdentity networkIdentity;

		// Token: 0x04001257 RID: 4695
		private float velocity;

		// Token: 0x04001258 RID: 4696
		private bool isNetworkControlled;
	}
}
