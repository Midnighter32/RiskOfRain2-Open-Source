using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000389 RID: 905
	[RequireComponent(typeof(CharacterMaster))]
	[RequireComponent(typeof(PingerController))]
	public class PlayerCharacterMasterController : NetworkBehaviour
	{
		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060012E5 RID: 4837 RVA: 0x0005CA25 File Offset: 0x0005AC25
		public static ReadOnlyCollection<PlayerCharacterMasterController> instances
		{
			get
			{
				return PlayerCharacterMasterController._instancesReadOnly;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060012E6 RID: 4838 RVA: 0x0005CA2C File Offset: 0x0005AC2C
		// (set) Token: 0x060012E7 RID: 4839 RVA: 0x0005CA34 File Offset: 0x0005AC34
		public CharacterMaster master { get; private set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060012E8 RID: 4840 RVA: 0x0005CA3D File Offset: 0x0005AC3D
		public bool hasEffectiveAuthority
		{
			get
			{
				return this.master.hasEffectiveAuthority;
			}
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0005CA4A File Offset: 0x0005AC4A
		private void OnSyncNetworkUserInstanceId(NetworkInstanceId value)
		{
			this.resolvedNetworkUserGameObjectInstance = null;
			this.resolvedNetworkUserInstance = null;
			this.networkUserResolved = (value == NetworkInstanceId.Invalid);
			this.NetworknetworkUserInstanceId = value;
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060012EA RID: 4842 RVA: 0x0005CA74 File Offset: 0x0005AC74
		// (set) Token: 0x060012EB RID: 4843 RVA: 0x0005CACC File Offset: 0x0005ACCC
		public GameObject networkUserObject
		{
			get
			{
				if (!this.networkUserResolved)
				{
					this.resolvedNetworkUserGameObjectInstance = Util.FindNetworkObject(this.networkUserInstanceId);
					this.resolvedNetworkUserInstance = null;
					if (this.resolvedNetworkUserGameObjectInstance)
					{
						this.resolvedNetworkUserInstance = this.resolvedNetworkUserGameObjectInstance.GetComponent<NetworkUser>();
						this.networkUserResolved = true;
					}
				}
				return this.resolvedNetworkUserGameObjectInstance;
			}
			set
			{
				NetworkInstanceId networknetworkUserInstanceId = NetworkInstanceId.Invalid;
				this.resolvedNetworkUserGameObjectInstance = null;
				this.resolvedNetworkUserInstance = null;
				this.networkUserResolved = true;
				if (value)
				{
					NetworkIdentity component = value.GetComponent<NetworkIdentity>();
					if (component)
					{
						networknetworkUserInstanceId = component.netId;
						this.resolvedNetworkUserGameObjectInstance = value;
						this.resolvedNetworkUserInstance = value.GetComponent<NetworkUser>();
					}
				}
				this.NetworknetworkUserInstanceId = networknetworkUserInstanceId;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060012EC RID: 4844 RVA: 0x0005CB2C File Offset: 0x0005AD2C
		public NetworkUser networkUser
		{
			get
			{
				if (!this.networkUserObject)
				{
					return null;
				}
				return this.resolvedNetworkUserInstance;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x0005CB43 File Offset: 0x0005AD43
		public bool isConnected
		{
			get
			{
				return this.networkUserObject;
			}
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0005CB50 File Offset: 0x0005AD50
		private void Awake()
		{
			this.master = base.GetComponent<CharacterMaster>();
			this.netid = base.GetComponent<NetworkIdentity>();
			this.pingerController = base.GetComponent<PingerController>();
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0005CB76 File Offset: 0x0005AD76
		private void OnEnable()
		{
			PlayerCharacterMasterController._instances.Add(this);
			if (PlayerCharacterMasterController.onPlayerAdded != null)
			{
				PlayerCharacterMasterController.onPlayerAdded(this);
			}
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0005CB95 File Offset: 0x0005AD95
		private void OnDisable()
		{
			PlayerCharacterMasterController._instances.Remove(this);
			if (PlayerCharacterMasterController.onPlayerRemoved != null)
			{
				PlayerCharacterMasterController.onPlayerRemoved(this);
			}
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0005CBB5 File Offset: 0x0005ADB5
		private void Start()
		{
			if (NetworkServer.active && this.networkUser)
			{
				this.CallRpcIncrementRunCount();
			}
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0005CBD4 File Offset: 0x0005ADD4
		[ClientRpc]
		private void RpcIncrementRunCount()
		{
			if (this.networkUser)
			{
				LocalUser localUser = this.networkUser.localUser;
				if (localUser != null)
				{
					localUser.userProfile.totalRunCount += 1u;
				}
			}
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0005CC10 File Offset: 0x0005AE10
		private void Update()
		{
			if (this.netid.hasAuthority)
			{
				this.SetBody(this.master.GetBodyObject());
				NetworkUser networkUser = this.networkUser;
				if (this.bodyInputs && networkUser && networkUser.inputPlayer != null)
				{
					this.sprintInputPressReceived |= networkUser.inputPlayer.GetButtonDown("Sprint");
					CameraRigController cameraRigController = networkUser.cameraRigController;
					if (cameraRigController)
					{
						if (networkUser.localUser != null && !networkUser.localUser.isUIFocused)
						{
							Vector2 vector = new Vector2(networkUser.inputPlayer.GetAxis("MoveHorizontal"), networkUser.inputPlayer.GetAxis("MoveVertical"));
							float sqrMagnitude = vector.sqrMagnitude;
							if (sqrMagnitude > 1f)
							{
								vector /= Mathf.Sqrt(sqrMagnitude);
							}
							if (this.bodyIsFlier)
							{
								this.bodyInputs.moveVector = cameraRigController.transform.right * vector.x + cameraRigController.transform.forward * vector.y;
							}
							else
							{
								float y = cameraRigController.transform.eulerAngles.y;
								this.bodyInputs.moveVector = Quaternion.Euler(0f, y, 0f) * new Vector3(vector.x, 0f, vector.y);
							}
						}
						else
						{
							this.bodyInputs.moveVector = Vector3.zero;
						}
						this.bodyInputs.aimDirection = (cameraRigController.crosshairWorldPosition - this.bodyInputs.aimOrigin).normalized;
					}
					CharacterEmoteDefinitions component = this.bodyInputs.GetComponent<CharacterEmoteDefinitions>();
					if (component)
					{
						if (Input.GetKeyDown("g"))
						{
							this.bodyInputs.emoteRequest = component.FindEmoteIndex("Point");
							return;
						}
						if (Input.GetKeyDown("t"))
						{
							this.bodyInputs.emoteRequest = component.FindEmoteIndex("Surprise");
						}
					}
				}
			}
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0005CE20 File Offset: 0x0005B020
		private void FixedUpdate()
		{
			NetworkUser networkUser = this.networkUser;
			if (this.bodyInputs)
			{
				if (networkUser && networkUser.localUser != null && !networkUser.localUser.isUIFocused)
				{
					Player inputPlayer = networkUser.localUser.inputPlayer;
					bool flag = false;
					if (this.body)
					{
						flag = this.body.isSprinting;
						if (this.sprintInputPressReceived)
						{
							this.sprintInputPressReceived = false;
							flag = !flag;
						}
						if (flag)
						{
							Vector3 aimDirection = this.bodyInputs.aimDirection;
							aimDirection.y = 0f;
							aimDirection.Normalize();
							Vector3 moveVector = this.bodyInputs.moveVector;
							moveVector.y = 0f;
							moveVector.Normalize();
							if (Vector3.Dot(aimDirection, moveVector) < PlayerCharacterMasterController.sprintMinAimMoveDot)
							{
								flag = false;
							}
						}
					}
					this.bodyInputs.skill1.PushState(inputPlayer.GetButton("PrimarySkill"));
					this.bodyInputs.skill2.PushState(inputPlayer.GetButton("SecondarySkill"));
					this.bodyInputs.skill3.PushState(inputPlayer.GetButton("UtilitySkill"));
					this.bodyInputs.skill4.PushState(inputPlayer.GetButton("SpecialSkill"));
					this.bodyInputs.interact.PushState(inputPlayer.GetButton("Interact"));
					this.bodyInputs.jump.PushState(inputPlayer.GetButton("Jump"));
					this.bodyInputs.sprint.PushState(flag);
					this.bodyInputs.activateEquipment.PushState(inputPlayer.GetButton("Equipment"));
					this.bodyInputs.ping.PushState(inputPlayer.GetButton("Ping"));
				}
				else
				{
					this.bodyInputs.skill1.PushState(false);
					this.bodyInputs.skill2.PushState(false);
					this.bodyInputs.skill3.PushState(false);
					this.bodyInputs.skill4.PushState(false);
					this.bodyInputs.interact.PushState(false);
					this.bodyInputs.jump.PushState(false);
					this.bodyInputs.sprint.PushState(false);
					this.bodyInputs.activateEquipment.PushState(false);
					this.bodyInputs.ping.PushState(false);
				}
				this.CheckPinging();
			}
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0005D088 File Offset: 0x0005B288
		private void CheckPinging()
		{
			if (this.hasEffectiveAuthority && this.body && this.bodyInputs && this.bodyInputs.ping.justPressed)
			{
				this.pingerController.AttemptPing(new Ray(this.bodyInputs.aimOrigin, this.bodyInputs.aimDirection), this.body.gameObject);
			}
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0005D0FC File Offset: 0x0005B2FC
		public string GetDisplayName()
		{
			string result = "";
			if (this.networkUserObject)
			{
				NetworkUser component = this.networkUserObject.GetComponent<NetworkUser>();
				if (component)
				{
					result = component.userName;
				}
			}
			return result;
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x0005D138 File Offset: 0x0005B338
		private void SetBody(GameObject newBody)
		{
			if (newBody)
			{
				this.body = newBody.GetComponent<CharacterBody>();
				this.bodyInputs = newBody.GetComponent<InputBankTest>();
				this.bodyIsFlier = newBody.GetComponent<RigidbodyMotor>();
				return;
			}
			this.body = null;
			this.bodyInputs = null;
			this.bodyIsFlier = false;
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x0005D18C File Offset: 0x0005B38C
		public bool preventGameOver
		{
			get
			{
				return this.master.preventGameOver;
			}
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0005D199 File Offset: 0x0005B399
		[Server]
		public void OnBodyDeath()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PlayerCharacterMasterController::OnBodyDeath()' called on client");
				return;
			}
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x00004507 File Offset: 0x00002707
		public void OnBodyStart()
		{
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060012FB RID: 4859 RVA: 0x0005D1B0 File Offset: 0x0005B3B0
		// (remove) Token: 0x060012FC RID: 4860 RVA: 0x0005D1E4 File Offset: 0x0005B3E4
		public static event Action<PlayerCharacterMasterController> onPlayerAdded;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060012FD RID: 4861 RVA: 0x0005D218 File Offset: 0x0005B418
		// (remove) Token: 0x060012FE RID: 4862 RVA: 0x0005D24C File Offset: 0x0005B44C
		public static event Action<PlayerCharacterMasterController> onPlayerRemoved;

		// Token: 0x060012FF RID: 4863 RVA: 0x0005D27F File Offset: 0x0005B47F
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				GameObject attacker = damageReport.damageInfo.attacker;
				if (attacker)
				{
					CharacterBody component = attacker.GetComponent<CharacterBody>();
					if (component)
					{
						GameObject masterObject = component.masterObject;
						if (masterObject)
						{
							PlayerCharacterMasterController component2 = masterObject.GetComponent<PlayerCharacterMasterController>();
							if (component2 && Util.CheckRoll(1f * component2.lunarCoinChanceMultiplier, 0f, null))
							{
								PickupDropletController.CreatePickupDroplet(PickupIndex.lunarCoin1, damageReport.victim.transform.position, Vector3.up * 10f);
								component2.lunarCoinChanceMultiplier *= 0.5f;
							}
						}
					}
				}
			};
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0005D2D0 File Offset: 0x0005B4D0
		static PlayerCharacterMasterController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerCharacterMasterController), PlayerCharacterMasterController.kRpcRpcIncrementRunCount, new NetworkBehaviour.CmdDelegate(PlayerCharacterMasterController.InvokeRpcRpcIncrementRunCount));
			NetworkCRC.RegisterBehaviour("PlayerCharacterMasterController", 0);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001303 RID: 4867 RVA: 0x0005D340 File Offset: 0x0005B540
		// (set) Token: 0x06001304 RID: 4868 RVA: 0x0005D353 File Offset: 0x0005B553
		public NetworkInstanceId NetworknetworkUserInstanceId
		{
			get
			{
				return this.networkUserInstanceId;
			}
			set
			{
				base.SetSyncVar<NetworkInstanceId>(value, ref this.networkUserInstanceId, 1u);
			}
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0005D367 File Offset: 0x0005B567
		protected static void InvokeRpcRpcIncrementRunCount(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcIncrementRunCount called on server.");
				return;
			}
			((PlayerCharacterMasterController)obj).RpcIncrementRunCount();
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0005D38C File Offset: 0x0005B58C
		public void CallRpcIncrementRunCount()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcIncrementRunCount called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)PlayerCharacterMasterController.kRpcRpcIncrementRunCount);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcIncrementRunCount");
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0005D3F8 File Offset: 0x0005B5F8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.networkUserInstanceId);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.networkUserInstanceId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x0005D464 File Offset: 0x0005B664
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.networkUserInstanceId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.networkUserInstanceId = reader.ReadNetworkId();
			}
		}

		// Token: 0x040016B8 RID: 5816
		private static List<PlayerCharacterMasterController> _instances = new List<PlayerCharacterMasterController>();

		// Token: 0x040016B9 RID: 5817
		private static ReadOnlyCollection<PlayerCharacterMasterController> _instancesReadOnly = new ReadOnlyCollection<PlayerCharacterMasterController>(PlayerCharacterMasterController._instances);

		// Token: 0x040016BA RID: 5818
		private CharacterBody body;

		// Token: 0x040016BB RID: 5819
		private InputBankTest bodyInputs;

		// Token: 0x040016BC RID: 5820
		private bool bodyIsFlier;

		// Token: 0x040016BE RID: 5822
		private PingerController pingerController;

		// Token: 0x040016BF RID: 5823
		[SyncVar]
		private NetworkInstanceId networkUserInstanceId;

		// Token: 0x040016C0 RID: 5824
		private GameObject resolvedNetworkUserGameObjectInstance;

		// Token: 0x040016C1 RID: 5825
		private bool networkUserResolved;

		// Token: 0x040016C2 RID: 5826
		private NetworkUser resolvedNetworkUserInstance;

		// Token: 0x040016C3 RID: 5827
		public float cameraMinPitch = -70f;

		// Token: 0x040016C4 RID: 5828
		public float cameraMaxPitch = 70f;

		// Token: 0x040016C5 RID: 5829
		public GameObject crosshair;

		// Token: 0x040016C6 RID: 5830
		public Vector3 crosshairPosition;

		// Token: 0x040016C7 RID: 5831
		private NetworkIdentity netid;

		// Token: 0x040016C8 RID: 5832
		private static readonly float sprintMinAimMoveDot = Mathf.Cos(1.0471976f);

		// Token: 0x040016C9 RID: 5833
		private bool sprintInputPressReceived;

		// Token: 0x040016CC RID: 5836
		private float lunarCoinChanceMultiplier = 0.5f;

		// Token: 0x040016CD RID: 5837
		private static int kRpcRpcIncrementRunCount = 1915650359;
	}
}
