using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F0 RID: 496
	[DisallowMultipleComponent]
	public class EffectComponent : MonoBehaviour
	{
		// Token: 0x06000A61 RID: 2657 RVA: 0x0002D910 File Offset: 0x0002BB10
		private void Start()
		{
			if (this.effectData == null)
			{
				Debug.LogErrorFormat(base.gameObject, "Object {0} should not be instantiated by means other than EffectManager.instance.SpawnEffect.", new object[]
				{
					base.gameObject
				});
			}
			Transform transform = null;
			if (this.positionAtReferencedTransform | this.parentToReferencedTransform)
			{
				transform = this.effectData.ResolveChildLocatorTransformReference();
			}
			if (transform)
			{
				if (this.positionAtReferencedTransform)
				{
					base.transform.position = transform.position;
					base.transform.rotation = transform.rotation;
				}
				if (this.parentToReferencedTransform)
				{
					base.transform.SetParent(transform, true);
				}
			}
			if (this.applyScale)
			{
				float scale = this.effectData.scale;
				base.transform.localScale = new Vector3(scale, scale, scale);
			}
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0002D9CF File Offset: 0x0002BBCF
		private void OnValidate()
		{
			if (!Application.isPlaying && this.effectIndex != EffectIndex.Invalid)
			{
				this.effectIndex = EffectIndex.Invalid;
			}
		}

		// Token: 0x04000ABF RID: 2751
		[HideInInspector]
		[Tooltip("This is assigned to the prefab automatically by EffectCatalog at runtime. Do not set this value manually.")]
		public EffectIndex effectIndex = EffectIndex.Invalid;

		// Token: 0x04000AC0 RID: 2752
		[NonSerialized]
		public EffectData effectData;

		// Token: 0x04000AC1 RID: 2753
		[Tooltip("Positions the effect at the transform referenced by the effect data if available.")]
		public bool positionAtReferencedTransform;

		// Token: 0x04000AC2 RID: 2754
		[Tooltip("Parents the effect to the transform object referenced by the effect data if available.")]
		public bool parentToReferencedTransform;

		// Token: 0x04000AC3 RID: 2755
		[Tooltip("Causes this object to adopt the scale received in the effectdata.")]
		public bool applyScale;

		// Token: 0x04000AC4 RID: 2756
		[Tooltip("The sound to play whenever this effect is dispatched, regardless of whether or not it actually ends up spawning.")]
		public string soundName;
	}
}
