using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034A RID: 842
	public class LimbMatcher : MonoBehaviour
	{
		// Token: 0x06001171 RID: 4465 RVA: 0x00056ACC File Offset: 0x00054CCC
		public void SetChildLocator(ChildLocator childLocator)
		{
			for (int i = 0; i < this.limbPairs.Length; i++)
			{
				LimbMatcher.LimbPair limbPair = this.limbPairs[i];
				Transform targetTransform = childLocator.FindChild(limbPair.targetChildLimb);
				this.limbPairs[i].targetTransform = targetTransform;
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00056B18 File Offset: 0x00054D18
		private void LateUpdate()
		{
			this.UpdateLimbs();
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x00056B20 File Offset: 0x00054D20
		private void UpdateLimbs()
		{
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
						Vector3 localScale = limbPair.originalTransform.localScale;
						localScale.y = num / originalLimbLength;
						limbPair.originalTransform.localScale = localScale;
					}
				}
			}
		}

		// Token: 0x04001579 RID: 5497
		public LimbMatcher.LimbPair[] limbPairs;

		// Token: 0x0200034B RID: 843
		[Serializable]
		public struct LimbPair
		{
			// Token: 0x0400157A RID: 5498
			public Transform originalTransform;

			// Token: 0x0400157B RID: 5499
			public string targetChildLimb;

			// Token: 0x0400157C RID: 5500
			public float originalLimbLength;

			// Token: 0x0400157D RID: 5501
			[NonSerialized]
			public Transform targetTransform;
		}
	}
}
