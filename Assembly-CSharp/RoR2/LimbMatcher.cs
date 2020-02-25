using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000272 RID: 626
	public class LimbMatcher : MonoBehaviour
	{
		// Token: 0x06000DEB RID: 3563 RVA: 0x0003E670 File Offset: 0x0003C870
		public void SetChildLocator(ChildLocator childLocator)
		{
			this.valid = true;
			for (int i = 0; i < this.limbPairs.Length; i++)
			{
				LimbMatcher.LimbPair limbPair = this.limbPairs[i];
				Transform transform = childLocator.FindChild(limbPair.targetChildLimb);
				if (!transform)
				{
					this.valid = false;
					return;
				}
				this.limbPairs[i].targetTransform = transform;
			}
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x0003E6D3 File Offset: 0x0003C8D3
		private void LateUpdate()
		{
			this.UpdateLimbs();
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x0003E6DC File Offset: 0x0003C8DC
		private void UpdateLimbs()
		{
			if (!this.valid)
			{
				return;
			}
			for (int i = 0; i < this.limbPairs.Length; i++)
			{
				LimbMatcher.LimbPair limbPair = this.limbPairs[i];
				Transform targetTransform = limbPair.targetTransform;
				if (targetTransform && limbPair.originalTransform)
				{
					limbPair.originalTransform.position = targetTransform.position;
					limbPair.originalTransform.rotation = targetTransform.rotation;
					if (i < this.limbPairs.Length - 1)
					{
						float num = Vector3.Magnitude(this.limbPairs[i + 1].targetTransform.position - targetTransform.position);
						float originalLimbLength = limbPair.originalLimbLength;
						if (this.scaleLimbs)
						{
							Vector3 localScale = limbPair.originalTransform.localScale;
							localScale.y = num / originalLimbLength;
							limbPair.originalTransform.localScale = localScale;
						}
					}
				}
			}
		}

		// Token: 0x04000DE9 RID: 3561
		public bool scaleLimbs = true;

		// Token: 0x04000DEA RID: 3562
		private bool valid;

		// Token: 0x04000DEB RID: 3563
		public LimbMatcher.LimbPair[] limbPairs;

		// Token: 0x02000273 RID: 627
		[Serializable]
		public struct LimbPair
		{
			// Token: 0x04000DEC RID: 3564
			public Transform originalTransform;

			// Token: 0x04000DED RID: 3565
			public string targetChildLimb;

			// Token: 0x04000DEE RID: 3566
			public float originalLimbLength;

			// Token: 0x04000DEF RID: 3567
			[NonSerialized]
			public Transform targetTransform;
		}
	}
}
