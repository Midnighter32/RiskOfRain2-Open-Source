using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Headstompers
{
	// Token: 0x02000843 RID: 2115
	public class HeadstompersCooldown : BaseHeadstompersState
	{
		// Token: 0x06002FDF RID: 12255 RVA: 0x000CD32C File Offset: 0x000CB52C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = HeadstompersCooldown.baseDuration;
			if (this.body)
			{
				Inventory inventory = this.body.inventory;
				int num = inventory ? inventory.GetItemCount(ItemIndex.FallBoots) : 1;
				if (num > 0)
				{
					this.duration /= (float)num;
					this.bodyMotor.velocity = Vector3.zero;
					if (NetworkServer.active && this.impactSpeed > 0f)
					{
						float num2 = 10f;
						float num3 = Mathf.Max(0f, this.impactSpeed - HeadstompersFall.fallSpeed);
						float num4 = 23f;
						float num5 = 7f;
						float num6 = num4 + num5 * num3;
						BlastAttack blastAttack = new BlastAttack();
						blastAttack.attacker = this.body.gameObject;
						blastAttack.inflictor = this.body.gameObject;
						blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
						blastAttack.position = this.impactPosition;
						blastAttack.procCoefficient = 0.5f;
						blastAttack.radius = num2;
						blastAttack.baseForce = 2000f;
						blastAttack.bonusForce = Vector3.up * 2000f;
						blastAttack.baseDamage = this.body.damage * num6;
						blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
						blastAttack.crit = Util.CheckRoll(this.body.crit, this.body.master);
						blastAttack.damageColorIndex = DamageColorIndex.Item;
						blastAttack.Fire();
						EffectData effectData = new EffectData();
						effectData.origin = this.impactPosition;
						effectData.scale = num2;
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/BootShockwave"), effectData, true);
					}
				}
			}
		}

		// Token: 0x06002FE0 RID: 12256 RVA: 0x000CD4D7 File Offset: 0x000CB6D7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(new HeadstompersIdle());
				return;
			}
		}

		// Token: 0x06002FE1 RID: 12257 RVA: 0x000CD506 File Offset: 0x000CB706
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.impactSpeed);
			writer.Write(this.impactPosition);
		}

		// Token: 0x06002FE2 RID: 12258 RVA: 0x000CD528 File Offset: 0x000CB728
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.impactSpeed = Mathf.Abs(reader.ReadSingle());
			this.impactPosition = reader.ReadVector3();
		}

		// Token: 0x04002DB3 RID: 11699
		public static float baseDuration = 10f;

		// Token: 0x04002DB4 RID: 11700
		public float impactSpeed;

		// Token: 0x04002DB5 RID: 11701
		public Vector3 impactPosition = Vector3.zero;

		// Token: 0x04002DB6 RID: 11702
		private float duration;
	}
}
