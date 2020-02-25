using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008BB RID: 2235
	public class FireMicroMissiles : BaseState
	{
		// Token: 0x0600321B RID: 12827 RVA: 0x000D8644 File Offset: 0x000D6844
		private void FireMissile(GameObject targetObject)
		{
			Ray aimRay = base.GetAimRay();
			if (this.modelTransform && this.modelTransform.GetComponent<ChildLocator>())
			{
				Transform transform = null;
				if (transform)
				{
					aimRay.origin = transform.position;
				}
			}
			FireMicroMissiles.effectPrefab;
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			if (this.aimAnimator)
			{
				this.aimAnimator.AimImmediate();
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(FireMicroMissiles.minSpread, FireMicroMissiles.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, aimRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireMicroMissiles.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireMicroMissiles.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireMicroMissiles.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, targetObject, -1f);
			}
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x000D8808 File Offset: 0x000D6A08
		public override void OnEnter()
		{
			base.OnEnter();
			this.currentTargetIndex = 0;
			this.fireInterval = FireMicroMissiles.baseFireInterval / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimAnimator = (this.modelTransform ? this.modelTransform.GetComponent<AimAnimator>() : null);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x000D8878 File Offset: 0x000D6A78
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				if (this.currentTargetIndex >= this.targetsList.Count && base.isAuthority)
				{
					this.outer.SetNextStateToMain();
					return;
				}
				List<GameObject> list = this.targetsList;
				int num = this.currentTargetIndex;
				this.currentTargetIndex = num + 1;
				GameObject targetObject = list[num];
				this.FireMissile(targetObject);
				this.fireTimer += this.fireInterval;
			}
		}

		// Token: 0x0600321F RID: 12831 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x000D8908 File Offset: 0x000D6B08
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((uint)this.targetsList.Count);
			for (int i = 0; i < this.targetsList.Count; i++)
			{
				writer.Write(this.targetsList[i]);
			}
		}

		// Token: 0x06003221 RID: 12833 RVA: 0x000D8958 File Offset: 0x000D6B58
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			uint num = reader.ReadUInt32();
			this.targetsList = new List<GameObject>((int)num);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				this.targetsList.Add(reader.ReadGameObject());
				num2++;
			}
		}

		// Token: 0x040030DB RID: 12507
		public static GameObject effectPrefab;

		// Token: 0x040030DC RID: 12508
		public static GameObject projectilePrefab;

		// Token: 0x040030DD RID: 12509
		public static float damageCoefficient = 1f;

		// Token: 0x040030DE RID: 12510
		public static float baseFireInterval = 0.1f;

		// Token: 0x040030DF RID: 12511
		public static float minSpread = 0f;

		// Token: 0x040030E0 RID: 12512
		public static float maxSpread = 5f;

		// Token: 0x040030E1 RID: 12513
		public static float arcAngle = 5f;

		// Token: 0x040030E2 RID: 12514
		public List<GameObject> targetsList;

		// Token: 0x040030E3 RID: 12515
		private Transform modelTransform;

		// Token: 0x040030E4 RID: 12516
		private AimAnimator aimAnimator;

		// Token: 0x040030E5 RID: 12517
		private float fireTimer;

		// Token: 0x040030E6 RID: 12518
		private float fireInterval;

		// Token: 0x040030E7 RID: 12519
		private int currentTargetIndex;
	}
}
