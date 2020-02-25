using System;
using System.Runtime.InteropServices;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200035A RID: 858
	public class TitanRockController : NetworkBehaviour
	{
		// Token: 0x060014D7 RID: 5335 RVA: 0x00058D08 File Offset: 0x00056F08
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.SetOwner(this.owner);
				return;
			}
			this.fireTimer = this.startDelay;
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x00058D2C File Offset: 0x00056F2C
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

		// Token: 0x060014D9 RID: 5337 RVA: 0x00058E5C File Offset: 0x0005705C
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

		// Token: 0x060014DA RID: 5338 RVA: 0x00058FA4 File Offset: 0x000571A4
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

		// Token: 0x060014DB RID: 5339 RVA: 0x00059000 File Offset: 0x00057200
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

		// Token: 0x060014DE RID: 5342 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x00059148 File Offset: 0x00057348
		// (set) Token: 0x060014E0 RID: 5344 RVA: 0x0005915C File Offset: 0x0005735C
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetOwner(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.owner, dirtyBit, ref this.___ownerNetId);
			}
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x000591AC File Offset: 0x000573AC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.owner);
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
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x00059218 File Offset: 0x00057418
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

		// Token: 0x060014E3 RID: 5347 RVA: 0x00059259 File Offset: 0x00057459
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04001366 RID: 4966
		[Tooltip("The child transform from which projectiles will be fired.")]
		public Transform fireTransform;

		// Token: 0x04001367 RID: 4967
		[Tooltip("How long it takes to start firing.")]
		public float startDelay = 4f;

		// Token: 0x04001368 RID: 4968
		[Tooltip("Firing interval.")]
		public float fireInterval = 1f;

		// Token: 0x04001369 RID: 4969
		[Tooltip("The prefab to fire as a projectile.")]
		public GameObject projectilePrefab;

		// Token: 0x0400136A RID: 4970
		[Tooltip("The damage coefficient to multiply by the owner's damage stat for the projectile's final damage value.")]
		public float damageCoefficient;

		// Token: 0x0400136B RID: 4971
		[Tooltip("The force of the projectile's damage.")]
		public float damageForce;

		// Token: 0x0400136C RID: 4972
		[SyncVar(hook = "SetOwner")]
		private GameObject owner;

		// Token: 0x0400136D RID: 4973
		private Transform targetTransform;

		// Token: 0x0400136E RID: 4974
		private Vector3 velocity;

		// Token: 0x0400136F RID: 4975
		private static readonly Vector3 targetLocalPosition = new Vector3(0f, 12f, -3f);

		// Token: 0x04001370 RID: 4976
		private float fireTimer;

		// Token: 0x04001371 RID: 4977
		private InputBankTest ownerInputBank;

		// Token: 0x04001372 RID: 4978
		private CharacterBody ownerCharacterBody;

		// Token: 0x04001373 RID: 4979
		private bool isCrit;

		// Token: 0x04001374 RID: 4980
		private bool foundOwner;

		// Token: 0x04001375 RID: 4981
		private NetworkInstanceId ___ownerNetId;
	}
}
