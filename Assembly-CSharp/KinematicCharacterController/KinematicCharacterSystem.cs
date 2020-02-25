using System;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000919 RID: 2329
	[DefaultExecutionOrder(-100)]
	public class KinematicCharacterSystem : MonoBehaviour
	{
		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x0600343E RID: 13374 RVA: 0x000E3A67 File Offset: 0x000E1C67
		// (set) Token: 0x0600343F RID: 13375 RVA: 0x000E3A70 File Offset: 0x000E1C70
		public static CharacterSystemInterpolationMethod InterpolationMethod
		{
			get
			{
				return KinematicCharacterSystem._internalInterpolationMethod;
			}
			set
			{
				KinematicCharacterSystem._internalInterpolationMethod = value;
				KinematicCharacterSystem.MoveActorsToDestination();
				RigidbodyInterpolation interpolation = (KinematicCharacterSystem._internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
				for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
				{
					KinematicCharacterSystem.CharacterMotors[i].Rigidbody.interpolation = interpolation;
				}
				for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
				{
					KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.interpolation = interpolation;
				}
			}
		}

		// Token: 0x06003440 RID: 13376 RVA: 0x000E3AEB File Offset: 0x000E1CEB
		public static void EnsureCreation()
		{
			if (KinematicCharacterSystem._instance == null)
			{
				GameObject gameObject = new GameObject("KinematicCharacterSystem");
				KinematicCharacterSystem._instance = gameObject.AddComponent<KinematicCharacterSystem>();
				gameObject.hideFlags = HideFlags.NotEditable;
				KinematicCharacterSystem._instance.hideFlags = HideFlags.NotEditable;
			}
		}

		// Token: 0x06003441 RID: 13377 RVA: 0x000E3B20 File Offset: 0x000E1D20
		public static KinematicCharacterSystem GetInstance()
		{
			return KinematicCharacterSystem._instance;
		}

		// Token: 0x06003442 RID: 13378 RVA: 0x000E3B27 File Offset: 0x000E1D27
		public static void SetCharacterMotorsCapacity(int capacity)
		{
			if (capacity < KinematicCharacterSystem.CharacterMotors.Count)
			{
				capacity = KinematicCharacterSystem.CharacterMotors.Count;
			}
			KinematicCharacterSystem.CharacterMotors.Capacity = capacity;
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x000E3B50 File Offset: 0x000E1D50
		public static void RegisterCharacterMotor(KinematicCharacterMotor motor)
		{
			KinematicCharacterSystem.CharacterMotors.Add(motor);
			RigidbodyInterpolation interpolation = (KinematicCharacterSystem._internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
			motor.Rigidbody.interpolation = interpolation;
		}

		// Token: 0x06003444 RID: 13380 RVA: 0x000E3B81 File Offset: 0x000E1D81
		public static void UnregisterCharacterMotor(KinematicCharacterMotor motor)
		{
			KinematicCharacterSystem.CharacterMotors.Remove(motor);
		}

		// Token: 0x06003445 RID: 13381 RVA: 0x000E3B8F File Offset: 0x000E1D8F
		public static void SetPhysicsMoversCapacity(int capacity)
		{
			if (capacity < KinematicCharacterSystem.PhysicsMovers.Count)
			{
				capacity = KinematicCharacterSystem.PhysicsMovers.Count;
			}
			KinematicCharacterSystem.PhysicsMovers.Capacity = capacity;
		}

		// Token: 0x06003446 RID: 13382 RVA: 0x000E3BB8 File Offset: 0x000E1DB8
		public static void RegisterPhysicsMover(PhysicsMover mover)
		{
			KinematicCharacterSystem.PhysicsMovers.Add(mover);
			RigidbodyInterpolation interpolation = (KinematicCharacterSystem._internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
			mover.Rigidbody.interpolation = interpolation;
		}

		// Token: 0x06003447 RID: 13383 RVA: 0x000E3BE9 File Offset: 0x000E1DE9
		public static void UnregisterPhysicsMover(PhysicsMover mover)
		{
			KinematicCharacterSystem.PhysicsMovers.Remove(mover);
		}

		// Token: 0x06003448 RID: 13384 RVA: 0x0002F7B2 File Offset: 0x0002D9B2
		private void OnDisable()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06003449 RID: 13385 RVA: 0x000E3BF7 File Offset: 0x000E1DF7
		private void Awake()
		{
			KinematicCharacterSystem._instance = this;
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x000E3C00 File Offset: 0x000E1E00
		private void FixedUpdate()
		{
			if (KinematicCharacterSystem.AutoSimulation)
			{
				float deltaTime = Time.deltaTime;
				if (deltaTime == 0f)
				{
					return;
				}
				KinematicCharacterSystem.PreSimulationUpdate(deltaTime);
				KinematicCharacterSystem.Simulate(deltaTime);
				KinematicCharacterSystem.PostSimulationUpdate(deltaTime);
			}
		}

		// Token: 0x0600344B RID: 13387 RVA: 0x000E3C35 File Offset: 0x000E1E35
		private void Update()
		{
			if (KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
			{
				KinematicCharacterSystem.CustomInterpolationUpdate();
			}
		}

		// Token: 0x0600344C RID: 13388 RVA: 0x000E3C44 File Offset: 0x000E1E44
		public static void Simulate(float deltaTime)
		{
			for (int i = 0; i < KinematicCharacterSystem.PhysicsMovers.Count; i++)
			{
				KinematicCharacterSystem.PhysicsMovers[i].VelocityUpdate(deltaTime);
			}
			for (int j = 0; j < KinematicCharacterSystem.CharacterMotors.Count; j++)
			{
				KinematicCharacterSystem.CharacterMotors[j].UpdatePhase1(deltaTime);
			}
			for (int k = 0; k < KinematicCharacterSystem.PhysicsMovers.Count; k++)
			{
				KinematicCharacterSystem.PhysicsMovers[k].Transform.SetPositionAndRotation(KinematicCharacterSystem.PhysicsMovers[k].TransientPosition, KinematicCharacterSystem.PhysicsMovers[k].TransientRotation);
				KinematicCharacterSystem.PhysicsMovers[k].Rigidbody.position = KinematicCharacterSystem.PhysicsMovers[k].TransientPosition;
				KinematicCharacterSystem.PhysicsMovers[k].Rigidbody.rotation = KinematicCharacterSystem.PhysicsMovers[k].TransientRotation;
			}
			for (int l = 0; l < KinematicCharacterSystem.CharacterMotors.Count; l++)
			{
				KinematicCharacterSystem.CharacterMotors[l].UpdatePhase2(deltaTime);
				KinematicCharacterSystem.CharacterMotors[l].Transform.SetPositionAndRotation(KinematicCharacterSystem.CharacterMotors[l].TransientPosition, KinematicCharacterSystem.CharacterMotors[l].TransientRotation);
				KinematicCharacterSystem.CharacterMotors[l].Rigidbody.position = KinematicCharacterSystem.CharacterMotors[l].TransientPosition;
				KinematicCharacterSystem.CharacterMotors[l].Rigidbody.rotation = KinematicCharacterSystem.CharacterMotors[l].TransientRotation;
			}
		}

		// Token: 0x0600344D RID: 13389 RVA: 0x000E3DE4 File Offset: 0x000E1FE4
		public static void Simulate(float deltaTime, KinematicCharacterMotor[] motors, int characterMotorsCount, PhysicsMover[] movers, int physicsMoversCount)
		{
			for (int i = 0; i < physicsMoversCount; i++)
			{
				movers[i].VelocityUpdate(deltaTime);
			}
			for (int j = 0; j < characterMotorsCount; j++)
			{
				motors[j].UpdatePhase1(deltaTime);
			}
			for (int k = 0; k < physicsMoversCount; k++)
			{
				movers[k].Transform.SetPositionAndRotation(movers[k].TransientPosition, movers[k].TransientRotation);
				movers[k].Rigidbody.position = movers[k].TransientPosition;
				movers[k].Rigidbody.rotation = movers[k].TransientRotation;
			}
			for (int l = 0; l < characterMotorsCount; l++)
			{
				motors[l].UpdatePhase2(deltaTime);
				motors[l].Transform.SetPositionAndRotation(motors[l].TransientPosition, motors[l].TransientRotation);
				motors[l].Rigidbody.position = motors[l].TransientPosition;
				motors[l].Rigidbody.rotation = motors[l].TransientRotation;
			}
		}

		// Token: 0x0600344E RID: 13390 RVA: 0x000E3ECC File Offset: 0x000E20CC
		public static void PreSimulationUpdate(float deltaTime)
		{
			if (KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
			{
				KinematicCharacterSystem.MoveActorsToDestination();
			}
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterSystem.CharacterMotors[i].InitialTickPosition = KinematicCharacterSystem.CharacterMotors[i].Transform.position;
				KinematicCharacterSystem.CharacterMotors[i].InitialTickRotation = KinematicCharacterSystem.CharacterMotors[i].Transform.rotation;
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				KinematicCharacterSystem.PhysicsMovers[j].InitialTickPosition = KinematicCharacterSystem.PhysicsMovers[j].Transform.position;
				KinematicCharacterSystem.PhysicsMovers[j].InitialTickRotation = KinematicCharacterSystem.PhysicsMovers[j].Transform.rotation;
			}
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x000E3FA4 File Offset: 0x000E21A4
		public static void PostSimulationUpdate(float deltaTime)
		{
			Physics.SyncTransforms();
			if (KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
			{
				KinematicCharacterSystem._lastCustomInterpolationStartTime = Time.time;
				KinematicCharacterSystem._lastCustomInterpolationDeltaTime = deltaTime;
			}
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterSystem.CharacterMotors[i].Rigidbody.position = KinematicCharacterSystem.CharacterMotors[i].InitialTickPosition;
				KinematicCharacterSystem.CharacterMotors[i].Rigidbody.rotation = KinematicCharacterSystem.CharacterMotors[i].InitialTickRotation;
				KinematicCharacterSystem.CharacterMotors[i].Rigidbody.MovePosition(KinematicCharacterSystem.CharacterMotors[i].TransientPosition);
				KinematicCharacterSystem.CharacterMotors[i].Rigidbody.MoveRotation(KinematicCharacterSystem.CharacterMotors[i].TransientRotation);
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.position = KinematicCharacterSystem.PhysicsMovers[j].InitialTickPosition;
				KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.rotation = KinematicCharacterSystem.PhysicsMovers[j].InitialTickRotation;
				KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.MovePosition(KinematicCharacterSystem.PhysicsMovers[j].TransientPosition);
				KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.MoveRotation(KinematicCharacterSystem.PhysicsMovers[j].TransientRotation);
			}
		}

		// Token: 0x06003450 RID: 13392 RVA: 0x000E412C File Offset: 0x000E232C
		private static void MoveActorsToDestination()
		{
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterSystem.CharacterMotors[i].Transform.SetPositionAndRotation(KinematicCharacterSystem.CharacterMotors[i].TransientPosition, KinematicCharacterSystem.CharacterMotors[i].TransientRotation);
				KinematicCharacterSystem.CharacterMotors[i].Rigidbody.position = KinematicCharacterSystem.CharacterMotors[i].TransientPosition;
				KinematicCharacterSystem.CharacterMotors[i].Rigidbody.rotation = KinematicCharacterSystem.CharacterMotors[i].TransientRotation;
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				KinematicCharacterSystem.PhysicsMovers[j].Transform.SetPositionAndRotation(KinematicCharacterSystem.PhysicsMovers[j].TransientPosition, KinematicCharacterSystem.PhysicsMovers[j].TransientRotation);
				KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.position = KinematicCharacterSystem.PhysicsMovers[j].TransientPosition;
				KinematicCharacterSystem.PhysicsMovers[j].Rigidbody.rotation = KinematicCharacterSystem.PhysicsMovers[j].TransientRotation;
			}
		}

		// Token: 0x06003451 RID: 13393 RVA: 0x000E4270 File Offset: 0x000E2470
		private static void CustomInterpolationUpdate()
		{
			float t = Mathf.Clamp01((Time.time - KinematicCharacterSystem._lastCustomInterpolationStartTime) / KinematicCharacterSystem._lastCustomInterpolationDeltaTime);
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterSystem.CharacterMotors[i].Transform.SetPositionAndRotation(Vector3.Lerp(KinematicCharacterSystem.CharacterMotors[i].InitialTickPosition, KinematicCharacterSystem.CharacterMotors[i].TransientPosition, t), Quaternion.Slerp(KinematicCharacterSystem.CharacterMotors[i].InitialTickRotation, KinematicCharacterSystem.CharacterMotors[i].TransientRotation, t));
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				KinematicCharacterSystem.PhysicsMovers[j].Transform.SetPositionAndRotation(Vector3.Lerp(KinematicCharacterSystem.PhysicsMovers[j].InitialTickPosition, KinematicCharacterSystem.PhysicsMovers[j].TransientPosition, t), Quaternion.Slerp(KinematicCharacterSystem.PhysicsMovers[j].InitialTickRotation, KinematicCharacterSystem.PhysicsMovers[j].TransientRotation, t));
			}
		}

		// Token: 0x040033D9 RID: 13273
		private static KinematicCharacterSystem _instance;

		// Token: 0x040033DA RID: 13274
		public static List<KinematicCharacterMotor> CharacterMotors = new List<KinematicCharacterMotor>(100);

		// Token: 0x040033DB RID: 13275
		public static List<PhysicsMover> PhysicsMovers = new List<PhysicsMover>(100);

		// Token: 0x040033DC RID: 13276
		public static bool AutoSimulation = true;

		// Token: 0x040033DD RID: 13277
		private static float _lastCustomInterpolationStartTime = -1f;

		// Token: 0x040033DE RID: 13278
		private static float _lastCustomInterpolationDeltaTime = -1f;

		// Token: 0x040033DF RID: 13279
		private const int CharacterMotorsBaseCapacity = 100;

		// Token: 0x040033E0 RID: 13280
		private const int PhysicsMoversBaseCapacity = 100;

		// Token: 0x040033E1 RID: 13281
		[SerializeField]
		private static CharacterSystemInterpolationMethod _internalInterpolationMethod = CharacterSystemInterpolationMethod.Custom;
	}
}
