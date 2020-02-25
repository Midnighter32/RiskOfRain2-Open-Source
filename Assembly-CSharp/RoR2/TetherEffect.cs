using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000357 RID: 855
	[RequireComponent(typeof(BezierCurveLine))]
	public class TetherEffect : MonoBehaviour
	{
		// Token: 0x060014C3 RID: 5315 RVA: 0x00058A0B File Offset: 0x00056C0B
		private void Start()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x00058A1C File Offset: 0x00056C1C
		private void Update()
		{
			bool flag = true;
			if (this.tetherEndTransform)
			{
				flag = false;
				this.bezierCurveLine.endTransform = this.tetherEndTransform;
				if ((this.tetherEndTransform.position - base.transform.position).sqrMagnitude > this.tetherMaxDistance * this.tetherMaxDistance)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (this.fadeOut)
				{
					this.fadeOut.enabled = true;
					return;
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04001357 RID: 4951
		public float tetherMaxDistance;

		// Token: 0x04001358 RID: 4952
		public Transform tetherEndTransform;

		// Token: 0x04001359 RID: 4953
		public AnimateShaderAlpha fadeOut;

		// Token: 0x0400135A RID: 4954
		private BezierCurveLine bezierCurveLine;
	}
}
