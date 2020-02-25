using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x02000497 RID: 1175
	[RequireComponent(typeof(CharacterMaster))]
	[RequireComponent(typeof(PlayerCharacterMasterController))]
	public class PlayerStatsComponent : NetworkBehaviour
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06001C75 RID: 7285 RVA: 0x00079A7C File Offset: 0x00077C7C
		// (set) Token: 0x06001C76 RID: 7286 RVA: 0x00079A84 File Offset: 0x00077C84
		public CharacterMaster characterMaster { get; private set; }

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06001C77 RID: 7287 RVA: 0x00079A8D File Offset: 0x00077C8D
		// (set) Token: 0x06001C78 RID: 7288 RVA: 0x00079A95 File Offset: 0x00077C95
		public PlayerCharacterMasterController playerCharacterMasterController { get; private set; }

		// Token: 0x06001C79 RID: 7289 RVA: 0x00079AA0 File Offset: 0x00077CA0
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

		// Token: 0x06001C7A RID: 7290 RVA: 0x00079AF8 File Offset: 0x00077CF8
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				this.SendUpdateToClient();
			}
			PlayerStatsComponent.instancesList.Remove(this);
		}

		// Token: 0x06001C7B RID: 7291 RVA: 0x00079B13 File Offset: 0x00077D13
		public static StatSheet FindBodyStatSheet(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatSheet(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x00079B2A File Offset: 0x00077D2A
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

		// Token: 0x06001C7D RID: 7293 RVA: 0x00079B4D File Offset: 0x00077D4D
		public static StatSheet FindMasterStatSheet(CharacterMaster master)
		{
			PlayerStatsComponent playerStatsComponent = PlayerStatsComponent.FindMasterStatsComponent(master);
			if (playerStatsComponent == null)
			{
				return null;
			}
			return playerStatsComponent.currentStats;
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x00079B60 File Offset: 0x00077D60
		public static PlayerStatsComponent FindBodyStatsComponent(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatsComponent(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x00079B77 File Offset: 0x00077D77
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

		// Token: 0x06001C80 RID: 7296 RVA: 0x00079B8F File Offset: 0x00077D8F
		public static PlayerStatsComponent FindMasterStatsComponent(CharacterMaster master)
		{
			if (master == null)
			{
				return null;
			}
			return master.playerStatsComponent;
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x00079B9C File Offset: 0x00077D9C
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

		// Token: 0x06001C82 RID: 7298 RVA: 0x00079BC2 File Offset: 0x00077DC2
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.ServerFixedUpdate();
			}
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x00079BD1 File Offset: 0x00077DD1
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

		// Token: 0x06001C84 RID: 7300 RVA: 0x00079BF4 File Offset: 0x00077DF4
		[Server]
		private void ServerFixedUpdate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::ServerFixedUpdate()' called on client");
				return;
			}
			float num = 0f;
			float runTime = 0f;
			if (Run.instance && !Run.instance.isRunStopwatchPaused)
			{
				num = Time.fixedDeltaTime;
				runTime = Run.instance.GetRunStopwatch();
			}
			StatManager.CharacterUpdateEvent e = default(StatManager.CharacterUpdateEvent);
			e.statsComponent = this;
			e.runTime = runTime;
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
				e.additionalTimeAlive += num;
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

		// Token: 0x06001C85 RID: 7301 RVA: 0x00079D88 File Offset: 0x00077F88
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

		// Token: 0x06001C86 RID: 7302 RVA: 0x00079E00 File Offset: 0x00078000
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

		// Token: 0x06001C87 RID: 7303 RVA: 0x00079E3C File Offset: 0x0007803C
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

		// Token: 0x06001C88 RID: 7304 RVA: 0x00079E6C File Offset: 0x0007806C
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

		// Token: 0x06001C89 RID: 7305 RVA: 0x00079EE8 File Offset: 0x000780E8
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

		// Token: 0x06001C8C RID: 7308 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x00079F9C File Offset: 0x0007819C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001976 RID: 6518
		public static readonly List<PlayerStatsComponent> instancesList = new List<PlayerStatsComponent>();

		// Token: 0x04001979 RID: 6521
		private float serverTransmitTimer;

		// Token: 0x0400197A RID: 6522
		private float serverTransmitInterval = 10f;

		// Token: 0x0400197B RID: 6523
		private Vector3 previousBodyPosition;

		// Token: 0x0400197C RID: 6524
		private GameObject cachedBodyObject;

		// Token: 0x0400197D RID: 6525
		private CharacterBody cachedCharacterBody;

		// Token: 0x0400197E RID: 6526
		private CharacterMotor cachedBodyCharacterMotor;

		// Token: 0x0400197F RID: 6527
		private Transform cachedBodyTransform;

		// Token: 0x04001980 RID: 6528
		public StatSheet currentStats;

		// Token: 0x04001981 RID: 6529
		private StatSheet clientDeltaStatsBuffer;

		// Token: 0x04001982 RID: 6530
		private StatSheet recordedStats;
	}
}
