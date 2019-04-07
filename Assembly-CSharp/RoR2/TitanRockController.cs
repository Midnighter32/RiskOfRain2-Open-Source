using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000402 RID: 1026
	public class TitanRockController : NetworkBehaviour
	{
		// Token: 0x060016D5 RID: 5845 RVA: 0x0006CA00 File Offset: 0x0006AC00
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.SetOwner(this.owner);
				return;
			}
			this.fireTimer = this.startDelay;
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x0006CA24 File Offset: 0x0006AC24
		public void SetOwner(GameObject newOwner)
		{
			this.ownerInputBank = null;
			this.ownerCharacterBody = null;
			this.isCrit = false;
			this.Networkowner = newOwner;
			if (this.owner)
			{
				this.ownerInputBank = this.owner.GetComponent<InputBankTest>();
				this.ownerCharacterBody = this.owner.GetComponent<CharacterBody>();
				ModelLocator component = this.owner.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
						if (component2)
						{
							this.targetTransform = component2.FindChild("Chest");
							if (this.targetTransform)
							{
								base.transform.rotation = this.targetTransform.rotation;
							}
						}
					}
				}
				base.transform.position = this.owner.transform.position + Vector3.down * 20f;
				if (NetworkServer.active && this.ownerCharacterBody)
				{
					CharacterMaster master = this.ownerCharacterBody.master;
					if (master)
					{
						this.isCrit = Util.CheckRoll(this.ownerCharacterBody.crit, master);
					}
				}
			}
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x0006CB54 File Offset: 0x0006AD54
		public void FixedUpdate()
		{
			if (this.targetTransform)
			{
				this.foundOwner = true;
				base.transform.position = Vector3.SmoothDamp(base.transform.position, this.targetTransform.TransformPoint(TitanRockController.targetLocalPosition), ref this.velocity, 1f);
				base.transform.rotation = this.targetTransform.rotation;
			}
			else if (this.foundOwner)
			{
				this.foundOwner = false;
				foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
				{
					particleSystem.main.gravityModifier = 1f;
					particleSystem.emission.enabled = false;
					particleSystem.noise.enabled = false;
					particleSystem.limitVelocityOverLifetime.enabled = false;
				}
				base.transform.Find("Debris").GetComponent<ParticleSystem>().collision.enabled = true;
				Light[] componentsInChildren2 = base.GetComponentsInChildren<Light>();
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					componentsInChildren2[i].enabled = false;
				}
				Util.PlaySound("Stop_titanboss_shift_loop", base.gameObject);
			}
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x0006CC9C File Offset: 0x0006AE9C
		[Server]
		private void FixedUpdateServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TitanRockController::FixedUpdateServer()' called on client");
				return;
			}
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				this.Fire();
				this.fireTimer += this.fireInterval;
			}
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x0006CCF8 File Offset: 0x0006AEF8
		[Server]
		private void Fire()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TitanRockController::Fire()' called on client");
				return;
			}
			if (this.ownerInputBank)
			{
				Vector3 position = this.fireTransform.position;
				Vector3 forward = this.ownerInputBank.aimDirection;
				RaycastHit raycastHit;
				if (Util.CharacterRaycast(this.owner, new Ray(this.ownerInputBank.aimOrigin, this.ownerInputBank.aimDirection), out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					forward = raycastHit.point - position;
				}
				float num = this.ownerCharacterBody ? this.ownerCharacterBody.damage : 1f;
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, position, Util.QuaternionSafeLookRotation(forward), this.owner, this.damageCoefficient * num, this.damageForce, this.isCrit, DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060016DD RID: 5853 RVA: 0x0006CE40 File Offset: 0x0006B040
		// (set) Token: 0x060016DE RID: 5854 RVA: 0x0006CE54 File Offset: 0x0006B054
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetOwner(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.owner, dirtyBit, ref this.___ownerNetId);
			}
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x0006CEA4 File Offset: 0x0006B0A4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.owner);
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
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x0006CF10 File Offset: 0x0006B110
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetOwner(reader.ReadGameObject());
			}
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x0006CF51 File Offset: 0x0006B151
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x040019FF RID: 6655
		[Tooltip("The child transform from which projectiles will be fired.")]
		public Transform fireTransform;

		// Token: 0x04001A00 RID: 6656
		[Tooltip("How long it takes to start firing.")]
		public float startDelay = 4f;

		// Token: 0x04001A01 RID: 6657
		[Tooltip("Firing interval.")]
		public float fireInterval = 1f;

		// Token: 0x04001A02 RID: 6658
		[Tooltip("The prefab to fire as a projectile.")]
		public GameObject projectilePrefab;

		// Token: 0x04001A03 RID: 6659
		[Tooltip("The damage coefficient to multiply by the owner's damage stat for the projectile's final damage value.")]
		public float damageCoefficient;

		// Token: 0x04001A04 RID: 6660
		[Tooltip("The force of the projectile's damage.")]
		public float damageForce;

		// Token: 0x04001A05 RID: 6661
		[SyncVar(hook = "SetOwner")]
		private GameObject owner;

		// Token: 0x04001A06 RID: 6662
		private Transform targetTransform;

		// Token: 0x04001A07 RID: 6663
		private Vector3 velocity;

		// Token: 0x04001A08 RID: 6664
		private static readonly Vector3 targetLocalPosition = new Vector3(0f, 12f, -3f);

		// Token: 0x04001A09 RID: 6665
		private float fireTimer;

		// Token: 0x04001A0A RID: 6666
		private InputBankTest ownerInputBank;

		// Token: 0x04001A0B RID: 6667
		private CharacterBody ownerCharacterBody;

		// Token: 0x04001A0C RID: 6668
		private bool isCrit;

		// Token: 0x04001A0D RID: 6669
		private bool foundOwner;

		// Token: 0x04001A0E RID: 6670
		private NetworkInstanceId ___ownerNetId;
	}
}
