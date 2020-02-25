using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BC RID: 700
	public class PickupDisplay : MonoBehaviour
	{
		// Token: 0x06000FC8 RID: 4040 RVA: 0x00045463 File Offset: 0x00043663
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

		// Token: 0x06000FC9 RID: 4041 RVA: 0x00045491 File Offset: 0x00043691
		private void DestroyModel()
		{
			if (this.modelObject)
			{
				UnityEngine.Object.Destroy(this.modelObject);
				this.modelObject = null;
				this.modelRenderer = null;
			}
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x000454BC File Offset: 0x000436BC
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
					float num = size.x * size.y * size.z;
					if (num <= 1E-45f)
					{
						Debug.LogError("PickupDisplay bounds are zero! This is not allowed!");
						num = 1f;
					}
					this.modelScale *= Mathf.Pow(PickupDisplay.idealVolume, 0.33333334f) / Mathf.Pow(num, 0.33333334f);
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
			if (this.tier1ParticleEffect)
			{
				this.tier1ParticleEffect.SetActive(false);
			}
			if (this.tier2ParticleEffect)
			{
				this.tier2ParticleEffect.SetActive(false);
			}
			if (this.tier3ParticleEffect)
			{
				this.tier3ParticleEffect.SetActive(false);
			}
			if (this.equipmentParticleEffect)
			{
				this.equipmentParticleEffect.SetActive(false);
			}
			if (this.lunarParticleEffect)
			{
				this.lunarParticleEffect.SetActive(false);
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

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x0004584A File Offset: 0x00043A4A
		// (set) Token: 0x06000FCC RID: 4044 RVA: 0x00045852 File Offset: 0x00043A52
		public Renderer modelRenderer { get; private set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000FCD RID: 4045 RVA: 0x0004585B File Offset: 0x00043A5B
		private Vector3 localModelPivotPosition
		{
			get
			{
				return Vector3.up * this.verticalWave.Evaluate(this.localTime);
			}
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x00045878 File Offset: 0x00043A78
		private void Start()
		{
			this.localTime = 0f;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x00045888 File Offset: 0x00043A88
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

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000458EC File Offset: 0x00043AEC
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
			Gizmos.DrawWireCube(Vector3.zero, PickupDisplay.idealModelBox);
			Gizmos.matrix = matrix;
		}

		// Token: 0x04000F48 RID: 3912
		[Tooltip("The vertical motion of the display model.")]
		public Wave verticalWave;

		// Token: 0x04000F49 RID: 3913
		[Tooltip("The speed in degrees/second at which the display model rotates about the y axis.")]
		public float spinSpeed = 75f;

		// Token: 0x04000F4A RID: 3914
		public GameObject tier1ParticleEffect;

		// Token: 0x04000F4B RID: 3915
		public GameObject tier2ParticleEffect;

		// Token: 0x04000F4C RID: 3916
		public GameObject tier3ParticleEffect;

		// Token: 0x04000F4D RID: 3917
		public GameObject equipmentParticleEffect;

		// Token: 0x04000F4E RID: 3918
		public GameObject lunarParticleEffect;

		// Token: 0x04000F4F RID: 3919
		public GameObject bossParticleEffect;

		// Token: 0x04000F50 RID: 3920
		[Tooltip("The particle system to tint.")]
		public ParticleSystem[] coloredParticleSystems;

		// Token: 0x04000F51 RID: 3921
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x04000F52 RID: 3922
		private bool hidden;

		// Token: 0x04000F53 RID: 3923
		public Highlight highlight;

		// Token: 0x04000F54 RID: 3924
		private static readonly Vector3 idealModelBox = Vector3.one;

		// Token: 0x04000F55 RID: 3925
		private static readonly float idealVolume = PickupDisplay.idealModelBox.x * PickupDisplay.idealModelBox.y * PickupDisplay.idealModelBox.z;

		// Token: 0x04000F56 RID: 3926
		private GameObject modelObject;

		// Token: 0x04000F58 RID: 3928
		private GameObject modelPrefab;

		// Token: 0x04000F59 RID: 3929
		private float modelScale;

		// Token: 0x04000F5A RID: 3930
		private float localTime;
	}
}
