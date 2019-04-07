using System;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000263 RID: 611
	public class BarrelInteraction : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000B58 RID: 2904 RVA: 0x00037FA9 File Offset: 0x000361A9
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x00037FC2 File Offset: 0x000361C2
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.opened)
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x00037FD0 File Offset: 0x000361D0
		private void Start()
		{
			if (Run.instance)
			{
				this.goldReward = (int)((float)this.goldReward * Run.instance.difficultyCoefficient);
				this.expReward = (uint)(this.expReward * Run.instance.difficultyCoefficient);
			}
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0003801C File Offset: 0x0003621C
		[Server]
		public void OnInteractionBegin(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BarrelInteraction::OnInteractionBegin(RoR2.Interactor)' called on client");
				return;
			}
			if (!this.opened)
			{
				this.Networkopened = true;
				EntityStateMachine component = base.GetComponent<EntityStateMachine>();
				if (component)
				{
					component.SetNextState(new Opening());
				}
				CharacterBody component2 = activator.GetComponent<CharacterBody>();
				if (component2)
				{
					TeamIndex objectTeam = TeamComponent.GetObjectTeam(component2.gameObject);
					TeamManager.instance.GiveTeamMoney(objectTeam, (uint)this.goldReward);
				}
				this.CoinDrop();
				ExperienceManager.instance.AwardExperience(base.transform.position, activator.GetComponent<CharacterBody>(), (ulong)this.expReward);
			}
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x000380BC File Offset: 0x000362BC
		[Server]
		private void CoinDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BarrelInteraction::CoinDrop()' called on client");
				return;
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter"), new EffectData
			{
				origin = base.transform.position,
				genericFloat = (float)this.goldReward
			}, true);
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x00038116 File Offset: 0x00036316
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000B62 RID: 2914 RVA: 0x00038138 File Offset: 0x00036338
		// (set) Token: 0x06000B63 RID: 2915 RVA: 0x0003814B File Offset: 0x0003634B
		public bool Networkopened
		{
			get
			{
				return this.opened;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.opened, 1u);
			}
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00038160 File Offset: 0x00036360
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.opened);
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
				writer.Write(this.opened);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x000381CC File Offset: 0x000363CC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.opened = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.opened = reader.ReadBoolean();
			}
		}

		// Token: 0x04000F5D RID: 3933
		public int goldReward;

		// Token: 0x04000F5E RID: 3934
		public uint expReward;

		// Token: 0x04000F5F RID: 3935
		public string displayNameToken = "BARREL1_NAME";

		// Token: 0x04000F60 RID: 3936
		public string contextToken;

		// Token: 0x04000F61 RID: 3937
		[SyncVar]
		private bool opened;
	}
}
