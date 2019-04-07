using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000436 RID: 1078
	public class HealingFollowerController : NetworkBehaviour
	{
		// Token: 0x060017F4 RID: 6132 RVA: 0x00072703 File Offset: 0x00070903
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

		// Token: 0x060017F5 RID: 6133 RVA: 0x00072738 File Offset: 0x00070938
		public void AssignNewTarget(GameObject target)
		{
			this.NetworktargetBodyObject = (target ? target : this.ownerBodyObject);
			this.cachedTargetBodyObject = this.targetBodyObject;
			this.OnTargetChanged();
			CharacterBody component = this.targetBodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				EffectManager.instance.SimpleImpactEffect(this.burstHealEffect, component.mainHurtBox.transform.position, Vector3.up, true);
			}
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x000727A8 File Offset: 0x000709A8
		private void OnTargetChanged()
		{
			this.cachedTargetHealthComponent = (this.cachedTargetBodyObject ? this.cachedTargetBodyObject.GetComponent<HealthComponent>() : null);
			if (NetworkServer.active)
			{
				this.DoHeal(this.fractionHealthBurst);
			}
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x000727E0 File Offset: 0x000709E0
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

		// Token: 0x060017F8 RID: 6136 RVA: 0x0007285C File Offset: 0x00070A5C
		private void Update()
		{
			this.UpdateMotion();
			base.transform.position += this.velocity * Time.deltaTime;
			base.transform.rotation = Quaternion.AngleAxis(this.rotationAngularVelocity * Time.deltaTime, Vector3.up) * base.transform.rotation;
			if (this.targetBodyObject)
			{
				this.indicator.transform.position = this.targetBodyObject.transform.position;
			}
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000728F4 File Offset: 0x00070AF4
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

		// Token: 0x060017FA RID: 6138 RVA: 0x0007293A File Offset: 0x00070B3A
		public override void OnStartClient()
		{
			base.OnStartClient();
			base.transform.position = this.GetDesiredPosition();
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x00072954 File Offset: 0x00070B54
		private Vector3 GetDesiredPosition()
		{
			GameObject gameObject = this.targetBodyObject ?? this.ownerBodyObject;
			if (!gameObject)
			{
				return base.transform.position + UnityEngine.Random.onUnitSphere;
			}
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			if (!component)
			{
				return gameObject.transform.position;
			}
			return component.corePosition;
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x000729B4 File Offset: 0x00070BB4
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

		// Token: 0x060017FE RID: 6142 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060017FF RID: 6143 RVA: 0x00072AA0 File Offset: 0x00070CA0
		// (set) Token: 0x06001800 RID: 6144 RVA: 0x00072AB3 File Offset: 0x00070CB3
		public GameObject NetworkownerBodyObject
		{
			get
			{
				return this.ownerBodyObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerBodyObject, 1u, ref this.___ownerBodyObjectNetId);
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06001801 RID: 6145 RVA: 0x00072AD0 File Offset: 0x00070CD0
		// (set) Token: 0x06001802 RID: 6146 RVA: 0x00072AE3 File Offset: 0x00070CE3
		public GameObject NetworktargetBodyObject
		{
			get
			{
				return this.targetBodyObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetBodyObject, 2u, ref this.___targetBodyObjectNetId);
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00072B00 File Offset: 0x00070D00
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.ownerBodyObject);
				writer.Write(this.targetBodyObject);
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
				writer.Write(this.ownerBodyObject);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
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

		// Token: 0x06001804 RID: 6148 RVA: 0x00072BAC File Offset: 0x00070DAC
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

		// Token: 0x06001805 RID: 6149 RVA: 0x00072C14 File Offset: 0x00070E14
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

		// Token: 0x04001B53 RID: 6995
		public float fractionHealthHealing = 0.01f;

		// Token: 0x04001B54 RID: 6996
		public float fractionHealthBurst = 0.05f;

		// Token: 0x04001B55 RID: 6997
		public float healingInterval = 1f;

		// Token: 0x04001B56 RID: 6998
		public float rotationAngularVelocity;

		// Token: 0x04001B57 RID: 6999
		public float acceleration = 20f;

		// Token: 0x04001B58 RID: 7000
		public float damping = 0.1f;

		// Token: 0x04001B59 RID: 7001
		public bool enableSpringMotion;

		// Token: 0x04001B5A RID: 7002
		[SyncVar]
		public GameObject ownerBodyObject;

		// Token: 0x04001B5B RID: 7003
		[SyncVar]
		public GameObject targetBodyObject;

		// Token: 0x04001B5C RID: 7004
		public GameObject burstHealEffect;

		// Token: 0x04001B5D RID: 7005
		public GameObject indicator;

		// Token: 0x04001B5E RID: 7006
		private GameObject cachedTargetBodyObject;

		// Token: 0x04001B5F RID: 7007
		private HealthComponent cachedTargetHealthComponent;

		// Token: 0x04001B60 RID: 7008
		private float healingTimer;

		// Token: 0x04001B61 RID: 7009
		private Vector3 velocity;

		// Token: 0x04001B62 RID: 7010
		private NetworkInstanceId ___ownerBodyObjectNetId;

		// Token: 0x04001B63 RID: 7011
		private NetworkInstanceId ___targetBodyObjectNetId;
	}
}
