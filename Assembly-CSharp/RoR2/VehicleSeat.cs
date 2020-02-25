using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using EntityStates;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200036B RID: 875
	[DisallowMultipleComponent]
	public class VehicleSeat : NetworkBehaviour, IInteractable
	{
		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x0005A97F File Offset: 0x00058B7F
		public CharacterBody currentPassengerBody
		{
			get
			{
				return this.passengerInfo.body;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x0005A98C File Offset: 0x00058B8C
		public InputBankTest currentPassengerInputBank
		{
			get
			{
				return this.passengerInfo.inputBank;
			}
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x0005A999 File Offset: 0x00058B99
		public string GetContextString(Interactor activator)
		{
			if (!this.passengerBodyObject)
			{
				return Language.GetString(this.enterVehicleContextString);
			}
			if (this.passengerBodyObject == activator.gameObject)
			{
				return Language.GetString(this.exitVehicleContextString);
			}
			return null;
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x0005A9D0 File Offset: 0x00058BD0
		public Interactability GetInteractability(Interactor activator)
		{
			CharacterBody component = activator.GetComponent<CharacterBody>();
			if (!this.passengerBodyObject)
			{
				Interactability? interactability = this.enterVehicleAllowedCheck.Evaluate(component);
				if (interactability == null)
				{
					return Interactability.Available;
				}
				return interactability.GetValueOrDefault();
			}
			else if (this.passengerBodyObject == activator.gameObject && this.passengerAssignmentTime.timeSince >= this.passengerAssignmentCooldown)
			{
				Interactability? interactability = this.exitVehicleAllowedCheck.Evaluate(component);
				if (interactability == null)
				{
					return Interactability.Available;
				}
				return interactability.GetValueOrDefault();
			}
			else
			{
				if (component && component.currentVehicle && component.currentVehicle != this)
				{
					return Interactability.ConditionsNotMet;
				}
				return Interactability.Disabled;
			}
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x0005AA74 File Offset: 0x00058C74
		public void OnInteractionBegin(Interactor activator)
		{
			if (!this.passengerBodyObject)
			{
				if (this.handleVehicleEnterRequestServer.Evaluate(activator.gameObject) == null)
				{
					this.SetPassenger(activator.gameObject);
					return;
				}
			}
			else if (activator.gameObject == this.passengerBodyObject && this.handleVehicleExitRequestServer.Evaluate(activator.gameObject) == null)
			{
				this.SetPassenger(null);
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x0000B933 File Offset: 0x00009B33
		public bool ShouldShowOnScanner()
		{
			return true;
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x0005AAE6 File Offset: 0x00058CE6
		private static bool shouldLog
		{
			get
			{
				return VehicleSeat.cvVehicleSeatDebug.value;
			}
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x0005AAF2 File Offset: 0x00058CF2
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.collider = base.GetComponent<Collider>();
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x0005AB0C File Offset: 0x00058D0C
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!NetworkServer.active && this.passengerBodyObject)
			{
				this.OnPassengerEnter(this.passengerBodyObject);
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x0600154D RID: 5453 RVA: 0x0005AB34 File Offset: 0x00058D34
		public bool hasPassenger
		{
			get
			{
				return this.passengerBodyObject;
			}
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x0005AB44 File Offset: 0x00058D44
		private void SetPassengerInternal(GameObject newPassengerBodyObject)
		{
			if (this.passengerBodyObject)
			{
				this.OnPassengerExit(this.passengerBodyObject);
			}
			this.NetworkpassengerBodyObject = newPassengerBodyObject;
			this.passengerInfo = default(VehicleSeat.PassengerInfo);
			this.passengerAssignmentTime = Run.FixedTimeStamp.now;
			if (this.passengerBodyObject)
			{
				this.OnPassengerEnter(this.passengerBodyObject);
			}
			if (VehicleSeat.shouldLog)
			{
				Debug.Log("End SetPassenger.");
			}
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x0005ABB4 File Offset: 0x00058DB4
		private void SetPassenger(GameObject newPassengerBodyObject)
		{
			string text = newPassengerBodyObject ? Util.GetBestBodyName(newPassengerBodyObject) : "null";
			if (VehicleSeat.shouldLog)
			{
				Debug.LogFormat("SetPassenger passenger={0}", new object[]
				{
					text
				});
			}
			if (base.syncVarHookGuard)
			{
				if (VehicleSeat.shouldLog)
				{
					Debug.Log("syncVarHookGuard==true Setting passengerBodyObject=newPassengerBodyObject");
				}
				this.NetworkpassengerBodyObject = newPassengerBodyObject;
				return;
			}
			if (VehicleSeat.shouldLog)
			{
				Debug.Log("syncVarHookGuard==false");
			}
			if (this.passengerBodyObject == newPassengerBodyObject)
			{
				if (VehicleSeat.shouldLog)
				{
					Debug.Log("ReferenceEquals(passengerBodyObject, newPassengerBodyObject)==true");
				}
				return;
			}
			if (VehicleSeat.shouldLog)
			{
				Debug.Log("ReferenceEquals(passengerBodyObject, newPassengerBodyObject)==false");
			}
			this.SetPassengerInternal(newPassengerBodyObject);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x0005AC57 File Offset: 0x00058E57
		private void OnPassengerMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
		{
			if (NetworkServer.active && this.ejectOnCollision && this.passengerAssignmentTime.timeSince > Time.fixedDeltaTime)
			{
				this.SetPassenger(null);
			}
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x0005AC84 File Offset: 0x00058E84
		private void ForcePassengerState()
		{
			if (this.passengerInfo.bodyStateMachine && this.passengerInfo.hasEffectiveAuthority)
			{
				Type type = this.passengerState.GetType();
				if (this.passengerInfo.bodyStateMachine.state.GetType() != type)
				{
					this.passengerInfo.bodyStateMachine.SetInterruptState(EntityState.Instantiate(this.passengerState), InterruptPriority.Vehicle);
				}
			}
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0005ACFB File Offset: 0x00058EFB
		private void Update()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0005AD03 File Offset: 0x00058F03
		private void FixedUpdate()
		{
			this.ForcePassengerState();
			this.UpdatePassengerPosition();
			if (this.passengerInfo.characterMotor)
			{
				this.passengerInfo.characterMotor.velocity = Vector3.zero;
			}
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0005AD38 File Offset: 0x00058F38
		private void UpdatePassengerPosition()
		{
			Vector3 position = this.seatPosition.position;
			if (this.passengerInfo.characterMotor)
			{
				this.passengerInfo.characterMotor.velocity = Vector3.zero;
				this.passengerInfo.characterMotor.Motor.BaseVelocity = Vector3.zero;
				this.passengerInfo.characterMotor.Motor.SetPosition(position, true);
				if (!this.disablePassengerMotor && Time.inFixedTimeStep)
				{
					this.passengerInfo.characterMotor.rootMotion = position - this.passengerInfo.transform.position;
					return;
				}
			}
			else if (this.passengerInfo.transform)
			{
				this.passengerInfo.transform.position = position;
			}
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0005AE04 File Offset: 0x00059004
		[Server]
		public bool AssignPassenger(GameObject bodyObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.VehicleSeat::AssignPassenger(UnityEngine.GameObject)' called on client");
				return false;
			}
			if (this.passengerBodyObject)
			{
				return false;
			}
			if (bodyObject)
			{
				CharacterBody component = bodyObject.GetComponent<CharacterBody>();
				if (component && component.currentVehicle)
				{
					component.currentVehicle.EjectPassenger(bodyObject);
				}
			}
			this.SetPassenger(bodyObject);
			return true;
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0005AE6E File Offset: 0x0005906E
		[Server]
		public void EjectPassenger(GameObject bodyObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VehicleSeat::EjectPassenger(UnityEngine.GameObject)' called on client");
				return;
			}
			if (bodyObject == this.passengerBodyObject)
			{
				this.SetPassenger(null);
			}
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x0005AE95 File Offset: 0x00059095
		[Server]
		public void EjectPassenger()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VehicleSeat::EjectPassenger()' called on client");
				return;
			}
			this.SetPassenger(null);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0005AEB3 File Offset: 0x000590B3
		private void OnDestroy()
		{
			this.SetPassenger(null);
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x0005AEBC File Offset: 0x000590BC
		private void OnPassengerEnter(GameObject passenger)
		{
			this.passengerInfo = new VehicleSeat.PassengerInfo(this.passengerBodyObject);
			if (this.passengerInfo.body)
			{
				this.passengerInfo.body.currentVehicle = this;
			}
			if (this.hidePassenger && this.passengerInfo.characterModel)
			{
				this.passengerInfo.characterModel.invisibilityCount++;
			}
			this.ForcePassengerState();
			if (this.passengerInfo.characterMotor)
			{
				if (this.disablePassengerMotor)
				{
					this.passengerInfo.characterMotor.enabled = false;
				}
				else
				{
					this.passengerInfo.characterMotor.onMovementHit += this.OnPassengerMovementHit;
				}
			}
			if (this.passengerInfo.collider && this.collider)
			{
				Physics.IgnoreCollision(this.collider, this.passengerInfo.collider, true);
			}
			if (this.passengerInfo.interactionDriver)
			{
				this.passengerInfo.interactionDriver.interactableOverride = base.gameObject;
			}
			if (VehicleSeat.shouldLog)
			{
				Debug.Log("Taking control of passengerBodyObject.");
				Debug.Log(this.passengerInfo.GetString());
			}
			Action<GameObject> action = this.onPassengerEnter;
			if (action != null)
			{
				action(this.passengerBodyObject);
			}
			UnityEvent unityEvent = this.onPassengerEnterUnityEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			Action<VehicleSeat, GameObject> action2 = VehicleSeat.onPassengerEnterGlobal;
			if (action2 == null)
			{
				return;
			}
			action2(this, this.passengerBodyObject);
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x0005B03C File Offset: 0x0005923C
		private void OnPassengerExit(GameObject passenger)
		{
			if (VehicleSeat.shouldLog)
			{
				Debug.Log("Releasing passenger.");
			}
			if (this.hidePassenger && this.passengerInfo.characterModel)
			{
				this.passengerInfo.characterModel.invisibilityCount--;
			}
			if (this.passengerInfo.body)
			{
				this.passengerInfo.body.currentVehicle = null;
			}
			if (this.passengerInfo.characterMotor)
			{
				if (this.disablePassengerMotor)
				{
					this.passengerInfo.characterMotor.enabled = true;
				}
				else
				{
					this.passengerInfo.characterMotor.onMovementHit -= this.OnPassengerMovementHit;
				}
				this.passengerInfo.characterMotor.velocity = Vector3.zero;
				this.passengerInfo.characterMotor.rootMotion = Vector3.zero;
				this.passengerInfo.characterMotor.Motor.BaseVelocity = Vector3.zero;
			}
			if (this.passengerInfo.collider && this.collider)
			{
				Physics.IgnoreCollision(this.collider, this.passengerInfo.collider, false);
			}
			if (this.passengerInfo.hasEffectiveAuthority)
			{
				if (this.passengerInfo.bodyStateMachine && this.passengerInfo.bodyStateMachine.CanInterruptState(InterruptPriority.Vehicle))
				{
					this.passengerInfo.bodyStateMachine.SetNextStateToMain();
				}
				Vector3 newPosition = this.exitPosition ? this.exitPosition.position : this.seatPosition.position;
				TeleportHelper.TeleportGameObject(this.passengerInfo.transform.gameObject, newPosition);
			}
			if (this.passengerInfo.interactionDriver && this.passengerInfo.interactionDriver.interactableOverride == base.gameObject)
			{
				this.passengerInfo.interactionDriver.interactableOverride = null;
			}
			if (this.rigidbody && this.passengerInfo.characterMotor)
			{
				this.passengerInfo.characterMotor.velocity = this.rigidbody.velocity * this.exitVelocityFraction;
			}
			Action<GameObject> action = this.onPassengerExit;
			if (action != null)
			{
				action(this.passengerBodyObject);
			}
			UnityEvent unityEvent = this.onPassengerExitUnityEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			Action<VehicleSeat, GameObject> action2 = VehicleSeat.onPassengerExitGlobal;
			if (action2 == null)
			{
				return;
			}
			action2(this, this.passengerBodyObject);
		}

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x0600155B RID: 5467 RVA: 0x0005B2B0 File Offset: 0x000594B0
		// (remove) Token: 0x0600155C RID: 5468 RVA: 0x0005B2E8 File Offset: 0x000594E8
		public event Action<GameObject> onPassengerEnter;

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x0600155D RID: 5469 RVA: 0x0005B320 File Offset: 0x00059520
		// (remove) Token: 0x0600155E RID: 5470 RVA: 0x0005B358 File Offset: 0x00059558
		public event Action<GameObject> onPassengerExit;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x0600155F RID: 5471 RVA: 0x0005B390 File Offset: 0x00059590
		// (remove) Token: 0x06001560 RID: 5472 RVA: 0x0005B3C4 File Offset: 0x000595C4
		public static event Action<VehicleSeat, GameObject> onPassengerEnterGlobal;

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06001561 RID: 5473 RVA: 0x0005B3F8 File Offset: 0x000595F8
		// (remove) Token: 0x06001562 RID: 5474 RVA: 0x0005B42C File Offset: 0x0005962C
		public static event Action<VehicleSeat, GameObject> onPassengerExitGlobal;

		// Token: 0x06001565 RID: 5477 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06001566 RID: 5478 RVA: 0x0005B4E4 File Offset: 0x000596E4
		// (set) Token: 0x06001567 RID: 5479 RVA: 0x0005B4F8 File Offset: 0x000596F8
		public GameObject NetworkpassengerBodyObject
		{
			get
			{
				return this.passengerBodyObject;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetPassenger(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.passengerBodyObject, dirtyBit, ref this.___passengerBodyObjectNetId);
			}
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0005B548 File Offset: 0x00059748
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.passengerBodyObject);
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
				writer.Write(this.passengerBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x0005B5B4 File Offset: 0x000597B4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___passengerBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetPassenger(reader.ReadGameObject());
			}
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0005B5F5 File Offset: 0x000597F5
		public override void PreStartClient()
		{
			if (!this.___passengerBodyObjectNetId.IsEmpty())
			{
				this.NetworkpassengerBodyObject = ClientScene.FindLocalObject(this.___passengerBodyObjectNetId);
			}
		}

		// Token: 0x040013D0 RID: 5072
		public SerializableEntityStateType passengerState;

		// Token: 0x040013D1 RID: 5073
		public Transform seatPosition;

		// Token: 0x040013D2 RID: 5074
		public Transform exitPosition;

		// Token: 0x040013D3 RID: 5075
		public bool ejectOnCollision;

		// Token: 0x040013D4 RID: 5076
		public bool hidePassenger = true;

		// Token: 0x040013D5 RID: 5077
		public float exitVelocityFraction = 1f;

		// Token: 0x040013D6 RID: 5078
		public UnityEvent onPassengerEnterUnityEvent;

		// Token: 0x040013D7 RID: 5079
		[FormerlySerializedAs("OnPassengerExitUnityEvent")]
		public UnityEvent onPassengerExitUnityEvent;

		// Token: 0x040013D8 RID: 5080
		public string enterVehicleContextString;

		// Token: 0x040013D9 RID: 5081
		public string exitVehicleContextString;

		// Token: 0x040013DA RID: 5082
		public bool disablePassengerMotor;

		// Token: 0x040013DB RID: 5083
		[SyncVar(hook = "SetPassenger")]
		private GameObject passengerBodyObject;

		// Token: 0x040013DC RID: 5084
		private VehicleSeat.PassengerInfo passengerInfo;

		// Token: 0x040013DD RID: 5085
		private Rigidbody rigidbody;

		// Token: 0x040013DE RID: 5086
		private Collider collider;

		// Token: 0x040013DF RID: 5087
		public CallbackCheck<Interactability, CharacterBody> enterVehicleAllowedCheck = new CallbackCheck<Interactability, CharacterBody>();

		// Token: 0x040013E0 RID: 5088
		public CallbackCheck<Interactability, CharacterBody> exitVehicleAllowedCheck = new CallbackCheck<Interactability, CharacterBody>();

		// Token: 0x040013E1 RID: 5089
		public CallbackCheck<bool, GameObject> handleVehicleEnterRequestServer = new CallbackCheck<bool, GameObject>();

		// Token: 0x040013E2 RID: 5090
		public CallbackCheck<bool, GameObject> handleVehicleExitRequestServer = new CallbackCheck<bool, GameObject>();

		// Token: 0x040013E3 RID: 5091
		private static readonly BoolConVar cvVehicleSeatDebug = new BoolConVar("vehicle_seat_debug", ConVarFlags.None, "0", "Enables debug logging for VehicleSeat.");

		// Token: 0x040013E4 RID: 5092
		private Run.FixedTimeStamp passengerAssignmentTime = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x040013E5 RID: 5093
		private readonly float passengerAssignmentCooldown = 0.2f;

		// Token: 0x040013EA RID: 5098
		private NetworkInstanceId ___passengerBodyObjectNetId;

		// Token: 0x0200036C RID: 876
		private struct PassengerInfo
		{
			// Token: 0x0600156B RID: 5483 RVA: 0x0005B61C File Offset: 0x0005981C
			public PassengerInfo(GameObject passengerBodyObject)
			{
				this.transform = passengerBodyObject.transform;
				this.body = passengerBodyObject.GetComponent<CharacterBody>();
				this.inputBank = passengerBodyObject.GetComponent<InputBankTest>();
				this.interactionDriver = passengerBodyObject.GetComponent<InteractionDriver>();
				this.characterMotor = passengerBodyObject.GetComponent<CharacterMotor>();
				this.networkIdentity = passengerBodyObject.GetComponent<NetworkIdentity>();
				this.collider = passengerBodyObject.GetComponent<Collider>();
				this.bodyStateMachine = null;
				passengerBodyObject.GetComponents<EntityStateMachine>(VehicleSeat.PassengerInfo.sharedBuffer);
				for (int i = 0; i < VehicleSeat.PassengerInfo.sharedBuffer.Count; i++)
				{
					EntityStateMachine entityStateMachine = VehicleSeat.PassengerInfo.sharedBuffer[i];
					if (string.CompareOrdinal(entityStateMachine.customName, "Body") == 0)
					{
						this.bodyStateMachine = entityStateMachine;
						break;
					}
				}
				VehicleSeat.PassengerInfo.sharedBuffer.Clear();
				this.characterModel = null;
				if (this.body.modelLocator && this.body.modelLocator.modelTransform)
				{
					this.characterModel = this.body.modelLocator.modelTransform.GetComponent<CharacterModel>();
				}
			}

			// Token: 0x1700028C RID: 652
			// (get) Token: 0x0600156C RID: 5484 RVA: 0x0005B720 File Offset: 0x00059920
			public bool hasEffectiveAuthority
			{
				get
				{
					return Util.HasEffectiveAuthority(this.networkIdentity);
				}
			}

			// Token: 0x0600156D RID: 5485 RVA: 0x0005B730 File Offset: 0x00059930
			public string GetString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("transform=").Append(this.transform).AppendLine();
				stringBuilder.Append("body=").Append(this.body).AppendLine();
				stringBuilder.Append("inputBank=").Append(this.inputBank).AppendLine();
				stringBuilder.Append("interactionDriver=").Append(this.interactionDriver).AppendLine();
				stringBuilder.Append("characterMotor=").Append(this.characterMotor).AppendLine();
				stringBuilder.Append("bodyStateMachine=").Append(this.bodyStateMachine).AppendLine();
				stringBuilder.Append("characterModel=").Append(this.characterModel).AppendLine();
				stringBuilder.Append("networkIdentity=").Append(this.networkIdentity).AppendLine();
				stringBuilder.Append("hasEffectiveAuthority=").Append(this.hasEffectiveAuthority).AppendLine();
				return stringBuilder.ToString();
			}

			// Token: 0x040013EB RID: 5099
			private static readonly List<EntityStateMachine> sharedBuffer = new List<EntityStateMachine>();

			// Token: 0x040013EC RID: 5100
			public readonly Transform transform;

			// Token: 0x040013ED RID: 5101
			public readonly CharacterBody body;

			// Token: 0x040013EE RID: 5102
			public readonly InputBankTest inputBank;

			// Token: 0x040013EF RID: 5103
			public readonly InteractionDriver interactionDriver;

			// Token: 0x040013F0 RID: 5104
			public readonly CharacterMotor characterMotor;

			// Token: 0x040013F1 RID: 5105
			public readonly EntityStateMachine bodyStateMachine;

			// Token: 0x040013F2 RID: 5106
			public readonly CharacterModel characterModel;

			// Token: 0x040013F3 RID: 5107
			public readonly NetworkIdentity networkIdentity;

			// Token: 0x040013F4 RID: 5108
			public readonly Collider collider;
		}

		// Token: 0x0200036D RID: 877
		// (Invoke) Token: 0x06001570 RID: 5488
		public delegate void InteractabilityCheckDelegate(CharacterBody activator, ref Interactability? resultOverride);
	}
}
