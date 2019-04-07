using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000301 RID: 769
	[ExecuteAlways]
	public class GlassesSize : MonoBehaviour
	{
		// Token: 0x06000FBF RID: 4031 RVA: 0x00004507 File Offset: 0x00002707
		private void Start()
		{
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x0004D6D3 File Offset: 0x0004B8D3
		private void Update()
		{
			this.UpdateGlasses();
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x0004D6DC File Offset: 0x0004B8DC
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

		// Token: 0x040013C9 RID: 5065
		public Transform glassesModelBase;

		// Token: 0x040013CA RID: 5066
		public Transform glassesBridgeLeft;

		// Token: 0x040013CB RID: 5067
		public Transform glassesBridgeRight;

		// Token: 0x040013CC RID: 5068
		public float bridgeOffsetScale;

		// Token: 0x040013CD RID: 5069
		public Vector3 offsetVector = Vector3.right;
	}
}
