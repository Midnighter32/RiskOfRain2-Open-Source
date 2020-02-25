using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000343 RID: 835
	public class StriderLegController : MonoBehaviour
	{
		// Token: 0x060013E9 RID: 5097 RVA: 0x00055124 File Offset: 0x00053324
		public Vector3 GetCenterOfStance()
		{
			Vector3 a = Vector3.zero;
			for (int i = 0; i < this.feet.Length; i++)
			{
				a += this.feet[i].transform.position;
			}
			return a / (float)this.feet.Length;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x00055178 File Offset: 0x00053378
		private void Awake()
		{
			for (int i = 0; i < this.feet.Length; i++)
			{
				this.feet[i].footState = StriderLegController.FootState.Planted;
				this.feet[i].plantPosition = this.feet[i].referenceTransform.position;
				this.feet[i].trailingTargetPosition = this.feet[i].plantPosition;
			}
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x000551F4 File Offset: 0x000533F4
		private void Update()
		{
			int num = 0;
			this.footRaycastTimer -= Time.deltaTime;
			for (int i = 0; i < this.feet.Length; i++)
			{
				Transform transform = this.feet[i].transform;
				Transform referenceTransform = this.feet[i].referenceTransform;
				Vector3 position = transform.position;
				Vector3 vector = Vector3.zero;
				float num2 = 0f;
				StriderLegController.FootState footState = this.feet[i].footState;
				if (footState != StriderLegController.FootState.Planted)
				{
					if (footState == StriderLegController.FootState.Replanting)
					{
						StriderLegController.FootInfo[] array = this.feet;
						int num3 = i;
						array[num3].stopwatch = array[num3].stopwatch + Time.deltaTime;
						Vector3 plantPosition = this.feet[i].plantPosition;
						Vector3 vector2 = referenceTransform.position;
						vector2 += Vector3.ProjectOnPlane(vector2 - plantPosition, Vector3.up).normalized * this.overstepDistance;
						float num4 = this.lerpCurve.Evaluate(this.feet[i].stopwatch / this.replantDuration);
						vector = Vector3.Lerp(plantPosition, vector2, num4);
						num2 = Mathf.Sin(num4 * 3.1415927f) * this.replantHeight;
						if (this.feet[i].stopwatch >= this.replantDuration)
						{
							this.feet[i].plantPosition = vector2;
							this.feet[i].stopwatch = 0f;
							this.feet[i].footState = StriderLegController.FootState.Planted;
							Util.PlaySound(this.footPlantString, transform.gameObject);
						}
					}
				}
				else
				{
					num++;
					vector = this.feet[i].plantPosition;
					if ((referenceTransform.position - vector).sqrMagnitude > this.stabilityRadius * this.stabilityRadius)
					{
						this.feet[i].footState = StriderLegController.FootState.Replanting;
						Util.PlaySound(this.footMoveString, transform.gameObject);
					}
				}
				Ray ray = default(Ray);
				ray.direction = transform.TransformDirection(this.footRaycastDirection.normalized);
				ray.origin = vector - ray.direction * this.raycastVerticalOffset;
				RaycastHit raycastHit;
				if (this.footRaycastTimer <= 0f && Physics.Raycast(ray, out raycastHit, this.maxRaycastDistance + this.raycastVerticalOffset, LayerIndex.world.mask))
				{
					vector = raycastHit.point;
				}
				vector.y += num2;
				this.feet[i].trailingTargetPosition = Vector3.SmoothDamp(this.feet[i].trailingTargetPosition, vector, ref this.feet[i].velocity, this.footDampTime);
				transform.position = this.feet[i].trailingTargetPosition;
			}
			if (this.rootTransform)
			{
				Vector3 localPosition = this.rootTransform.localPosition;
				float num5 = (1f - (float)num / (float)this.feet.Length) * this.rootOffsetHeight;
				float target = localPosition.z - num5;
				float z = Mathf.SmoothDamp(localPosition.z, target, ref this.rootVelocity, this.rootSmoothDamp);
				this.rootTransform.localPosition = new Vector3(localPosition.x, localPosition.y, z);
			}
			if (this.footRaycastTimer <= 0f)
			{
				this.footRaycastTimer = 1f / this.footRaycastFrequency;
			}
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0003AEFF File Offset: 0x000390FF
		public Vector3 GetArcPosition(Vector3 start, Vector3 end, float arcHeight, float t)
		{
			return Vector3.Lerp(start, end, Mathf.Sin(t * 3.1415927f * 0.5f)) + new Vector3(0f, Mathf.Sin(t * 3.1415927f) * arcHeight, 0f);
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x00055584 File Offset: 0x00053784
		public void OnDrawGizmos()
		{
			for (int i = 0; i < this.feet.Length; i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(this.feet[i].transform.position, this.feet[i].transform.TransformVector(this.footRaycastDirection));
			}
		}

		// Token: 0x040012A8 RID: 4776
		[Header("Foot Settings")]
		public Transform centerOfGravity;

		// Token: 0x040012A9 RID: 4777
		public StriderLegController.FootInfo[] feet;

		// Token: 0x040012AA RID: 4778
		public Vector3 footRaycastDirection;

		// Token: 0x040012AB RID: 4779
		public float raycastVerticalOffset;

		// Token: 0x040012AC RID: 4780
		public float maxRaycastDistance;

		// Token: 0x040012AD RID: 4781
		public float footDampTime;

		// Token: 0x040012AE RID: 4782
		public float stabilityRadius;

		// Token: 0x040012AF RID: 4783
		public float replantDuration;

		// Token: 0x040012B0 RID: 4784
		public float replantHeight;

		// Token: 0x040012B1 RID: 4785
		public float overstepDistance;

		// Token: 0x040012B2 RID: 4786
		public AnimationCurve lerpCurve;

		// Token: 0x040012B3 RID: 4787
		public string footPlantString;

		// Token: 0x040012B4 RID: 4788
		public string footMoveString;

		// Token: 0x040012B5 RID: 4789
		public float footRaycastFrequency = 0.2f;

		// Token: 0x040012B6 RID: 4790
		[Header("Root Settings")]
		public Transform rootTransform;

		// Token: 0x040012B7 RID: 4791
		public float rootSpringConstant;

		// Token: 0x040012B8 RID: 4792
		public float rootDampingConstant;

		// Token: 0x040012B9 RID: 4793
		public float rootOffsetHeight;

		// Token: 0x040012BA RID: 4794
		public float rootSmoothDamp;

		// Token: 0x040012BB RID: 4795
		private float rootVelocity;

		// Token: 0x040012BC RID: 4796
		private float footRaycastTimer;

		// Token: 0x02000344 RID: 836
		[Serializable]
		public struct FootInfo
		{
			// Token: 0x040012BD RID: 4797
			public Transform transform;

			// Token: 0x040012BE RID: 4798
			public Transform referenceTransform;

			// Token: 0x040012BF RID: 4799
			[HideInInspector]
			public Vector3 velocity;

			// Token: 0x040012C0 RID: 4800
			[HideInInspector]
			public StriderLegController.FootState footState;

			// Token: 0x040012C1 RID: 4801
			[HideInInspector]
			public Vector3 plantPosition;

			// Token: 0x040012C2 RID: 4802
			[HideInInspector]
			public Vector3 trailingTargetPosition;

			// Token: 0x040012C3 RID: 4803
			[HideInInspector]
			public float stopwatch;
		}

		// Token: 0x02000345 RID: 837
		public enum FootState
		{
			// Token: 0x040012C5 RID: 4805
			Planted,
			// Token: 0x040012C6 RID: 4806
			Replanting
		}
	}
}
