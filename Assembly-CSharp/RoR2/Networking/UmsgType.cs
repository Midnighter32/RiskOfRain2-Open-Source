using System;

namespace RoR2.Networking
{
	// Token: 0x02000567 RID: 1383
	public class UmsgType
	{
		// Token: 0x04001E09 RID: 7689
		public const short Lowest = 48;

		// Token: 0x04001E0A RID: 7690
		public const short SetEntityState = 48;

		// Token: 0x04001E0B RID: 7691
		public const short PlayerFireProjectile = 49;

		// Token: 0x04001E0C RID: 7692
		public const short ReleaseProjectilePredictionId = 50;

		// Token: 0x04001E0D RID: 7693
		public const short CharacterTransformUpdate = 51;

		// Token: 0x04001E0E RID: 7694
		public const short Effect = 52;

		// Token: 0x04001E0F RID: 7695
		public const short BulletDamage = 53;

		// Token: 0x04001E10 RID: 7696
		public const short UpdateTime = 54;

		// Token: 0x04001E11 RID: 7697
		public const short CreateExpEffect = 55;

		// Token: 0x04001E12 RID: 7698
		public const short ResetSkills = 56;

		// Token: 0x04001E13 RID: 7699
		public const short PickupItem = 57;

		// Token: 0x04001E14 RID: 7700
		public const short StatsUpdate = 58;

		// Token: 0x04001E15 RID: 7701
		public const short BroadcastChat = 59;

		// Token: 0x04001E16 RID: 7702
		public const short DamageDealt = 60;

		// Token: 0x04001E17 RID: 7703
		public const short Heal = 61;

		// Token: 0x04001E18 RID: 7704
		public const short ConstructTurret = 62;

		// Token: 0x04001E19 RID: 7705
		public const short AmmoPackPickup = 63;

		// Token: 0x04001E1A RID: 7706
		public const short Test = 64;

		// Token: 0x04001E1B RID: 7707
		public const short Ping = 65;

		// Token: 0x04001E1C RID: 7708
		public const short PingResponse = 66;

		// Token: 0x04001E1D RID: 7709
		public const short Kick = 67;

		// Token: 0x04001E1E RID: 7710
		public const short Teleport = 68;

		// Token: 0x04001E1F RID: 7711
		public const short SetJetpackJumpState = 69;

		// Token: 0x04001E20 RID: 7712
		public const short PreGameRuleVoteControllerSendClientVotes = 70;

		// Token: 0x04001E21 RID: 7713
		public const short OverlapAttackHits = 71;

		// Token: 0x04001E22 RID: 7714
		public const short EmitPointSound = 72;

		// Token: 0x04001E23 RID: 7715
		public const short EmitEntitySound = 73;

		// Token: 0x04001E24 RID: 7716
		public const short SetClientAuth = 74;

		// Token: 0x04001E25 RID: 7717
		public const short ReportBlastAttackDamage = 75;

		// Token: 0x04001E26 RID: 7718
		public const short Highest = 75;
	}
}
