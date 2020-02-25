using System;
using System.Runtime.InteropServices;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000159 RID: 345
	public sealed class BarrelInteraction : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x0600062E RID: 1582 RVA: 0x00019B4D File Offset: 0x00017D4D
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken);
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00019B66 File Offset: 0x00017D66
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.opened)
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00019B74 File Offset: 0x00017D74
		private void Start()
		{
			if (Run.instance)
			{
				this.goldReward = (int)((float)this.goldReward * Run.instance.difficultyCoefficient);
				this.expReward = (uint)(this.expReward * Run.instance.difficultyCoefficient);
			}
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00019BC0 File Offset: 0x00017DC0
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

		// Token: 0x06000633 RID: 1587 RVA: 0x00019C60 File Offset: 0x00017E60
		[Server]
		private void CoinDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BarrelInteraction::CoinDrop()' called on client");
				return;
			}
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter"), new EffectData
			{
				origin = base.transform.position,
				genericFloat = (float)this.goldReward
			}, true);
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00019CB5 File Offset: 0x00017EB5
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00019CC2 File Offset: 0x00017EC2
		public void OnEnable()
		{
			InstanceTracker.Add<BarrelInteraction>(this);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00019CCA File Offset: 0x00017ECA
		public void OnDisable()
		{
			InstanceTracker.Remove<BarrelInteraction>(this);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00019CD2 File Offset: 0x00017ED2
		public bool ShouldShowOnScanner()
		{
			return !this.opened;
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x00019CF0 File Offset: 0x00017EF0
		// (set) Token: 0x0600063C RID: 1596 RVA: 0x00019D03 File Offset: 0x00017F03
		public bool Networkopened
		{
			get
			{
				return this.opened;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.opened, 1U);
			}
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00019D18 File Offset: 0x00017F18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.opened);
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
				writer.Write(this.opened);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00019D84 File Offset: 0x00017F84
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

		// Token: 0x040006A3 RID: 1699
		public int goldReward;

		// Token: 0x040006A4 RID: 1700
		public uint expReward;

		// Token: 0x040006A5 RID: 1701
		public string displayNameToken = "BARREL1_NAME";

		// Token: 0x040006A6 RID: 1702
		public string contextToken;

		// Token: 0x040006A7 RID: 1703
		[SyncVar]
		private bool opened;
	}
}
