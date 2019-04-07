using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000258 RID: 600
	public class AnimationEvents : MonoBehaviour
	{
		// Token: 0x06000B2A RID: 2858 RVA: 0x00037700 File Offset: 0x00035900
		private void Start()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			this.entityLocator = base.GetComponent<EntityLocator>();
			this.meshRenderer = base.GetComponentInChildren<Renderer>();
			this.characterModel = base.GetComponent<CharacterModel>();
			if (this.characterModel)
			{
				this.body = this.characterModel.body.gameObject;
				if (this.body)
				{
					this.modelLocator = this.body.GetComponent<ModelLocator>();
				}
			}
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x00037780 File Offset: 0x00035980
		public void UpdateIKState(AnimationEvent animationEvent)
		{
			IIKTargetBehavior component = this.childLocator.FindChild(animationEvent.stringParameter).GetComponent<IIKTargetBehavior>();
			if (component != null)
			{
				component.UpdateIKState(animationEvent.intParameter);
			}
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x000377B3 File Offset: 0x000359B3
		public void PlaySound(string soundString)
		{
			Util.PlaySound(soundString, this.soundCenter ? this.soundCenter : this.body);
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x000377D7 File Offset: 0x000359D7
		public void NormalizeToFloor()
		{
			if (this.modelLocator)
			{
				this.modelLocator.normalizeToFloor = true;
			}
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x000377F4 File Offset: 0x000359F4
		public void CreateEffect(AnimationEvent animationEvent)
		{
			Transform transform = this.childLocator.FindChild(animationEvent.stringParameter);
			EffectData effectData = new EffectData();
			effectData.origin = transform.position;
			effectData.SetChildLocatorTransformReference(base.gameObject, this.childLocator.FindChildIndex(animationEvent.stringParameter));
			EffectManager.instance.SpawnEffect((GameObject)animationEvent.objectReferenceParameter, effectData, animationEvent.intParameter != 0);
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x00037864 File Offset: 0x00035A64
		public void CreatePrefab(AnimationEvent animationEvent)
		{
			GameObject gameObject = (GameObject)animationEvent.objectReferenceParameter;
			string stringParameter = animationEvent.stringParameter;
			int intParameter = animationEvent.intParameter;
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild(stringParameter);
				if (transform)
				{
					if (intParameter == 0)
					{
						UnityEngine.Object.Instantiate<GameObject>(gameObject, transform.position, Quaternion.identity);
						return;
					}
					UnityEngine.Object.Instantiate<GameObject>(gameObject, transform.position, transform.rotation, transform);
					return;
				}
				else if (gameObject)
				{
					UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation);
					return;
				}
			}
			else if (gameObject)
			{
				UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation);
			}
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x00037924 File Offset: 0x00035B24
		public void ItemDrop()
		{
			if (NetworkServer.active && this.entityLocator)
			{
				ChestBehavior component = this.entityLocator.entity.GetComponent<ChestBehavior>();
				if (component)
				{
					component.ItemDrop();
					return;
				}
				Debug.Log("Parent has no item drops!");
			}
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x00037970 File Offset: 0x00035B70
		public void BeginPrint(AnimationEvent animationEvent)
		{
			if (this.meshRenderer)
			{
				Material material = (Material)animationEvent.objectReferenceParameter;
				float floatParameter = animationEvent.floatParameter;
				float maxPrintHeight = (float)animationEvent.intParameter;
				this.meshRenderer.material = material;
				this.printTime = 0f;
				MaterialPropertyBlock printPropertyBlock = new MaterialPropertyBlock();
				base.StartCoroutine(this.startPrint(floatParameter, maxPrintHeight, printPropertyBlock));
			}
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x000379D2 File Offset: 0x00035BD2
		private IEnumerator startPrint(float maxPrintTime, float maxPrintHeight, MaterialPropertyBlock printPropertyBlock)
		{
			if (this.meshRenderer)
			{
				while (this.printHeight < maxPrintHeight)
				{
					this.printTime += Time.deltaTime;
					this.printHeight = this.printTime / maxPrintTime * maxPrintHeight;
					this.meshRenderer.GetPropertyBlock(printPropertyBlock);
					printPropertyBlock.Clear();
					printPropertyBlock.SetFloat("_SliceHeight", this.printHeight);
					this.meshRenderer.SetPropertyBlock(printPropertyBlock);
					yield return new WaitForEndOfFrame();
				}
			}
			yield break;
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x000379F8 File Offset: 0x00035BF8
		public void SetChildEnable(AnimationEvent animationEvent)
		{
			string stringParameter = animationEvent.stringParameter;
			bool active = animationEvent.intParameter > 0;
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild(stringParameter);
				if (transform)
				{
					transform.gameObject.SetActive(active);
				}
			}
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x00037A48 File Offset: 0x00035C48
		public void SwapMaterial(AnimationEvent animationEvent)
		{
			Material material = (Material)animationEvent.objectReferenceParameter;
			if (this.meshRenderer)
			{
				this.meshRenderer.material = material;
			}
		}

		// Token: 0x04000F3C RID: 3900
		public GameObject soundCenter;

		// Token: 0x04000F3D RID: 3901
		private GameObject body;

		// Token: 0x04000F3E RID: 3902
		private CharacterModel characterModel;

		// Token: 0x04000F3F RID: 3903
		private ChildLocator childLocator;

		// Token: 0x04000F40 RID: 3904
		private EntityLocator entityLocator;

		// Token: 0x04000F41 RID: 3905
		private Renderer meshRenderer;

		// Token: 0x04000F42 RID: 3906
		private ModelLocator modelLocator;

		// Token: 0x04000F43 RID: 3907
		private float printHeight;

		// Token: 0x04000F44 RID: 3908
		private float printTime;
	}
}
