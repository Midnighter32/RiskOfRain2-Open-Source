using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035F RID: 863
	public class MonsterCounter : MonoBehaviour
	{
		// Token: 0x060011BD RID: 4541 RVA: 0x00057D31 File Offset: 0x00055F31
		private int CountEnemies()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00057D3E File Offset: 0x00055F3E
		private void Update()
		{
			this.enemyList = this.CountEnemies();
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00057D4C File Offset: 0x00055F4C
		private void OnGUI()
		{
			GUI.Label(new Rect(12f, 160f, 200f, 25f), "Living Monsters: " + this.enemyList);
		}

		// Token: 0x040015DB RID: 5595
		private int enemyList;
	}
}
