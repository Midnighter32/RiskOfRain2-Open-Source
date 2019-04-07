using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Headstompers
{
	// Token: 0x02000161 RID: 353
	public class HeadstompersCooldown : BaseHeadstompersState
	{
		// Token: 0x060006DA RID: 1754 RVA: 0x00020BF8 File Offset: 0x0001EDF8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = HeadstompersCooldown.baseDuration;
			if (NetworkServer.active && this.body && this.impactSpeed > 0f)
			{
				Inventory inventory = this.body.inventory;
				int num = inventory ? inventory.GetItemCount(ItemIndex.FallBoots) : 0;
				this.bodyMotor.velocity = Vector3.zero;
				if (num > 0)
				{
					this.duration /= (float)num;
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
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/BootShockwave"), effectData, true);
				}
			}
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x00020DA8 File Offset: 0x0001EFA8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.stopwatch += Time.deltaTime;
				if (this.stopwatch >= this.duration)
				{
					this.outer.SetNextState(new HeadstompersIdle());
					return;
				}
			}
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x00020DF4 File Offset: 0x0001EFF4
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.impactSpeed);
			writer.Write(this.impactPosition);
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x00020E16 File Offset: 0x0001F016
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.impactSpeed = Mathf.Abs(reader.ReadSingle());
			this.impactPosition = reader.ReadVector3();
		}

		// Token: 0x0400086C RID: 2156
		private float stopwatch;

		// Token: 0x0400086D RID: 2157
		public static float baseDuration = 10f;

		// Token: 0x0400086E RID: 2158
		public float impactSpeed;

		// Token: 0x0400086F RID: 2159
		public Vector3 impactPosition = Vector3.zero;

		// Token: 0x04000870 RID: 2160
		private float duration;
	}
}
