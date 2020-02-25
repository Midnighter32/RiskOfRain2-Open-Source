using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000396 RID: 918
	public class HealingFollowerController : NetworkBehaviour
	{
		// Token: 0x0600164B RID: 5707 RVA: 0x0006002F File Offset: 0x0005E22F
		private void FixedUpdate()
		{
			if (this.cachedTargetBodyObject != this.targetBodyObject)
			{
				this.cachedTargetBodyObject = this.targetBodyObject;
				this.OnTargetChanged();
			}
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x00060064 File Offset: 0x0005E264
		[Server]
		public void AssignNewTarget(GameObject target)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealingFollowerController::AssignNewTarget(UnityEngine.GameObject)' called on client");
				return;
			}
			this.NetworktargetBodyObject = (target ? target : this.ownerBodyObject);
			this.cachedTargetBodyObject = this.targetBodyObject;
			this.cachedTargetHealthComponent = (this.cachedTargetBodyObject ? this.cachedTargetBodyObject.GetComponent<HealthComponent>() : null);
			this.OnTargetChanged();
			if (this.targetBodyObject.GetComponent<CharacterBody>())
			{
				EffectManager.SimpleImpactEffect(this.burstHealEffect, this.GetTargetPosition(), Vector3.up, true);
			}
			if (NetworkServer.active)
			{
				this.DoHeal(this.fractionHealthBurst);
			}
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0006010C File Offset: 0x0005E30C
		private void OnTargetChanged()
		{
			this.cachedTargetHealthComponent = (this.cachedTargetBodyObject ? this.cachedTargetBodyObject.GetComponent<HealthComponent>() : null);
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x00060130 File Offset: 0x0005E330
		private void FixedUpdateServer()
		{
			this.healingTimer -= Time.fixedDeltaTime;
			if (this.healingTimer <= 0f)
			{
				this.healingTimer = this.healingInterval;
				this.DoHeal(this.fractionHealthHealing * this.healingInterval);
			}
			if (!this.targetBodyObject)
			{
				this.NetworktargetBodyObject = this.ownerBodyObject;
			}
			if (!this.ownerBodyObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x000601AC File Offset: 0x0005E3AC
		private void Update()
		{
			this.UpdateMotion();
			base.transform.position += this.velocity * Time.deltaTime;
			base.transform.rotation = Quaternion.AngleAxis(this.rotationAngularVelocity * Time.deltaTime, Vector3.up) * base.transform.rotation;
			if (this.targetBodyObject)
			{
				this.indicator.transform.position = this.GetTargetPosition();
			}
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x0006023C File Offset: 0x0005E43C
		[Server]
		private void DoHeal(float healFraction)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealingFollowerController::DoHeal(System.Single)' called on client");
				return;
			}
			if (!this.cachedTargetHealthComponent)
			{
				return;
			}
			this.cachedTargetHealthComponent.HealFraction(healFraction, default(ProcChainMask));
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x00060282 File Offset: 0x0005E482
		public override void OnStartClient()
		{
			base.OnStartClient();
			base.transform.position = this.GetDesiredPosition();
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x0006029C File Offset: 0x0005E49C
		private Vector3 GetTargetPosition()
		{
			GameObject gameObject = this.targetBodyObject ?? this.ownerBodyObject;
			if (!gameObject)
			{
				return base.transform.position;
			}
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			if (!component)
			{
				return gameObject.transform.position;
			}
			return component.corePosition;
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x000602EF File Offset: 0x0005E4EF
		private Vector3 GetDesiredPosition()
		{
			return this.GetTargetPosition();
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x000602F8 File Offset: 0x0005E4F8
		private void UpdateMotion()
		{
			Vector3 desiredPosition = this.GetDesiredPosition();
			if (this.enableSpringMotion)
			{
				Vector3 lhs = desiredPosition - base.transform.position;
				if (lhs != Vector3.zero)
				{
					Vector3 a = lhs.normalized * this.acceleration;
					Vector3 b = this.velocity * -this.damping;
					this.velocity += (a + b) * Time.deltaTime;
					return;
				}
			}
			else
			{
				base.transform.position = Vector3.SmoothDamp(base.transform.position, desiredPosition, ref this.velocity, this.damping);
			}
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06001657 RID: 5719 RVA: 0x000603E4 File Offset: 0x0005E5E4
		// (set) Token: 0x06001658 RID: 5720 RVA: 0x000603F7 File Offset: 0x0005E5F7
		public GameObject NetworkownerBodyObject
		{
			get
			{
				return this.ownerBodyObject;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerBodyObject, 1U, ref this.___ownerBodyObjectNetId);
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06001659 RID: 5721 RVA: 0x00060414 File Offset: 0x0005E614
		// (set) Token: 0x0600165A RID: 5722 RVA: 0x00060427 File Offset: 0x0005E627
		public GameObject NetworktargetBodyObject
		{
			get
			{
				return this.targetBodyObject;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetBodyObject, 2U, ref this.___targetBodyObjectNetId);
			}
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x00060444 File Offset: 0x0005E644
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.ownerBodyObject);
				writer.Write(this.targetBodyObject);
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
				writer.Write(this.ownerBodyObject);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.targetBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x000604F0 File Offset: 0x0005E6F0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___ownerBodyObjectNetId = reader.ReadNetworkId();
				this.___targetBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.ownerBodyObject = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.targetBodyObject = reader.ReadGameObject();
			}
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x00060558 File Offset: 0x0005E758
		public override void PreStartClient()
		{
			if (!this.___ownerBodyObjectNetId.IsEmpty())
			{
				this.NetworkownerBodyObject = ClientScene.FindLocalObject(this.___ownerBodyObjectNetId);
			}
			if (!this.___targetBodyObjectNetId.IsEmpty())
			{
				this.NetworktargetBodyObject = ClientScene.FindLocalObject(this.___targetBodyObjectNetId);
			}
		}

		// Token: 0x040014F9 RID: 5369
		public float fractionHealthHealing = 0.01f;

		// Token: 0x040014FA RID: 5370
		public float fractionHealthBurst = 0.05f;

		// Token: 0x040014FB RID: 5371
		public float healingInterval = 1f;

		// Token: 0x040014FC RID: 5372
		public float rotationAngularVelocity;

		// Token: 0x040014FD RID: 5373
		public float acceleration = 20f;

		// Token: 0x040014FE RID: 5374
		public float damping = 0.1f;

		// Token: 0x040014FF RID: 5375
		public bool enableSpringMotion;

		// Token: 0x04001500 RID: 5376
		[SyncVar]
		public GameObject ownerBodyObject;

		// Token: 0x04001501 RID: 5377
		[SyncVar]
		public GameObject targetBodyObject;

		// Token: 0x04001502 RID: 5378
		public GameObject burstHealEffect;

		// Token: 0x04001503 RID: 5379
		public GameObject indicator;

		// Token: 0x04001504 RID: 5380
		private GameObject cachedTargetBodyObject;

		// Token: 0x04001505 RID: 5381
		private HealthComponent cachedTargetHealthComponent;

		// Token: 0x04001506 RID: 5382
		private float healingTimer;

		// Token: 0x04001507 RID: 5383
		private Vector3 velocity;

		// Token: 0x04001508 RID: 5384
		private NetworkInstanceId ___ownerBodyObjectNetId;

		// Token: 0x04001509 RID: 5385
		private NetworkInstanceId ___targetBodyObjectNetId;
	}
}
