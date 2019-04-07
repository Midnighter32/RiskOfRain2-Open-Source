using System;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006CE RID: 1742
	[DefaultExecutionOrder(-100)]
	public class KinematicCharacterSystem : MonoBehaviour
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060026F4 RID: 9972 RVA: 0x000B461F File Offset: 0x000B281F
		// (set) Token: 0x060026F5 RID: 9973 RVA: 0x000B4628 File Offset: 0x000B2828
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

		// Token: 0x060026F6 RID: 9974 RVA: 0x000B46A3 File Offset: 0x000B28A3
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

		// Token: 0x060026F7 RID: 9975 RVA: 0x000B46D8 File Offset: 0x000B28D8
		public static KinematicCharacterSystem GetInstance()
		{
			return KinematicCharacterSystem._instance;
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x000B46DF File Offset: 0x000B28DF
		public static void SetCharacterMotorsCapacity(int capacity)
		{
			if (capacity < KinematicCharacterSystem.CharacterMotors.Count)
			{
				capacity = KinematicCharacterSystem.CharacterMotors.Count;
			}
			KinematicCharacterSystem.CharacterMotors.Capacity = capacity;
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x000B4708 File Offset: 0x000B2908
		public static void RegisterCharacterMotor(KinematicCharacterMotor motor)
		{
			KinematicCharacterSystem.CharacterMotors.Add(motor);
			RigidbodyInterpolation interpolation = (KinematicCharacterSystem._internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
			motor.Rigidbody.interpolation = interpolation;
		}

		// Token: 0x060026FA RID: 9978 RVA: 0x000B4739 File Offset: 0x000B2939
		public static void UnregisterCharacterMotor(KinematicCharacterMotor motor)
		{
			KinematicCharacterSystem.CharacterMotors.Remove(motor);
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x000B4747 File Offset: 0x000B2947
		public static void SetPhysicsMoversCapacity(int capacity)
		{
			if (capacity < KinematicCharacterSystem.PhysicsMovers.Count)
			{
				capacity = KinematicCharacterSystem.PhysicsMovers.Count;
			}
			KinematicCharacterSystem.PhysicsMovers.Capacity = capacity;
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x000B4770 File Offset: 0x000B2970
		public static void RegisterPhysicsMover(PhysicsMover mover)
		{
			KinematicCharacterSystem.PhysicsMovers.Add(mover);
			RigidbodyInterpolation interpolation = (KinematicCharacterSystem._internalInterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
			mover.Rigidbody.interpolation = interpolation;
		}

		// Token: 0x060026FD RID: 9981 RVA: 0x000B47A1 File Offset: 0x000B29A1
		public static void UnregisterPhysicsMover(PhysicsMover mover)
		{
			KinematicCharacterSystem.PhysicsMovers.Remove(mover);
		}

		// Token: 0x060026FE RID: 9982 RVA: 0x0004A8F2 File Offset: 0x00048AF2
		private void OnDisable()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060026FF RID: 9983 RVA: 0x000B47AF File Offset: 0x000B29AF
		private void Awake()
		{
			KinematicCharacterSystem._instance = this;
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x000B47B8 File Offset: 0x000B29B8
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

		// Token: 0x06002701 RID: 9985 RVA: 0x000B47ED File Offset: 0x000B29ED
		private void Update()
		{
			if (KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Custom)
			{
				KinematicCharacterSystem.CustomInterpolationUpdate();
			}
		}

		// Token: 0x06002702 RID: 9986 RVA: 0x000B47FC File Offset: 0x000B29FC
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

		// Token: 0x06002703 RID: 9987 RVA: 0x000B499C File Offset: 0x000B2B9C
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

		// Token: 0x06002704 RID: 9988 RVA: 0x000B4A84 File Offset: 0x000B2C84
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

		// Token: 0x06002705 RID: 9989 RVA: 0x000B4B5C File Offset: 0x000B2D5C
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

		// Token: 0x06002706 RID: 9990 RVA: 0x000B4CE4 File Offset: 0x000B2EE4
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

		// Token: 0x06002707 RID: 9991 RVA: 0x000B4E28 File Offset: 0x000B3028
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

		// Token: 0x04002940 RID: 10560
		private static KinematicCharacterSystem _instance;

		// Token: 0x04002941 RID: 10561
		public static List<KinematicCharacterMotor> CharacterMotors = new List<KinematicCharacterMotor>(100);

		// Token: 0x04002942 RID: 10562
		public static List<PhysicsMover> PhysicsMovers = new List<PhysicsMover>(100);

		// Token: 0x04002943 RID: 10563
		public static bool AutoSimulation = true;

		// Token: 0x04002944 RID: 10564
		private static float _lastCustomInterpolationStartTime = -1f;

		// Token: 0x04002945 RID: 10565
		private static float _lastCustomInterpolationDeltaTime = -1f;

		// Token: 0x04002946 RID: 10566
		private const int CharacterMotorsBaseCapacity = 100;

		// Token: 0x04002947 RID: 10567
		private const int PhysicsMoversBaseCapacity = 100;

		// Token: 0x04002948 RID: 10568
		[SerializeField]
		private static CharacterSystemInterpolationMethod _internalInterpolationMethod = CharacterSystemInterpolationMethod.Custom;
	}
}
