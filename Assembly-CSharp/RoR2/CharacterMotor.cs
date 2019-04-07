using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000294 RID: 660
	[RequireComponent(typeof(CharacterBody))]
	public class CharacterMotor : BaseCharacterController, ILifeBehavior
	{
		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000D3E RID: 3390 RVA: 0x00041F70 File Offset: 0x00040170
		public float walkSpeed
		{
			get
			{
				return this.body.moveSpeed * this.walkSpeedPenaltyCoefficient;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000D3F RID: 3391 RVA: 0x00041F84 File Offset: 0x00040184
		public float acceleration
		{
			get
			{
				return this.body.acceleration;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000D40 RID: 3392 RVA: 0x00041F91 File Offset: 0x00040191
		public bool atRest
		{
			get
			{
				return this.restStopwatch > 1f;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000D42 RID: 3394 RVA: 0x00041FA9 File Offset: 0x000401A9
		// (set) Token: 0x06000D41 RID: 3393 RVA: 0x00041FA0 File Offset: 0x000401A0
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000D43 RID: 3395 RVA: 0x00041FB1 File Offset: 0x000401B1
		public Vector3 estimatedFloorNormal
		{
			get
			{
				return base.Motor.GroundingStatus.GroundNormal;
			}
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x00041FC3 File Offset: 0x000401C3
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00041FD8 File Offset: 0x000401D8
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.capsuleCollider = base.GetComponent<CapsuleCollider>();
			this.previousPosition = base.transform.position;
			base.Motor.Rigidbody.mass = this.mass;
			base.Motor.CollidableLayers = LayerIndex.defaultLayer.collisionMask;
			base.Motor.MaxStableSlopeAngle = 70f;
			base.Motor.MaxStableDenivelationAngle = 55f;
			if (this.generateParametersOnAwake)
			{
				this.GenerateParameters();
			}
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x0004206A File Offset: 0x0004026A
		private void Start()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x0004206A File Offset: 0x0004026A
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x0004206A File Offset: 0x0004026A
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00042072 File Offset: 0x00040272
		private void OnEnable()
		{
			CharacterMotor.instancesList.Add(this);
			base.Motor.enabled = true;
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x0004208B File Offset: 0x0004028B
		private void OnDisable()
		{
			base.Motor.enabled = false;
			CharacterMotor.instancesList.Remove(this);
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x000420A8 File Offset: 0x000402A8
		private void PreMove(float deltaTime)
		{
			if (this.hasEffectiveAuthority)
			{
				float num = this.acceleration;
				float num2 = this.velocity.y;
				num2 += Physics.gravity.y * deltaTime;
				if (this.isGrounded)
				{
					num2 = Mathf.Max(num2, 0f);
				}
				else
				{
					num *= (this.disableAirControlUntilCollision ? 0f : this.airControl);
				}
				Vector2 vector = new Vector2(this.velocity.x, this.velocity.z);
				Vector2 a = Vector2.zero;
				Vector3 v = Vector2.zero;
				if (this.canWalk)
				{
					a = new Vector2(this.moveDirection.x, this.moveDirection.z);
					if (this.body.isSprinting)
					{
						float magnitude = a.magnitude;
						if (magnitude < 1f && magnitude > 0f)
						{
							a /= magnitude;
						}
					}
					v = a * this.walkSpeed;
				}
				vector = Vector2.MoveTowards(vector, v, num * deltaTime);
				this.velocity = new Vector3(vector.x, num2, vector.y);
			}
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x000421D1 File Offset: 0x000403D1
		public void OnDeathStart()
		{
			this.alive = false;
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x000421DC File Offset: 0x000403DC
		private void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (fixedDeltaTime == 0f)
			{
				return;
			}
			Vector3 position = base.transform.position;
			if ((this.previousPosition - position).sqrMagnitude < 0.00062500004f * fixedDeltaTime)
			{
				this.restStopwatch += fixedDeltaTime;
			}
			else
			{
				this.restStopwatch = 0f;
			}
			this.previousPosition = position;
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x00042243 File Offset: 0x00040443
		private void GenerateParameters()
		{
			this.slopeLimit = 70f;
			this.stepOffset = Mathf.Min(this.capsuleHeight * 0.1f, 0.2f);
			this.stepHandlingMethod = StepHandlingMethod.None;
			this.ledgeHandling = false;
			this.interactiveRigidbodyHandling = true;
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000D4F RID: 3407 RVA: 0x00042281 File Offset: 0x00040481
		private bool canWalk
		{
			get
			{
				return !this.muteWalkMotion && this.alive;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000D50 RID: 3408 RVA: 0x00042293 File Offset: 0x00040493
		public bool isGrounded
		{
			get
			{
				if (!this.hasEffectiveAuthority)
				{
					return this.netIsGrounded;
				}
				return base.Motor.GroundingStatus.IsStableOnGround;
			}
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x000422B4 File Offset: 0x000404B4
		public void ApplyForce(Vector3 force, bool alwaysApply = false)
		{
			if (NetworkServer.active && !this.hasEffectiveAuthority)
			{
				this.CallRpcApplyForce(force, alwaysApply);
				return;
			}
			if (this.mass != 0f)
			{
				Vector3 vector = force * (1f / this.mass);
				if (vector.y < 6f && this.isGrounded && !alwaysApply)
				{
					vector.y = 0f;
				}
				this.velocity += vector;
			}
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x0004232F File Offset: 0x0004052F
		[ClientRpc]
		private void RpcApplyForce(Vector3 force, bool alwaysApply)
		{
			if (!NetworkServer.active)
			{
				this.ApplyForce(force, alwaysApply);
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x00042340 File Offset: 0x00040540
		// (set) Token: 0x06000D54 RID: 3412 RVA: 0x00042348 File Offset: 0x00040548
		public Vector3 moveDirection
		{
			get
			{
				return this._moveDirection;
			}
			set
			{
				this._moveDirection = value;
				this._moveDirection.y = 0f;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000D55 RID: 3413 RVA: 0x00042361 File Offset: 0x00040561
		// (set) Token: 0x06000D56 RID: 3414 RVA: 0x0004236E File Offset: 0x0004056E
		private float slopeLimit
		{
			get
			{
				return base.Motor.MaxStableSlopeAngle;
			}
			set
			{
				base.Motor.MaxStableSlopeAngle = value;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000D57 RID: 3415 RVA: 0x0004237C File Offset: 0x0004057C
		// (set) Token: 0x06000D58 RID: 3416 RVA: 0x00042389 File Offset: 0x00040589
		public float stepOffset
		{
			get
			{
				return base.Motor.MaxStepHeight;
			}
			set
			{
				base.Motor.MaxStepHeight = value;
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000D59 RID: 3417 RVA: 0x00042397 File Offset: 0x00040597
		public float capsuleHeight
		{
			get
			{
				return this.capsuleCollider.height;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000D5A RID: 3418 RVA: 0x000423A4 File Offset: 0x000405A4
		public float capsuleRadius
		{
			get
			{
				return this.capsuleCollider.radius;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000D5B RID: 3419 RVA: 0x000423B4 File Offset: 0x000405B4
		// (set) Token: 0x06000D5C RID: 3420 RVA: 0x000423D0 File Offset: 0x000405D0
		public StepHandlingMethod stepHandlingMethod
		{
			get
			{
				return base.Motor.StepHandling = StepHandlingMethod.None;
			}
			set
			{
				base.Motor.StepHandling = value;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000D5D RID: 3421 RVA: 0x000423DE File Offset: 0x000405DE
		// (set) Token: 0x06000D5E RID: 3422 RVA: 0x000423EB File Offset: 0x000405EB
		public bool ledgeHandling
		{
			get
			{
				return base.Motor.LedgeHandling;
			}
			set
			{
				base.Motor.LedgeHandling = value;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000D5F RID: 3423 RVA: 0x000423F9 File Offset: 0x000405F9
		// (set) Token: 0x06000D60 RID: 3424 RVA: 0x00042406 File Offset: 0x00040606
		public bool interactiveRigidbodyHandling
		{
			get
			{
				return base.Motor.InteractiveRigidbodyHandling;
			}
			set
			{
				base.Motor.InteractiveRigidbodyHandling = value;
			}
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x00042414 File Offset: 0x00040614
		private void OnHitGround(Vector3 fallVelocity)
		{
			if (NetworkServer.active)
			{
				GlobalEventManager.instance.OnCharacterHitGround(this.body, fallVelocity);
				return;
			}
			this.CallCmdHitGround(fallVelocity);
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x00042436 File Offset: 0x00040636
		[Command]
		private void CmdHitGround(Vector3 fallVelocity)
		{
			this.OnHitGround(fallVelocity);
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0004243F File Offset: 0x0004063F
		public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			currentRotation = Quaternion.identity;
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x0004244C File Offset: 0x0004064C
		public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			currentVelocity = this.velocity;
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x0004245C File Offset: 0x0004065C
		public override void BeforeCharacterUpdate(float deltaTime)
		{
			if (this.rootMotion != Vector3.zero)
			{
				base.Motor.MoveCharacter(base.transform.position + this.rootMotion);
				this.rootMotion = Vector3.zero;
			}
			this.PreMove(deltaTime);
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x000424B0 File Offset: 0x000406B0
		public override void PostGroundingUpdate(float deltaTime)
		{
			if (base.Motor.GroundingStatus.IsStableOnGround != base.Motor.LastGroundingStatus.IsStableOnGround)
			{
				if (base.Motor.GroundingStatus.IsStableOnGround)
				{
					this.OnLanded();
					return;
				}
				this.OnLeaveStableGround();
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x00042500 File Offset: 0x00040700
		private void OnLanded()
		{
			this.jumpCount = 0;
			CharacterMotor.HitGroundInfo hitGroundInfo = new CharacterMotor.HitGroundInfo
			{
				velocity = this.lastVelocity,
				position = base.Motor.GroundingStatus.GroundPoint
			};
			if (NetworkServer.active)
			{
				CharacterMotor.HitGroundDelegate hitGroundDelegate = this.onHitGround;
				if (hitGroundDelegate != null)
				{
					hitGroundDelegate(ref hitGroundInfo);
				}
				GlobalEventManager.instance.OnCharacterHitGround(this.body, hitGroundInfo.velocity);
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				CharacterMotor.HitGroundDelegate hitGroundDelegate2 = this.onHitGround;
				if (hitGroundDelegate2 != null)
				{
					hitGroundDelegate2(ref hitGroundInfo);
				}
				this.CallCmdHitGround(hitGroundInfo.velocity);
			}
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x0004259A File Offset: 0x0004079A
		private void OnLeaveStableGround()
		{
			if (this.jumpCount < 1)
			{
				this.jumpCount = 1;
			}
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x000425AC File Offset: 0x000407AC
		public override void AfterCharacterUpdate(float deltaTime)
		{
			this.lastVelocity = this.velocity;
			this.velocity = base.Motor.BaseVelocity;
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x000425CB File Offset: 0x000407CB
		public override bool IsColliderValidForCollisions(Collider coll)
		{
			return !coll.isTrigger && coll != base.Motor.Capsule;
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000D6C RID: 3436 RVA: 0x000425E8 File Offset: 0x000407E8
		// (remove) Token: 0x06000D6D RID: 3437 RVA: 0x00042620 File Offset: 0x00040820
		public event CharacterMotor.HitGroundDelegate onHitGround;

		// Token: 0x06000D6E RID: 3438 RVA: 0x00042655 File Offset: 0x00040855
		public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			this.disableAirControlUntilCollision = false;
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x00004507 File Offset: 0x00002707
		public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x00042698 File Offset: 0x00040898
		static CharacterMotor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMotor), CharacterMotor.kCmdCmdHitGround, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeCmdCmdHitGround));
			CharacterMotor.kRpcRpcApplyForce = -1753076289;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterMotor), CharacterMotor.kRpcRpcApplyForce, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeRpcRpcApplyForce));
			NetworkCRC.RegisterBehaviour("CharacterMotor", 0);
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x00042712 File Offset: 0x00040912
		protected static void InvokeCmdCmdHitGround(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdHitGround called on client.");
				return;
			}
			((CharacterMotor)obj).CmdHitGround(reader.ReadVector3());
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x0004273C File Offset: 0x0004093C
		public void CallCmdHitGround(Vector3 fallVelocity)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdHitGround called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdHitGround(fallVelocity);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterMotor.kCmdCmdHitGround);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(fallVelocity);
			base.SendCommandInternal(networkWriter, 0, "CmdHitGround");
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x000427C6 File Offset: 0x000409C6
		protected static void InvokeRpcRpcApplyForce(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcApplyForce called on server.");
				return;
			}
			((CharacterMotor)obj).RpcApplyForce(reader.ReadVector3(), reader.ReadBoolean());
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x000427F8 File Offset: 0x000409F8
		public void CallRpcApplyForce(Vector3 force, bool alwaysApply)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcApplyForce called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterMotor.kRpcRpcApplyForce);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(force);
			networkWriter.Write(alwaysApply);
			this.SendRPCInternal(networkWriter, 0, "RpcApplyForce");
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x00042878 File Offset: 0x00040A78
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			bool flag2;
			return flag2 || flag;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00042891 File Offset: 0x00040A91
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
		}

		// Token: 0x0400115A RID: 4442
		public static readonly List<CharacterMotor> instancesList = new List<CharacterMotor>();

		// Token: 0x0400115B RID: 4443
		[HideInInspector]
		public float walkSpeedPenaltyCoefficient = 1f;

		// Token: 0x0400115C RID: 4444
		[Tooltip("The character direction component to supply a move vector to.")]
		public CharacterDirection characterDirection;

		// Token: 0x0400115D RID: 4445
		[Tooltip("Whether or not a move vector supplied to this component can cause movement. Use this when the object is driven by root motion.")]
		public bool muteWalkMotion;

		// Token: 0x0400115E RID: 4446
		[Tooltip("The mass of this character.")]
		public float mass = 1f;

		// Token: 0x0400115F RID: 4447
		[Tooltip("The air control value of this character as a fraction of ground control.")]
		public float airControl = 0.25f;

		// Token: 0x04001160 RID: 4448
		[Tooltip("Disables Air Control for things like jumppads")]
		public bool disableAirControlUntilCollision;

		// Token: 0x04001161 RID: 4449
		[Tooltip("Auto-assigns parameters skin width, slope angle, and step offset as a function of the Character Motor's radius and height")]
		public bool generateParametersOnAwake = true;

		// Token: 0x04001162 RID: 4450
		private CharacterBody body;

		// Token: 0x04001163 RID: 4451
		private CapsuleCollider capsuleCollider;

		// Token: 0x04001164 RID: 4452
		private bool alive = true;

		// Token: 0x04001165 RID: 4453
		private const float restDuration = 1f;

		// Token: 0x04001166 RID: 4454
		private const float restVelocityThreshold = 0.025f;

		// Token: 0x04001167 RID: 4455
		private const float restVelocityThresholdSqr = 0.00062500004f;

		// Token: 0x04001168 RID: 4456
		public const float slipStartAngle = 70f;

		// Token: 0x04001169 RID: 4457
		public const float slipEndAngle = 55f;

		// Token: 0x0400116A RID: 4458
		private float restStopwatch;

		// Token: 0x0400116B RID: 4459
		private Vector3 previousPosition;

		// Token: 0x0400116D RID: 4461
		[NonSerialized]
		public int jumpCount;

		// Token: 0x0400116E RID: 4462
		[NonSerialized]
		public bool netIsGrounded;

		// Token: 0x0400116F RID: 4463
		[NonSerialized]
		public Vector3 velocity;

		// Token: 0x04001170 RID: 4464
		private Vector3 lastVelocity;

		// Token: 0x04001171 RID: 4465
		[NonSerialized]
		public Vector3 rootMotion;

		// Token: 0x04001172 RID: 4466
		private Vector3 _moveDirection;

		// Token: 0x04001174 RID: 4468
		private static int kRpcRpcApplyForce;

		// Token: 0x04001175 RID: 4469
		private static int kCmdCmdHitGround = 2030335022;

		// Token: 0x02000295 RID: 661
		public struct HitGroundInfo
		{
			// Token: 0x06000D79 RID: 3449 RVA: 0x0004289B File Offset: 0x00040A9B
			public override string ToString()
			{
				return string.Format("velocity={0} position={1}", this.velocity, this.position);
			}

			// Token: 0x04001176 RID: 4470
			public Vector3 velocity;

			// Token: 0x04001177 RID: 4471
			public Vector3 position;
		}

		// Token: 0x02000296 RID: 662
		// (Invoke) Token: 0x06000D7B RID: 3451
		public delegate void HitGroundDelegate(ref CharacterMotor.HitGroundInfo hitGroundInfo);
	}
}
