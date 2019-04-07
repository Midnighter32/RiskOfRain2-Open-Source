using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F2 RID: 754
	public class FireAuraController : NetworkBehaviour
	{
		// Token: 0x06000F3D RID: 3901 RVA: 0x0004B430 File Offset: 0x00049630
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			CharacterBody characterBody = null;
			float num = 0f;
			if (this.owner)
			{
				characterBody = this.owner.GetComponent<CharacterBody>();
				num = (characterBody ? Mathf.Lerp(characterBody.radius * 0.5f, characterBody.radius * 6f, 1f - Mathf.Abs(-1f + 2f * this.timer / 8f)) : 0f);
				base.transform.position = this.owner.transform.position;
				base.transform.localScale = new Vector3(num, num, num);
			}
			if (NetworkServer.active)
			{
				if (!this.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				this.attackStopwatch += Time.fixedDeltaTime;
				if (characterBody && this.attackStopwatch >= 0.25f)
				{
					this.attackStopwatch = 0f;
					BlastAttack blastAttack = new BlastAttack();
					blastAttack.attacker = this.owner;
					blastAttack.inflictor = base.gameObject;
					blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
					blastAttack.position = base.transform.position;
					blastAttack.procCoefficient = 0.1f;
					blastAttack.radius = num;
					blastAttack.baseForce = 0f;
					blastAttack.baseDamage = 1f * characterBody.damage;
					blastAttack.bonusForce = Vector3.zero;
					blastAttack.crit = false;
					blastAttack.damageType = DamageType.Generic;
					blastAttack.Fire();
				}
				if (this.timer >= 8f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000F40 RID: 3904 RVA: 0x0004B5F0 File Offset: 0x000497F0
		// (set) Token: 0x06000F41 RID: 3905 RVA: 0x0004B603 File Offset: 0x00049803
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 1u, ref this.___ownerNetId);
			}
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0004B620 File Offset: 0x00049820
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

		// Token: 0x06000F43 RID: 3907 RVA: 0x0004B68C File Offset: 0x0004988C
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
				this.owner = reader.ReadGameObject();
			}
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0004B6CD File Offset: 0x000498CD
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04001359 RID: 4953
		private const float fireAttackRadiusMin = 0.5f;

		// Token: 0x0400135A RID: 4954
		private const float fireAttackRadiusMax = 6f;

		// Token: 0x0400135B RID: 4955
		private const float fireDamageCoefficient = 1f;

		// Token: 0x0400135C RID: 4956
		private const float fireProcCoefficient = 0.1f;

		// Token: 0x0400135D RID: 4957
		private const float maxTimer = 8f;

		// Token: 0x0400135E RID: 4958
		private float timer;

		// Token: 0x0400135F RID: 4959
		private float attackStopwatch;

		// Token: 0x04001360 RID: 4960
		[SyncVar]
		public GameObject owner;

		// Token: 0x04001361 RID: 4961
		private NetworkInstanceId ___ownerNetId;
	}
}
