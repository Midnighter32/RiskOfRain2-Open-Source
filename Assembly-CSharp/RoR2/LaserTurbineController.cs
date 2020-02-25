using System;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200026F RID: 623
	[RequireComponent(typeof(NetworkIdentity))]
	[RequireComponent(typeof(EntityStateMachine))]
	[RequireComponent(typeof(NetworkStateMachine))]
	[RequireComponent(typeof(GenericOwnership))]
	public class LaserTurbineController : NetworkBehaviour
	{
		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x0003E0AF File Offset: 0x0003C2AF
		// (set) Token: 0x06000DD0 RID: 3536 RVA: 0x0003E0B7 File Offset: 0x0003C2B7
		public float charge { get; private set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x0003E0C0 File Offset: 0x0003C2C0
		public CharacterBody ownerBody
		{
			get
			{
				return this.cachedOwnerBody;
			}
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x0003E0C8 File Offset: 0x0003C2C8
		private void Awake()
		{
			this.genericOwnership = base.GetComponent<GenericOwnership>();
			this.genericOwnership.onOwnerChanged += this.OnOwnerChanged;
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0003E0F0 File Offset: 0x0003C2F0
		public override void OnStartServer()
		{
			base.OnStartServer();
			LaserTurbineController.SpinChargeState networkspinChargeState = this.spinChargeState;
			networkspinChargeState.initialSpin = this.minSpin;
			networkspinChargeState.snapshotTime = Run.FixedTimeStamp.now;
			this.NetworkspinChargeState = networkspinChargeState;
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x0003E12A File Offset: 0x0003C32A
		private void Update()
		{
			if (NetworkClient.active)
			{
				this.UpdateClient();
			}
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x0003E13C File Offset: 0x0003C33C
		private void FixedUpdate()
		{
			Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
			this.spin = this.spinChargeState.CalcCurrentSpinValue(now, this.spinDecayRate, this.minSpin);
			this.charge = this.spinChargeState.CalcCurrentChargeValue(now, this.spinDecayRate, this.minSpin);
			if (this.turbineDisplayRoot)
			{
				this.turbineDisplayRoot.gameObject.SetActive(this.showTurbineDisplay);
			}
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x0003E1AE File Offset: 0x0003C3AE
		private void OnEnable()
		{
			if (NetworkServer.active)
			{
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeathGlobalServer;
			}
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x0003E1C8 File Offset: 0x0003C3C8
		private void OnDisable()
		{
			GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeathGlobalServer;
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x0003E1DC File Offset: 0x0003C3DC
		[Client]
		private void UpdateClient()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.LaserTurbineController::UpdateClient()' called on server");
				return;
			}
			float num = HGMath.CircleAreaToRadius(this.charge * HGMath.CircleRadiusToArea(1f));
			this.chargeIndicator.localScale = new Vector3(num, num, num);
			Vector3 localEulerAngles = this.spinIndicator.localEulerAngles;
			localEulerAngles.y += this.spin * Time.deltaTime * this.visualSpinRate;
			this.spinIndicator.localEulerAngles = localEulerAngles;
			AkSoundEngine.SetRTPCValue(this.spinRtpc, this.spin * this.spinRtpcScale, base.gameObject);
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x0003E280 File Offset: 0x0003C480
		[Server]
		public void ExpendCharge()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.LaserTurbineController::ExpendCharge()' called on client");
				return;
			}
			Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
			float num = this.spinChargeState.CalcCurrentSpinValue(now, this.spinDecayRate, this.minSpin);
			num += this.spinPerKill;
			LaserTurbineController.SpinChargeState networkspinChargeState = new LaserTurbineController.SpinChargeState
			{
				initialSpin = num,
				initialCharge = 0f,
				snapshotTime = now
			};
			this.NetworkspinChargeState = networkspinChargeState;
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x0003E2F7 File Offset: 0x0003C4F7
		private void OnCharacterDeathGlobalServer(DamageReport damageReport)
		{
			if (damageReport.attacker == this.genericOwnership.ownerObject && damageReport.attacker != null)
			{
				this.OnOwnerKilledOtherServer();
			}
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x0003E31C File Offset: 0x0003C51C
		private void OnOwnerKilledOtherServer()
		{
			Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
			float num = this.spinChargeState.CalcCurrentSpinValue(now, this.spinDecayRate, this.minSpin);
			float initialCharge = this.spinChargeState.CalcCurrentChargeValue(now, this.spinDecayRate, this.minSpin);
			num = Mathf.Min(num + this.spinPerKill, this.maxSpin);
			LaserTurbineController.SpinChargeState networkspinChargeState = new LaserTurbineController.SpinChargeState
			{
				initialSpin = num,
				initialCharge = initialCharge,
				snapshotTime = now
			};
			this.NetworkspinChargeState = networkspinChargeState;
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x0003E39F File Offset: 0x0003C59F
		private void OnOwnerChanged(GameObject newOwner)
		{
			this.cachedOwnerBody = (newOwner ? newOwner.GetComponent<CharacterBody>() : null);
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000DDF RID: 3551 RVA: 0x0003E3D8 File Offset: 0x0003C5D8
		// (set) Token: 0x06000DE0 RID: 3552 RVA: 0x0003E3EB File Offset: 0x0003C5EB
		public LaserTurbineController.SpinChargeState NetworkspinChargeState
		{
			get
			{
				return this.spinChargeState;
			}
			[param: In]
			set
			{
				base.SetSyncVar<LaserTurbineController.SpinChargeState>(value, ref this.spinChargeState, 1U);
			}
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x0003E400 File Offset: 0x0003C600
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteSpinChargeState_LaserTurbineController(writer, this.spinChargeState);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteSpinChargeState_LaserTurbineController(writer, this.spinChargeState);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x0003E46C File Offset: 0x0003C66C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.spinChargeState = GeneratedNetworkCode._ReadSpinChargeState_LaserTurbineController(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.spinChargeState = GeneratedNetworkCode._ReadSpinChargeState_LaserTurbineController(reader);
			}
		}

		// Token: 0x04000DD5 RID: 3541
		public float spinPerKill;

		// Token: 0x04000DD6 RID: 3542
		public float spinDecayRate;

		// Token: 0x04000DD7 RID: 3543
		public float minSpin;

		// Token: 0x04000DD8 RID: 3544
		public float maxSpin;

		// Token: 0x04000DD9 RID: 3545
		public float visualSpinRate = 7200f;

		// Token: 0x04000DDA RID: 3546
		public Transform chargeIndicator;

		// Token: 0x04000DDB RID: 3547
		public Transform spinIndicator;

		// Token: 0x04000DDC RID: 3548
		public Transform turbineDisplayRoot;

		// Token: 0x04000DDD RID: 3549
		public bool showTurbineDisplay;

		// Token: 0x04000DDE RID: 3550
		public string spinRtpc;

		// Token: 0x04000DDF RID: 3551
		public float spinRtpcScale;

		// Token: 0x04000DE0 RID: 3552
		private GenericOwnership genericOwnership;

		// Token: 0x04000DE1 RID: 3553
		[SyncVar]
		private LaserTurbineController.SpinChargeState spinChargeState = LaserTurbineController.SpinChargeState.zero;

		// Token: 0x04000DE2 RID: 3554
		private float spin;

		// Token: 0x04000DE4 RID: 3556
		private CharacterBody cachedOwnerBody;

		// Token: 0x02000270 RID: 624
		[Serializable]
		private struct SpinChargeState : IEquatable<LaserTurbineController.SpinChargeState>
		{
			// Token: 0x06000DE3 RID: 3555 RVA: 0x0003E4AD File Offset: 0x0003C6AD
			public float CalcCurrentSpinValue(Run.FixedTimeStamp currentTime, float spinDecayRate, float minSpin)
			{
				return Mathf.Max(this.initialSpin - spinDecayRate * (currentTime - this.snapshotTime), minSpin);
			}

			// Token: 0x06000DE4 RID: 3556 RVA: 0x0003E4CC File Offset: 0x0003C6CC
			public float CalcCurrentChargeValue(Run.FixedTimeStamp currentTime, float spinDecayRate, float minSpin)
			{
				float num = currentTime - this.snapshotTime;
				float num2 = minSpin * num;
				float num3 = this.initialSpin - minSpin;
				float t = Mathf.Min(Trajectory.CalculateFlightDuration(num3, -spinDecayRate) * 0.5f, num);
				float num4 = Trajectory.CalculatePositionYAtTime(0f, num3, t, -spinDecayRate);
				return Mathf.Min(this.initialCharge + num2 + num4, 1f);
			}

			// Token: 0x06000DE5 RID: 3557 RVA: 0x0003E533 File Offset: 0x0003C733
			public bool Equals(LaserTurbineController.SpinChargeState other)
			{
				return this.initialCharge.Equals(other.initialCharge) && this.initialSpin.Equals(other.initialSpin) && this.snapshotTime.Equals(other.snapshotTime);
			}

			// Token: 0x06000DE6 RID: 3558 RVA: 0x0003E570 File Offset: 0x0003C770
			public override bool Equals(object obj)
			{
				if (obj is LaserTurbineController.SpinChargeState)
				{
					LaserTurbineController.SpinChargeState other = (LaserTurbineController.SpinChargeState)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06000DE7 RID: 3559 RVA: 0x0003E597 File Offset: 0x0003C797
			public override int GetHashCode()
			{
				return (this.initialCharge.GetHashCode() * 397 ^ this.initialSpin.GetHashCode()) * 397 ^ this.snapshotTime.GetHashCode();
			}

			// Token: 0x04000DE5 RID: 3557
			public float initialCharge;

			// Token: 0x04000DE6 RID: 3558
			public float initialSpin;

			// Token: 0x04000DE7 RID: 3559
			public Run.FixedTimeStamp snapshotTime;

			// Token: 0x04000DE8 RID: 3560
			public static readonly LaserTurbineController.SpinChargeState zero = new LaserTurbineController.SpinChargeState
			{
				initialCharge = 0f,
				initialSpin = 0f,
				snapshotTime = Run.FixedTimeStamp.negativeInfinity
			};
		}
	}
}
