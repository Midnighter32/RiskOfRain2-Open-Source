using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000384 RID: 900
	public class PickupDisplay : MonoBehaviour
	{
		// Token: 0x060012BC RID: 4796 RVA: 0x0005BDF5 File Offset: 0x00059FF5
		public void SetPickupIndex(PickupIndex newPickupIndex, bool newHidden = false)
		{
			if (this.pickupIndex == newPickupIndex && this.hidden == newHidden)
			{
				return;
			}
			this.pickupIndex = newPickupIndex;
			this.hidden = newHidden;
			this.RebuildModel();
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0005BE23 File Offset: 0x0005A023
		private void DestroyModel()
		{
			if (this.modelObject)
			{
				UnityEngine.Object.Destroy(this.modelObject);
				this.modelObject = null;
				this.modelRenderer = null;
			}
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0005BE4C File Offset: 0x0005A04C
		private void RebuildModel()
		{
			GameObject y = this.hidden ? this.pickupIndex.GetHiddenPickupDisplayPrefab() : this.pickupIndex.GetPickupDisplayPrefab();
			if (this.modelPrefab == y)
			{
				return;
			}
			this.DestroyModel();
			this.modelPrefab = y;
			this.modelScale = base.transform.lossyScale.x;
			if (this.modelPrefab != null)
			{
				this.modelObject = UnityEngine.Object.Instantiate<GameObject>(this.modelPrefab);
				this.modelRenderer = this.modelObject.GetComponentInChildren<Renderer>();
				if (this.modelRenderer)
				{
					this.modelObject.transform.rotation = Quaternion.identity;
					Vector3 size = this.modelRenderer.bounds.size;
					float f = size.x * size.y * size.z;
					this.modelScale *= Mathf.Pow(PickupDisplay.idealVolume, 0.33333334f) / Mathf.Pow(f, 0.33333334f);
					if (this.highlight)
					{
						this.highlight.targetRenderer = this.modelRenderer;
						this.highlight.isOn = true;
						this.highlight.pickupIndex = this.pickupIndex;
					}
				}
				this.modelObject.transform.parent = base.transform;
				this.modelObject.transform.localPosition = this.localModelPivotPosition;
				this.modelObject.transform.localRotation = Quaternion.identity;
				this.modelObject.transform.localScale = new Vector3(this.modelScale, this.modelScale, this.modelScale);
			}
			if (this.pickupIndex.itemIndex != ItemIndex.None)
			{
				switch (ItemCatalog.GetItemDef(this.pickupIndex.itemIndex).tier)
				{
				case ItemTier.Tier1:
					if (this.tier1ParticleEffect)
					{
						this.tier1ParticleEffect.SetActive(true);
					}
					break;
				case ItemTier.Tier2:
					if (this.tier2ParticleEffect)
					{
						this.tier2ParticleEffect.SetActive(true);
					}
					break;
				case ItemTier.Tier3:
					if (this.tier3ParticleEffect)
					{
						this.tier3ParticleEffect.SetActive(true);
					}
					break;
				}
			}
			else if (this.pickupIndex.equipmentIndex != EquipmentIndex.None && this.equipmentParticleEffect)
			{
				this.equipmentParticleEffect.SetActive(true);
			}
			if (this.bossParticleEffect)
			{
				this.bossParticleEffect.SetActive(this.pickupIndex.IsBoss());
			}
			if (this.lunarParticleEffect)
			{
				this.lunarParticleEffect.SetActive(this.pickupIndex.IsLunar());
			}
			foreach (ParticleSystem particleSystem in this.coloredParticleSystems)
			{
				particleSystem.gameObject.SetActive(this.modelPrefab != null);
				particleSystem.main.startColor = this.pickupIndex.GetPickupColor();
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060012BF RID: 4799 RVA: 0x0005C145 File Offset: 0x0005A345
		// (set) Token: 0x060012C0 RID: 4800 RVA: 0x0005C14D File Offset: 0x0005A34D
		public Renderer modelRenderer { get; private set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060012C1 RID: 4801 RVA: 0x0005C156 File Offset: 0x0005A356
		private Vector3 localModelPivotPosition
		{
			get
			{
				return Vector3.up * this.verticalWave.Evaluate(this.localTime);
			}
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0005C173 File Offset: 0x0005A373
		private void Start()
		{
			this.localTime = 0f;
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0005C180 File Offset: 0x0005A380
		private void Update()
		{
			this.localTime += Time.deltaTime;
			if (this.modelObject)
			{
				Transform transform = this.modelObject.transform;
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.y = this.spinSpeed * this.localTime;
				transform.localEulerAngles = localEulerAngles;
				transform.localPosition = this.localModelPivotPosition;
			}
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0005C1E4 File Offset: 0x0005A3E4
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
			Gizmos.DrawWireCube(Vector3.zero, PickupDisplay.idealModelBox);
			Gizmos.matrix = matrix;
		}

		// Token: 0x0400168B RID: 5771
		[Tooltip("The vertical motion of the display model.")]
		public Wave verticalWave;

		// Token: 0x0400168C RID: 5772
		[Tooltip("The speed in degrees/second at which the display model rotates about the y axis.")]
		public float spinSpeed = 75f;

		// Token: 0x0400168D RID: 5773
		public GameObject tier1ParticleEffect;

		// Token: 0x0400168E RID: 5774
		public GameObject tier2ParticleEffect;

		// Token: 0x0400168F RID: 5775
		public GameObject tier3ParticleEffect;

		// Token: 0x04001690 RID: 5776
		public GameObject equipmentParticleEffect;

		// Token: 0x04001691 RID: 5777
		public GameObject lunarParticleEffect;

		// Token: 0x04001692 RID: 5778
		public GameObject bossParticleEffect;

		// Token: 0x04001693 RID: 5779
		[Tooltip("The particle system to tint.")]
		public ParticleSystem[] coloredParticleSystems;

		// Token: 0x04001694 RID: 5780
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x04001695 RID: 5781
		private bool hidden;

		// Token: 0x04001696 RID: 5782
		public Highlight highlight;

		// Token: 0x04001697 RID: 5783
		private static readonly Vector3 idealModelBox = Vector3.one;

		// Token: 0x04001698 RID: 5784
		private static readonly float idealVolume = PickupDisplay.idealModelBox.x * PickupDisplay.idealModelBox.y * PickupDisplay.idealModelBox.z;

		// Token: 0x04001699 RID: 5785
		private GameObject modelObject;

		// Token: 0x0400169B RID: 5787
		private GameObject modelPrefab;

		// Token: 0x0400169C RID: 5788
		private float modelScale;

		// Token: 0x0400169D RID: 5789
		private float localTime;
	}
}
