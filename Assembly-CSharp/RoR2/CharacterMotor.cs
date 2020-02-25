using System;
using System.Collections.Generic;
using System.Globalization;
using KinematicCharacterController;
using RoR2.ConVar;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200019A RID: 410
	[RequireComponent(typeof(CharacterBody))]
	public class CharacterMotor : BaseCharacterController, ILifeBehavior, IDisplacementReceiver
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x00025E8F File Offset: 0x0002408F
		public float walkSpeed
		{
			get
			{
				return this.body.moveSpeed * this.walkSpeedPenaltyCoefficient;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600089C RID: 2204 RVA: 0x00025EA3 File Offset: 0x000240A3
		public float acceleration
		{
			get
			{
				return this.body.acceleration;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600089D RID: 2205 RVA: 0x00025EB0 File Offset: 0x000240B0
		public bool atRest
		{
			get
			{
				return this.restStopwatch > 1f;
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x00025EC8 File Offset: 0x000240C8
		// (set) Token: 0x0600089E RID: 2206 RVA: 0x00025EBF File Offset: 0x000240BF
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060008A0 RID: 2208 RVA: 0x00025ED0 File Offset: 0x000240D0
		public Vector3 estimatedGroundNormal
		{
			get
			{
				if (!this.hasEffectiveAuthority)
				{
					return this.netGroundNormal;
				}
				return base.Motor.GroundingStatus.GroundNormal;
			}
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x00025EF1 File Offset: 0x000240F1
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
			base.Motor.enabled = (CharacterMotor.enableMotorWithoutAuthority || this.hasEffectiveAuthority);
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00025F20 File Offset: 0x00024120
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.capsuleCollider = base.GetComponent<CapsuleCollider>();
			this.previousPosition = base.transform.position;
			base.Motor.Rigidbody.mass = this.mass;
			base.Motor.MaxStableSlopeAngle = 70f;
			base.Motor.MaxStableDenivelationAngle = 55f;
			base.Motor.RebuildCollidableLayers();
			if (this.generateParametersOnAwake)
			{
				this.GenerateParameters();
			}
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00025FA5 File Offset: 0x000241A5
		private void Start()
		{
			this.UpdateAuthority();
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00025FA5 File Offset: 0x000241A5
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x00025FA5 File Offset: 0x000241A5
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00025FAD File Offset: 0x000241AD
		private void OnEnable()
		{
			CharacterMotor.instancesList.Add(this);
			base.Motor.enabled = true;
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00025FC6 File Offset: 0x000241C6
		private void OnDisable()
		{
			base.Motor.enabled = false;
			CharacterMotor.instancesList.Remove(this);
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00025FE0 File Offset: 0x000241E0
		private void PreMove(float deltaTime)
		{
			if (this.hasEffectiveAuthority)
			{
				float num = this.acceleration;
				if (!this.isGrounded)
				{
					num *= (this.disableAirControlUntilCollision ? 0f : this.airControl);
				}
				Vector3 a = this.moveDirection;
				if (!this.isFlying)
				{
					a.y = 0f;
				}
				if (this.body.isSprinting)
				{
					float magnitude = a.magnitude;
					if (magnitude < 1f && magnitude > 0f)
					{
						float d = 1f / a.magnitude;
						a *= d;
					}
				}
				Vector3 target = a * this.walkSpeed;
				if (!this.isFlying)
				{
					target.y = this.velocity.y;
				}
				this.velocity = Vector3.MoveTowards(this.velocity, target, num * deltaTime);
				if (this.useGravity)
				{
					ref float ptr = ref this.velocity.y;
					ptr += Physics.gravity.y * deltaTime;
					if (this.isGrounded)
					{
						ptr = Mathf.Max(ptr, 0f);
					}
				}
			}
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x000260F3 File Offset: 0x000242F3
		public void OnDeathStart()
		{
			this.alive = false;
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x000260FC File Offset: 0x000242FC
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
			if (this.netIsGrounded)
			{
				this.lastGroundedTime = Run.FixedTimeStamp.now;
			}
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x00026178 File Offset: 0x00024378
		private void OnValidate()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (!component.mass.Equals(this.mass))
			{
				component.mass = this.mass;
			}
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x000261AE File Offset: 0x000243AE
		private void GenerateParameters()
		{
			this.slopeLimit = 70f;
			this.stepOffset = Mathf.Min(this.capsuleHeight * 0.1f, 0.2f);
			this.stepHandlingMethod = StepHandlingMethod.None;
			this.ledgeHandling = false;
			this.interactiveRigidbodyHandling = true;
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x000261EC File Offset: 0x000243EC
		private bool canWalk
		{
			get
			{
				return !this.muteWalkMotion && this.alive;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x000261FE File Offset: 0x000243FE
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

		// Token: 0x060008AF RID: 2223 RVA: 0x00026220 File Offset: 0x00024420
		public void ApplyForce(Vector3 force, bool alwaysApply = false, bool disableAirControlUntilCollision = false)
		{
			if (NetworkServer.active && !this.hasEffectiveAuthority)
			{
				this.CallRpcApplyForce(force, alwaysApply, disableAirControlUntilCollision);
				return;
			}
			if (this.mass != 0f)
			{
				Vector3 vector = force * (1f / this.mass);
				if (vector.y < 6f && this.isGrounded && !alwaysApply)
				{
					vector.y = 0f;
				}
				if (vector.y > 0f)
				{
					base.Motor.ForceUnground();
				}
				this.velocity += vector;
				if (disableAirControlUntilCollision)
				{
					this.disableAirControlUntilCollision = true;
				}
			}
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x000262BE File Offset: 0x000244BE
		[ClientRpc]
		private void RpcApplyForce(Vector3 force, bool alwaysApply, bool disableAirControlUntilCollision)
		{
			if (!NetworkServer.active)
			{
				this.ApplyForce(force, alwaysApply, disableAirControlUntilCollision);
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x000262D0 File Offset: 0x000244D0
		// (set) Token: 0x060008B2 RID: 2226 RVA: 0x000262D8 File Offset: 0x000244D8
		public Vector3 moveDirection
		{
			get
			{
				return this._moveDirection;
			}
			set
			{
				this._moveDirection = value;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x000262E1 File Offset: 0x000244E1
		// (set) Token: 0x060008B4 RID: 2228 RVA: 0x000262EE File Offset: 0x000244EE
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

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x000262FC File Offset: 0x000244FC
		// (set) Token: 0x060008B6 RID: 2230 RVA: 0x00026309 File Offset: 0x00024509
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

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x00026317 File Offset: 0x00024517
		public float capsuleHeight
		{
			get
			{
				return this.capsuleCollider.height;
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x00026324 File Offset: 0x00024524
		public float capsuleRadius
		{
			get
			{
				return this.capsuleCollider.radius;
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x00026334 File Offset: 0x00024534
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x00026350 File Offset: 0x00024550
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

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x0002635E File Offset: 0x0002455E
		// (set) Token: 0x060008BC RID: 2236 RVA: 0x0002636B File Offset: 0x0002456B
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

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x00026379 File Offset: 0x00024579
		// (set) Token: 0x060008BE RID: 2238 RVA: 0x00026386 File Offset: 0x00024586
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

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x00026394 File Offset: 0x00024594
		// (set) Token: 0x060008C0 RID: 2240 RVA: 0x0002639C File Offset: 0x0002459C
		public Run.FixedTimeStamp lastGroundedTime { get; private set; } = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x060008C1 RID: 2241 RVA: 0x000263A5 File Offset: 0x000245A5
		public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			currentRotation = Quaternion.identity;
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x000263B2 File Offset: 0x000245B2
		public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			currentVelocity = this.velocity;
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x000263C0 File Offset: 0x000245C0
		public override void BeforeCharacterUpdate(float deltaTime)
		{
			float num = CharacterMotor.cvCMotorSafeCollisionStepThreshold.value * CharacterMotor.cvCMotorSafeCollisionStepThreshold.value;
			if (this.rootMotion != Vector3.zero)
			{
				Vector3 b = this.rootMotion;
				this.rootMotion = Vector3.zero;
				base.Motor.SafeMovement = (b.sqrMagnitude >= num);
				base.Motor.MoveCharacter(base.transform.position + b);
			}
			this.PreMove(deltaTime);
			float sqrMagnitude = (this.velocity * Time.fixedDeltaTime).sqrMagnitude;
			base.Motor.SafeMovement = (sqrMagnitude >= num);
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x00026470 File Offset: 0x00024670
		public override void PostGroundingUpdate(float deltaTime)
		{
			if (base.Motor.GroundingStatus.IsStableOnGround != base.Motor.LastGroundingStatus.IsStableOnGround)
			{
				this.netIsGrounded = base.Motor.GroundingStatus.IsStableOnGround;
				if (base.Motor.GroundingStatus.IsStableOnGround)
				{
					this.OnLanded();
					return;
				}
				this.OnLeaveStableGround();
			}
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x000264D4 File Offset: 0x000246D4
		private void OnLanded()
		{
			this.jumpCount = 0;
			CharacterMotor.HitGroundInfo hitGroundInfo = new CharacterMotor.HitGroundInfo
			{
				velocity = this.lastVelocity,
				position = base.Motor.GroundingStatus.GroundPoint
			};
			if (this.hasEffectiveAuthority)
			{
				if (NetworkServer.active)
				{
					this.OnHitGround(hitGroundInfo);
					return;
				}
				this.CallCmdHitGround(hitGroundInfo);
			}
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00026534 File Offset: 0x00024734
		[Server]
		private void OnHitGround(CharacterMotor.HitGroundInfo hitGroundInfo)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMotor::OnHitGround(RoR2.CharacterMotor/HitGroundInfo)' called on client");
				return;
			}
			GlobalEventManager.instance.OnCharacterHitGround(this.body, hitGroundInfo.velocity);
			CharacterMotor.HitGroundDelegate hitGroundDelegate = this.onHitGround;
			if (hitGroundDelegate == null)
			{
				return;
			}
			hitGroundDelegate(ref hitGroundInfo);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00026573 File Offset: 0x00024773
		[Command]
		private void CmdHitGround(CharacterMotor.HitGroundInfo hitGroundInfo)
		{
			this.OnHitGround(hitGroundInfo);
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0002657C File Offset: 0x0002477C
		private void OnLeaveStableGround()
		{
			if (this.jumpCount < 1)
			{
				this.jumpCount = 1;
			}
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x0002658E File Offset: 0x0002478E
		public override void AfterCharacterUpdate(float deltaTime)
		{
			this.lastVelocity = this.velocity;
			this.velocity = base.Motor.BaseVelocity;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x000265AD File Offset: 0x000247AD
		public override bool IsColliderValidForCollisions(Collider coll)
		{
			return !coll.isTrigger && coll != base.Motor.Capsule;
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060008CC RID: 2252 RVA: 0x000265CC File Offset: 0x000247CC
		// (remove) Token: 0x060008CD RID: 2253 RVA: 0x00026604 File Offset: 0x00024804
		public event CharacterMotor.HitGroundDelegate onHitGround;

		// Token: 0x060008CE RID: 2254 RVA: 0x0002663C File Offset: 0x0002483C
		public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			this.disableAirControlUntilCollision = false;
			if (this.onMovementHit != null)
			{
				CharacterMotor.MovementHitInfo movementHitInfo = new CharacterMotor.MovementHitInfo
				{
					velocity = this.velocity,
					hitCollider = hitCollider
				};
				this.onMovementHit(ref movementHitInfo);
			}
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060008CF RID: 2255 RVA: 0x00026684 File Offset: 0x00024884
		// (remove) Token: 0x060008D0 RID: 2256 RVA: 0x000266BC File Offset: 0x000248BC
		public event CharacterMotor.MovementHitDelegate onMovementHit;

		// Token: 0x060008D1 RID: 2257 RVA: 0x0000409B File Offset: 0x0000229B
		public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x000266F1 File Offset: 0x000248F1
		public void AddDisplacement(Vector3 displacement)
		{
			this.rootMotion += displacement;
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x0002675C File Offset: 0x0002495C
		static CharacterMotor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMotor), CharacterMotor.kCmdCmdHitGround, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeCmdCmdHitGround));
			CharacterMotor.kRpcRpcApplyForce = -1753076289;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterMotor), CharacterMotor.kRpcRpcApplyForce, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeRpcRpcApplyForce));
			NetworkCRC.RegisterBehaviour("CharacterMotor", 0);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x00026803 File Offset: 0x00024A03
		protected static void InvokeCmdCmdHitGround(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdHitGround called on client.");
				return;
			}
			((CharacterMotor)obj).CmdHitGround(GeneratedNetworkCode._ReadHitGroundInfo_CharacterMotor(reader));
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0002682C File Offset: 0x00024A2C
		public void CallCmdHitGround(CharacterMotor.HitGroundInfo hitGroundInfo)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdHitGround called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdHitGround(hitGroundInfo);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterMotor.kCmdCmdHitGround);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteHitGroundInfo_CharacterMotor(networkWriter, hitGroundInfo);
			base.SendCommandInternal(networkWriter, 0, "CmdHitGround");
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x000268B6 File Offset: 0x00024AB6
		protected static void InvokeRpcRpcApplyForce(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcApplyForce called on server.");
				return;
			}
			((CharacterMotor)obj).RpcApplyForce(reader.ReadVector3(), reader.ReadBoolean(), reader.ReadBoolean());
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x000268EC File Offset: 0x00024AEC
		public void CallRpcApplyForce(Vector3 force, bool alwaysApply, bool disableAirControlUntilCollision)
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
			networkWriter.Write(disableAirControlUntilCollision);
			this.SendRPCInternal(networkWriter, 0, "RpcApplyForce");
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x00026974 File Offset: 0x00024B74
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			bool flag2;
			return flag2 || flag;
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0002698D File Offset: 0x00024B8D
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
		}

		// Token: 0x04000911 RID: 2321
		public static readonly List<CharacterMotor> instancesList = new List<CharacterMotor>();

		// Token: 0x04000912 RID: 2322
		[HideInInspector]
		public float walkSpeedPenaltyCoefficient = 1f;

		// Token: 0x04000913 RID: 2323
		[Tooltip("The character direction component to supply a move vector to.")]
		public CharacterDirection characterDirection;

		// Token: 0x04000914 RID: 2324
		[Tooltip("Whether or not a move vector supplied to this component can cause movement. Use this when the object is driven by root motion.")]
		public bool muteWalkMotion;

		// Token: 0x04000915 RID: 2325
		[Tooltip("The mass of this character.")]
		public float mass = 1f;

		// Token: 0x04000916 RID: 2326
		[Tooltip("The air control value of this character as a fraction of ground control.")]
		public float airControl = 0.25f;

		// Token: 0x04000917 RID: 2327
		[Tooltip("Disables Air Control for things like jumppads")]
		public bool disableAirControlUntilCollision;

		// Token: 0x04000918 RID: 2328
		[Tooltip("Auto-assigns parameters skin width, slope angle, and step offset as a function of the Character Motor's radius and height")]
		public bool generateParametersOnAwake = true;

		// Token: 0x04000919 RID: 2329
		[Tooltip("Whether or not this character uses gravity.")]
		public bool useGravity = true;

		// Token: 0x0400091A RID: 2330
		[Tooltip("Whether this character has three-dimensional or two-dimensional movement capabilities.")]
		public bool isFlying;

		// Token: 0x0400091B RID: 2331
		private CharacterBody body;

		// Token: 0x0400091C RID: 2332
		private CapsuleCollider capsuleCollider;

		// Token: 0x0400091D RID: 2333
		private static readonly bool enableMotorWithoutAuthority = false;

		// Token: 0x0400091E RID: 2334
		private bool alive = true;

		// Token: 0x0400091F RID: 2335
		private const float restDuration = 1f;

		// Token: 0x04000920 RID: 2336
		private const float restVelocityThreshold = 0.025f;

		// Token: 0x04000921 RID: 2337
		private const float restVelocityThresholdSqr = 0.00062500004f;

		// Token: 0x04000922 RID: 2338
		public const float slipStartAngle = 70f;

		// Token: 0x04000923 RID: 2339
		public const float slipEndAngle = 55f;

		// Token: 0x04000924 RID: 2340
		private float restStopwatch;

		// Token: 0x04000925 RID: 2341
		private Vector3 previousPosition;

		// Token: 0x04000927 RID: 2343
		[NonSerialized]
		public int jumpCount;

		// Token: 0x04000928 RID: 2344
		[NonSerialized]
		public bool netIsGrounded;

		// Token: 0x04000929 RID: 2345
		[NonSerialized]
		public Vector3 netGroundNormal;

		// Token: 0x0400092A RID: 2346
		[NonSerialized]
		public Vector3 velocity;

		// Token: 0x0400092B RID: 2347
		private Vector3 lastVelocity;

		// Token: 0x0400092C RID: 2348
		[NonSerialized]
		public Vector3 rootMotion;

		// Token: 0x0400092D RID: 2349
		private Vector3 _moveDirection;

		// Token: 0x0400092F RID: 2351
		private static readonly FloatConVar cvCMotorSafeCollisionStepThreshold = new FloatConVar("cmotor_safe_collision_step_threshold", ConVarFlags.Cheat, 1.0833334f.ToString(CultureInfo.InvariantCulture), "How large of a movement in meters/fixedTimeStep is needed to trigger more expensive \"safe\" collisions to prevent tunneling.");

		// Token: 0x04000930 RID: 2352
		private int _safeCollisionEnableCount;

		// Token: 0x04000933 RID: 2355
		private static int kRpcRpcApplyForce;

		// Token: 0x04000934 RID: 2356
		private static int kCmdCmdHitGround = 2030335022;

		// Token: 0x0200019B RID: 411
		[Serializable]
		public struct HitGroundInfo
		{
			// Token: 0x060008DC RID: 2268 RVA: 0x00026997 File Offset: 0x00024B97
			public override string ToString()
			{
				return string.Format("velocity={0} position={1}", this.velocity, this.position);
			}

			// Token: 0x04000935 RID: 2357
			public Vector3 velocity;

			// Token: 0x04000936 RID: 2358
			public Vector3 position;
		}

		// Token: 0x0200019C RID: 412
		// (Invoke) Token: 0x060008DE RID: 2270
		public delegate void HitGroundDelegate(ref CharacterMotor.HitGroundInfo hitGroundInfo);

		// Token: 0x0200019D RID: 413
		public struct MovementHitInfo
		{
			// Token: 0x04000937 RID: 2359
			public Vector3 velocity;

			// Token: 0x04000938 RID: 2360
			public Collider hitCollider;
		}

		// Token: 0x0200019E RID: 414
		// (Invoke) Token: 0x060008E2 RID: 2274
		public delegate void MovementHitDelegate(ref CharacterMotor.MovementHitInfo movementHitInfo);
	}
}
