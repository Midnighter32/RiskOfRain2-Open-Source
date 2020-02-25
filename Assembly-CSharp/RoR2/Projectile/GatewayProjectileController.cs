using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004F3 RID: 1267
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class GatewayProjectileController : NetworkBehaviour, IInteractable
	{
		// Token: 0x06001E20 RID: 7712 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		public string GetContextString(Interactor activator)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001E21 RID: 7713 RVA: 0x00081CF0 File Offset: 0x0007FEF0
		public Interactability GetInteractability(Interactor activator)
		{
			if (!this.linkedObject)
			{
				return Interactability.ConditionsNotMet;
			}
			return Interactability.Available;
		}

		// Token: 0x06001E22 RID: 7714 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		public void OnInteractionBegin(Interactor activator)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001E23 RID: 7715 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldShowOnScanner()
		{
			return false;
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x00081D04 File Offset: 0x0007FF04
		private void SetLinkedObject(GameObject newLinkedObject)
		{
			if (this.linkedObject != newLinkedObject)
			{
				this.NetworklinkedObject = newLinkedObject;
				this.linkedGatewayProjectileController = (this.linkedObject ? this.linkedObject.GetComponent<GatewayProjectileController>() : null);
				if (this.linkedGatewayProjectileController)
				{
					this.linkedGatewayProjectileController.SetLinkedObject(base.gameObject);
				}
			}
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x00081D65 File Offset: 0x0007FF65
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileTargetComponent = base.GetComponent<ProjectileTargetComponent>();
		}

		// Token: 0x06001E27 RID: 7719 RVA: 0x00081D80 File Offset: 0x0007FF80
		public override void OnStartServer()
		{
			base.OnStartServer();
			if (this.projectileTargetComponent.target)
			{
				this.SetLinkedObject(this.projectileTargetComponent.target.gameObject);
				return;
			}
			FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
			fireProjectileInfo.position = base.transform.position;
			fireProjectileInfo.rotation = base.transform.rotation;
			fireProjectileInfo.target = base.gameObject;
			fireProjectileInfo.owner = this.projectileController.owner;
			fireProjectileInfo.speedOverride = 0f;
			fireProjectileInfo.projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/GatewayProjectile");
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x06001E29 RID: 7721 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06001E2A RID: 7722 RVA: 0x00081E30 File Offset: 0x00080030
		// (set) Token: 0x06001E2B RID: 7723 RVA: 0x00081E44 File Offset: 0x00080044
		public GameObject NetworklinkedObject
		{
			get
			{
				return this.linkedObject;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetLinkedObject(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.linkedObject, dirtyBit, ref this.___linkedObjectNetId);
			}
		}

		// Token: 0x06001E2C RID: 7724 RVA: 0x00081E94 File Offset: 0x00080094
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.linkedObject);
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
				writer.Write(this.linkedObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E2D RID: 7725 RVA: 0x00081F00 File Offset: 0x00080100
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___linkedObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetLinkedObject(reader.ReadGameObject());
			}
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x00081F41 File Offset: 0x00080141
		public override void PreStartClient()
		{
			if (!this.___linkedObjectNetId.IsEmpty())
			{
				this.NetworklinkedObject = ClientScene.FindLocalObject(this.___linkedObjectNetId);
			}
		}

		// Token: 0x04001B55 RID: 6997
		private ProjectileController projectileController;

		// Token: 0x04001B56 RID: 6998
		private ProjectileTargetComponent projectileTargetComponent;

		// Token: 0x04001B57 RID: 6999
		[SyncVar(hook = "SetLinkedObject")]
		private GameObject linkedObject;

		// Token: 0x04001B58 RID: 7000
		private GatewayProjectileController linkedGatewayProjectileController;

		// Token: 0x04001B59 RID: 7001
		private NetworkInstanceId ___linkedObjectNetId;
	}
}
