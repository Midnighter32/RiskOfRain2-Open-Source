using System;
using ThreeEyedGames;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000148 RID: 328
	public class AnimateShaderAlpha : MonoBehaviour
	{
		// Token: 0x060005CF RID: 1487 RVA: 0x000180CC File Offset: 0x000162CC
		private void Start()
		{
			this.targetRenderer = base.GetComponent<Renderer>();
			if (this.targetRenderer)
			{
				this.materials = this.targetRenderer.materials;
			}
			if (this.decal)
			{
				this.initialFade = this.decal.Fade;
			}
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x00018124 File Offset: 0x00016324
		private void Update()
		{
			this.time = Mathf.Min(this.timeMax, this.time + Time.deltaTime);
			float num = this.alphaCurve.Evaluate(this.time / this.timeMax);
			if (this.decal)
			{
				this.decal.Fade = num * this.initialFade;
			}
			else
			{
				foreach (Material material in this.materials)
				{
					this._propBlock = new MaterialPropertyBlock();
					this.targetRenderer.GetPropertyBlock(this._propBlock);
					this._propBlock.SetFloat("_ExternalAlpha", num);
					this.targetRenderer.SetPropertyBlock(this._propBlock);
				}
			}
			if (this.time >= this.timeMax)
			{
				if (this.disableOnEnd)
				{
					base.enabled = false;
				}
				if (this.destroyOnEnd)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x0400064F RID: 1615
		public AnimationCurve alphaCurve;

		// Token: 0x04000650 RID: 1616
		private Renderer targetRenderer;

		// Token: 0x04000651 RID: 1617
		private MaterialPropertyBlock _propBlock;

		// Token: 0x04000652 RID: 1618
		private Material[] materials;

		// Token: 0x04000653 RID: 1619
		public float timeMax = 5f;

		// Token: 0x04000654 RID: 1620
		[Tooltip("Optional field if you want to animate Decal 'Fade' rather than renderer _ExternalAlpha.")]
		public Decal decal;

		// Token: 0x04000655 RID: 1621
		public bool destroyOnEnd;

		// Token: 0x04000656 RID: 1622
		public bool disableOnEnd;

		// Token: 0x04000657 RID: 1623
		[HideInInspector]
		public float time;

		// Token: 0x04000658 RID: 1624
		private float initialFade;
	}
}
