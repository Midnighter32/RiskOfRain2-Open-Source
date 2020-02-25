using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x0200035B RID: 859
	[RequireComponent(typeof(EffectComponent))]
	public class Tracer : MonoBehaviour
	{
		// Token: 0x060014E4 RID: 5348 RVA: 0x00059280 File Offset: 0x00057480
		private void Start()
		{
			EffectComponent component = base.GetComponent<EffectComponent>();
			this.endPos = component.effectData.origin;
			Transform transform = component.effectData.ResolveChildLocatorTransformReference();
			this.startPos = (transform ? transform.position : component.effectData.start);
			if (this.reverse)
			{
				Util.Swap<Vector3>(ref this.endPos, ref this.startPos);
			}
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

		// Token: 0x060014E5 RID: 5349 RVA: 0x000593F8 File Offset: 0x000575F8
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

		// Token: 0x04001376 RID: 4982
		[Tooltip("A child transform which will be placed at the start of the tracer path upon creation.")]
		public Transform startTransform;

		// Token: 0x04001377 RID: 4983
		[Tooltip("Child object to scale to the length of this tracer and burst particles on based on that length. Optional.")]
		public GameObject beamObject;

		// Token: 0x04001378 RID: 4984
		[Tooltip("The number of particles to emit per meter of length if using a beam object.")]
		public float beamDensity = 10f;

		// Token: 0x04001379 RID: 4985
		[Tooltip("The travel speed of this tracer.")]
		public float speed = 1f;

		// Token: 0x0400137A RID: 4986
		[Tooltip("Child transform which will be moved to the head of the tracer.")]
		public Transform headTransform;

		// Token: 0x0400137B RID: 4987
		[Tooltip("Child transform which will be moved to the tail of the tracer.")]
		public Transform tailTransform;

		// Token: 0x0400137C RID: 4988
		[Tooltip("The maximum distance between head and tail transforms.")]
		public float length = 1f;

		// Token: 0x0400137D RID: 4989
		[Tooltip("Reverses the travel direction of the tracer.")]
		public bool reverse;

		// Token: 0x0400137E RID: 4990
		[Tooltip("The event that runs when the tail reaches the destination.")]
		public UnityEvent onTailReachedDestination;

		// Token: 0x0400137F RID: 4991
		private Vector3 startPos;

		// Token: 0x04001380 RID: 4992
		private Vector3 endPos;

		// Token: 0x04001381 RID: 4993
		private float distanceTraveled;

		// Token: 0x04001382 RID: 4994
		private float totalDistance;

		// Token: 0x04001383 RID: 4995
		private Vector3 normal;
	}
}
