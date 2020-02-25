using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200024B RID: 587
	public class IcicleAuraController : NetworkBehaviour
	{
		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x00039E04 File Offset: 0x00038004
		private int maxIcicleCount
		{
			get
			{
				int num = 1;
				if (this.cachedOwnerInfo.characterBody.inventory)
				{
					num = this.cachedOwnerInfo.characterBody.inventory.GetItemCount(ItemIndex.Icicle);
				}
				return this.baseIcicleMax + (num - 1) * this.icicleMaxPerStack;
			}
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x00039E53 File Offset: 0x00038053
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x00039E64 File Offset: 0x00038064
		private void FixedUpdate()
		{
			if (this.cachedOwnerInfo.gameObject != this.owner)
			{
				this.cachedOwnerInfo = new IcicleAuraController.OwnerInfo(this.owner);
			}
			this.UpdateRadius();
			if (NetworkServer.active)
			{
				if (!this.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				for (int i = this.icicleLifetimes.Count - 1; i >= 0; i--)
				{
					List<float> list = this.icicleLifetimes;
					int index = i;
					list[index] -= Time.fixedDeltaTime;
					if (this.icicleLifetimes[i] <= 0f)
					{
						this.icicleLifetimes.RemoveAt(i);
					}
				}
				this.NetworkfinalIcicleCount = Mathf.Min((this.icicleLifetimes.Count > 0) ? (this.icicleCountOnFirstKill + this.icicleLifetimes.Count) : 0, this.maxIcicleCount);
				this.attackStopwatch += Time.fixedDeltaTime;
			}
			if (this.cachedOwnerInfo.characterBody)
			{
				if (this.finalIcicleCount > 0)
				{
					if (this.lastIcicleCount == 0)
					{
						this.OnIciclesActivated();
					}
					if (this.lastIcicleCount < this.finalIcicleCount)
					{
						this.OnIcicleGained();
					}
				}
				else if (this.lastIcicleCount > 0)
				{
					this.OnIciclesDeactivated();
				}
				this.lastIcicleCount = this.finalIcicleCount;
			}
			if (NetworkServer.active && this.cachedOwnerInfo.characterBody && this.finalIcicleCount > 0 && this.attackStopwatch >= this.baseIcicleAttackInterval)
			{
				this.attackStopwatch = 0f;
				new BlastAttack
				{
					attacker = this.owner,
					inflictor = base.gameObject,
					teamIndex = this.cachedOwnerInfo.characterBody.teamComponent.teamIndex,
					position = this.transform.position,
					procCoefficient = this.icicleProcCoefficientPerTick,
					radius = this.actualRadius,
					baseForce = 0f,
					baseDamage = this.cachedOwnerInfo.characterBody.damage * (this.icicleDamageCoefficientPerTick + this.icicleDamageCoefficientPerTickPerIcicle * (float)this.finalIcicleCount),
					bonusForce = Vector3.zero,
					crit = false,
					damageType = DamageType.Generic,
					falloffModel = BlastAttack.FalloffModel.None,
					damageColorIndex = DamageColorIndex.Item
				}.Fire();
			}
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0003A0C4 File Offset: 0x000382C4
		private void UpdateRadius()
		{
			if (this.owner)
			{
				this.actualRadius = (this.cachedOwnerInfo.characterBody ? (this.cachedOwnerInfo.characterBody.radius + this.icicleBaseRadius + this.icicleRadiusPerIcicle * (float)this.finalIcicleCount) : 0f);
			}
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x0003A124 File Offset: 0x00038324
		private void UpdateVisuals()
		{
			if (this.cachedOwnerInfo.gameObject)
			{
				this.transform.position = (this.cachedOwnerInfo.characterBody ? this.cachedOwnerInfo.characterBody.corePosition : this.cachedOwnerInfo.transform.position);
			}
			float num = Mathf.SmoothDamp(this.transform.localScale.x, this.actualRadius, ref this.scaleVelocity, 0.5f);
			this.transform.localScale = new Vector3(num, num, num);
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0003A1BC File Offset: 0x000383BC
		private void OnIciclesDeactivated()
		{
			Util.PlaySound("Stop_item_proc_icicle", base.gameObject);
			ParticleSystem[] array = this.auraParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].main.loop = false;
			}
			if (this.cachedOwnerInfo.cameraTargetParams)
			{
				this.cachedOwnerInfo.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0003A224 File Offset: 0x00038424
		private void OnIciclesActivated()
		{
			Util.PlaySound("Play_item_proc_icicle", base.gameObject);
			if (this.cachedOwnerInfo.cameraTargetParams)
			{
				this.cachedOwnerInfo.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			foreach (ParticleSystem particleSystem in this.auraParticles)
			{
				particleSystem.main.loop = true;
				particleSystem.Play();
			}
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0003A294 File Offset: 0x00038494
		private void OnIcicleGained()
		{
			ParticleSystem[] array = this.procParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x0003A2BE File Offset: 0x000384BE
		private void LateUpdate()
		{
			this.UpdateVisuals();
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x0003A2C6 File Offset: 0x000384C6
		public void OnOwnerKillOther()
		{
			this.icicleLifetimes.Add(this.icicleDuration);
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0003A2D9 File Offset: 0x000384D9
		public void OnDestroy()
		{
			this.OnIciclesDeactivated();
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000CFC RID: 3324 RVA: 0x0003A364 File Offset: 0x00038564
		// (set) Token: 0x06000CFD RID: 3325 RVA: 0x0003A377 File Offset: 0x00038577
		public int NetworkfinalIcicleCount
		{
			get
			{
				return this.finalIcicleCount;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.finalIcicleCount, 1U);
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000CFE RID: 3326 RVA: 0x0003A38C File Offset: 0x0003858C
		// (set) Token: 0x06000CFF RID: 3327 RVA: 0x0003A39F File Offset: 0x0003859F
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 2U, ref this.___ownerNetId);
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0003A3BC File Offset: 0x000385BC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.finalIcicleCount);
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
				writer.WritePackedUInt32((uint)this.finalIcicleCount);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
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

		// Token: 0x06000D01 RID: 3329 RVA: 0x0003A468 File Offset: 0x00038668
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.finalIcicleCount = (int)reader.ReadPackedUInt32();
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.finalIcicleCount = (int)reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.owner = reader.ReadGameObject();
			}
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0003A4CE File Offset: 0x000386CE
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04000CF8 RID: 3320
		public float baseIcicleAttackInterval = 0.25f;

		// Token: 0x04000CF9 RID: 3321
		public float icicleBaseRadius = 10f;

		// Token: 0x04000CFA RID: 3322
		public float icicleRadiusPerIcicle = 2f;

		// Token: 0x04000CFB RID: 3323
		public float icicleDamageCoefficientPerTick = 2f;

		// Token: 0x04000CFC RID: 3324
		public float icicleDamageCoefficientPerTickPerIcicle = 1f;

		// Token: 0x04000CFD RID: 3325
		public float icicleDuration = 5f;

		// Token: 0x04000CFE RID: 3326
		public float icicleProcCoefficientPerTick = 0.2f;

		// Token: 0x04000CFF RID: 3327
		public int icicleCountOnFirstKill = 2;

		// Token: 0x04000D00 RID: 3328
		public int baseIcicleMax = 6;

		// Token: 0x04000D01 RID: 3329
		public int icicleMaxPerStack = 3;

		// Token: 0x04000D02 RID: 3330
		private List<float> icicleLifetimes = new List<float>();

		// Token: 0x04000D03 RID: 3331
		private float attackStopwatch;

		// Token: 0x04000D04 RID: 3332
		private int lastIcicleCount;

		// Token: 0x04000D05 RID: 3333
		[SyncVar]
		private int finalIcicleCount;

		// Token: 0x04000D06 RID: 3334
		[SyncVar]
		public GameObject owner;

		// Token: 0x04000D07 RID: 3335
		private IcicleAuraController.OwnerInfo cachedOwnerInfo;

		// Token: 0x04000D08 RID: 3336
		public ParticleSystem[] auraParticles;

		// Token: 0x04000D09 RID: 3337
		public ParticleSystem[] procParticles;

		// Token: 0x04000D0A RID: 3338
		private new Transform transform;

		// Token: 0x04000D0B RID: 3339
		private float actualRadius;

		// Token: 0x04000D0C RID: 3340
		private float scaleVelocity;

		// Token: 0x04000D0D RID: 3341
		private NetworkInstanceId ___ownerNetId;

		// Token: 0x0200024C RID: 588
		private struct OwnerInfo
		{
			// Token: 0x06000D03 RID: 3331 RVA: 0x0003A4F4 File Offset: 0x000386F4
			public OwnerInfo(GameObject gameObject)
			{
				this.gameObject = gameObject;
				if (gameObject)
				{
					this.transform = gameObject.transform;
					this.characterBody = gameObject.GetComponent<CharacterBody>();
					this.cameraTargetParams = gameObject.GetComponent<CameraTargetParams>();
					return;
				}
				this.transform = null;
				this.characterBody = null;
				this.cameraTargetParams = null;
			}

			// Token: 0x04000D0E RID: 3342
			public readonly GameObject gameObject;

			// Token: 0x04000D0F RID: 3343
			public readonly Transform transform;

			// Token: 0x04000D10 RID: 3344
			public readonly CharacterBody characterBody;

			// Token: 0x04000D11 RID: 3345
			public readonly CameraTargetParams cameraTargetParams;
		}
	}
}
