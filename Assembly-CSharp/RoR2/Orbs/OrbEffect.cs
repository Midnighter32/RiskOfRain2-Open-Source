using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.Orbs
{
	// Token: 0x0200051C RID: 1308
	[RequireComponent(typeof(EffectComponent))]
	public class OrbEffect : MonoBehaviour
	{
		// Token: 0x06001D64 RID: 7524 RVA: 0x00088F84 File Offset: 0x00087184
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
				EffectManager.instance.SpawnEffect(this.startEffect, effectData, false);
			}
			this.startVelocity.x = Mathf.Lerp(this.startVelocity1.x, this.startVelocity2.x, UnityEngine.Random.value);
			this.startVelocity.y = Mathf.Lerp(this.startVelocity1.y, this.startVelocity2.y, UnityEngine.Random.value);
			this.startVelocity.z = Mathf.Lerp(this.startVelocity1.z, this.startVelocity2.z, UnityEngine.Random.value);
			this.endVelocity.x = Mathf.Lerp(this.endVelocity1.x, this.endVelocity2.x, UnityEngine.Random.value);
			this.endVelocity.y = Mathf.Lerp(this.endVelocity1.y, this.endVelocity2.y, UnityEngine.Random.value);
			this.endVelocity.z = Mathf.Lerp(this.endVelocity1.z, this.endVelocity2.z, UnityEngine.Random.value);
			this.UpdateOrb(0f);
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x000891AE File Offset: 0x000873AE
		private void Update()
		{
			this.UpdateOrb(Time.deltaTime);
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x000891BC File Offset: 0x000873BC
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
			if (num == 1f)
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
					EffectManager.instance.SpawnEffect(this.endEffect, effectData, false);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.previousPosition = vector;
			this.age += deltaTime;
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x00089310 File Offset: 0x00087510
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

		// Token: 0x06001D68 RID: 7528 RVA: 0x0008936D File Offset: 0x0008756D
		public void InstantiatePrefab(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x0008938C File Offset: 0x0008758C
		public void InstantiateEffect(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position
			}, false);
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x000893B0 File Offset: 0x000875B0
		public void InstantiateEffectCopyRotation(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = base.transform.rotation
			}, false);
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x000893E5 File Offset: 0x000875E5
		public void InstantiateEffectOppositeFacing(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = Util.QuaternionSafeLookRotation(-base.transform.forward)
			}, false);
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x00089424 File Offset: 0x00087624
		public void InstantiatePrefabOppositeFacing(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, Util.QuaternionSafeLookRotation(-base.transform.forward));
		}

		// Token: 0x04001FAA RID: 8106
		private Transform targetTransform;

		// Token: 0x04001FAB RID: 8107
		private float duration;

		// Token: 0x04001FAC RID: 8108
		private Vector3 startPosition;

		// Token: 0x04001FAD RID: 8109
		private Vector3 previousPosition;

		// Token: 0x04001FAE RID: 8110
		private Vector3 lastKnownTargetPosition;

		// Token: 0x04001FAF RID: 8111
		private float age;

		// Token: 0x04001FB0 RID: 8112
		[Header("Curve Parameters")]
		public Vector3 startVelocity1;

		// Token: 0x04001FB1 RID: 8113
		public Vector3 startVelocity2;

		// Token: 0x04001FB2 RID: 8114
		public Vector3 endVelocity1;

		// Token: 0x04001FB3 RID: 8115
		public Vector3 endVelocity2;

		// Token: 0x04001FB4 RID: 8116
		private Vector3 startVelocity;

		// Token: 0x04001FB5 RID: 8117
		private Vector3 endVelocity;

		// Token: 0x04001FB6 RID: 8118
		public AnimationCurve movementCurve;

		// Token: 0x04001FB7 RID: 8119
		public BezierCurveLine bezierCurveLine;

		// Token: 0x04001FB8 RID: 8120
		public bool faceMovement = true;

		// Token: 0x04001FB9 RID: 8121
		[Header("Start Effect")]
		[Tooltip("An effect prefab to spawn on Start")]
		public GameObject startEffect;

		// Token: 0x04001FBA RID: 8122
		public float startEffectScale = 1f;

		// Token: 0x04001FBB RID: 8123
		public bool startEffectCopiesRotation;

		// Token: 0x04001FBC RID: 8124
		[Tooltip("An effect prefab to spawn on end")]
		[Header("End Effect")]
		public GameObject endEffect;

		// Token: 0x04001FBD RID: 8125
		public float endEffectScale = 1f;

		// Token: 0x04001FBE RID: 8126
		public bool endEffectCopiesRotation;

		// Token: 0x04001FBF RID: 8127
		[Header("Arrival Behavior")]
		public UnityEvent onArrival;
	}
}
