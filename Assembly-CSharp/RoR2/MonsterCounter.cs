using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200028D RID: 653
	public class MonsterCounter : MonoBehaviour
	{
		// Token: 0x06000E86 RID: 3718 RVA: 0x000407BB File Offset: 0x0003E9BB
		private int CountEnemies()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x000407C8 File Offset: 0x0003E9C8
		private void Update()
		{
			this.enemyList = this.CountEnemies();
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x000407D6 File Offset: 0x0003E9D6
		private void OnGUI()
		{
			GUI.Label(new Rect(12f, 160f, 200f, 25f), "Living Monsters: " + this.enemyList);
		}

		// Token: 0x04000E6D RID: 3693
		private int enemyList;
	}
}
