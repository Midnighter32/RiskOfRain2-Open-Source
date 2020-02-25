using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Rewired;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C1 RID: 705
	[RequireComponent(typeof(CharacterMaster))]
	[RequireComponent(typeof(PingerController))]
	public class PlayerCharacterMasterController : NetworkBehaviour
	{
		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x00046125 File Offset: 0x00044325
		public static ReadOnlyCollection<PlayerCharacterMasterController> instances
		{
			get
			{
				return PlayerCharacterMasterController._instancesReadOnly;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x0004612C File Offset: 0x0004432C
		private bool bodyIsFlier
		{
			get
			{
				return !this.bodyMotor || this.bodyMotor.isFlying;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x00046148 File Offset: 0x00044348
		// (set) Token: 0x06000FF4 RID: 4084 RVA: 0x00046150 File Offset: 0x00044350
		public CharacterMaster master { get; private set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x00046159 File Offset: 0x00044359
		public bool hasEffectiveAuthority
		{
			get
			{
				return this.master.hasEffectiveAuthority;
			}
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x00046166 File Offset: 0x00044366
		private void OnSyncNetworkUserInstanceId(NetworkInstanceId value)
		{
			this.resolvedNetworkUserGameObjectInstance = null;
			this.resolvedNetworkUserInstance = null;
			this.networkUserResolved = (value == NetworkInstanceId.Invalid);
			this.NetworknetworkUserInstanceId = value;
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x00046190 File Offset: 0x00044390
		// (set) Token: 0x06000FF8 RID: 4088 RVA: 0x000461E8 File Offset: 0x000443E8
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

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x00046248 File Offset: 0x00044448
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

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x0004625F File Offset: 0x0004445F
		public bool isConnected
		{
			get
			{
				return this.networkUserObject;
			}
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x0004626C File Offset: 0x0004446C
		private void Awake()
		{
			this.master = base.GetComponent<CharacterMaster>();
			this.netid = base.GetComponent<NetworkIdentity>();
			this.pingerController = base.GetComponent<PingerController>();
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00046292 File Offset: 0x00044492
		private void OnEnable()
		{
			PlayerCharacterMasterController._instances.Add(this);
			if (PlayerCharacterMasterController.onPlayerAdded != null)
			{
				PlayerCharacterMasterController.onPlayerAdded(this);
			}
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x000462B1 File Offset: 0x000444B1
		private void OnDisable()
		{
			PlayerCharacterMasterController._instances.Remove(this);
			if (PlayerCharacterMasterController.onPlayerRemoved != null)
			{
				PlayerCharacterMasterController.onPlayerRemoved(this);
			}
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x000462D1 File Offset: 0x000444D1
		private void Start()
		{
			if (NetworkServer.active && this.networkUser)
			{
				this.CallRpcIncrementRunCount();
			}
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x000462F0 File Offset: 0x000444F0
		[ClientRpc]
		private void RpcIncrementRunCount()
		{
			if (this.networkUser)
			{
				LocalUser localUser = this.networkUser.localUser;
				if (localUser != null)
				{
					localUser.userProfile.totalRunCount += 1U;
				}
			}
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x0004632C File Offset: 0x0004452C
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

		// Token: 0x06001001 RID: 4097 RVA: 0x0004653C File Offset: 0x0004473C
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
							if ((this.body.bodyFlags & CharacterBody.BodyFlags.SprintAnyDirection) == CharacterBody.BodyFlags.None && Vector3.Dot(aimDirection, moveVector) < PlayerCharacterMasterController.sprintMinAimMoveDot)
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

		// Token: 0x06001002 RID: 4098 RVA: 0x000467B4 File Offset: 0x000449B4
		private void CheckPinging()
		{
			if (this.hasEffectiveAuthority && this.body && this.bodyInputs && this.bodyInputs.ping.justPressed)
			{
				this.pingerController.AttemptPing(new Ray(this.bodyInputs.aimOrigin, this.bodyInputs.aimDirection), this.body.gameObject);
			}
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x00046828 File Offset: 0x00044A28
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

		// Token: 0x06001004 RID: 4100 RVA: 0x00046864 File Offset: 0x00044A64
		private void SetBody(GameObject newBody)
		{
			if (newBody)
			{
				this.body = newBody.GetComponent<CharacterBody>();
			}
			else
			{
				this.body = null;
			}
			if (this.body)
			{
				this.bodyInputs = this.body.inputBank;
				this.bodyMotor = this.body.characterMotor;
				return;
			}
			this.bodyInputs = null;
			this.bodyMotor = null;
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06001005 RID: 4101 RVA: 0x000468CC File Offset: 0x00044ACC
		public bool preventGameOver
		{
			get
			{
				return this.master.preventGameOver;
			}
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x000468D9 File Offset: 0x00044AD9
		[Server]
		public void OnBodyDeath()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PlayerCharacterMasterController::OnBodyDeath()' called on client");
				return;
			}
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnBodyStart()
		{
		}

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001008 RID: 4104 RVA: 0x000468F0 File Offset: 0x00044AF0
		// (remove) Token: 0x06001009 RID: 4105 RVA: 0x00046924 File Offset: 0x00044B24
		public static event Action<PlayerCharacterMasterController> onPlayerAdded;

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x0600100A RID: 4106 RVA: 0x00046958 File Offset: 0x00044B58
		// (remove) Token: 0x0600100B RID: 4107 RVA: 0x0004698C File Offset: 0x00044B8C
		public static event Action<PlayerCharacterMasterController> onPlayerRemoved;

		// Token: 0x0600100C RID: 4108 RVA: 0x000469BF File Offset: 0x00044BBF
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				CharacterMaster characterMaster = damageReport.attackerMaster;
				if (characterMaster)
				{
					if (characterMaster.minionOwnership.ownerMaster)
					{
						characterMaster = characterMaster.minionOwnership.ownerMaster;
					}
					PlayerCharacterMasterController component = characterMaster.GetComponent<PlayerCharacterMasterController>();
					if (component && Util.CheckRoll(1f * component.lunarCoinChanceMultiplier, 0f, null))
					{
						PickupDropletController.CreatePickupDroplet(PickupIndex.Find("LunarCoin.Coin0"), damageReport.victim.transform.position, Vector3.up * 10f);
						component.lunarCoinChanceMultiplier *= 0.5f;
					}
				}
			};
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x00046A10 File Offset: 0x00044C10
		static PlayerCharacterMasterController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerCharacterMasterController), PlayerCharacterMasterController.kRpcRpcIncrementRunCount, new NetworkBehaviour.CmdDelegate(PlayerCharacterMasterController.InvokeRpcRpcIncrementRunCount));
			NetworkCRC.RegisterBehaviour("PlayerCharacterMasterController", 0);
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x00046A80 File Offset: 0x00044C80
		// (set) Token: 0x06001011 RID: 4113 RVA: 0x00046A93 File Offset: 0x00044C93
		public NetworkInstanceId NetworknetworkUserInstanceId
		{
			get
			{
				return this.networkUserInstanceId;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncNetworkUserInstanceId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkInstanceId>(value, ref this.networkUserInstanceId, dirtyBit);
			}
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x00046AD2 File Offset: 0x00044CD2
		protected static void InvokeRpcRpcIncrementRunCount(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcIncrementRunCount called on server.");
				return;
			}
			((PlayerCharacterMasterController)obj).RpcIncrementRunCount();
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x00046AF8 File Offset: 0x00044CF8
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

		// Token: 0x06001014 RID: 4116 RVA: 0x00046B64 File Offset: 0x00044D64
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.networkUserInstanceId);
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
				writer.Write(this.networkUserInstanceId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x00046BD0 File Offset: 0x00044DD0
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
				this.OnSyncNetworkUserInstanceId(reader.ReadNetworkId());
			}
		}

		// Token: 0x04000F75 RID: 3957
		private static List<PlayerCharacterMasterController> _instances = new List<PlayerCharacterMasterController>();

		// Token: 0x04000F76 RID: 3958
		private static ReadOnlyCollection<PlayerCharacterMasterController> _instancesReadOnly = new ReadOnlyCollection<PlayerCharacterMasterController>(PlayerCharacterMasterController._instances);

		// Token: 0x04000F77 RID: 3959
		private CharacterBody body;

		// Token: 0x04000F78 RID: 3960
		private InputBankTest bodyInputs;

		// Token: 0x04000F79 RID: 3961
		private CharacterMotor bodyMotor;

		// Token: 0x04000F7B RID: 3963
		private PingerController pingerController;

		// Token: 0x04000F7C RID: 3964
		[SyncVar(hook = "OnSyncNetworkUserInstanceId")]
		private NetworkInstanceId networkUserInstanceId;

		// Token: 0x04000F7D RID: 3965
		private GameObject resolvedNetworkUserGameObjectInstance;

		// Token: 0x04000F7E RID: 3966
		private bool networkUserResolved;

		// Token: 0x04000F7F RID: 3967
		private NetworkUser resolvedNetworkUserInstance;

		// Token: 0x04000F80 RID: 3968
		public float cameraMinPitch = -70f;

		// Token: 0x04000F81 RID: 3969
		public float cameraMaxPitch = 70f;

		// Token: 0x04000F82 RID: 3970
		public GameObject crosshair;

		// Token: 0x04000F83 RID: 3971
		public Vector3 crosshairPosition;

		// Token: 0x04000F84 RID: 3972
		private NetworkIdentity netid;

		// Token: 0x04000F85 RID: 3973
		private static readonly float sprintMinAimMoveDot = Mathf.Cos(1.0471976f);

		// Token: 0x04000F86 RID: 3974
		private bool sprintInputPressReceived;

		// Token: 0x04000F89 RID: 3977
		private float lunarCoinChanceMultiplier = 0.5f;

		// Token: 0x04000F8A RID: 3978
		private static int kRpcRpcIncrementRunCount = 1915650359;
	}
}
