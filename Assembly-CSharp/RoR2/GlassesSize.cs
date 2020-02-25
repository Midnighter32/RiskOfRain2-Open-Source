using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000217 RID: 535
	[ExecuteAlways]
	public class GlassesSize : MonoBehaviour
	{
		// Token: 0x06000BBB RID: 3003 RVA: 0x0000409B File Offset: 0x0000229B
		private void Start()
		{
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x000331AD File Offset: 0x000313AD
		private void Update()
		{
			this.UpdateGlasses();
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x000331B8 File Offset: 0x000313B8
		private void UpdateGlasses()
		{
			Vector3 localScale = base.transform.localScale;
			float num = Mathf.Max(localScale.y, localScale.z);
			Vector3 localScale2 = new Vector3(1f / localScale.x * num, 1f / localScale.y * num, 1f / localScale.z * num);
			if (this.glassesModelBase)
			{
				this.glassesModelBase.transform.localScale = localScale2;
			}
			if (this.glassesBridgeLeft && this.glassesBridgeRight)
			{
				float num2 = (localScale.x / num - 1f) * this.bridgeOffsetScale;
				this.glassesBridgeLeft.transform.localPosition = this.offsetVector * -num2;
				this.glassesBridgeRight.transform.localPosition = this.offsetVector * num2;
			}
		}

		// Token: 0x04000BD8 RID: 3032
		public Transform glassesModelBase;

		// Token: 0x04000BD9 RID: 3033
		public Transform glassesBridgeLeft;

		// Token: 0x04000BDA RID: 3034
		public Transform glassesBridgeRight;

		// Token: 0x04000BDB RID: 3035
		public float bridgeOffsetScale;

		// Token: 0x04000BDC RID: 3036
		public Vector3 offsetVector = Vector3.right;
	}
}
