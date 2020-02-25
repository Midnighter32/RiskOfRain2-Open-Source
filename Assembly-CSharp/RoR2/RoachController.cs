using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E8 RID: 744
	public class RoachController : MonoBehaviour
	{
		// Token: 0x06001102 RID: 4354 RVA: 0x0004AE30 File Offset: 0x00049030
		private void Awake()
		{
			this.roachTransforms = new Transform[this.roaches.Length];
			for (int i = 0; i < this.roachTransforms.Length; i++)
			{
				this.roachTransforms[i] = UnityEngine.Object.Instantiate<GameObject>(this.roachParams.roachPrefab, this.roaches[i].keyFrames[0].position, this.roaches[i].keyFrames[0].rotation).transform;
			}
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x0004AEB8 File Offset: 0x000490B8
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

		// Token: 0x06001104 RID: 4356 RVA: 0x0004AEFC File Offset: 0x000490FC
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

		// Token: 0x06001105 RID: 4357 RVA: 0x0004B0BA File Offset: 0x000492BA
		public void BakeRoaches()
		{
			this.BakeRoaches2();
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0004B0C4 File Offset: 0x000492C4
		private void ClearRoachPathEditors()
		{
			for (int i = base.transform.childCount - 1; i > 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x0004B100 File Offset: 0x00049300
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

		// Token: 0x06001108 RID: 4360 RVA: 0x0004B184 File Offset: 0x00049384
		public RoachController.RoachPathEditorComponent AddPathEditorObject()
		{
			GameObject gameObject = new GameObject("Roach Path (Temporary)");
			gameObject.hideFlags = HideFlags.DontSave;
			gameObject.transform.SetParent(base.transform, false);
			RoachController.RoachPathEditorComponent roachPathEditorComponent = gameObject.AddComponent<RoachController.RoachPathEditorComponent>();
			roachPathEditorComponent.roachController = this;
			return roachPathEditorComponent;
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0004B1B8 File Offset: 0x000493B8
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

		// Token: 0x0600110A RID: 4362 RVA: 0x0004B26D File Offset: 0x0004946D
		private void SetRoachPosition(int i, Vector3 position, Quaternion rotation)
		{
			this.roachTransforms[i].SetPositionAndRotation(position, rotation);
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0004B280 File Offset: 0x00049480
		private void Update()
		{
			for (int i = 0; i < this.roaches.Length; i++)
			{
				this.UpdateRoach(i);
			}
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0004B2A7 File Offset: 0x000494A7
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

		// Token: 0x0600110D RID: 4365 RVA: 0x0004B2D5 File Offset: 0x000494D5
		private void OnTriggerEnter(Collider other)
		{
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component != null && component.isPlayerControlled)
			{
				this.Scatter();
			}
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x0004B2F4 File Offset: 0x000494F4
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, this.placementSpreadMax * 0.5f, this.placementMaxDistance, 0f, 1f);
		}

		// Token: 0x0400107C RID: 4220
		public RoachParams roachParams;

		// Token: 0x0400107D RID: 4221
		public int roachCount;

		// Token: 0x0400107E RID: 4222
		public float placementSpreadMin = 1f;

		// Token: 0x0400107F RID: 4223
		public float placementSpreadMax = 25f;

		// Token: 0x04001080 RID: 4224
		public float placementMaxDistance = 10f;

		// Token: 0x04001081 RID: 4225
		public RoachController.Roach[] roaches;

		// Token: 0x04001082 RID: 4226
		private Transform[] roachTransforms;

		// Token: 0x04001083 RID: 4227
		private bool scattered;

		// Token: 0x04001084 RID: 4228
		private Run.TimeStamp scatterStartTime = Run.TimeStamp.positiveInfinity;

		// Token: 0x04001085 RID: 4229
		private const string roachScatterSoundString = "Play_env_roach_scatter";

		// Token: 0x020002E9 RID: 745
		[Serializable]
		public struct KeyFrame
		{
			// Token: 0x04001086 RID: 4230
			public float time;

			// Token: 0x04001087 RID: 4231
			public Vector3 position;

			// Token: 0x04001088 RID: 4232
			public Quaternion rotation;
		}

		// Token: 0x020002EA RID: 746
		[Serializable]
		public struct Roach
		{
			// Token: 0x04001089 RID: 4233
			public RoachController.KeyFrame[] keyFrames;
		}

		// Token: 0x020002EB RID: 747
		private class SimulatedRoach : IDisposable
		{
			// Token: 0x1700020F RID: 527
			// (get) Token: 0x06001110 RID: 4368 RVA: 0x0004B38A File Offset: 0x0004958A
			// (set) Token: 0x06001111 RID: 4369 RVA: 0x0004B392 File Offset: 0x00049592
			public Transform transform { get; private set; }

			// Token: 0x17000210 RID: 528
			// (get) Token: 0x06001112 RID: 4370 RVA: 0x0004B39B File Offset: 0x0004959B
			// (set) Token: 0x06001113 RID: 4371 RVA: 0x0004B3A3 File Offset: 0x000495A3
			public float age { get; private set; }

			// Token: 0x17000211 RID: 529
			// (get) Token: 0x06001114 RID: 4372 RVA: 0x0004B3AC File Offset: 0x000495AC
			// (set) Token: 0x06001115 RID: 4373 RVA: 0x0004B3B4 File Offset: 0x000495B4
			public bool finished { get; private set; }

			// Token: 0x17000212 RID: 530
			// (get) Token: 0x06001116 RID: 4374 RVA: 0x0004B3BD File Offset: 0x000495BD
			private bool onGround
			{
				get
				{
					return this.groundNormal != Vector3.zero;
				}
			}

			// Token: 0x06001117 RID: 4375 RVA: 0x0004B3D0 File Offset: 0x000495D0
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

			// Token: 0x06001118 RID: 4376 RVA: 0x0004B4AC File Offset: 0x000496AC
			private void SetUpVector(Vector3 desiredUp)
			{
				Vector3 right = this.transform.right;
				Vector3 up = this.transform.up;
				this.transform.Rotate(right, Vector3.SignedAngle(up, desiredUp, right), Space.World);
			}

			// Token: 0x06001119 RID: 4377 RVA: 0x0004B4E8 File Offset: 0x000496E8
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

			// Token: 0x0600111A RID: 4378 RVA: 0x0004B664 File Offset: 0x00049864
			private void OnBump()
			{
				this.TurnDesiredMovement(UnityEngine.Random.Range(-90f, 90f));
				this.currentSpeed *= -0.5f;
				if (this.roachParams.chanceToFinishOnBump < UnityEngine.Random.value)
				{
					this.finished = true;
				}
			}

			// Token: 0x0600111B RID: 4379 RVA: 0x0004B6B4 File Offset: 0x000498B4
			private void TurnDesiredMovement(float degrees)
			{
				Quaternion rotation = Quaternion.AngleAxis(degrees, this.transform.up);
				this.desiredMovement = rotation * this.desiredMovement;
			}

			// Token: 0x0600111C RID: 4380 RVA: 0x0004B6E5 File Offset: 0x000498E5
			private void TurnBody(float degrees)
			{
				this.transform.Rotate(Vector3.up, degrees, Space.Self);
			}

			// Token: 0x0600111D RID: 4381 RVA: 0x0004B6FC File Offset: 0x000498FC
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

			// Token: 0x0600111E RID: 4382 RVA: 0x0004B778 File Offset: 0x00049978
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

			// Token: 0x0600111F RID: 4383 RVA: 0x0004B8E8 File Offset: 0x00049AE8
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

			// Token: 0x06001120 RID: 4384 RVA: 0x0004B96A File Offset: 0x00049B6A
			public void Dispose()
			{
				UnityEngine.Object.DestroyImmediate(this.transform.gameObject);
				this.transform = null;
			}

			// Token: 0x0400108A RID: 4234
			private Vector3 initialFleeNormal;

			// Token: 0x0400108B RID: 4235
			private Vector3 desiredMovement;

			// Token: 0x0400108C RID: 4236
			private RoachParams roachParams;

			// Token: 0x04001090 RID: 4240
			private float reorientTimer;

			// Token: 0x04001091 RID: 4241
			private float backupTimer;

			// Token: 0x04001092 RID: 4242
			private Vector3 velocity = Vector3.zero;

			// Token: 0x04001093 RID: 4243
			private float currentSpeed;

			// Token: 0x04001094 RID: 4244
			private float desiredSpeed;

			// Token: 0x04001095 RID: 4245
			private float turnVelocity;

			// Token: 0x04001096 RID: 4246
			private Vector3 groundNormal;

			// Token: 0x04001097 RID: 4247
			private float simulationDuration;

			// Token: 0x020002EC RID: 748
			private struct RaycastResult
			{
				// Token: 0x04001098 RID: 4248
				public bool didHit;

				// Token: 0x04001099 RID: 4249
				public Vector3 point;

				// Token: 0x0400109A RID: 4250
				public Vector3 normal;

				// Token: 0x0400109B RID: 4251
				public float distance;
			}
		}

		// Token: 0x020002ED RID: 749
		public class RoachPathEditorComponent : MonoBehaviour
		{
			// Token: 0x17000213 RID: 531
			// (get) Token: 0x06001121 RID: 4385 RVA: 0x0004B983 File Offset: 0x00049B83
			public int nodeCount
			{
				get
				{
					return base.transform.childCount;
				}
			}

			// Token: 0x06001122 RID: 4386 RVA: 0x0004B990 File Offset: 0x00049B90
			public RoachController.RoachNodeEditorComponent AddNode()
			{
				GameObject gameObject = new GameObject("Roach Path Node (Temporary)");
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(base.transform);
				RoachController.RoachNodeEditorComponent roachNodeEditorComponent = gameObject.AddComponent<RoachController.RoachNodeEditorComponent>();
				roachNodeEditorComponent.path = this;
				return roachNodeEditorComponent;
			}

			// Token: 0x06001123 RID: 4387 RVA: 0x0004B9C4 File Offset: 0x00049BC4
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

			// Token: 0x0400109C RID: 4252
			public RoachController roachController;
		}

		// Token: 0x020002EE RID: 750
		public class RoachNodeEditorComponent : MonoBehaviour
		{
			// Token: 0x06001125 RID: 4389 RVA: 0x0004BA24 File Offset: 0x00049C24
			public void FacePosition(Vector3 position)
			{
				Vector3 position2 = base.transform.position;
				Vector3 up = base.transform.up;
				Quaternion rotation = Quaternion.LookRotation(position - position2, up);
				base.transform.rotation = rotation;
				base.transform.up = up;
			}

			// Token: 0x0400109D RID: 4253
			public RoachController.RoachPathEditorComponent path;
		}
	}
}
