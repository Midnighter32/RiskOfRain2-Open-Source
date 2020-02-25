using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000200 RID: 512
	public class FireAuraController : NetworkBehaviour
	{
		// Token: 0x06000AEB RID: 2795 RVA: 0x00030364 File Offset: 0x0002E564
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

		// Token: 0x06000AED RID: 2797 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x00030528 File Offset: 0x0002E728
		// (set) Token: 0x06000AEF RID: 2799 RVA: 0x0003053B File Offset: 0x0002E73B
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 1U, ref this.___ownerNetId);
			}
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x00030558 File Offset: 0x0002E758
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

		// Token: 0x06000AF1 RID: 2801 RVA: 0x000305C4 File Offset: 0x0002E7C4
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

		// Token: 0x06000AF2 RID: 2802 RVA: 0x00030605 File Offset: 0x0002E805
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04000B3B RID: 2875
		private const float fireAttackRadiusMin = 0.5f;

		// Token: 0x04000B3C RID: 2876
		private const float fireAttackRadiusMax = 6f;

		// Token: 0x04000B3D RID: 2877
		private const float fireDamageCoefficient = 1f;

		// Token: 0x04000B3E RID: 2878
		private const float fireProcCoefficient = 0.1f;

		// Token: 0x04000B3F RID: 2879
		private const float maxTimer = 8f;

		// Token: 0x04000B40 RID: 2880
		private float timer;

		// Token: 0x04000B41 RID: 2881
		private float attackStopwatch;

		// Token: 0x04000B42 RID: 2882
		[SyncVar]
		public GameObject owner;

		// Token: 0x04000B43 RID: 2883
		private NetworkInstanceId ___ownerNetId;
	}
}
