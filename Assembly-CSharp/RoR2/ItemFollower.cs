using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000343 RID: 835
	public class ItemFollower : MonoBehaviour
	{
		// Token: 0x06001145 RID: 4421 RVA: 0x00055DBC File Offset: 0x00053FBC
		private void Start()
		{
			if (!this.followerInstance)
			{
				this.followerInstance = UnityEngine.Object.Instantiate<GameObject>(this.followerPrefab, this.targetObject.transform.position, Quaternion.identity);
				this.followerInstance.transform.localScale = base.transform.localScale;
			}
			if (this.followerCurve)
			{
				this.v0 = this.followerCurve.v0;
				this.v1 = this.followerCurve.v1;
			}
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x00055E48 File Offset: 0x00054048
		private void Update()
		{
			Transform transform = this.followerInstance.transform;
			Transform transform2 = this.targetObject.transform;
			transform.position = Vector3.SmoothDamp(transform.position, transform2.position, ref this.velocityDistance, this.distanceDampTime);
			transform.rotation = transform2.rotation;
			if (this.followerCurve)
			{
				this.followerCurve.v0 = base.transform.TransformVector(this.v0);
				this.followerCurve.v1 = transform.TransformVector(this.v1);
				this.followerCurve.p1 = transform.position;
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x00055EED File Offset: 0x000540ED
		private void OnDestroy()
		{
			if (this.followerInstance)
			{
				UnityEngine.Object.Destroy(this.followerInstance);
			}
		}

		// Token: 0x04001552 RID: 5458
		public GameObject followerPrefab;

		// Token: 0x04001553 RID: 5459
		public GameObject targetObject;

		// Token: 0x04001554 RID: 5460
		public BezierCurveLine followerCurve;

		// Token: 0x04001555 RID: 5461
		public float distanceDampTime;

		// Token: 0x04001556 RID: 5462
		public float distanceMaxSpeed;

		// Token: 0x04001557 RID: 5463
		private Vector3 velocityDistance;

		// Token: 0x04001558 RID: 5464
		private Vector3 v0;

		// Token: 0x04001559 RID: 5465
		private Vector3 v1;

		// Token: 0x0400155A RID: 5466
		[HideInInspector]
		public GameObject followerInstance;
	}
}
