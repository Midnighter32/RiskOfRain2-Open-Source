using System;
using System.Collections;
using Generics.Dynamics;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000149 RID: 329
	public class AnimationEvents : MonoBehaviour
	{
		// Token: 0x060005D2 RID: 1490 RVA: 0x00018224 File Offset: 0x00016424
		private void Start()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			this.entityLocator = base.GetComponent<EntityLocator>();
			this.meshRenderer = base.GetComponentInChildren<Renderer>();
			this.characterModel = base.GetComponent<CharacterModel>();
			if (this.characterModel && this.characterModel.body)
			{
				this.bodyObject = this.characterModel.body.gameObject;
				this.modelLocator = this.bodyObject.GetComponent<ModelLocator>();
			}
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x000182A8 File Offset: 0x000164A8
		public void UpdateIKState(AnimationEvent animationEvent)
		{
			IIKTargetBehavior component = this.childLocator.FindChild(animationEvent.stringParameter).GetComponent<IIKTargetBehavior>();
			if (component != null)
			{
				component.UpdateIKState(animationEvent.intParameter);
			}
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x000182DB File Offset: 0x000164DB
		public void PlaySound(string soundString)
		{
			Util.PlaySound(soundString, this.soundCenter ? this.soundCenter : this.bodyObject);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x000182FF File Offset: 0x000164FF
		public void NormalizeToFloor()
		{
			if (this.modelLocator)
			{
				this.modelLocator.normalizeToFloor = true;
			}
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0001831A File Offset: 0x0001651A
		public void SetIK(AnimationEvent animationEvent)
		{
			if (this.modelLocator)
			{
				this.modelLocator.modelTransform.GetComponent<InverseKinematics>().enabled = (animationEvent.intParameter != 0);
			}
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0001834C File Offset: 0x0001654C
		public void CreateEffect(AnimationEvent animationEvent)
		{
			Transform transform = base.transform;
			int num = -1;
			if (!string.IsNullOrEmpty(animationEvent.stringParameter))
			{
				num = this.childLocator.FindChildIndex(animationEvent.stringParameter);
				if (num != -1)
				{
					transform = this.childLocator.FindChild(num);
				}
			}
			bool transmit = animationEvent.intParameter != 0;
			EffectData effectData = new EffectData();
			effectData.origin = transform.position;
			effectData.SetChildLocatorTransformReference(this.bodyObject, num);
			EffectManager.SpawnEffect((GameObject)animationEvent.objectReferenceParameter, effectData, transmit);
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x000183D0 File Offset: 0x000165D0
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
					UnityEngine.Object.Instantiate<GameObject>(gameObject, transform.position, transform.rotation).transform.parent = transform;
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

		// Token: 0x060005D9 RID: 1497 RVA: 0x00018498 File Offset: 0x00016698
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

		// Token: 0x060005DA RID: 1498 RVA: 0x000184E4 File Offset: 0x000166E4
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

		// Token: 0x060005DB RID: 1499 RVA: 0x00018546 File Offset: 0x00016746
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

		// Token: 0x060005DC RID: 1500 RVA: 0x0001856C File Offset: 0x0001676C
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

		// Token: 0x060005DD RID: 1501 RVA: 0x000185BC File Offset: 0x000167BC
		public void SwapMaterial(AnimationEvent animationEvent)
		{
			Material material = (Material)animationEvent.objectReferenceParameter;
			if (this.meshRenderer)
			{
				this.meshRenderer.material = material;
			}
		}

		// Token: 0x04000659 RID: 1625
		public GameObject soundCenter;

		// Token: 0x0400065A RID: 1626
		private GameObject bodyObject;

		// Token: 0x0400065B RID: 1627
		private CharacterModel characterModel;

		// Token: 0x0400065C RID: 1628
		private ChildLocator childLocator;

		// Token: 0x0400065D RID: 1629
		private EntityLocator entityLocator;

		// Token: 0x0400065E RID: 1630
		private Renderer meshRenderer;

		// Token: 0x0400065F RID: 1631
		private ModelLocator modelLocator;

		// Token: 0x04000660 RID: 1632
		private float printHeight;

		// Token: 0x04000661 RID: 1633
		private float printTime;
	}
}
