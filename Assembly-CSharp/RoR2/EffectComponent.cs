using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002DE RID: 734
	[DisallowMultipleComponent]
	public class EffectComponent : MonoBehaviour
	{
		// Token: 0x06000EAE RID: 3758 RVA: 0x000485F8 File Offset: 0x000467F8
		private void Start()
		{
			if (this.effectData == null)
			{
				Debug.LogErrorFormat(base.gameObject, "Object {0} should not be instantiated by means other than EffectManager.instance.SpawnEffect.", new object[]
				{
					base.gameObject
				});
			}
			if (this.positionAtReferencedTransform)
			{
				Transform transform = this.effectData.ResolveChildLocatorTransformReference();
				if (transform)
				{
					base.transform.position = transform.position;
					base.transform.rotation = transform.rotation;
				}
			}
			if (this.parentToReferencedTransform)
			{
				Transform transform2 = this.effectData.ResolveChildLocatorTransformReference();
				if (transform2)
				{
					base.transform.parent = transform2;
				}
			}
			if (this.applyScale)
			{
				base.transform.localScale = new Vector3(this.effectData.scale, this.effectData.scale, this.effectData.scale);
			}
		}

		// Token: 0x040012C5 RID: 4805
		[NonSerialized]
		public EffectData effectData;

		// Token: 0x040012C6 RID: 4806
		[Tooltip("Positions the effect at the transform referenced by the effect data if available.")]
		public bool positionAtReferencedTransform;

		// Token: 0x040012C7 RID: 4807
		[Tooltip("Parents the effect to the transform object referenced by the effect data if available.")]
		public bool parentToReferencedTransform;

		// Token: 0x040012C8 RID: 4808
		[Tooltip("Causes this object to adopt the scale received in the effectdata.")]
		public bool applyScale;
	}
}
