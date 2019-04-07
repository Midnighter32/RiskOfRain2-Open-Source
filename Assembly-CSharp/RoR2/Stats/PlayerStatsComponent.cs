using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004F2 RID: 1266
	[RequireComponent(typeof(CharacterMaster))]
	[RequireComponent(typeof(PlayerCharacterMasterController))]
	public class PlayerStatsComponent : NetworkBehaviour
	{
		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06001C97 RID: 7319 RVA: 0x00085574 File Offset: 0x00083774
		// (set) Token: 0x06001C98 RID: 7320 RVA: 0x0008557C File Offset: 0x0008377C
		public CharacterMaster characterMaster { get; private set; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06001C99 RID: 7321 RVA: 0x00085585 File Offset: 0x00083785
		// (set) Token: 0x06001C9A RID: 7322 RVA: 0x0008558D File Offset: 0x0008378D
		public PlayerCharacterMasterController playerCharacterMasterController { get; private set; }

		// Token: 0x06001C9B RID: 7323 RVA: 0x00085598 File Offset: 0x00083798
		private void Awake()
		{
			this.playerCharacterMasterController = base.GetComponent<PlayerCharacterMasterController>();
			this.characterMaster = base.GetComponent<CharacterMaster>();
			PlayerStatsComponent.instancesList.Add(this);
			this.currentStats = StatSheet.New();
			if (NetworkClient.active)
			{
				this.recordedStats = StatSheet.New();
				this.clientDeltaStatsBuffer = StatSheet.New();
			}
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x000855F0 File Offset: 0x000837F0
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				this.SendUpdateToClient();
			}
			PlayerStatsComponent.instancesList.Remove(this);
		}

		// Token: 0x06001C9D RID: 7325 RVA: 0x0008560B File Offset: 0x0008380B
		public static StatSheet FindBodyStatSheet(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatSheet(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x00085622 File Offset: 0x00083822
		public static StatSheet FindBodyStatSheet(CharacterBody characterBody)
		{
			if (characterBody == null)
			{
				return null;
			}
			CharacterMaster master = characterBody.master;
			if (master == null)
			{
				return null;
			}
			PlayerStatsComponent component = master.GetComponent<PlayerStatsComponent>();
			if (component == null)
			{
				return null;
			}
			return component.currentStats;
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x00085645 File Offset: 0x00083845
		public static PlayerStatsComponent FindBodyStatsComponent(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatsComponent(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x0008565C File Offset: 0x0008385C
		public static PlayerStatsComponent FindBodyStatsComponent(CharacterBody characterBody)
		{
			if (characterBody == null)
			{
				return null;
			}
			CharacterMaster master = characterBody.master;
			if (master == null)
			{
				return null;
			}
			return master.GetComponent<PlayerStatsComponent>();
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x00085674 File Offset: 0x00083874
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				if (NetworkServer.active)
				{
					PlayerStatsComponent playerStatsComponent = PlayerStatsComponent.FindBodyStatsComponent(damageReport.victim.gameObject);
					if (playerStatsComponent)
					{
						playerStatsComponent.serverTransmitTimer = 0f;
					}
				}
			};
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x0008569A File Offset: 0x0008389A
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x000856A9 File Offset: 0x000838A9
		[Server]
		public void ForceNextTransmit()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::ForceNextTransmit()' called on client");
				return;
			}
			this.serverTransmitTimer = 0f;
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x000856CC File Offset: 0x000838CC
		[Server]
		private void ServerUpdate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::ServerUpdate()' called on client");
				return;
			}
			StatManager.CharacterUpdateEvent e = default(StatManager.CharacterUpdateEvent);
			e.statsComponent = this;
			e.runTime = Run.FixedTimeStamp.now.t;
			GameObject bodyObject = this.characterMaster.GetBodyObject();
			if (bodyObject != this.cachedBodyObject)
			{
				this.cachedBodyObject = bodyObject;
				this.cachedBodyObject = bodyObject;
				this.cachedBodyTransform = ((bodyObject != null) ? bodyObject.transform : null);
				if (this.cachedBodyTransform)
				{
					this.previousBodyPosition = this.cachedBodyTransform.position;
				}
				this.cachedCharacterBody = ((bodyObject != null) ? bodyObject.GetComponent<CharacterBody>() : null);
				this.cachedBodyCharacterMotor = ((bodyObject != null) ? bodyObject.GetComponent<CharacterMotor>() : null);
			}
			if (this.cachedBodyTransform)
			{
				Vector3 position = this.cachedBodyTransform.position;
				e.additionalDistanceTraveled = Vector3.Distance(position, this.previousBodyPosition);
				this.previousBodyPosition = position;
			}
			if (this.characterMaster.alive)
			{
				e.additionalTimeAlive += Time.deltaTime;
			}
			if (this.cachedCharacterBody)
			{
				e.level = (int)this.cachedCharacterBody.level;
			}
			StatManager.PushCharacterUpdateEvent(e);
			this.serverTransmitTimer -= Time.fixedDeltaTime;
			if (this.serverTransmitTimer <= 0f)
			{
				this.serverTransmitTimer = this.serverTransmitInterval;
				this.SendUpdateToClient();
			}
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x00085834 File Offset: 0x00083A34
		[Server]
		private void SendUpdateToClient()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::SendUpdateToClient()' called on client");
				return;
			}
			NetworkUser networkUser = this.playerCharacterMasterController.networkUser;
			if (networkUser)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(58);
				networkWriter.Write(base.gameObject);
				this.currentStats.Write(networkWriter);
				networkWriter.FinishMessage();
				networkUser.connectionToClient.SendWriter(networkWriter, this.GetNetworkChannel());
			}
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x000858AC File Offset: 0x00083AAC
		[NetworkMessageHandler(client = true, msgType = 58)]
		private static void HandleStatsUpdate(NetworkMessage netMsg)
		{
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (gameObject)
			{
				PlayerStatsComponent component = gameObject.GetComponent<PlayerStatsComponent>();
				if (component)
				{
					component.InstanceHandleStatsUpdate(netMsg.reader);
				}
			}
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x000858E8 File Offset: 0x00083AE8
		[Client]
		private void InstanceHandleStatsUpdate(NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Stats.PlayerStatsComponent::InstanceHandleStatsUpdate(UnityEngine.Networking.NetworkReader)' called on server");
				return;
			}
			if (!NetworkServer.active)
			{
				this.currentStats.Read(reader);
			}
			this.FlushStatsToUserProfile();
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x00085918 File Offset: 0x00083B18
		[Client]
		private void FlushStatsToUserProfile()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Stats.PlayerStatsComponent::FlushStatsToUserProfile()' called on server");
				return;
			}
			StatSheet.GetDelta(this.clientDeltaStatsBuffer, this.currentStats, this.recordedStats);
			StatSheet.Copy(this.currentStats, this.recordedStats);
			NetworkUser networkUser = this.playerCharacterMasterController.networkUser;
			LocalUser localUser = (networkUser != null) ? networkUser.localUser : null;
			if (localUser == null)
			{
				return;
			}
			UserProfile userProfile = localUser.userProfile;
			if (userProfile == null)
			{
				return;
			}
			userProfile.ApplyDeltaStatSheet(this.clientDeltaStatsBuffer);
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x00085994 File Offset: 0x00083B94
		[ConCommand(commandName = "print_stats", flags = ConVarFlags.None, helpText = "Prints all current stats of the sender.")]
		private static void CCPrintStats(ConCommandArgs args)
		{
			GameObject senderMasterObject = args.senderMasterObject;
			StatSheet statSheet;
			if (senderMasterObject == null)
			{
				statSheet = null;
			}
			else
			{
				PlayerStatsComponent component = senderMasterObject.GetComponent<PlayerStatsComponent>();
				statSheet = ((component != null) ? component.currentStats : null);
			}
			StatSheet statSheet2 = statSheet;
			if (statSheet2 == null)
			{
				return;
			}
			string[] array = new string[statSheet2.fields.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = string.Format("[\"{0}\"]={1}", statSheet2.fields[i].name, statSheet2.fields[i].ToString());
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x00085A48 File Offset: 0x00083C48
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001EB0 RID: 7856
		public static readonly List<PlayerStatsComponent> instancesList = new List<PlayerStatsComponent>();

		// Token: 0x04001EB3 RID: 7859
		private float serverTransmitTimer;

		// Token: 0x04001EB4 RID: 7860
		private float serverTransmitInterval = 10f;

		// Token: 0x04001EB5 RID: 7861
		private Vector3 previousBodyPosition;

		// Token: 0x04001EB6 RID: 7862
		private GameObject cachedBodyObject;

		// Token: 0x04001EB7 RID: 7863
		private CharacterBody cachedCharacterBody;

		// Token: 0x04001EB8 RID: 7864
		private CharacterMotor cachedBodyCharacterMotor;

		// Token: 0x04001EB9 RID: 7865
		private Transform cachedBodyTransform;

		// Token: 0x04001EBA RID: 7866
		public StatSheet currentStats;

		// Token: 0x04001EBB RID: 7867
		private StatSheet clientDeltaStatsBuffer;

		// Token: 0x04001EBC RID: 7868
		private StatSheet recordedStats;
	}
}
