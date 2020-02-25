using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200020B RID: 523
	public sealed class GenericInteraction : NetworkBehaviour, IInteractable
	{
		// Token: 0x06000B2B RID: 2859 RVA: 0x000316FF File Offset: 0x0002F8FF
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

		// Token: 0x06000B2C RID: 2860 RVA: 0x0003171D File Offset: 0x0002F91D
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

		// Token: 0x06000B2D RID: 2861 RVA: 0x0003173B File Offset: 0x0002F93B
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

		// Token: 0x06000B2E RID: 2862 RVA: 0x00031759 File Offset: 0x0002F959
		string IInteractable.GetContextString(Interactor activator)
		{
			if (this.contextToken == "")
			{
				return null;
			}
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0003177A File Offset: 0x0002F97A
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return this.shouldIgnoreSpherecastForInteractibility;
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x00031782 File Offset: 0x0002F982
		Interactability IInteractable.GetInteractability(Interactor activator)
		{
			return this.interactability;
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0003178A File Offset: 0x0002F98A
		void IInteractable.OnInteractionBegin(Interactor activator)
		{
			this.onActivation.Invoke(activator);
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x00031798 File Offset: 0x0002F998
		private void OnEnable()
		{
			InstanceTracker.Add<GenericInteraction>(this);
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x000317A0 File Offset: 0x0002F9A0
		private void OnDisable()
		{
			InstanceTracker.Remove<GenericInteraction>(this);
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x000317A8 File Offset: 0x0002F9A8
		public bool ShouldShowOnScanner()
		{
			return this.shouldShowOnScanner && this.interactability > Interactability.Disabled;
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000B37 RID: 2871 RVA: 0x000317D4 File Offset: 0x0002F9D4
		// (set) Token: 0x06000B38 RID: 2872 RVA: 0x000317E7 File Offset: 0x0002F9E7
		public Interactability Networkinteractability
		{
			get
			{
				return this.interactability;
			}
			[param: In]
			set
			{
				base.SetSyncVar<Interactability>(value, ref this.interactability, 1U);
			}
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x000317FC File Offset: 0x0002F9FC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.interactability);
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
				writer.Write((int)this.interactability);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x00031868 File Offset: 0x0002FA68
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

		// Token: 0x04000B92 RID: 2962
		[SyncVar]
		public Interactability interactability = Interactability.Available;

		// Token: 0x04000B93 RID: 2963
		public bool shouldIgnoreSpherecastForInteractibility;

		// Token: 0x04000B94 RID: 2964
		public string contextToken;

		// Token: 0x04000B95 RID: 2965
		public GenericInteraction.InteractorUnityEvent onActivation;

		// Token: 0x04000B96 RID: 2966
		public bool shouldShowOnScanner = true;

		// Token: 0x0200020C RID: 524
		[Serializable]
		public class InteractorUnityEvent : UnityEvent<Interactor>
		{
		}
	}
}
