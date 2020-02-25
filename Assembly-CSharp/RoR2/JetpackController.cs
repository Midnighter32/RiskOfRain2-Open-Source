using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000268 RID: 616
	public class JetpackController : NetworkBehaviour
	{
		// Token: 0x06000DA3 RID: 3491 RVA: 0x0003D329 File Offset: 0x0003B529
		private void OnEnable()
		{
			JetpackController.instancesList.Add(this);
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x0003D336 File Offset: 0x0003B536
		private void OnDisable()
		{
			JetpackController.instancesList.Remove(this);
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0003D344 File Offset: 0x0003B544
		public static JetpackController FindJetpackController(GameObject targetObject)
		{
			if (!targetObject)
			{
				return null;
			}
			for (int i = 0; i < JetpackController.instancesList.Count; i++)
			{
				if (JetpackController.instancesList[i].targetObject == targetObject)
				{
					return JetpackController.instancesList[i];
				}
			}
			return null;
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x0003D398 File Offset: 0x0003B598
		private void Start()
		{
			this.SetupWings();
			if (this.targetObject)
			{
				this.targetBody = this.targetObject.GetComponent<CharacterBody>();
				this.targetCharacterMotor = this.targetObject.GetComponent<CharacterMotor>();
				this.targetInputBank = this.targetObject.GetComponent<InputBankTest>();
				this.targetHasAuthority = Util.HasEffectiveAuthority(this.targetObject);
				if (NetworkServer.active && this.targetBody)
				{
					this.targetBody.AddBuff(BuffIndex.BugWings);
				}
			}
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x0003D41D File Offset: 0x0003B61D
		private void OnDestroy()
		{
			if (NetworkServer.active && this.targetBody)
			{
				this.targetBody.RemoveBuff(BuffIndex.BugWings);
			}
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x0003D440 File Offset: 0x0003B640
		public void ResetTimer()
		{
			this.stopwatch = 0f;
			if (NetworkServer.active)
			{
				this.CallRpcResetTimer();
			}
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x0003D45A File Offset: 0x0003B65A
		[ClientRpc]
		private void RpcResetTimer()
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.ResetTimer();
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x0003D46A File Offset: 0x0003B66A
		private void SyncJumpInputActive(bool state)
		{
			if (this.jumpInputActive == state)
			{
				return;
			}
			if (this.targetHasAuthority)
			{
				return;
			}
			this.NetworkjumpInputActive = state;
		}

		// Token: 0x06000DAB RID: 3499 RVA: 0x0003D486 File Offset: 0x0003B686
		private void SetJumpInputActive(bool state)
		{
			if (this.jumpInputActive == state)
			{
				return;
			}
			if (this.targetHasAuthority && !NetworkServer.active)
			{
				this.SendJetpackJumpState(state);
			}
			this.NetworkjumpInputActive = state;
		}

		// Token: 0x06000DAC RID: 3500 RVA: 0x0003D4B0 File Offset: 0x0003B6B0
		[Client]
		private void SendJetpackJumpState(bool state)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.JetpackController::SendJetpackJumpState(System.Boolean)' called on server");
				return;
			}
			JetpackController.stateMessageBuffer.jetpackControllerObject = base.gameObject;
			JetpackController.stateMessageBuffer.state = state;
			NetworkManager.singleton.client.connection.Send(69, JetpackController.stateMessageBuffer);
		}

		// Token: 0x06000DAD RID: 3501 RVA: 0x0003D509 File Offset: 0x0003B709
		[NetworkMessageHandler(msgType = 69, client = false, server = true)]
		private static void HandleSendJumpInputActive(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<JetpackController.SetJetpackJumpStateMessage>(JetpackController.stateMessageBuffer);
			GameObject jetpackControllerObject = JetpackController.stateMessageBuffer.jetpackControllerObject;
			if (jetpackControllerObject == null)
			{
				return;
			}
			JetpackController component = jetpackControllerObject.GetComponent<JetpackController>();
			if (component == null)
			{
				return;
			}
			component.SetJumpInputActive(JetpackController.stateMessageBuffer.state);
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x0003D540 File Offset: 0x0003B740
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active && !this.wingTransform)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.targetObject)
			{
				base.transform.position = this.targetObject.transform.position;
			}
			if (this.targetObject && this.targetHasAuthority)
			{
				this.SetJumpInputActive(this.targetInputBank && this.targetInputBank.jump.down && this.stopwatch < this.duration);
				if (this.targetCharacterMotor)
				{
					if (this.jumpInputActive)
					{
						Vector3 velocity = this.targetCharacterMotor.velocity;
						velocity.y = Mathf.Max(Mathf.MoveTowards(velocity.y, this.targetBody.jumpPower / 3f, this.acceleration * Time.fixedDeltaTime), velocity.y);
						this.targetCharacterMotor.velocity = velocity;
						this.targetCharacterMotor.disableAirControlUntilCollision = false;
					}
					else
					{
						this.targetCharacterMotor.velocity += new Vector3(0f, Physics.gravity.y * -0.5f * Time.fixedDeltaTime, 0f);
					}
				}
			}
			if (this.stopwatch >= this.duration)
			{
				bool flag = !this.targetCharacterMotor || !this.targetCharacterMotor.isGrounded;
				if (this.wingAnimator)
				{
					this.wingAnimator.SetBool("wingsReady", false);
				}
				this.ShowMotionLines(false);
				if (NetworkServer.active && !flag)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
			}
			else if (this.wingAnimator)
			{
				this.wingAnimator.SetBool("wingsReady", true);
				if (this.jumpInputActive)
				{
					this.wingAnimator.SetFloat("fly.playbackRate", 10f, 0.1f, Time.fixedDeltaTime);
				}
				else
				{
					this.wingAnimator.SetFloat("fly.playbackRate", 0f, 0.2f, Time.fixedDeltaTime);
				}
				this.ShowMotionLines(this.jumpInputActive);
			}
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x0003D788 File Offset: 0x0003B988
		private Transform FindWings()
		{
			ModelLocator component = this.targetObject.GetComponent<ModelLocator>();
			if (component)
			{
				Transform modelTransform = component.modelTransform;
				if (modelTransform)
				{
					CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
					if (component2)
					{
						List<GameObject> equipmentDisplayObjects = component2.GetEquipmentDisplayObjects(EquipmentIndex.Jetpack);
						if (equipmentDisplayObjects.Count > 0)
						{
							return equipmentDisplayObjects[0].transform;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x0003D7E8 File Offset: 0x0003B9E8
		private void ShowMotionLines(bool showWings)
		{
			for (int i = 0; i < this.wingMotions.Length; i++)
			{
				if (this.wingMotions[i])
				{
					this.wingMotions[i].SetActive(showWings);
				}
			}
			this.wingMeshObject.SetActive(!showWings);
			if (this.hasBegunSoundLoop != showWings)
			{
				if (showWings)
				{
					if (showWings)
					{
						Util.PlaySound("Play_item_use_bugWingFlapLoop", base.gameObject);
					}
				}
				else
				{
					Util.PlaySound("Stop_item_use_bugWingFlapLoop", base.gameObject);
				}
				this.hasBegunSoundLoop = showWings;
			}
		}

		// Token: 0x06000DB1 RID: 3505 RVA: 0x0003D870 File Offset: 0x0003BA70
		public void SetupWings()
		{
			this.wingTransform = this.FindWings();
			if (this.wingTransform)
			{
				this.wingAnimator = this.wingTransform.GetComponentInChildren<Animator>();
				ChildLocator component = this.wingTransform.GetComponent<ChildLocator>();
				if (this.wingAnimator)
				{
					this.wingAnimator.SetBool("wingsReady", true);
				}
				if (component)
				{
					this.wingMotions = new GameObject[4];
					this.wingMotions[0] = component.FindChild("WingMotionLargeL").gameObject;
					this.wingMotions[1] = component.FindChild("WingMotionLargeR").gameObject;
					this.wingMotions[2] = component.FindChild("WingMotionSmallL").gameObject;
					this.wingMotions[3] = component.FindChild("WingMotionSmallR").gameObject;
					this.wingMeshObject = component.FindChild("WingMesh").gameObject;
				}
			}
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x0003D964 File Offset: 0x0003BB64
		static JetpackController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(JetpackController), JetpackController.kRpcRpcResetTimer, new NetworkBehaviour.CmdDelegate(JetpackController.InvokeRpcRpcResetTimer));
			NetworkCRC.RegisterBehaviour("JetpackController", 0);
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000DB5 RID: 3509 RVA: 0x0003D9C0 File Offset: 0x0003BBC0
		// (set) Token: 0x06000DB6 RID: 3510 RVA: 0x0003D9D3 File Offset: 0x0003BBD3
		public GameObject NetworktargetObject
		{
			get
			{
				return this.targetObject;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetObject, 1U, ref this.___targetObjectNetId);
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000DB7 RID: 3511 RVA: 0x0003D9F0 File Offset: 0x0003BBF0
		// (set) Token: 0x06000DB8 RID: 3512 RVA: 0x0003DA03 File Offset: 0x0003BC03
		public bool NetworkjumpInputActive
		{
			get
			{
				return this.jumpInputActive;
			}
			[param: In]
			set
			{
				uint dirtyBit = 2U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SyncJumpInputActive(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.jumpInputActive, dirtyBit);
			}
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x0003DA42 File Offset: 0x0003BC42
		protected static void InvokeRpcRpcResetTimer(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcResetTimer called on server.");
				return;
			}
			((JetpackController)obj).RpcResetTimer();
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x0003DA68 File Offset: 0x0003BC68
		public void CallRpcResetTimer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcResetTimer called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)JetpackController.kRpcRpcResetTimer);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcResetTimer");
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x0003DAD4 File Offset: 0x0003BCD4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.targetObject);
				writer.Write(this.jumpInputActive);
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
				writer.Write(this.targetObject);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.jumpInputActive);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x0003DB80 File Offset: 0x0003BD80
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___targetObjectNetId = reader.ReadNetworkId();
				this.jumpInputActive = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.targetObject = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.SyncJumpInputActive(reader.ReadBoolean());
			}
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x0003DBE6 File Offset: 0x0003BDE6
		public override void PreStartClient()
		{
			if (!this.___targetObjectNetId.IsEmpty())
			{
				this.NetworktargetObject = ClientScene.FindLocalObject(this.___targetObjectNetId);
			}
		}

		// Token: 0x04000DAC RID: 3500
		private static readonly List<JetpackController> instancesList = new List<JetpackController>();

		// Token: 0x04000DAD RID: 3501
		[SyncVar]
		public GameObject targetObject;

		// Token: 0x04000DAE RID: 3502
		public float duration;

		// Token: 0x04000DAF RID: 3503
		public float acceleration;

		// Token: 0x04000DB0 RID: 3504
		private float stopwatch;

		// Token: 0x04000DB1 RID: 3505
		private CharacterBody targetBody;

		// Token: 0x04000DB2 RID: 3506
		private Transform wingTransform;

		// Token: 0x04000DB3 RID: 3507
		private Animator wingAnimator;

		// Token: 0x04000DB4 RID: 3508
		private GameObject[] wingMotions;

		// Token: 0x04000DB5 RID: 3509
		private GameObject wingMeshObject;

		// Token: 0x04000DB6 RID: 3510
		private CharacterMotor targetCharacterMotor;

		// Token: 0x04000DB7 RID: 3511
		private InputBankTest targetInputBank;

		// Token: 0x04000DB8 RID: 3512
		private bool targetHasAuthority;

		// Token: 0x04000DB9 RID: 3513
		private bool hasBegunSoundLoop;

		// Token: 0x04000DBA RID: 3514
		[SyncVar(hook = "SyncJumpInputActive")]
		private bool jumpInputActive;

		// Token: 0x04000DBB RID: 3515
		private static JetpackController.SetJetpackJumpStateMessage stateMessageBuffer = new JetpackController.SetJetpackJumpStateMessage();

		// Token: 0x04000DBC RID: 3516
		private NetworkInstanceId ___targetObjectNetId;

		// Token: 0x04000DBD RID: 3517
		private static int kRpcRpcResetTimer = 1278379706;

		// Token: 0x02000269 RID: 617
		private class SetJetpackJumpStateMessage : MessageBase
		{
			// Token: 0x06000DBF RID: 3519 RVA: 0x0003DC0A File Offset: 0x0003BE0A
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.jetpackControllerObject);
				writer.Write(this.state);
			}

			// Token: 0x06000DC0 RID: 3520 RVA: 0x0003DC24 File Offset: 0x0003BE24
			public override void Deserialize(NetworkReader reader)
			{
				this.jetpackControllerObject = reader.ReadGameObject();
				this.state = reader.ReadBoolean();
			}

			// Token: 0x04000DBE RID: 3518
			public GameObject jetpackControllerObject;

			// Token: 0x04000DBF RID: 3519
			public bool state;
		}
	}
}
