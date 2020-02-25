using System;
using RoR2;

namespace EntityStates.Treebot.TreebotFlower
{
	// Token: 0x02000758 RID: 1880
	public class SpawnState : BaseState
	{
		// Token: 0x06002B80 RID: 11136 RVA: 0x000B77AB File Offset: 0x000B59AB
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(SpawnState.enterSoundString, base.gameObject);
			base.PlayAnimation("Base", "Spawn", "Spawn.playbackRate", SpawnState.duration);
		}

		// Token: 0x06002B81 RID: 11137 RVA: 0x000B77DE File Offset: 0x000B59DE
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration)
			{
				this.outer.SetNextState(new TreebotFlower2Projectile());
			}
		}

		// Token: 0x04002787 RID: 10119
		public static float duration;

		// Token: 0x04002788 RID: 10120
		public static string enterSoundString;
	}
}
