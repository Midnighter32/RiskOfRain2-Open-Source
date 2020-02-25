using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000539 RID: 1337
	public class CharacterNetworkTransform : NetworkBehaviour
	{
		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06001F7F RID: 8063 RVA: 0x00088C6C File Offset: 0x00086E6C
		public static ReadOnlyCollection<CharacterNetworkTransform> readOnlyInstancesList
		{
			get
			{
				return CharacterNetworkTransform._readOnlyInstancesList;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06001F81 RID: 8065 RVA: 0x00088C7C File Offset: 0x00086E7C
		// (set) Token: 0x06001F80 RID: 8064 RVA: 0x00088C73 File Offset: 0x00086E73
		public new Transform transform { get; private set; }

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06001F83 RID: 8067 RVA: 0x00088C8D File Offset: 0x00086E8D
		// (set) Token: 0x06001F82 RID: 8066 RVA: 0x00088C84 File Offset: 0x00086E84
		public InputBankTest inputBank { get; private set; }

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06001F85 RID: 8069 RVA: 0x00088C9E File Offset: 0x00086E9E
		// (set) Token: 0x06001F84 RID: 8068 RVA: 0x00088C95 File Offset: 0x00086E95
		public CharacterMotor characterMotor { get; set; }

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06001F87 RID: 8071 RVA: 0x00088CAF File Offset: 0x00086EAF
		// (set) Token: 0x06001F86 RID: 8070 RVA: 0x00088CA6 File Offset: 0x00086EA6
		public CharacterDirection characterDirection { get; private set; }

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06001F89 RID: 8073 RVA: 0x00088CC0 File Offset: 0x00086EC0
		// (set) Token: 0x06001F88 RID: 8072 RVA: 0x00088CB7 File Offset: 0x00086EB7
		private Rigidbody rigidbody { get; set; }

		// Token: 0x06001F8A RID: 8074 RVA: 0x00088CC8 File Offset: 0x00086EC8
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

		// Token: 0x06001F8B RID: 8075 RVA: 0x00088D78 File Offset: 0x00086F78
		private CharacterNetworkTransform.Snapshot BuildSnapshot()
		{
			return new CharacterNetworkTransform.Snapshot
			{
				serverTime = GameNetworkManager.singleton.serverFixedTime,
				position = this.transform.position,
				moveVector = (this.inputBank ? this.inputBank.moveVector : Vector3.zero),
				aimDirection = (this.inputBank ? this.inputBank.aimDirection : Vector3.zero),
				rotation = (this.characterDirection ? Quaternion.Euler(0f, this.characterDirection.yaw, 0f) : this.transform.rotation),
				isGrounded = (this.characterMotor && this.characterMotor.isGrounded),
				groundNormal = (this.characterMotor ? this.characterMotor.estimatedGroundNormal : Vector3.zero)
			};
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x00088E80 File Offset: 0x00087080
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

		// Token: 0x06001F8D RID: 8077 RVA: 0x00089000 File Offset: 0x00087200
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

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06001F8E RID: 8078 RVA: 0x00089067 File Offset: 0x00087267
		public float interpolationDelay
		{
			get
			{
				return this.positionTransmitInterval * this.interpolationFactor;
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06001F90 RID: 8080 RVA: 0x0008907F File Offset: 0x0008727F
		// (set) Token: 0x06001F8F RID: 8079 RVA: 0x00089076 File Offset: 0x00087276
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06001F91 RID: 8081 RVA: 0x00089087 File Offset: 0x00087287
		private void Start()
		{
			this.newestNetSnapshot = this.BuildSnapshot();
			this.UpdateAuthority();
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x0008909B File Offset: 0x0008729B
		private void OnEnable()
		{
			bool flag = CharacterNetworkTransform.instancesList.Contains(this);
			CharacterNetworkTransform.instancesList.Add(this);
			if (flag)
			{
				Debug.LogError("Instance already in list!");
			}
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000890BF File Offset: 0x000872BF
		private void OnDisable()
		{
			CharacterNetworkTransform.instancesList.Remove(this);
			if (CharacterNetworkTransform.instancesList.Contains(this))
			{
				Debug.LogError("Instance was not fully removed from list!");
			}
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x000890E4 File Offset: 0x000872E4
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
			if (this.rigidbody)
			{
				this.rigidbody.isKinematic = (!this.hasEffectiveAuthority || this.rigidbodyStartedKinematic);
			}
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x00089120 File Offset: 0x00087320
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x00089120 File Offset: 0x00087320
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x00089128 File Offset: 0x00087328
		private void ApplyCurrentSnapshot(float currentTime)
		{
			CharacterNetworkTransform.Snapshot snapshot = this.CalcCurrentSnapshot(currentTime, this.interpolationDelay);
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
				this.characterMotor.netGroundNormal = snapshot.groundNormal;
				if (this.characterMotor.Motor.enabled)
				{
					this.characterMotor.Motor.MoveCharacter(snapshot.position);
				}
				else
				{
					this.characterMotor.Motor.SetPosition(snapshot.position, true);
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

		// Token: 0x06001F98 RID: 8088 RVA: 0x00089270 File Offset: 0x00087470
		private void FixedUpdate()
		{
			if (!this.hasEffectiveAuthority)
			{
				this.ApplyCurrentSnapshot(GameNetworkManager.singleton.serverFixedTime);
				return;
			}
			this.newestNetSnapshot = this.BuildSnapshot();
		}

		// Token: 0x06001F9B RID: 8091 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001F9C RID: 8092 RVA: 0x000892F0 File Offset: 0x000874F0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F9D RID: 8093 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001D36 RID: 7478
		private static List<CharacterNetworkTransform> instancesList = new List<CharacterNetworkTransform>();

		// Token: 0x04001D37 RID: 7479
		private static ReadOnlyCollection<CharacterNetworkTransform> _readOnlyInstancesList = new ReadOnlyCollection<CharacterNetworkTransform>(CharacterNetworkTransform.instancesList);

		// Token: 0x04001D3D RID: 7485
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.1f;

		// Token: 0x04001D3E RID: 7486
		[HideInInspector]
		public float lastPositionTransmitTime = float.NegativeInfinity;

		// Token: 0x04001D3F RID: 7487
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 2f;

		// Token: 0x04001D40 RID: 7488
		public CharacterNetworkTransform.Snapshot newestNetSnapshot;

		// Token: 0x04001D41 RID: 7489
		private List<CharacterNetworkTransform.Snapshot> snapshots = new List<CharacterNetworkTransform.Snapshot>();

		// Token: 0x04001D42 RID: 7490
		public bool debugDuplicatePositions;

		// Token: 0x04001D43 RID: 7491
		public bool debugSnapshotReceived;

		// Token: 0x04001D44 RID: 7492
		private bool rigidbodyStartedKinematic = true;

		// Token: 0x0200053A RID: 1338
		public struct Snapshot
		{
			// Token: 0x06001F9E RID: 8094 RVA: 0x00089300 File Offset: 0x00087500
			private static bool LerpGroundNormal(ref CharacterNetworkTransform.Snapshot a, ref CharacterNetworkTransform.Snapshot b, float t, out Vector3 groundNormal)
			{
				groundNormal = Vector3.zero;
				bool flag = (t > 0f) ? b.isGrounded : a.isGrounded;
				if (flag)
				{
					if (b.isGrounded)
					{
						if (a.isGrounded)
						{
							groundNormal = Vector3.Slerp(a.groundNormal, b.groundNormal, t);
							return flag;
						}
						groundNormal = b.groundNormal;
						return flag;
					}
					else
					{
						groundNormal = a.groundNormal;
					}
				}
				return flag;
			}

			// Token: 0x06001F9F RID: 8095 RVA: 0x00089374 File Offset: 0x00087574
			public static CharacterNetworkTransform.Snapshot Lerp(CharacterNetworkTransform.Snapshot a, CharacterNetworkTransform.Snapshot b, float t)
			{
				Vector3 vector;
				bool flag = CharacterNetworkTransform.Snapshot.LerpGroundNormal(ref a, ref b, t, out vector);
				return new CharacterNetworkTransform.Snapshot
				{
					position = Vector3.Lerp(a.position, b.position, t),
					moveVector = Vector3.Lerp(a.moveVector, b.moveVector, t),
					aimDirection = Vector3.Slerp(a.aimDirection, b.moveVector, t),
					rotation = Quaternion.Lerp(a.rotation, b.rotation, t),
					isGrounded = flag,
					groundNormal = vector
				};
			}

			// Token: 0x06001FA0 RID: 8096 RVA: 0x0008940C File Offset: 0x0008760C
			public static CharacterNetworkTransform.Snapshot Interpolate(CharacterNetworkTransform.Snapshot a, CharacterNetworkTransform.Snapshot b, float serverTime)
			{
				float t = (serverTime - a.serverTime) / (b.serverTime - a.serverTime);
				Vector3 vector;
				bool flag = CharacterNetworkTransform.Snapshot.LerpGroundNormal(ref a, ref b, t, out vector);
				return new CharacterNetworkTransform.Snapshot
				{
					serverTime = serverTime,
					position = Vector3.LerpUnclamped(a.position, b.position, t),
					moveVector = Vector3.Lerp(a.moveVector, b.moveVector, t),
					aimDirection = Vector3.Slerp(a.aimDirection, b.aimDirection, t),
					rotation = Quaternion.Lerp(a.rotation, b.rotation, t),
					isGrounded = flag,
					groundNormal = vector
				};
			}

			// Token: 0x04001D46 RID: 7494
			public float serverTime;

			// Token: 0x04001D47 RID: 7495
			public Vector3 position;

			// Token: 0x04001D48 RID: 7496
			public Vector3 moveVector;

			// Token: 0x04001D49 RID: 7497
			public Vector3 aimDirection;

			// Token: 0x04001D4A RID: 7498
			public Quaternion rotation;

			// Token: 0x04001D4B RID: 7499
			public bool isGrounded;

			// Token: 0x04001D4C RID: 7500
			public Vector3 groundNormal;
		}
	}
}
