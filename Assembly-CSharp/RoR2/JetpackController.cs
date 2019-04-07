using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000344 RID: 836
	public class JetpackController : NetworkBehaviour
	{
		// Token: 0x06001149 RID: 4425 RVA: 0x00055F07 File Offset: 0x00054107
		private void OnEnable()
		{
			JetpackController.instancesList.Add(this);
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00055F14 File Offset: 0x00054114
		private void OnDisable()
		{
			JetpackController.instancesList.Remove(this);
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x00055F24 File Offset: 0x00054124
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

		// Token: 0x0600114C RID: 4428 RVA: 0x00055F78 File Offset: 0x00054178
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

		// Token: 0x0600114D RID: 4429 RVA: 0x00055FFD File Offset: 0x000541FD
		private void OnDestroy()
		{
			if (NetworkServer.active && this.targetBody)
			{
				this.targetBody.RemoveBuff(BuffIndex.BugWings);
			}
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x00056020 File Offset: 0x00054220
		public void ResetTimer()
		{
			this.stopwatch = 0f;
			if (NetworkServer.active)
			{
				this.CallRpcResetTimer();
			}
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0005603A File Offset: 0x0005423A
		[ClientRpc]
		private void RpcResetTimer()
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.ResetTimer();
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0005604A File Offset: 0x0005424A
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

		// Token: 0x06001151 RID: 4433 RVA: 0x00056066 File Offset: 0x00054266
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

		// Token: 0x06001152 RID: 4434 RVA: 0x00056090 File Offset: 0x00054290
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

		// Token: 0x06001153 RID: 4435 RVA: 0x000560E9 File Offset: 0x000542E9
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

		// Token: 0x06001154 RID: 4436 RVA: 0x00056120 File Offset: 0x00054320
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

		// Token: 0x06001155 RID: 4437 RVA: 0x0005635C File Offset: 0x0005455C
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

		// Token: 0x06001156 RID: 4438 RVA: 0x000563BC File Offset: 0x000545BC
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

		// Token: 0x06001157 RID: 4439 RVA: 0x00056444 File Offset: 0x00054644
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

		// Token: 0x06001159 RID: 4441 RVA: 0x00056538 File Offset: 0x00054738
		static JetpackController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(JetpackController), JetpackController.kRpcRpcResetTimer, new NetworkBehaviour.CmdDelegate(JetpackController.InvokeRpcRpcResetTimer));
			NetworkCRC.RegisterBehaviour("JetpackController", 0);
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600115B RID: 4443 RVA: 0x00056594 File Offset: 0x00054794
		// (set) Token: 0x0600115C RID: 4444 RVA: 0x000565A7 File Offset: 0x000547A7
		public GameObject NetworktargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetObject, 1u, ref this.___targetObjectNetId);
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600115D RID: 4445 RVA: 0x000565C4 File Offset: 0x000547C4
		// (set) Token: 0x0600115E RID: 4446 RVA: 0x000565D7 File Offset: 0x000547D7
		public bool NetworkjumpInputActive
		{
			get
			{
				return this.jumpInputActive;
			}
			set
			{
				uint dirtyBit = 2u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SyncJumpInputActive(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.jumpInputActive, dirtyBit);
			}
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00056616 File Offset: 0x00054816
		protected static void InvokeRpcRpcResetTimer(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcResetTimer called on server.");
				return;
			}
			((JetpackController)obj).RpcResetTimer();
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x0005663C File Offset: 0x0005483C
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

		// Token: 0x06001161 RID: 4449 RVA: 0x000566A8 File Offset: 0x000548A8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.targetObject);
				writer.Write(this.jumpInputActive);
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
				writer.Write(this.targetObject);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
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

		// Token: 0x06001162 RID: 4450 RVA: 0x00056754 File Offset: 0x00054954
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

		// Token: 0x06001163 RID: 4451 RVA: 0x000567BA File Offset: 0x000549BA
		public override void PreStartClient()
		{
			if (!this.___targetObjectNetId.IsEmpty())
			{
				this.NetworktargetObject = ClientScene.FindLocalObject(this.___targetObjectNetId);
			}
		}

		// Token: 0x0400155B RID: 5467
		private static readonly List<JetpackController> instancesList = new List<JetpackController>();

		// Token: 0x0400155C RID: 5468
		[SyncVar]
		public GameObject targetObject;

		// Token: 0x0400155D RID: 5469
		public float duration;

		// Token: 0x0400155E RID: 5470
		public float acceleration;

		// Token: 0x0400155F RID: 5471
		private float stopwatch;

		// Token: 0x04001560 RID: 5472
		private CharacterBody targetBody;

		// Token: 0x04001561 RID: 5473
		private Transform wingTransform;

		// Token: 0x04001562 RID: 5474
		private Animator wingAnimator;

		// Token: 0x04001563 RID: 5475
		private GameObject[] wingMotions;

		// Token: 0x04001564 RID: 5476
		private GameObject wingMeshObject;

		// Token: 0x04001565 RID: 5477
		private CharacterMotor targetCharacterMotor;

		// Token: 0x04001566 RID: 5478
		private InputBankTest targetInputBank;

		// Token: 0x04001567 RID: 5479
		private bool targetHasAuthority;

		// Token: 0x04001568 RID: 5480
		private bool hasBegunSoundLoop;

		// Token: 0x04001569 RID: 5481
		[SyncVar(hook = "SyncJumpInputActive")]
		private bool jumpInputActive;

		// Token: 0x0400156A RID: 5482
		private static JetpackController.SetJetpackJumpStateMessage stateMessageBuffer = new JetpackController.SetJetpackJumpStateMessage();

		// Token: 0x0400156B RID: 5483
		private NetworkInstanceId ___targetObjectNetId;

		// Token: 0x0400156C RID: 5484
		private static int kRpcRpcResetTimer = 1278379706;

		// Token: 0x02000345 RID: 837
		private class SetJetpackJumpStateMessage : MessageBase
		{
			// Token: 0x06001165 RID: 4453 RVA: 0x000567DE File Offset: 0x000549DE
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.jetpackControllerObject);
				writer.Write(this.state);
			}

			// Token: 0x06001166 RID: 4454 RVA: 0x000567F8 File Offset: 0x000549F8
			public override void Deserialize(NetworkReader reader)
			{
				this.jetpackControllerObject = reader.ReadGameObject();
				this.state = reader.ReadBoolean();
			}

			// Token: 0x0400156D RID: 5485
			public GameObject jetpackControllerObject;

			// Token: 0x0400156E RID: 5486
			public bool state;
		}
	}
}
