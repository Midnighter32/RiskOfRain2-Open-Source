using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000266 RID: 614
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class ItemDisplayRuleComponent : MonoBehaviour
	{
		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x0003CFA5 File Offset: 0x0003B1A5
		// (set) Token: 0x06000D95 RID: 3477 RVA: 0x0003CFAD File Offset: 0x0003B1AD
		public ItemDisplayRuleType ruleType
		{
			get
			{
				return this._ruleType;
			}
			set
			{
				this._ruleType = value;
				if (this._ruleType != ItemDisplayRuleType.ParentedPrefab)
				{
					this.prefab = null;
				}
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000D96 RID: 3478 RVA: 0x0003CFC5 File Offset: 0x0003B1C5
		// (set) Token: 0x06000D97 RID: 3479 RVA: 0x0003CFCD File Offset: 0x0003B1CD
		public GameObject prefab
		{
			get
			{
				return this._prefab;
			}
			set
			{
				if (!this.prefabInstance || this._prefab != value)
				{
					this._prefab = value;
					this.BuildPreview();
				}
			}
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x0003CFF7 File Offset: 0x0003B1F7
		private void Start()
		{
			this.BuildPreview();
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x0003CFFF File Offset: 0x0003B1FF
		private void DestroyPreview()
		{
			if (this.prefabInstance)
			{
				UnityEngine.Object.DestroyImmediate(this.prefabInstance);
			}
			this.prefabInstance = null;
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x0003D020 File Offset: 0x0003B220
		private void BuildPreview()
		{
			this.DestroyPreview();
			if (this.prefab)
			{
				this.prefabInstance = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
				this.prefabInstance.name = "Preview";
				this.prefabInstance.transform.parent = base.transform;
				this.prefabInstance.transform.localPosition = Vector3.zero;
				this.prefabInstance.transform.localRotation = Quaternion.identity;
				this.prefabInstance.transform.localScale = Vector3.one;
				ItemDisplayRuleComponent.SetPreviewFlags(this.prefabInstance.transform);
			}
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x0003D0CC File Offset: 0x0003B2CC
		private static void SetPreviewFlags(Transform transform)
		{
			transform.gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
			foreach (object obj in transform)
			{
				ItemDisplayRuleComponent.SetPreviewFlags((Transform)obj);
			}
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x0003D12C File Offset: 0x0003B32C
		private void OnDestroy()
		{
			this.DestroyPreview();
		}

		// Token: 0x04000D9A RID: 3482
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x04000D9B RID: 3483
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x04000D9C RID: 3484
		public LimbFlags limbMask;

		// Token: 0x04000D9D RID: 3485
		[SerializeField]
		[HideInInspector]
		private ItemDisplayRuleType _ruleType;

		// Token: 0x04000D9E RID: 3486
		public string nameInLocator;

		// Token: 0x04000D9F RID: 3487
		[SerializeField]
		[HideInInspector]
		private GameObject _prefab;

		// Token: 0x04000DA0 RID: 3488
		private GameObject prefabInstance;
	}
}
