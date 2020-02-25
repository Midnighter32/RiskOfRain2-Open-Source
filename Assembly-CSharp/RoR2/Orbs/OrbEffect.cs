using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.Orbs
{
	// Token: 0x020004D6 RID: 1238
	[RequireComponent(typeof(EffectComponent))]
	public class OrbEffect : MonoBehaviour
	{
		// Token: 0x06001D95 RID: 7573 RVA: 0x0007E2C4 File Offset: 0x0007C4C4
		private void Start()
		{
			EffectComponent component = base.GetComponent<EffectComponent>();
			this.startPosition = component.effectData.origin;
			this.previousPosition = this.startPosition;
			GameObject gameObject = component.effectData.ResolveHurtBoxReference();
			this.targetTransform = (gameObject ? gameObject.transform : null);
			this.duration = component.effectData.genericFloat;
			if (this.duration == 0f)
			{
				Debug.LogFormat("Zero duration for effect \"{0}\"", new object[]
				{
					base.gameObject.name
				});
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			this.lastKnownTargetPosition = (this.targetTransform ? this.targetTransform.position : this.startPosition);
			if (this.startEffect)
			{
				EffectData effectData = new EffectData
				{
					origin = base.transform.position,
					scale = this.startEffectScale
				};
				if (this.startEffectCopiesRotation)
				{
					effectData.rotation = base.transform.rotation;
				}
				EffectManager.SpawnEffect(this.startEffect, effectData, false);
			}
			this.startVelocity.x = Mathf.Lerp(this.startVelocity1.x, this.startVelocity2.x, UnityEngine.Random.value);
			this.startVelocity.y = Mathf.Lerp(this.startVelocity1.y, this.startVelocity2.y, UnityEngine.Random.value);
			this.startVelocity.z = Mathf.Lerp(this.startVelocity1.z, this.startVelocity2.z, UnityEngine.Random.value);
			this.endVelocity.x = Mathf.Lerp(this.endVelocity1.x, this.endVelocity2.x, UnityEngine.Random.value);
			this.endVelocity.y = Mathf.Lerp(this.endVelocity1.y, this.endVelocity2.y, UnityEngine.Random.value);
			this.endVelocity.z = Mathf.Lerp(this.endVelocity1.z, this.endVelocity2.z, UnityEngine.Random.value);
			this.UpdateOrb(0f);
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x0007E4E9 File Offset: 0x0007C6E9
		private void Update()
		{
			this.UpdateOrb(Time.deltaTime);
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x0007E4F8 File Offset: 0x0007C6F8
		private void UpdateOrb(float deltaTime)
		{
			if (this.targetTransform)
			{
				this.lastKnownTargetPosition = this.targetTransform.position;
			}
			float num = Mathf.Clamp01(this.age / this.duration);
			float num2 = this.movementCurve.Evaluate(num);
			Vector3 vector = Vector3.Lerp(this.startPosition + this.startVelocity * num2, this.lastKnownTargetPosition + this.endVelocity * (1f - num2), num2);
			base.transform.position = vector;
			if (this.faceMovement && vector != this.previousPosition)
			{
				base.transform.forward = vector - this.previousPosition;
			}
			this.UpdateBezier();
			if (num == 1f || (this.callArrivalIfTargetIsGone && this.targetTransform == null))
			{
				this.onArrival.Invoke();
				if (this.endEffect)
				{
					EffectData effectData = new EffectData
					{
						origin = base.transform.position,
						scale = this.endEffectScale
					};
					if (this.endEffectCopiesRotation)
					{
						effectData.rotation = base.transform.rotation;
					}
					EffectManager.SpawnEffect(this.endEffect, effectData, false);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.previousPosition = vector;
			this.age += deltaTime;
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x0007E65C File Offset: 0x0007C85C
		private void UpdateBezier()
		{
			if (this.bezierCurveLine)
			{
				this.bezierCurveLine.p1 = this.startPosition;
				this.bezierCurveLine.v0 = this.endVelocity;
				this.bezierCurveLine.v1 = this.startVelocity;
				this.bezierCurveLine.UpdateBezier(0f);
			}
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0007E6B9 File Offset: 0x0007C8B9
		public void InstantiatePrefab(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0007E6D8 File Offset: 0x0007C8D8
		public void InstantiateEffect(GameObject prefab)
		{
			EffectManager.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position
			}, false);
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0007E6F7 File Offset: 0x0007C8F7
		public void InstantiateEffectCopyRotation(GameObject prefab)
		{
			EffectManager.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = base.transform.rotation
			}, false);
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0007E727 File Offset: 0x0007C927
		public void InstantiateEffectOppositeFacing(GameObject prefab)
		{
			EffectManager.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = Util.QuaternionSafeLookRotation(-base.transform.forward)
			}, false);
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x0007E761 File Offset: 0x0007C961
		public void InstantiatePrefabOppositeFacing(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, Util.QuaternionSafeLookRotation(-base.transform.forward));
		}

		// Token: 0x04001AC2 RID: 6850
		private Transform targetTransform;

		// Token: 0x04001AC3 RID: 6851
		private float duration;

		// Token: 0x04001AC4 RID: 6852
		private Vector3 startPosition;

		// Token: 0x04001AC5 RID: 6853
		private Vector3 previousPosition;

		// Token: 0x04001AC6 RID: 6854
		private Vector3 lastKnownTargetPosition;

		// Token: 0x04001AC7 RID: 6855
		private float age;

		// Token: 0x04001AC8 RID: 6856
		[Header("Curve Parameters")]
		public Vector3 startVelocity1;

		// Token: 0x04001AC9 RID: 6857
		public Vector3 startVelocity2;

		// Token: 0x04001ACA RID: 6858
		public Vector3 endVelocity1;

		// Token: 0x04001ACB RID: 6859
		public Vector3 endVelocity2;

		// Token: 0x04001ACC RID: 6860
		private Vector3 startVelocity;

		// Token: 0x04001ACD RID: 6861
		private Vector3 endVelocity;

		// Token: 0x04001ACE RID: 6862
		public AnimationCurve movementCurve;

		// Token: 0x04001ACF RID: 6863
		public BezierCurveLine bezierCurveLine;

		// Token: 0x04001AD0 RID: 6864
		public bool faceMovement = true;

		// Token: 0x04001AD1 RID: 6865
		public bool callArrivalIfTargetIsGone;

		// Token: 0x04001AD2 RID: 6866
		[Header("Start Effect")]
		[Tooltip("An effect prefab to spawn on Start")]
		public GameObject startEffect;

		// Token: 0x04001AD3 RID: 6867
		public float startEffectScale = 1f;

		// Token: 0x04001AD4 RID: 6868
		public bool startEffectCopiesRotation;

		// Token: 0x04001AD5 RID: 6869
		[Tooltip("An effect prefab to spawn on end")]
		[Header("End Effect")]
		public GameObject endEffect;

		// Token: 0x04001AD6 RID: 6870
		public float endEffectScale = 1f;

		// Token: 0x04001AD7 RID: 6871
		public bool endEffectCopiesRotation;

		// Token: 0x04001AD8 RID: 6872
		[Header("Arrival Behavior")]
		public UnityEvent onArrival;
	}
}
