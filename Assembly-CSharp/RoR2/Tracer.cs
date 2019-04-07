using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000403 RID: 1027
	[RequireComponent(typeof(EffectComponent))]
	public class Tracer : MonoBehaviour
	{
		// Token: 0x060016E2 RID: 5858 RVA: 0x0006CF78 File Offset: 0x0006B178
		private void Start()
		{
			EffectComponent component = base.GetComponent<EffectComponent>();
			this.endPos = component.effectData.origin;
			Transform transform = component.effectData.ResolveChildLocatorTransformReference();
			this.startPos = (transform ? transform.position : component.effectData.start);
			Vector3 vector = this.endPos - this.startPos;
			this.distanceTraveled = 0f;
			this.totalDistance = Vector3.Magnitude(vector);
			if (this.totalDistance != 0f)
			{
				this.normal = vector * (1f / this.totalDistance);
				base.transform.rotation = Util.QuaternionSafeLookRotation(this.normal);
			}
			else
			{
				this.normal = Vector3.zero;
			}
			if (this.beamObject)
			{
				this.beamObject.transform.position = this.startPos + vector * 0.5f;
				ParticleSystem component2 = this.beamObject.GetComponent<ParticleSystem>();
				if (component2)
				{
					component2.shape.radius = this.totalDistance * 0.5f;
					component2.Emit(Mathf.FloorToInt(this.totalDistance * this.beamDensity) - 1);
				}
			}
			if (this.startTransform)
			{
				this.startTransform.position = this.startPos;
			}
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x0006D0D8 File Offset: 0x0006B2D8
		private void Update()
		{
			if (this.distanceTraveled > this.totalDistance)
			{
				this.onTailReachedDestination.Invoke();
				return;
			}
			this.distanceTraveled += this.speed * Time.deltaTime;
			float d = Mathf.Clamp(this.distanceTraveled, 0f, this.totalDistance);
			float d2 = Mathf.Clamp(this.distanceTraveled - this.length, 0f, this.totalDistance);
			if (this.headTransform)
			{
				this.headTransform.position = this.startPos + d * this.normal;
			}
			if (this.tailTransform)
			{
				this.tailTransform.position = this.startPos + d2 * this.normal;
			}
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x0006D1AC File Offset: 0x0006B3AC
		public static GameObject CreateTracer(GameObject tracerPrefab, Vector3 startPosition, Vector3 endPosition)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(tracerPrefab, endPosition, (startPosition != endPosition) ? Util.QuaternionSafeLookRotation(endPosition - startPosition) : Quaternion.identity);
			Tracer component = gameObject.GetComponent<Tracer>();
			if (!component)
			{
				Debug.LogErrorFormat("Attempting to create tracer from a prefab that lacks a Tracer component! Prefab name: {0}", new object[]
				{
					tracerPrefab.name
				});
			}
			else
			{
				component.startPos = startPosition;
				component.endPos = endPosition;
				if (component.startTransform)
				{
					component.startTransform.position = startPosition;
				}
			}
			return gameObject;
		}

		// Token: 0x04001A0F RID: 6671
		[Tooltip("A child transform which will be placed at the start of the tracer path upon creation.")]
		public Transform startTransform;

		// Token: 0x04001A10 RID: 6672
		[Tooltip("Child object to scale to the length of this tracer and burst particles on based on that length. Optional.")]
		public GameObject beamObject;

		// Token: 0x04001A11 RID: 6673
		[Tooltip("The number of particles to emit per meter of length if using a beam object.")]
		public float beamDensity = 10f;

		// Token: 0x04001A12 RID: 6674
		[Tooltip("The travel speed of this tracer.")]
		public float speed = 1f;

		// Token: 0x04001A13 RID: 6675
		[Tooltip("Child transform which will be moved to the head of the tracer.")]
		public Transform headTransform;

		// Token: 0x04001A14 RID: 6676
		[Tooltip("Child transform which will be moved to the tail of the tracer.")]
		public Transform tailTransform;

		// Token: 0x04001A15 RID: 6677
		[Tooltip("The maximum distance between head and tail transforms.")]
		public float length = 1f;

		// Token: 0x04001A16 RID: 6678
		[Tooltip("The event that runs when the tail reaches the destination.")]
		public UnityEvent onTailReachedDestination;

		// Token: 0x04001A17 RID: 6679
		private Vector3 startPos;

		// Token: 0x04001A18 RID: 6680
		private Vector3 endPos;

		// Token: 0x04001A19 RID: 6681
		private float distanceTraveled;

		// Token: 0x04001A1A RID: 6682
		private float totalDistance;

		// Token: 0x04001A1B RID: 6683
		private Vector3 normal;
	}
}
