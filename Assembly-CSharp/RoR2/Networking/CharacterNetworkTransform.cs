using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000570 RID: 1392
	public class CharacterNetworkTransform : NetworkBehaviour
	{
		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06001EF3 RID: 7923 RVA: 0x00092168 File Offset: 0x00090368
		public static ReadOnlyCollection<CharacterNetworkTransform> readOnlyInstancesList
		{
			get
			{
				return CharacterNetworkTransform._readOnlyInstancesList;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06001EF5 RID: 7925 RVA: 0x00092178 File Offset: 0x00090378
		// (set) Token: 0x06001EF4 RID: 7924 RVA: 0x0009216F File Offset: 0x0009036F
		public new Transform transform { get; private set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06001EF7 RID: 7927 RVA: 0x00092189 File Offset: 0x00090389
		// (set) Token: 0x06001EF6 RID: 7926 RVA: 0x00092180 File Offset: 0x00090380
		public InputBankTest inputBank { get; private set; }

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06001EF9 RID: 7929 RVA: 0x0009219A File Offset: 0x0009039A
		// (set) Token: 0x06001EF8 RID: 7928 RVA: 0x00092191 File Offset: 0x00090391
		public CharacterMotor characterMotor { get; set; }

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001EFB RID: 7931 RVA: 0x000921AB File Offset: 0x000903AB
		// (set) Token: 0x06001EFA RID: 7930 RVA: 0x000921A2 File Offset: 0x000903A2
		public CharacterDirection characterDirection { get; private set; }

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001EFD RID: 7933 RVA: 0x000921BC File Offset: 0x000903BC
		// (set) Token: 0x06001EFC RID: 7932 RVA: 0x000921B3 File Offset: 0x000903B3
		private Rigidbody rigidbody { get; set; }

		// Token: 0x06001EFE RID: 7934 RVA: 0x000921C4 File Offset: 0x000903C4
		private CharacterNetworkTransform.Snapshot CalcCurrentSnapshot(float time, float interpolationDelay)
		{
			float num = time - interpolationDelay;
			if (this.snapshots.Count < 2)
			{
				CharacterNetworkTransform.Snapshot result = (this.snapshots.Count == 0) ? this.BuildSnapshot() : this.snapshots[0];
				result.serverTime = num;
				return result;
			}
			int num2 = 0;
			while (num2 < this.snapshots.Count - 2 && (this.snapshots[num2].serverTime > num || this.snapshots[num2 + 1].serverTime < num))
			{
				num2++;
			}
			return CharacterNetworkTransform.Snapshot.Interpolate(this.snapshots[num2], this.snapshots[num2 + 1], num);
		}

		// Token: 0x06001EFF RID: 7935 RVA: 0x00092274 File Offset: 0x00090474
		private CharacterNetworkTransform.Snapshot BuildSnapshot()
		{
			return new CharacterNetworkTransform.Snapshot
			{
				serverTime = ((GameNetworkManager)NetworkManager.singleton).serverFixedTime,
				position = this.transform.position,
				moveVector = (this.inputBank ? this.inputBank.moveVector : Vector3.zero),
				aimDirection = (this.inputBank ? this.inputBank.aimDirection : Vector3.zero),
				rotation = (this.characterDirection ? Quaternion.Euler(0f, this.characterDirection.yaw, 0f) : this.transform.rotation),
				isGrounded = (this.characterMotor && this.characterMotor.isGrounded)
			};
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x0009235C File Offset: 0x0009055C
		public void PushSnapshot(CharacterNetworkTransform.Snapshot newSnapshot)
		{
			if (this.debugSnapshotReceived)
			{
				Debug.LogFormat("{0} CharacterNetworkTransform snapshot received.", new object[]
				{
					base.gameObject
				});
			}
			if (this.snapshots.Count > 0 && newSnapshot.serverTime == this.snapshots[this.snapshots.Count - 1].serverTime)
			{
				Debug.Log("Received duplicate time!");
			}
			if (this.debugDuplicatePositions && this.snapshots.Count > 0 && newSnapshot.position == this.snapshots[this.snapshots.Count - 1].position)
			{
				Debug.Log("Received duplicate position!");
			}
			if (((this.snapshots.Count > 0) ? this.snapshots[this.snapshots.Count - 1].serverTime : float.NegativeInfinity) < newSnapshot.serverTime)
			{
				this.snapshots.Add(newSnapshot);
				this.newestNetSnapshot = newSnapshot;
				Debug.DrawLine(newSnapshot.position + Vector3.up, newSnapshot.position + Vector3.down, Color.white, 0.25f);
			}
			float num = ((GameNetworkManager)NetworkManager.singleton).serverFixedTime - this.interpolationDelay * 3f;
			while (this.snapshots.Count > 2 && this.snapshots[1].serverTime < num)
			{
				this.snapshots.RemoveAt(0);
			}
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x000924DC File Offset: 0x000906DC
		private void Awake()
		{
			this.transform = base.transform;
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterMotor = base.GetComponent<CharacterMotor>();
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			if (this.rigidbody)
			{
				this.rigidbodyStartedKinematic = this.rigidbody.isKinematic;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06001F02 RID: 7938 RVA: 0x00092543 File Offset: 0x00090743
		public float interpolationDelay
		{
			get
			{
				return this.positionTransmitInterval * this.interpolationFactor;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06001F04 RID: 7940 RVA: 0x0009255B File Offset: 0x0009075B
		// (set) Token: 0x06001F03 RID: 7939 RVA: 0x00092552 File Offset: 0x00090752
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06001F05 RID: 7941 RVA: 0x00092563 File Offset: 0x00090763
		private void Start()
		{
			this.newestNetSnapshot = this.BuildSnapshot();
			this.UpdateAuthority();
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x00092577 File Offset: 0x00090777
		private void OnEnable()
		{
			bool flag = CharacterNetworkTransform.instancesList.Contains(this);
			CharacterNetworkTransform.instancesList.Add(this);
			if (flag)
			{
				Debug.LogError("Instance already in list!");
			}
		}

		// Token: 0x06001F07 RID: 7943 RVA: 0x0009259B File Offset: 0x0009079B
		private void OnDisable()
		{
			CharacterNetworkTransform.instancesList.Remove(this);
			if (CharacterNetworkTransform.instancesList.Contains(this))
			{
				Debug.LogError("Instance was not fully removed from list!");
			}
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x000925C0 File Offset: 0x000907C0
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
			if (this.rigidbody)
			{
				this.rigidbody.isKinematic = (!this.hasEffectiveAuthority || this.rigidbodyStartedKinematic);
			}
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x000925FC File Offset: 0x000907FC
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x000925FC File Offset: 0x000907FC
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F0B RID: 7947 RVA: 0x00092604 File Offset: 0x00090804
		private void FixedUpdate()
		{
			if (this.hasEffectiveAuthority)
			{
				this.newestNetSnapshot = this.BuildSnapshot();
				return;
			}
			CharacterNetworkTransform.Snapshot snapshot = this.CalcCurrentSnapshot(GameNetworkManager.singleton.serverFixedTime, this.interpolationDelay);
			if (!this.characterMotor)
			{
				if (this.rigidbodyStartedKinematic)
				{
					this.transform.position = snapshot.position;
				}
				else
				{
					this.rigidbody.MovePosition(snapshot.position);
				}
			}
			if (this.inputBank)
			{
				this.inputBank.moveVector = snapshot.moveVector;
				this.inputBank.aimDirection = snapshot.aimDirection;
			}
			if (this.characterMotor)
			{
				this.characterMotor.netIsGrounded = snapshot.isGrounded;
				KinematicCharacterMotor motor = this.characterMotor.Motor;
				if (motor != null)
				{
					motor.MoveCharacter(snapshot.position);
				}
			}
			if (this.characterDirection)
			{
				this.characterDirection.yaw = snapshot.rotation.eulerAngles.y;
				return;
			}
			if (this.rigidbodyStartedKinematic)
			{
				this.transform.rotation = snapshot.rotation;
				return;
			}
			this.rigidbody.MoveRotation(snapshot.rotation);
		}

		// Token: 0x06001F0E RID: 7950 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001F0F RID: 7951 RVA: 0x00092790 File Offset: 0x00090990
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040021AD RID: 8621
		private static List<CharacterNetworkTransform> instancesList = new List<CharacterNetworkTransform>();

		// Token: 0x040021AE RID: 8622
		private static ReadOnlyCollection<CharacterNetworkTransform> _readOnlyInstancesList = new ReadOnlyCollection<CharacterNetworkTransform>(CharacterNetworkTransform.instancesList);

		// Token: 0x040021B4 RID: 8628
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.1f;

		// Token: 0x040021B5 RID: 8629
		[HideInInspector]
		public float lastPositionTransmitTime = float.NegativeInfinity;

		// Token: 0x040021B6 RID: 8630
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 2f;

		// Token: 0x040021B7 RID: 8631
		public CharacterNetworkTransform.Snapshot newestNetSnapshot;

		// Token: 0x040021B8 RID: 8632
		private List<CharacterNetworkTransform.Snapshot> snapshots = new List<CharacterNetworkTransform.Snapshot>();

		// Token: 0x040021B9 RID: 8633
		public bool debugDuplicatePositions;

		// Token: 0x040021BA RID: 8634
		public bool debugSnapshotReceived;

		// Token: 0x040021BB RID: 8635
		private bool rigidbodyStartedKinematic = true;

		// Token: 0x02000571 RID: 1393
		public struct Snapshot
		{
			// Token: 0x06001F11 RID: 7953 RVA: 0x000927A0 File Offset: 0x000909A0
			public static CharacterNetworkTransform.Snapshot Lerp(CharacterNetworkTransform.Snapshot a, CharacterNetworkTransform.Snapshot b, float t)
			{
				return new CharacterNetworkTransform.Snapshot
				{
					position = Vector3.Lerp(a.position, b.position, t),
					moveVector = Vector3.Lerp(a.moveVector, b.moveVector, t),
					aimDirection = Vector3.Slerp(a.aimDirection, b.moveVector, t),
					rotation = Quaternion.Lerp(a.rotation, b.rotation, t),
					isGrounded = ((t > 0.5f) ? b.isGrounded : a.isGrounded)
				};
			}

			// Token: 0x06001F12 RID: 7954 RVA: 0x00092838 File Offset: 0x00090A38
			public static CharacterNetworkTransform.Snapshot Interpolate(CharacterNetworkTransform.Snapshot a, CharacterNetworkTransform.Snapshot b, float serverTime)
			{
				float num = (serverTime - a.serverTime) / (b.serverTime - a.serverTime);
				return new CharacterNetworkTransform.Snapshot
				{
					serverTime = serverTime,
					position = Vector3.LerpUnclamped(a.position, b.position, num),
					moveVector = Vector3.Lerp(a.moveVector, b.moveVector, num),
					aimDirection = Vector3.Slerp(a.aimDirection, b.aimDirection, num),
					rotation = Quaternion.Lerp(a.rotation, b.rotation, num),
					isGrounded = ((num > 0.5f) ? b.isGrounded : a.isGrounded)
				};
			}

			// Token: 0x040021BD RID: 8637
			public float serverTime;

			// Token: 0x040021BE RID: 8638
			public Vector3 position;

			// Token: 0x040021BF RID: 8639
			public Vector3 moveVector;

			// Token: 0x040021C0 RID: 8640
			public Vector3 aimDirection;

			// Token: 0x040021C1 RID: 8641
			public Quaternion rotation;

			// Token: 0x040021C2 RID: 8642
			public bool isGrounded;

			// Token: 0x040021C3 RID: 8643
			public const int maxNetworkSize = 57;
		}
	}
}
