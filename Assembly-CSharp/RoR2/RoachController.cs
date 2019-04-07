using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A8 RID: 936
	public class RoachController : MonoBehaviour
	{
		// Token: 0x060013D1 RID: 5073 RVA: 0x00061180 File Offset: 0x0005F380
		private void Awake()
		{
			this.roachTransforms = new Transform[this.roaches.Length];
			for (int i = 0; i < this.roachTransforms.Length; i++)
			{
				this.roachTransforms[i] = UnityEngine.Object.Instantiate<GameObject>(this.roachParams.roachPrefab, this.roaches[i].keyFrames[0].position, this.roaches[i].keyFrames[0].rotation).transform;
			}
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x00061208 File Offset: 0x0005F408
		private void OnDestroy()
		{
			for (int i = 0; i < this.roachTransforms.Length; i++)
			{
				if (this.roachTransforms[i])
				{
					UnityEngine.Object.Destroy(this.roachTransforms[i].gameObject);
				}
			}
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x0006124C File Offset: 0x0005F44C
		public void BakeRoaches2()
		{
			List<RoachController.Roach> list = new List<RoachController.Roach>();
			for (int i = 0; i < this.roachCount; i++)
			{
				Ray ray = new Ray(base.transform.position, Util.ApplySpread(base.transform.forward, this.placementSpreadMin, this.placementSpreadMax, 1f, 1f, 0f, 0f));
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, this.placementMaxDistance, LayerIndex.world.mask))
				{
					RoachController.SimulatedRoach simulatedRoach = new RoachController.SimulatedRoach(raycastHit.point + raycastHit.normal * 0.01f, raycastHit.normal, ray.direction, this.roachParams);
					float keyframeInterval = this.roachParams.keyframeInterval;
					List<RoachController.KeyFrame> list2 = new List<RoachController.KeyFrame>();
					while (!simulatedRoach.finished)
					{
						simulatedRoach.Simulate(keyframeInterval);
						list2.Add(new RoachController.KeyFrame
						{
							position = simulatedRoach.transform.position,
							rotation = simulatedRoach.transform.rotation,
							time = simulatedRoach.age
						});
					}
					RoachController.KeyFrame keyFrame = list2[list2.Count - 1];
					keyFrame.position += keyFrame.rotation * (Vector3.down * 0.25f);
					list2[list2.Count - 1] = keyFrame;
					simulatedRoach.Dispose();
					list.Add(new RoachController.Roach
					{
						keyFrames = list2.ToArray()
					});
				}
			}
			this.roaches = list.ToArray();
		}

		// Token: 0x060013D4 RID: 5076 RVA: 0x0006140A File Offset: 0x0005F60A
		public void BakeRoaches()
		{
			this.BakeRoaches2();
		}

		// Token: 0x060013D5 RID: 5077 RVA: 0x00061414 File Offset: 0x0005F614
		private void ClearRoachPathEditors()
		{
			for (int i = base.transform.childCount - 1; i > 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x00061450 File Offset: 0x0005F650
		public void DebakeRoaches()
		{
			this.ClearRoachPathEditors();
			for (int i = 0; i < this.roaches.Length; i++)
			{
				RoachController.Roach roach = this.roaches[i];
				RoachController.RoachPathEditorComponent roachPathEditorComponent = this.AddPathEditorObject();
				for (int j = 0; j < roach.keyFrames.Length; j++)
				{
					RoachController.KeyFrame keyFrame = roach.keyFrames[j];
					RoachController.RoachNodeEditorComponent roachNodeEditorComponent = roachPathEditorComponent.AddNode();
					roachNodeEditorComponent.transform.position = keyFrame.position;
					roachNodeEditorComponent.transform.rotation = keyFrame.rotation;
				}
			}
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x000614D4 File Offset: 0x0005F6D4
		public RoachController.RoachPathEditorComponent AddPathEditorObject()
		{
			GameObject gameObject = new GameObject("Roach Path (Temporary)");
			gameObject.hideFlags = HideFlags.DontSave;
			gameObject.transform.SetParent(base.transform, false);
			RoachController.RoachPathEditorComponent roachPathEditorComponent = gameObject.AddComponent<RoachController.RoachPathEditorComponent>();
			roachPathEditorComponent.roachController = this;
			return roachPathEditorComponent;
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x00061508 File Offset: 0x0005F708
		private void UpdateRoach(int i)
		{
			RoachController.KeyFrame[] keyFrames = this.roaches[i].keyFrames;
			float num = Mathf.Min(this.scatterStartTime.timeSince, keyFrames[keyFrames.Length - 1].time);
			for (int j = 1; j < keyFrames.Length; j++)
			{
				if (num <= keyFrames[j].time)
				{
					RoachController.KeyFrame keyFrame = keyFrames[j - 1];
					RoachController.KeyFrame keyFrame2 = keyFrames[j];
					float t = Mathf.InverseLerp(keyFrame.time, keyFrame2.time, num);
					this.SetRoachPosition(i, Vector3.Lerp(keyFrame.position, keyFrame2.position, t), Quaternion.Slerp(keyFrame.rotation, keyFrame2.rotation, t));
					return;
				}
			}
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x000615BD File Offset: 0x0005F7BD
		private void SetRoachPosition(int i, Vector3 position, Quaternion rotation)
		{
			this.roachTransforms[i].SetPositionAndRotation(position, rotation);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x000615D0 File Offset: 0x0005F7D0
		private void Update()
		{
			for (int i = 0; i < this.roaches.Length; i++)
			{
				this.UpdateRoach(i);
			}
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x000615F7 File Offset: 0x0005F7F7
		private void Scatter()
		{
			if (this.scattered)
			{
				return;
			}
			Util.PlaySound("Play_env_roach_scatter", base.gameObject);
			this.scattered = true;
			this.scatterStartTime = Run.TimeStamp.now;
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x00061625 File Offset: 0x0005F825
		private void OnTriggerEnter(Collider other)
		{
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component != null && component.isPlayerControlled)
			{
				this.Scatter();
			}
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x00061644 File Offset: 0x0005F844
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, this.placementSpreadMax * 0.5f, this.placementMaxDistance, 0f, 1f);
		}

		// Token: 0x0400179F RID: 6047
		public RoachParams roachParams;

		// Token: 0x040017A0 RID: 6048
		public int roachCount;

		// Token: 0x040017A1 RID: 6049
		public float placementSpreadMin = 1f;

		// Token: 0x040017A2 RID: 6050
		public float placementSpreadMax = 25f;

		// Token: 0x040017A3 RID: 6051
		public float placementMaxDistance = 10f;

		// Token: 0x040017A4 RID: 6052
		public RoachController.Roach[] roaches;

		// Token: 0x040017A5 RID: 6053
		private Transform[] roachTransforms;

		// Token: 0x040017A6 RID: 6054
		private bool scattered;

		// Token: 0x040017A7 RID: 6055
		private Run.TimeStamp scatterStartTime = Run.TimeStamp.positiveInfinity;

		// Token: 0x040017A8 RID: 6056
		private const string roachScatterSoundString = "Play_env_roach_scatter";

		// Token: 0x020003A9 RID: 937
		[Serializable]
		public struct KeyFrame
		{
			// Token: 0x040017A9 RID: 6057
			public float time;

			// Token: 0x040017AA RID: 6058
			public Vector3 position;

			// Token: 0x040017AB RID: 6059
			public Quaternion rotation;
		}

		// Token: 0x020003AA RID: 938
		[Serializable]
		public struct Roach
		{
			// Token: 0x040017AC RID: 6060
			public RoachController.KeyFrame[] keyFrames;
		}

		// Token: 0x020003AB RID: 939
		private class SimulatedRoach : IDisposable
		{
			// Token: 0x170001BA RID: 442
			// (get) Token: 0x060013DF RID: 5087 RVA: 0x000616DA File Offset: 0x0005F8DA
			// (set) Token: 0x060013E0 RID: 5088 RVA: 0x000616E2 File Offset: 0x0005F8E2
			public Transform transform { get; private set; }

			// Token: 0x170001BB RID: 443
			// (get) Token: 0x060013E1 RID: 5089 RVA: 0x000616EB File Offset: 0x0005F8EB
			// (set) Token: 0x060013E2 RID: 5090 RVA: 0x000616F3 File Offset: 0x0005F8F3
			public float age { get; private set; }

			// Token: 0x170001BC RID: 444
			// (get) Token: 0x060013E3 RID: 5091 RVA: 0x000616FC File Offset: 0x0005F8FC
			// (set) Token: 0x060013E4 RID: 5092 RVA: 0x00061704 File Offset: 0x0005F904
			public bool finished { get; private set; }

			// Token: 0x170001BD RID: 445
			// (get) Token: 0x060013E5 RID: 5093 RVA: 0x0006170D File Offset: 0x0005F90D
			private bool onGround
			{
				get
				{
					return this.groundNormal != Vector3.zero;
				}
			}

			// Token: 0x060013E6 RID: 5094 RVA: 0x00061720 File Offset: 0x0005F920
			public SimulatedRoach(Vector3 position, Vector3 groundNormal, Vector3 initialFleeNormal, RoachParams roachParams)
			{
				this.roachParams = roachParams;
				GameObject gameObject = new GameObject("SimulatedRoach");
				this.transform = gameObject.transform;
				this.transform.position = position;
				this.transform.up = groundNormal;
				this.transform.Rotate(this.transform.up, UnityEngine.Random.Range(0f, 360f));
				this.transform.forward = UnityEngine.Random.onUnitSphere;
				this.groundNormal = groundNormal;
				this.initialFleeNormal = initialFleeNormal;
				this.desiredMovement = UnityEngine.Random.onUnitSphere;
				this.age = UnityEngine.Random.Range(roachParams.minReactionTime, roachParams.maxReactionTime);
				this.simulationDuration = this.age + UnityEngine.Random.Range(roachParams.minSimulationDuration, roachParams.maxSimulationDuration);
			}

			// Token: 0x060013E7 RID: 5095 RVA: 0x000617FC File Offset: 0x0005F9FC
			private void SetUpVector(Vector3 desiredUp)
			{
				Vector3 right = this.transform.right;
				Vector3 up = this.transform.up;
				this.transform.Rotate(right, Vector3.SignedAngle(up, desiredUp, right), Space.World);
			}

			// Token: 0x060013E8 RID: 5096 RVA: 0x00061838 File Offset: 0x0005FA38
			public void Simulate(float deltaTime)
			{
				this.age += deltaTime;
				if (this.onGround)
				{
					this.SetUpVector(this.groundNormal);
					this.turnVelocity += UnityEngine.Random.Range(-this.roachParams.wiggle, this.roachParams.wiggle) * deltaTime;
					this.TurnDesiredMovement(this.turnVelocity * deltaTime);
					Vector3 up = this.transform.up;
					Vector3 normalized = Vector3.ProjectOnPlane(this.desiredMovement, up).normalized;
					float value = Vector3.SignedAngle(this.transform.forward, normalized, up);
					this.TurnBody(Mathf.Clamp(value, -this.turnVelocity * deltaTime, this.turnVelocity * deltaTime));
					this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.roachParams.maxSpeed, deltaTime * this.roachParams.acceleration);
					this.StepGround(this.currentSpeed * deltaTime);
				}
				else
				{
					this.velocity += Physics.gravity * deltaTime;
					this.StepAir(this.velocity);
				}
				this.reorientTimer -= deltaTime;
				if (this.reorientTimer <= 0f)
				{
					this.desiredMovement = this.initialFleeNormal;
					this.reorientTimer = UnityEngine.Random.Range(this.roachParams.reorientTimerMin, this.roachParams.reorientTimerMax);
				}
				if (this.age >= this.simulationDuration)
				{
					this.finished = true;
				}
			}

			// Token: 0x060013E9 RID: 5097 RVA: 0x000619B4 File Offset: 0x0005FBB4
			private void OnBump()
			{
				this.TurnDesiredMovement(UnityEngine.Random.Range(-90f, 90f));
				this.currentSpeed *= -0.5f;
				if (this.roachParams.chanceToFinishOnBump < UnityEngine.Random.value)
				{
					this.finished = true;
				}
			}

			// Token: 0x060013EA RID: 5098 RVA: 0x00061A04 File Offset: 0x0005FC04
			private void TurnDesiredMovement(float degrees)
			{
				Quaternion rotation = Quaternion.AngleAxis(degrees, this.transform.up);
				this.desiredMovement = rotation * this.desiredMovement;
			}

			// Token: 0x060013EB RID: 5099 RVA: 0x00061A35 File Offset: 0x0005FC35
			private void TurnBody(float degrees)
			{
				this.transform.Rotate(Vector3.up, degrees, Space.Self);
			}

			// Token: 0x060013EC RID: 5100 RVA: 0x00061A4C File Offset: 0x0005FC4C
			private void StepAir(Vector3 movement)
			{
				RoachController.SimulatedRoach.RaycastResult raycastResult = RoachController.SimulatedRoach.SimpleRaycast(new Ray(this.transform.position, movement), movement.magnitude);
				Debug.DrawLine(this.transform.position, raycastResult.point, Color.magenta, 10f, false);
				if (raycastResult.didHit)
				{
					this.groundNormal = raycastResult.normal;
					this.velocity = Vector3.zero;
				}
				this.transform.position = raycastResult.point;
			}

			// Token: 0x060013ED RID: 5101 RVA: 0x00061AC8 File Offset: 0x0005FCC8
			private void StepGround(float distance)
			{
				this.groundNormal = Vector3.zero;
				Vector3 up = this.transform.up;
				Vector3 forward = this.transform.forward;
				float stepSize = this.roachParams.stepSize;
				Vector3 vector = up * stepSize;
				Vector3 vector2 = this.transform.position;
				vector2 += vector;
				Debug.DrawLine(this.transform.position, vector2, Color.red, 10f, false);
				RoachController.SimulatedRoach.RaycastResult raycastResult = RoachController.SimulatedRoach.SimpleRaycast(new Ray(vector2, forward), distance);
				Debug.DrawLine(vector2, raycastResult.point, Color.green, 10f, false);
				vector2 = raycastResult.point;
				if (raycastResult.didHit)
				{
					if (Vector3.Dot(raycastResult.normal, forward) < -0.5f)
					{
						this.OnBump();
					}
					this.groundNormal = raycastResult.normal;
				}
				else
				{
					RoachController.SimulatedRoach.RaycastResult raycastResult2 = RoachController.SimulatedRoach.SimpleRaycast(new Ray(vector2, -vector), stepSize * 2f);
					if (raycastResult2.didHit)
					{
						Debug.DrawLine(vector2, raycastResult2.point, Color.blue, 10f, false);
						vector2 = raycastResult2.point;
						this.groundNormal = raycastResult2.normal;
					}
					else
					{
						Debug.DrawLine(vector2, vector2 - vector, Color.white, 10f);
						vector2 -= vector;
					}
				}
				if (this.groundNormal == Vector3.zero)
				{
					this.currentSpeed = 0f;
				}
				this.transform.position = vector2;
			}

			// Token: 0x060013EE RID: 5102 RVA: 0x00061C38 File Offset: 0x0005FE38
			private static RoachController.SimulatedRoach.RaycastResult SimpleRaycast(Ray ray, float maxDistance)
			{
				RaycastHit raycastHit;
				bool flag = Physics.Raycast(ray, out raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				return new RoachController.SimulatedRoach.RaycastResult
				{
					didHit = flag,
					point = (flag ? raycastHit.point : ray.GetPoint(maxDistance)),
					normal = (flag ? raycastHit.normal : Vector3.zero),
					distance = (flag ? raycastHit.distance : maxDistance)
				};
			}

			// Token: 0x060013EF RID: 5103 RVA: 0x00061CBA File Offset: 0x0005FEBA
			public void Dispose()
			{
				UnityEngine.Object.DestroyImmediate(this.transform.gameObject);
				this.transform = null;
			}

			// Token: 0x040017AD RID: 6061
			private Vector3 initialFleeNormal;

			// Token: 0x040017AE RID: 6062
			private Vector3 desiredMovement;

			// Token: 0x040017AF RID: 6063
			private RoachParams roachParams;

			// Token: 0x040017B3 RID: 6067
			private float reorientTimer;

			// Token: 0x040017B4 RID: 6068
			private float backupTimer;

			// Token: 0x040017B5 RID: 6069
			private Vector3 velocity = Vector3.zero;

			// Token: 0x040017B6 RID: 6070
			private float currentSpeed;

			// Token: 0x040017B7 RID: 6071
			private float desiredSpeed;

			// Token: 0x040017B8 RID: 6072
			private float turnVelocity;

			// Token: 0x040017B9 RID: 6073
			private Vector3 groundNormal;

			// Token: 0x040017BA RID: 6074
			private float simulationDuration;

			// Token: 0x020003AC RID: 940
			private struct RaycastResult
			{
				// Token: 0x040017BB RID: 6075
				public bool didHit;

				// Token: 0x040017BC RID: 6076
				public Vector3 point;

				// Token: 0x040017BD RID: 6077
				public Vector3 normal;

				// Token: 0x040017BE RID: 6078
				public float distance;
			}
		}

		// Token: 0x020003AD RID: 941
		public class RoachPathEditorComponent : MonoBehaviour
		{
			// Token: 0x170001BE RID: 446
			// (get) Token: 0x060013F0 RID: 5104 RVA: 0x00061CD3 File Offset: 0x0005FED3
			public int nodeCount
			{
				get
				{
					return base.transform.childCount;
				}
			}

			// Token: 0x060013F1 RID: 5105 RVA: 0x00061CE0 File Offset: 0x0005FEE0
			public RoachController.RoachNodeEditorComponent AddNode()
			{
				GameObject gameObject = new GameObject("Roach Path Node (Temporary)");
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(base.transform);
				RoachController.RoachNodeEditorComponent roachNodeEditorComponent = gameObject.AddComponent<RoachController.RoachNodeEditorComponent>();
				roachNodeEditorComponent.path = this;
				return roachNodeEditorComponent;
			}

			// Token: 0x060013F2 RID: 5106 RVA: 0x00061D14 File Offset: 0x0005FF14
			private void OnDrawGizmosSelected()
			{
				Gizmos.color = Color.white;
				int num = 0;
				while (num + 1 < this.nodeCount)
				{
					Vector3 position = base.transform.GetChild(num).transform.position;
					Vector3 position2 = base.transform.GetChild(num + 1).transform.position;
					Gizmos.DrawLine(position, position2);
					num++;
				}
			}

			// Token: 0x040017BF RID: 6079
			public RoachController roachController;
		}

		// Token: 0x020003AE RID: 942
		public class RoachNodeEditorComponent : MonoBehaviour
		{
			// Token: 0x060013F4 RID: 5108 RVA: 0x00061D74 File Offset: 0x0005FF74
			public void FacePosition(Vector3 position)
			{
				Vector3 position2 = base.transform.position;
				Vector3 up = base.transform.up;
				Quaternion rotation = Quaternion.LookRotation(position - position2, up);
				base.transform.rotation = rotation;
				base.transform.up = up;
			}

			// Token: 0x040017C0 RID: 6080
			public RoachController.RoachPathEditorComponent path;
		}
	}
}
