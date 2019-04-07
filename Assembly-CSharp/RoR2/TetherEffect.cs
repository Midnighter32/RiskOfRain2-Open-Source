using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FF RID: 1023
	[RequireComponent(typeof(BezierCurveLine))]
	public class TetherEffect : MonoBehaviour
	{
		// Token: 0x060016C4 RID: 5828 RVA: 0x0006C743 File Offset: 0x0006A943
		private void Start()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x0006C754 File Offset: 0x0006A954
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

		// Token: 0x040019F0 RID: 6640
		public float tetherMaxDistance;

		// Token: 0x040019F1 RID: 6641
		public Transform tetherEndTransform;

		// Token: 0x040019F2 RID: 6642
		public AnimateShaderAlpha fadeOut;

		// Token: 0x040019F3 RID: 6643
		private BezierCurveLine bezierCurveLine;
	}
}
