using System;
using System.Runtime.InteropServices;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002BE RID: 702
	public class PingerController : NetworkBehaviour
	{
		// Token: 0x06000FDC RID: 4060 RVA: 0x00045B7C File Offset: 0x00043D7C
		private void RebuildPing(PingerController.PingInfo pingInfo)
		{
			if (!pingInfo.active && this.pingIndicator != null)
			{
				if (this.pingIndicator)
				{
					UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
				}
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

		// Token: 0x06000FDD RID: 4061 RVA: 0x00045C3C File Offset: 0x00043E3C
		private void OnDestroy()
		{
			if (this.pingIndicator)
			{
				UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
			}
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x00045C5B File Offset: 0x00043E5B
		private void OnSyncCurrentPing(PingerController.PingInfo newPingInfo)
		{
			if (base.hasAuthority)
			{
				return;
			}
			this.SetCurrentPing(newPingInfo);
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x00045C6D File Offset: 0x00043E6D
		private void SetCurrentPing(PingerController.PingInfo newPingInfo)
		{
			this.NetworkcurrentPing = newPingInfo;
			this.RebuildPing(this.currentPing);
			if (base.hasAuthority)
			{
				this.CallCmdPing(this.currentPing);
			}
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x00045C96 File Offset: 0x00043E96
		[Command]
		private void CmdPing(PingerController.PingInfo incomingPing)
		{
			this.NetworkcurrentPing = incomingPing;
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00045CA0 File Offset: 0x00043EA0
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

		// Token: 0x06000FE2 RID: 4066 RVA: 0x00045CF4 File Offset: 0x00043EF4
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
			if (!RoR2Application.isInSinglePlayer)
			{
				this.pingStock--;
			}
			HurtBox hurtBox = null;
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(bodyObject, aimRay, out raycastHit, 1000f, LayerIndex.entityPrecise.mask | LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
			{
				hurtBox = raycastHit.collider.GetComponent<HurtBox>();
			}
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

		// Token: 0x06000FE4 RID: 4068 RVA: 0x00045EED File Offset: 0x000440ED
		static PingerController()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(PingerController), PingerController.kCmdCmdPing, new NetworkBehaviour.CmdDelegate(PingerController.InvokeCmdCmdPing));
			NetworkCRC.RegisterBehaviour("PingerController", 0);
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x00045F28 File Offset: 0x00044128
		// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x00045F3B File Offset: 0x0004413B
		public PingerController.PingInfo NetworkcurrentPing
		{
			get
			{
				return this.currentPing;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncCurrentPing(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PingerController.PingInfo>(value, ref this.currentPing, dirtyBit);
			}
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00045F7A File Offset: 0x0004417A
		protected static void InvokeCmdCmdPing(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdPing called on client.");
				return;
			}
			((PingerController)obj).CmdPing(GeneratedNetworkCode._ReadPingInfo_PingerController(reader));
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00045FA4 File Offset: 0x000441A4
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

		// Token: 0x06000FEA RID: 4074 RVA: 0x00046030 File Offset: 0x00044230
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePingInfo_PingerController(writer, this.currentPing);
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
				GeneratedNetworkCode._WritePingInfo_PingerController(writer, this.currentPing);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x0004609C File Offset: 0x0004429C
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

		// Token: 0x04000F5D RID: 3933
		private int pingStock = 3;

		// Token: 0x04000F5E RID: 3934
		private float pingRechargeStopwatch;

		// Token: 0x04000F5F RID: 3935
		private const int maximumPingStock = 3;

		// Token: 0x04000F60 RID: 3936
		private const float pingRechargeInterval = 1.5f;

		// Token: 0x04000F61 RID: 3937
		private static readonly PingerController.PingInfo emptyPing;

		// Token: 0x04000F62 RID: 3938
		private PingIndicator pingIndicator;

		// Token: 0x04000F63 RID: 3939
		[SyncVar(hook = "OnSyncCurrentPing")]
		public PingerController.PingInfo currentPing;

		// Token: 0x04000F64 RID: 3940
		private static int kCmdCmdPing = 1170265357;

		// Token: 0x020002BF RID: 703
		[Serializable]
		public struct PingInfo
		{
			// Token: 0x170001F6 RID: 502
			// (get) Token: 0x06000FEC RID: 4076 RVA: 0x000460DD File Offset: 0x000442DD
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

			// Token: 0x04000F65 RID: 3941
			public bool active;

			// Token: 0x04000F66 RID: 3942
			public Vector3 origin;

			// Token: 0x04000F67 RID: 3943
			public Vector3 normal;

			// Token: 0x04000F68 RID: 3944
			public NetworkIdentity targetNetworkIdentity;
		}
	}
}
