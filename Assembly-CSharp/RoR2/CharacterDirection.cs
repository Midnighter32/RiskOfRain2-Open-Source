using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200018B RID: 395
	public class CharacterDirection : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x00022A59 File Offset: 0x00020C59
		// (set) Token: 0x060007F9 RID: 2041 RVA: 0x00022A23 File Offset: 0x00020C23
		public float yaw
		{
			get
			{
				return this._yaw;
			}
			set
			{
				this._yaw = value;
				if (this.targetTransform)
				{
					this.targetTransform.rotation = Quaternion.Euler(0f, this._yaw, 0f);
				}
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060007FB RID: 2043 RVA: 0x00022A64 File Offset: 0x00020C64
		public Vector3 animatorForward
		{
			get
			{
				if (!this.overrideAnimatorForwardTransform)
				{
					return this.forward;
				}
				float y = this.overrideAnimatorForwardTransform.eulerAngles.y;
				return Quaternion.Euler(0f, y, 0f) * Vector3.forward;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x00022AE7 File Offset: 0x00020CE7
		// (set) Token: 0x060007FC RID: 2044 RVA: 0x00022AB0 File Offset: 0x00020CB0
		public Vector3 forward
		{
			get
			{
				return Quaternion.Euler(0f, this.yaw, 0f) * Vector3.forward;
			}
			set
			{
				value.y = 0f;
				this.yaw = Util.QuaternionSafeLookRotation(value, Vector3.up).eulerAngles.y;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x00022B11 File Offset: 0x00020D11
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x00022B08 File Offset: 0x00020D08
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000800 RID: 2048 RVA: 0x00022B19 File Offset: 0x00020D19
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00022B2C File Offset: 0x00020D2C
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x00022B2C File Offset: 0x00020D2C
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x00022B34 File Offset: 0x00020D34
		private void Start()
		{
			this.UpdateAuthority();
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				this.modelAnimator = component.modelTransform.GetComponent<Animator>();
			}
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00022B67 File Offset: 0x00020D67
		private void Update()
		{
			this.Simulate(Time.deltaTime);
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00022B74 File Offset: 0x00020D74
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00022B80 File Offset: 0x00020D80
		private static int PickIndex(float angle)
		{
			float num = Mathf.Sign(angle);
			int num2 = Mathf.CeilToInt((angle * num - 22.5f) * 0.022222223f);
			return Mathf.Clamp(CharacterDirection.paramsMidIndex + num2 * (int)num, 0, CharacterDirection.turnAnimatorParamsSets.Length - 1);
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00022BC4 File Offset: 0x00020DC4
		private void Simulate(float deltaTime)
		{
			Quaternion quaternion = Quaternion.Euler(0f, this.yaw, 0f);
			if (this.hasEffectiveAuthority)
			{
				if (this.driveFromRootRotation)
				{
					Quaternion rhs = this.rootMotionAccumulator.ExtractRootRotation();
					if (this.targetTransform)
					{
						this.targetTransform.rotation = quaternion * rhs;
						float y = this.targetTransform.rotation.eulerAngles.y;
						this.yaw = y;
						float angle = 0f;
						if (this.moveVector.sqrMagnitude > 0f)
						{
							angle = Util.AngleSigned(Vector3.ProjectOnPlane(this.moveVector, Vector3.up), this.targetTransform.forward, -Vector3.up);
						}
						int num = CharacterDirection.PickIndex(angle);
						CharacterDirection.turnAnimatorParamsSets[num].Apply(this.modelAnimator);
					}
				}
				this.targetVector = this.moveVector;
				this.targetVector.y = 0f;
				if (this.targetVector != Vector3.zero && deltaTime != 0f)
				{
					this.targetVector.Normalize();
					Quaternion quaternion2 = Util.QuaternionSafeLookRotation(this.targetVector, Vector3.up);
					float num2 = Mathf.SmoothDampAngle(this.yaw, quaternion2.eulerAngles.y, ref this.yRotationVelocity, 360f / this.turnSpeed * 0.25f, float.PositiveInfinity, deltaTime);
					quaternion = Quaternion.Euler(0f, num2, 0f);
					this.yaw = num2;
				}
				if (this.targetTransform)
				{
					this.targetTransform.rotation = quaternion;
				}
			}
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00023004 File Offset: 0x00021204
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400086C RID: 2156
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x0400086D RID: 2157
		[Tooltip("The transform to rotate.")]
		public Transform targetTransform;

		// Token: 0x0400086E RID: 2158
		[Tooltip("The transform to take the rotation from for animator purposes. Commonly the root node.")]
		public Transform overrideAnimatorForwardTransform;

		// Token: 0x0400086F RID: 2159
		public RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04000870 RID: 2160
		public Animator modelAnimator;

		// Token: 0x04000871 RID: 2161
		[Tooltip("The character direction is set by root rotation, rather than moveVector.")]
		public bool driveFromRootRotation;

		// Token: 0x04000872 RID: 2162
		[Tooltip("The maximum turn rate in degrees/second.")]
		public float turnSpeed = 360f;

		// Token: 0x04000873 RID: 2163
		private float yRotationVelocity;

		// Token: 0x04000874 RID: 2164
		private float _yaw;

		// Token: 0x04000875 RID: 2165
		private Vector3 targetVector = Vector3.zero;

		// Token: 0x04000877 RID: 2167
		private const float offset = 22.5f;

		// Token: 0x04000878 RID: 2168
		private static readonly CharacterDirection.TurnAnimatorParamsSet[] turnAnimatorParamsSets = new CharacterDirection.TurnAnimatorParamsSet[]
		{
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = -180f,
				angleMax = -112.5f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = true
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = -112.5f,
				angleMax = -67.5f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = true,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = -67.5f,
				angleMax = -22.5f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = true,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = 22.5f,
				angleMax = 67.5f,
				turnRight45 = true,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = 67.5f,
				angleMax = 112.5f,
				turnRight45 = false,
				turnRight90 = true,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = 112.5f,
				angleMax = 180f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = true,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			}
		};

		// Token: 0x04000879 RID: 2169
		private static readonly int paramsMidIndex = CharacterDirection.turnAnimatorParamsSets.Length >> 1;

		// Token: 0x0200018C RID: 396
		private struct TurnAnimatorParamsSet
		{
			// Token: 0x0600080D RID: 2061 RVA: 0x00023014 File Offset: 0x00021214
			public void Apply(Animator animator)
			{
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight45ParamHash, this.turnRight45);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight90ParamHash, this.turnRight90);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight135ParamHash, this.turnRight135);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft45ParamHash, this.turnLeft45);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft90ParamHash, this.turnLeft90);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft135ParamHash, this.turnLeft135);
			}

			// Token: 0x0400087A RID: 2170
			public float angleMin;

			// Token: 0x0400087B RID: 2171
			public float angleMax;

			// Token: 0x0400087C RID: 2172
			public bool turnRight45;

			// Token: 0x0400087D RID: 2173
			public bool turnRight90;

			// Token: 0x0400087E RID: 2174
			public bool turnRight135;

			// Token: 0x0400087F RID: 2175
			public bool turnLeft45;

			// Token: 0x04000880 RID: 2176
			public bool turnLeft90;

			// Token: 0x04000881 RID: 2177
			public bool turnLeft135;

			// Token: 0x04000882 RID: 2178
			private static readonly int turnRight45ParamHash = Animator.StringToHash("turnRight45");

			// Token: 0x04000883 RID: 2179
			private static readonly int turnRight90ParamHash = Animator.StringToHash("turnRight90");

			// Token: 0x04000884 RID: 2180
			private static readonly int turnRight135ParamHash = Animator.StringToHash("turnRight135");

			// Token: 0x04000885 RID: 2181
			private static readonly int turnLeft45ParamHash = Animator.StringToHash("turnLeft45");

			// Token: 0x04000886 RID: 2182
			private static readonly int turnLeft90ParamHash = Animator.StringToHash("turnLeft90");

			// Token: 0x04000887 RID: 2183
			private static readonly int turnLeft135ParamHash = Animator.StringToHash("turnLeft135");
		}
	}
}
