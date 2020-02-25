using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000204 RID: 516
	public class FollowerItemDisplayComponent : MonoBehaviour
	{
		// Token: 0x06000B02 RID: 2818 RVA: 0x00030D39 File Offset: 0x0002EF39
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00030D48 File Offset: 0x0002EF48
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

		// Token: 0x04000B70 RID: 2928
		public Transform target;

		// Token: 0x04000B71 RID: 2929
		public Vector3 localPosition;

		// Token: 0x04000B72 RID: 2930
		public Quaternion localRotation;

		// Token: 0x04000B73 RID: 2931
		public Vector3 localScale;

		// Token: 0x04000B74 RID: 2932
		private new Transform transform;
	}
}
