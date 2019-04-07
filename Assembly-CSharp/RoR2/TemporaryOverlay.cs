using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FB RID: 1019
	public class TemporaryOverlay : MonoBehaviour
	{
		// Token: 0x060016B5 RID: 5813 RVA: 0x0006C3E1 File Offset: 0x0006A5E1
		private void Start()
		{
			this.SetupMaterial();
			if (this.inspectorCharacterModel)
			{
				this.AddToCharacerModel(this.inspectorCharacterModel);
			}
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x0006C402 File Offset: 0x0006A602
		private void SetupMaterial()
		{
			if (!this.materialInstance)
			{
				this.materialInstance = new Material(this.originalMaterial);
			}
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x0006C422 File Offset: 0x0006A622
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

		// Token: 0x060016B8 RID: 5816 RVA: 0x0006C44C File Offset: 0x0006A64C
		public void RemoveFromCharacterModel()
		{
			if (this.assignedCharacterModel)
			{
				this.assignedCharacterModel.temporaryOverlays.Remove(this);
				this.isAssigned = false;
				this.assignedCharacterModel = null;
			}
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x0006C47B File Offset: 0x0006A67B
		private void OnDestroy()
		{
			this.RemoveFromCharacterModel();
			if (this.materialInstance)
			{
				UnityEngine.Object.Destroy(this.materialInstance);
			}
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x0006C49C File Offset: 0x0006A69C
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
								EffectManager.instance.SpawnEffect(this.destroyEffectPrefab, new EffectData
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

		// Token: 0x040019D6 RID: 6614
		public Material originalMaterial;

		// Token: 0x040019D7 RID: 6615
		[HideInInspector]
		public Material materialInstance;

		// Token: 0x040019D8 RID: 6616
		private bool isAssigned;

		// Token: 0x040019D9 RID: 6617
		private CharacterModel assignedCharacterModel;

		// Token: 0x040019DA RID: 6618
		public CharacterModel inspectorCharacterModel;

		// Token: 0x040019DB RID: 6619
		public bool animateShaderAlpha;

		// Token: 0x040019DC RID: 6620
		public AnimationCurve alphaCurve;

		// Token: 0x040019DD RID: 6621
		public float duration;

		// Token: 0x040019DE RID: 6622
		public bool destroyComponentOnEnd;

		// Token: 0x040019DF RID: 6623
		public bool destroyObjectOnEnd;

		// Token: 0x040019E0 RID: 6624
		public GameObject destroyEffectPrefab;

		// Token: 0x040019E1 RID: 6625
		public string destroyEffectChildString;

		// Token: 0x040019E2 RID: 6626
		private float stopwatch;
	}
}
