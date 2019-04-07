using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FC RID: 764
	public class GenericInteraction : NetworkBehaviour, IInteractable
	{
		// Token: 0x06000F73 RID: 3955 RVA: 0x0004C2A3 File Offset: 0x0004A4A3
		[Server]
		public void SetInteractabilityAvailable()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericInteraction::SetInteractabilityAvailable()' called on client");
				return;
			}
			this.Networkinteractability = Interactability.Available;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0004C2C1 File Offset: 0x0004A4C1
		[Server]
		public void SetInteractabilityConditionsNotMet()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericInteraction::SetInteractabilityConditionsNotMet()' called on client");
				return;
			}
			this.Networkinteractability = Interactability.ConditionsNotMet;
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x0004C2DF File Offset: 0x0004A4DF
		[Server]
		public void SetInteractabilityDisabled()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericInteraction::SetInteractabilityDisabled()' called on client");
				return;
			}
			this.Networkinteractability = Interactability.Disabled;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0004C2FD File Offset: 0x0004A4FD
		string IInteractable.GetContextString(Interactor activator)
		{
			if (this.contextToken == "")
			{
				return null;
			}
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x0004C31E File Offset: 0x0004A51E
		Interactability IInteractable.GetInteractability(Interactor activator)
		{
			return this.interactability;
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x0004C326 File Offset: 0x0004A526
		void IInteractable.OnInteractionBegin(Interactor activator)
		{
			this.onActivation.Invoke();
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000F7C RID: 3964 RVA: 0x0004C344 File Offset: 0x0004A544
		// (set) Token: 0x06000F7D RID: 3965 RVA: 0x0004C357 File Offset: 0x0004A557
		public Interactability Networkinteractability
		{
			get
			{
				return this.interactability;
			}
			set
			{
				base.SetSyncVar<Interactability>(value, ref this.interactability, 1u);
			}
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0004C36C File Offset: 0x0004A56C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.interactability);
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
				writer.Write((int)this.interactability);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0004C3D8 File Offset: 0x0004A5D8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.interactability = (Interactability)reader.ReadInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.interactability = (Interactability)reader.ReadInt32();
			}
		}

		// Token: 0x04001392 RID: 5010
		[SyncVar]
		public Interactability interactability = Interactability.Available;

		// Token: 0x04001393 RID: 5011
		public string contextToken;

		// Token: 0x04001394 RID: 5012
		public UnityEvent onActivation;
	}
}
