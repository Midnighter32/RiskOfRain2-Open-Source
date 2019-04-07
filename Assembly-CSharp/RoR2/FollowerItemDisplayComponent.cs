using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F5 RID: 757
	public class FollowerItemDisplayComponent : MonoBehaviour
	{
		// Token: 0x06000F4B RID: 3915 RVA: 0x0004B919 File Offset: 0x00049B19
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0004B928 File Offset: 0x00049B28
		private void LateUpdate()
		{
			if (!this.target)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			Quaternion rotation = this.target.rotation;
			this.transform.position = this.target.position + rotation * this.localPosition;
			this.transform.rotation = rotation * this.localRotation;
			this.transform.localScale = this.localScale;
		}

		// Token: 0x04001371 RID: 4977
		public Transform target;

		// Token: 0x04001372 RID: 4978
		public Vector3 localPosition;

		// Token: 0x04001373 RID: 4979
		public Quaternion localRotation;

		// Token: 0x04001374 RID: 4980
		public Vector3 localScale;

		// Token: 0x04001375 RID: 4981
		private new Transform transform;
	}
}
