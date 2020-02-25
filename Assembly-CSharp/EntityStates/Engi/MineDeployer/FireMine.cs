using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.MineDeployer
{
	// Token: 0x02000874 RID: 2164
	public class FireMine : BaseMineDeployerState
	{
		// Token: 0x060030C6 RID: 12486 RVA: 0x000D1F04 File Offset: 0x000D0104
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				FireMine.ResolveVelocities();
				Transform transform = base.transform.Find("FirePoint");
				ProjectileDamage component = base.GetComponent<ProjectileDamage>();
				Vector3 forward = transform.TransformVector(FireMine.velocities[this.fireIndex]);
				FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
				{
					crit = component.crit,
					damage = component.damage,
					damageColorIndex = component.damageColorIndex,
					force = component.force,
					owner = base.owner,
					position = transform.position,
					procChainMask = base.projectileController.procChainMask,
					projectilePrefab = FireMine.projectilePrefab,
					rotation = Quaternion.LookRotation(forward),
					fuseOverride = -1f,
					useFuseOverride = false,
					speedOverride = forward.magnitude,
					useSpeedOverride = true
				};
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

		// Token: 0x060030C7 RID: 12487 RVA: 0x000D2010 File Offset: 0x000D0210
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && FireMine.duration <= base.fixedAge)
			{
				int num = this.fireIndex + 1;
				if (num < FireMine.velocities.Length)
				{
					this.outer.SetNextState(new FireMine
					{
						fireIndex = num
					});
					return;
				}
				this.outer.SetNextState(new WaitForDeath());
			}
		}

		// Token: 0x060030C8 RID: 12488 RVA: 0x000D2074 File Offset: 0x000D0274
		private static Vector3[] GeneratePoints(float radius)
		{
			Vector3[] array = new Vector3[9];
			Quaternion rotation = Quaternion.AngleAxis(60f, Vector3.up);
			Quaternion rotation2 = Quaternion.AngleAxis(120f, Vector3.up);
			Vector3 forward = Vector3.forward;
			array[0] = forward;
			array[1] = rotation2 * array[0];
			array[2] = rotation2 * array[1];
			float num = 1f;
			float num2 = Vector3.Distance(array[0], array[1]);
			float d = Mathf.Sqrt(num * num + num2 * num2) / num;
			array[3] = rotation * (array[2] * d);
			array[4] = rotation2 * array[3];
			array[5] = rotation2 * array[4];
			d = 1f;
			array[6] = rotation * (array[5] * d);
			array[7] = rotation2 * array[6];
			array[8] = rotation2 * array[7];
			float d2 = radius / array[8].magnitude;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] *= d2;
			}
			return array;
		}

		// Token: 0x060030C9 RID: 12489 RVA: 0x000D21DC File Offset: 0x000D03DC
		private static Vector3[] GenerateHexPoints(float radius)
		{
			Vector3[] array = new Vector3[6];
			Quaternion rotation = Quaternion.AngleAxis(60f, Vector3.up);
			ref Vector3 ptr = ref array[0];
			ptr = Vector3.forward * radius;
			for (int i = 1; i < array.Length; i++)
			{
				Vector3[] array2 = array;
				int num = i;
				array2[num] = rotation * ptr;
				ptr = ref array2[num];
			}
			return array;
		}

		// Token: 0x060030CA RID: 12490 RVA: 0x000D2244 File Offset: 0x000D0444
		private static Vector3[] GeneratePointsFromPattern(GameObject patternObject)
		{
			Transform transform = patternObject.transform;
			Vector3 position = transform.position;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeInHierarchy)
				{
					list.Add(child.position - position);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060030CB RID: 12491 RVA: 0x000D22A8 File Offset: 0x000D04A8
		private static Vector3[] GenerateVelocitiesFromPoints(Vector3[] points, float apex)
		{
			Vector3[] array = new Vector3[points.Length];
			float num = Trajectory.CalculateInitialYSpeedForHeight(apex);
			for (int i = 0; i < points.Length; i++)
			{
				Vector3 normalized = points[i].normalized;
				float d = Trajectory.CalculateGroundSpeedToClearDistance(num, points[i].magnitude);
				Vector3 vector = normalized * d;
				vector.y = num;
				array[i] = vector;
			}
			return array;
		}

		// Token: 0x060030CC RID: 12492 RVA: 0x000D230C File Offset: 0x000D050C
		private static void ResolveVelocities()
		{
			if (FireMine.velocitiesResolved)
			{
				return;
			}
			FireMine.velocities = FireMine.GenerateVelocitiesFromPoints(FireMine.GeneratePoints(FireMine.patternRadius), FireMine.launchApex);
			if (!Application.isEditor)
			{
				FireMine.velocitiesResolved = true;
			}
		}

		// Token: 0x04002F09 RID: 12041
		public static float duration;

		// Token: 0x04002F0A RID: 12042
		public static float launchApex;

		// Token: 0x04002F0B RID: 12043
		public static float patternRadius;

		// Token: 0x04002F0C RID: 12044
		public static GameObject projectilePrefab;

		// Token: 0x04002F0D RID: 12045
		private int fireIndex;

		// Token: 0x04002F0E RID: 12046
		private static Vector3[] velocities;

		// Token: 0x04002F0F RID: 12047
		private static bool velocitiesResolved;
	}
}
