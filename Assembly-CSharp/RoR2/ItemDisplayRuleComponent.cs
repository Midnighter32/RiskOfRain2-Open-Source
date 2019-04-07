using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000342 RID: 834
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class ItemDisplayRuleComponent : MonoBehaviour
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x0600113B RID: 4411 RVA: 0x00055C15 File Offset: 0x00053E15
		// (set) Token: 0x0600113C RID: 4412 RVA: 0x00055C1D File Offset: 0x00053E1D
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

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600113D RID: 4413 RVA: 0x00055C35 File Offset: 0x00053E35
		// (set) Token: 0x0600113E RID: 4414 RVA: 0x00055C3D File Offset: 0x00053E3D
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

		// Token: 0x0600113F RID: 4415 RVA: 0x00055C67 File Offset: 0x00053E67
		private void Start()
		{
			this.BuildPreview();
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x00055C6F File Offset: 0x00053E6F
		private void DestroyPreview()
		{
			if (this.prefabInstance)
			{
				UnityEngine.Object.DestroyImmediate(this.prefabInstance);
			}
			this.prefabInstance = null;
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x00055C90 File Offset: 0x00053E90
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

		// Token: 0x06001142 RID: 4418 RVA: 0x00055D3C File Offset: 0x00053F3C
		private static void SetPreviewFlags(Transform transform)
		{
			transform.gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
			foreach (object obj in transform)
			{
				ItemDisplayRuleComponent.SetPreviewFlags((Transform)obj);
			}
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x00055D9C File Offset: 0x00053F9C
		private void OnDestroy()
		{
			this.DestroyPreview();
		}

		// Token: 0x0400154B RID: 5451
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x0400154C RID: 5452
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x0400154D RID: 5453
		public LimbFlags limbMask;

		// Token: 0x0400154E RID: 5454
		[SerializeField]
		[HideInInspector]
		private ItemDisplayRuleType _ruleType;

		// Token: 0x0400154F RID: 5455
		public string nameInLocator;

		// Token: 0x04001550 RID: 5456
		[SerializeField]
		[HideInInspector]
		private GameObject _prefab;

		// Token: 0x04001551 RID: 5457
		private GameObject prefabInstance;
	}
}
