using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000353 RID: 851
	public class TemporaryOverlay : MonoBehaviour
	{
		// Token: 0x060014B4 RID: 5300 RVA: 0x000586AD File Offset: 0x000568AD
		private void Start()
		{
			this.SetupMaterial();
			if (this.inspectorCharacterModel)
			{
				this.AddToCharacerModel(this.inspectorCharacterModel);
			}
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x000586CE File Offset: 0x000568CE
		private void SetupMaterial()
		{
			if (!this.materialInstance)
			{
				this.materialInstance = new Material(this.originalMaterial);
			}
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x000586EE File Offset: 0x000568EE
		public void AddToCharacerModel(CharacterModel characterModel)
		{
			this.SetupMaterial();
			if (characterModel)
			{
				characterModel.temporaryOverlays.Add(this);
				this.isAssigned = true;
				this.assignedCharacterModel = characterModel;
			}
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x00058718 File Offset: 0x00056918
		public void RemoveFromCharacterModel()
		{
			if (this.assignedCharacterModel)
			{
				this.assignedCharacterModel.temporaryOverlays.Remove(this);
				this.isAssigned = false;
				this.assignedCharacterModel = null;
			}
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x00058747 File Offset: 0x00056947
		private void OnDestroy()
		{
			this.RemoveFromCharacterModel();
			if (this.materialInstance)
			{
				UnityEngine.Object.Destroy(this.materialInstance);
			}
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x00058768 File Offset: 0x00056968
		private void Update()
		{
			if (this.animateShaderAlpha)
			{
				this.stopwatch += Time.deltaTime;
				float value = this.alphaCurve.Evaluate(this.stopwatch / this.duration);
				this.materialInstance.SetFloat("_ExternalAlpha", value);
				if (this.stopwatch >= this.duration && (this.destroyComponentOnEnd || this.destroyObjectOnEnd))
				{
					if (this.destroyEffectPrefab)
					{
						ChildLocator component = base.GetComponent<ChildLocator>();
						if (component)
						{
							Transform transform = component.FindChild(this.destroyEffectChildString);
							if (transform)
							{
								EffectManager.SpawnEffect(this.destroyEffectPrefab, new EffectData
								{
									origin = transform.position,
									rotation = transform.rotation
								}, true);
							}
						}
					}
					if (this.destroyObjectOnEnd)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this);
				}
			}
		}

		// Token: 0x0400133D RID: 4925
		public Material originalMaterial;

		// Token: 0x0400133E RID: 4926
		[HideInInspector]
		public Material materialInstance;

		// Token: 0x0400133F RID: 4927
		private bool isAssigned;

		// Token: 0x04001340 RID: 4928
		private CharacterModel assignedCharacterModel;

		// Token: 0x04001341 RID: 4929
		public CharacterModel inspectorCharacterModel;

		// Token: 0x04001342 RID: 4930
		public bool animateShaderAlpha;

		// Token: 0x04001343 RID: 4931
		public AnimationCurve alphaCurve;

		// Token: 0x04001344 RID: 4932
		public float duration;

		// Token: 0x04001345 RID: 4933
		public bool destroyComponentOnEnd;

		// Token: 0x04001346 RID: 4934
		public bool destroyObjectOnEnd;

		// Token: 0x04001347 RID: 4935
		public GameObject destroyEffectPrefab;

		// Token: 0x04001348 RID: 4936
		public string destroyEffectChildString;

		// Token: 0x04001349 RID: 4937
		private float stopwatch;
	}
}
