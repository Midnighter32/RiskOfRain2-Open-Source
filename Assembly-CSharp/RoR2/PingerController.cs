using System;
using System.Linq;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000386 RID: 902
	public class PingerController : NetworkBehaviour
	{
		// Token: 0x060012D0 RID: 4816 RVA: 0x0005C474 File Offset: 0x0005A674
		private void RebuildPing(PingerController.PingInfo pingInfo)
		{
			if (!pingInfo.active && this.pingIndicator != null)
			{
				UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
				this.pingIndicator = null;
				return;
			}
			if (!this.pingIndicator)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/PingIndicator"));
				this.pingIndicator = gameObject.GetComponent<PingIndicator>();
				this.pingIndicator.pingOwner = base.gameObject;
			}
			this.pingIndicator.pingOrigin = pingInfo.origin;
			this.pingIndicator.pingNormal = pingInfo.normal;
			this.pingIndicator.pingTarget = pingInfo.targetGameObject;
			this.pingIndicator.RebuildPing();
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x0005C527 File Offset: 0x0005A727
		private void OnDestroy()
		{
			if (this.pingIndicator)
			{
				UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
			}
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x0005C546 File Offset: 0x0005A746
		private void OnSyncCurrentPing(PingerController.PingInfo newPingInfo)
		{
			if (base.hasAuthority)
			{
				return;
			}
			this.SetCurrentPing(newPingInfo);
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0005C558 File Offset: 0x0005A758
		private void SetCurrentPing(PingerController.PingInfo newPingInfo)
		{
			this.NetworkcurrentPing = newPingInfo;
			this.RebuildPing(this.currentPing);
			if (base.hasAuthority)
			{
				this.CallCmdPing(this.currentPing);
			}
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x0005C581 File Offset: 0x0005A781
		[Command]
		private void CmdPing(PingerController.PingInfo incomingPing)
		{
			this.NetworkcurrentPing = incomingPing;
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x0005C58C File Offset: 0x0005A78C
		private void FixedUpdate()
		{
			if (base.hasAuthority)
			{
				this.pingRechargeStopwatch -= Time.fixedDeltaTime;
				if (this.pingRechargeStopwatch <= 0f)
				{
					this.pingStock = Mathf.Min(this.pingStock + 1, 3);
					this.pingRechargeStopwatch = 1.5f;
				}
			}
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0005C5E0 File Offset: 0x0005A7E0
		public void AttemptPing(Ray aimRay, GameObject bodyObject)
		{
			if (this.pingStock <= 0)
			{
				Chat.AddMessage(Language.GetString("PLAYER_PING_COOLDOWN"));
				return;
			}
			PingerController.PingInfo pingInfo = new PingerController.PingInfo
			{
				active = true
			};
			this.pingStock--;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.maxDistanceFilter = 1000f;
			bullseyeSearch.maxAngleFilter = 10f;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(bodyObject);
			HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			RaycastHit raycastHit;
			if (hurtBox && hurtBox.healthComponent)
			{
				Transform transform = hurtBox.healthComponent.transform;
				pingInfo.origin = transform.position;
				pingInfo.targetNetworkIdentity = hurtBox.healthComponent.GetComponent<NetworkIdentity>();
			}
			else if (Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.Collide))
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				NetworkIdentity networkIdentity = gameObject.GetComponentInParent<NetworkIdentity>();
				if (!networkIdentity)
				{
					EntityLocator entityLocator = gameObject.transform.parent ? gameObject.transform.parent.GetComponentInChildren<EntityLocator>() : gameObject.GetComponent<EntityLocator>();
					if (entityLocator)
					{
						gameObject = entityLocator.entity;
						networkIdentity = gameObject.GetComponent<NetworkIdentity>();
					}
				}
				pingInfo.origin = raycastHit.point;
				pingInfo.normal = raycastHit.normal;
				pingInfo.targetNetworkIdentity = networkIdentity;
			}
			if (pingInfo.targetNetworkIdentity != null && pingInfo.targetNetworkIdentity == this.currentPing.targetNetworkIdentity)
			{
				pingInfo = PingerController.emptyPing;
				this.pingStock++;
			}
			this.SetCurrentPing(pingInfo);
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0005C7EA File Offset: 0x0005A9EA
		static PingerController()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(PingerController), PingerController.kCmdCmdPing, new NetworkBehaviour.CmdDelegate(PingerController.InvokeCmdCmdPing));
			NetworkCRC.RegisterBehaviour("PingerController", 0);
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060012DA RID: 4826 RVA: 0x0005C828 File Offset: 0x0005AA28
		// (set) Token: 0x060012DB RID: 4827 RVA: 0x0005C83B File Offset: 0x0005AA3B
		public PingerController.PingInfo NetworkcurrentPing
		{
			get
			{
				return this.currentPing;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncCurrentPing(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PingerController.PingInfo>(value, ref this.currentPing, dirtyBit);
			}
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0005C87A File Offset: 0x0005AA7A
		protected static void InvokeCmdCmdPing(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdPing called on client.");
				return;
			}
			((PingerController)obj).CmdPing(GeneratedNetworkCode._ReadPingInfo_PingerController(reader));
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0005C8A4 File Offset: 0x0005AAA4
		public void CallCmdPing(PingerController.PingInfo incomingPing)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdPing called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdPing(incomingPing);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)PingerController.kCmdCmdPing);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WritePingInfo_PingerController(networkWriter, incomingPing);
			base.SendCommandInternal(networkWriter, 0, "CmdPing");
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0005C930 File Offset: 0x0005AB30
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePingInfo_PingerController(writer, this.currentPing);
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
				GeneratedNetworkCode._WritePingInfo_PingerController(writer, this.currentPing);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0005C99C File Offset: 0x0005AB9C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.currentPing = GeneratedNetworkCode._ReadPingInfo_PingerController(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncCurrentPing(GeneratedNetworkCode._ReadPingInfo_PingerController(reader));
			}
		}

		// Token: 0x040016A0 RID: 5792
		private int pingStock = 3;

		// Token: 0x040016A1 RID: 5793
		private float pingRechargeStopwatch;

		// Token: 0x040016A2 RID: 5794
		private const int maximumPingStock = 3;

		// Token: 0x040016A3 RID: 5795
		private const float pingRechargeInterval = 1.5f;

		// Token: 0x040016A4 RID: 5796
		private static readonly PingerController.PingInfo emptyPing;

		// Token: 0x040016A5 RID: 5797
		private PingIndicator pingIndicator;

		// Token: 0x040016A6 RID: 5798
		[SyncVar(hook = "OnSyncCurrentPing")]
		public PingerController.PingInfo currentPing;

		// Token: 0x040016A7 RID: 5799
		private static int kCmdCmdPing = 1170265357;

		// Token: 0x02000387 RID: 903
		[Serializable]
		public struct PingInfo
		{
			// Token: 0x170001A4 RID: 420
			// (get) Token: 0x060012E0 RID: 4832 RVA: 0x0005C9DD File Offset: 0x0005ABDD
			public GameObject targetGameObject
			{
				get
				{
					if (!this.targetNetworkIdentity)
					{
						return null;
					}
					return this.targetNetworkIdentity.gameObject;
				}
			}

			// Token: 0x040016A8 RID: 5800
			public bool active;

			// Token: 0x040016A9 RID: 5801
			public Vector3 origin;

			// Token: 0x040016AA RID: 5802
			public Vector3 normal;

			// Token: 0x040016AB RID: 5803
			public NetworkIdentity targetNetworkIdentity;
		}
	}
}
