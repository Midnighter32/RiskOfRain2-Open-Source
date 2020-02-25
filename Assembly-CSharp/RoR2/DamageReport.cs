using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200011B RID: 283
	public class DamageReport
	{
		// Token: 0x06000522 RID: 1314 RVA: 0x00014908 File Offset: 0x00012B08
		public DamageReport(DamageInfo damageInfo, HealthComponent victim, float damageDealt)
		{
			this.damageInfo = damageInfo;
			this.victim = victim;
			this.victimBody = (victim ? victim.body : null);
			CharacterBody characterBody = this.victimBody;
			this.victimBodyIndex = ((characterBody != null) ? characterBody.bodyIndex : -1);
			CharacterBody characterBody2 = this.victimBody;
			this.victimTeamIndex = ((characterBody2 != null) ? characterBody2.teamComponent.teamIndex : TeamIndex.None);
			CharacterBody characterBody3 = this.victimBody;
			this.victimMaster = ((characterBody3 != null) ? characterBody3.master : null);
			CharacterBody characterBody4 = this.victimBody;
			this.victimIsElite = (characterBody4 != null && characterBody4.isElite);
			CharacterBody characterBody5 = this.victimBody;
			this.victimIsBoss = (characterBody5 != null && characterBody5.isBoss);
			this.attacker = damageInfo.attacker;
			this.attackerBody = (this.attacker ? this.attacker.GetComponent<CharacterBody>() : null);
			CharacterBody characterBody6 = this.attackerBody;
			this.attackerBodyIndex = ((characterBody6 != null) ? characterBody6.bodyIndex : -1);
			CharacterBody characterBody7 = this.attackerBody;
			this.attackerTeamIndex = ((characterBody7 != null) ? characterBody7.teamComponent.teamIndex : TeamIndex.None);
			CharacterBody characterBody8 = this.attackerBody;
			this.attackerMaster = ((characterBody8 != null) ? characterBody8.master : null);
			CharacterMaster characterMaster = this.attackerMaster;
			this.attackerOwnerMaster = ((characterMaster != null) ? characterMaster.minionOwnership.ownerMaster : null);
			CharacterMaster characterMaster2 = this.attackerOwnerMaster;
			int? num;
			if (characterMaster2 == null)
			{
				num = null;
			}
			else
			{
				CharacterBody body = characterMaster2.GetBody();
				num = ((body != null) ? new int?(body.bodyIndex) : null);
			}
			this.attackerOwnerBodyIndex = (num ?? -1);
			this.dotType = damageInfo.dotIndex;
			this.damageDealt = damageDealt;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00014AB4 File Offset: 0x00012CB4
		public StringBuilder AppendToStringBuilderMultiline(StringBuilder stringBuilder)
		{
			stringBuilder.Append("VictimBody=").AppendLine(DamageReport.<AppendToStringBuilderMultiline>g__ObjToString|18_0(this.victimBody));
			stringBuilder.Append("VictimTeamIndex=").AppendLine(this.victimTeamIndex.ToString());
			stringBuilder.Append("VictimMaster=").AppendLine(DamageReport.<AppendToStringBuilderMultiline>g__ObjToString|18_0(this.victimMaster));
			stringBuilder.Append("AttackerBody=").AppendLine(DamageReport.<AppendToStringBuilderMultiline>g__ObjToString|18_0(this.attackerBody));
			stringBuilder.Append("AttackerTeamIndex=").AppendLine(this.attackerTeamIndex.ToString());
			stringBuilder.Append("AttackerMaster=").AppendLine(DamageReport.<AppendToStringBuilderMultiline>g__ObjToString|18_0(this.attackerMaster));
			stringBuilder.Append("Inflictor=").AppendLine(DamageReport.<AppendToStringBuilderMultiline>g__ObjToString|18_0(this.damageInfo.inflictor));
			stringBuilder.Append("Damage=").AppendLine(this.damageInfo.damage.ToString());
			stringBuilder.Append("Crit=").AppendLine(this.damageInfo.crit.ToString());
			stringBuilder.Append("ProcChainMask=").AppendLine(this.damageInfo.procChainMask.ToString());
			stringBuilder.Append("ProcCoefficient=").AppendLine(this.damageInfo.procCoefficient.ToString());
			stringBuilder.Append("DamageType=").AppendLine(this.damageInfo.damageType.ToString());
			stringBuilder.Append("DotIndex=").AppendLine(this.damageInfo.dotIndex.ToString());
			stringBuilder.Append("DamageColorIndex=").AppendLine(this.damageInfo.damageColorIndex.ToString());
			stringBuilder.Append("Position=").AppendLine(this.damageInfo.position.ToString());
			stringBuilder.Append("Force=").AppendLine(this.damageInfo.force.ToString());
			stringBuilder.Append("Rejected=").AppendLine(this.damageInfo.rejected.ToString());
			return stringBuilder;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00014D05 File Offset: 0x00012F05
		public override string ToString()
		{
			return this.AppendToStringBuilderMultiline(new StringBuilder()).ToString();
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00014D17 File Offset: 0x00012F17
		[CompilerGenerated]
		internal static string <AppendToStringBuilderMultiline>g__ObjToString|18_0(object obj)
		{
			if (obj == null)
			{
				return "null";
			}
			return obj.ToString();
		}

		// Token: 0x04000540 RID: 1344
		public readonly HealthComponent victim;

		// Token: 0x04000541 RID: 1345
		public readonly CharacterBody victimBody;

		// Token: 0x04000542 RID: 1346
		public readonly int victimBodyIndex;

		// Token: 0x04000543 RID: 1347
		public readonly TeamIndex victimTeamIndex;

		// Token: 0x04000544 RID: 1348
		public readonly CharacterMaster victimMaster;

		// Token: 0x04000545 RID: 1349
		public readonly bool victimIsElite;

		// Token: 0x04000546 RID: 1350
		public readonly bool victimIsBoss;

		// Token: 0x04000547 RID: 1351
		public readonly DamageInfo damageInfo;

		// Token: 0x04000548 RID: 1352
		public readonly GameObject attacker;

		// Token: 0x04000549 RID: 1353
		public readonly CharacterBody attackerBody;

		// Token: 0x0400054A RID: 1354
		public readonly int attackerBodyIndex;

		// Token: 0x0400054B RID: 1355
		public readonly TeamIndex attackerTeamIndex;

		// Token: 0x0400054C RID: 1356
		public readonly CharacterMaster attackerMaster;

		// Token: 0x0400054D RID: 1357
		public readonly CharacterMaster attackerOwnerMaster;

		// Token: 0x0400054E RID: 1358
		public readonly int attackerOwnerBodyIndex;

		// Token: 0x0400054F RID: 1359
		public readonly DotController.DotIndex dotType;

		// Token: 0x04000550 RID: 1360
		public readonly float damageDealt;
	}
}
