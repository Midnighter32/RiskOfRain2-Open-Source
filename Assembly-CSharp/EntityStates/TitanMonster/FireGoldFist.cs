using System;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000171 RID: 369
	internal class FireGoldFist : FireFist
	{
		// Token: 0x0600071E RID: 1822 RVA: 0x00022558 File Offset: 0x00020758
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

		// Token: 0x040008D8 RID: 2264
		public static int fistCount;

		// Token: 0x040008D9 RID: 2265
		public static float distanceBetweenFists;

		// Token: 0x040008DA RID: 2266
		public static float delayBetweenFists;
	}
}
