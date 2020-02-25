using System;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000857 RID: 2135
	public class FireGoldFist : FireFist
	{
		// Token: 0x06003039 RID: 12345 RVA: 0x000CF348 File Offset: 0x000CD548
		protected override void PlacePredictedAttack()
		{
			int num = 0;
			Vector3 predictedTargetPosition = this.predictedTargetPosition;
			Vector3 a = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward;
			for (int i = -(FireGoldFist.fistCount / 2); i < FireGoldFist.fistCount / 2; i++)
			{
				Vector3 vector = predictedTargetPosition + a * FireGoldFist.distanceBetweenFists * (float)i;
				float num2 = 60f;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(vector + Vector3.up * (num2 / 2f), Vector3.down), out raycastHit, num2, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					vector = raycastHit.point;
				}
				base.PlaceSingleDelayBlast(vector, FireGoldFist.delayBetweenFists * (float)num);
				num++;
			}
		}

		// Token: 0x04002E44 RID: 11844
		public static int fistCount;

		// Token: 0x04002E45 RID: 11845
		public static float distanceBetweenFists;

		// Token: 0x04002E46 RID: 11846
		public static float delayBetweenFists;
	}
}
