using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A8 RID: 424
	internal class FireMicroMissiles : BaseState
	{
		// Token: 0x06000842 RID: 2114 RVA: 0x00029550 File Offset: 0x00027750
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

		// Token: 0x06000843 RID: 2115 RVA: 0x00029714 File Offset: 0x00027914
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

		// Token: 0x06000844 RID: 2116 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00029784 File Offset: 0x00027984
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

		// Token: 0x06000846 RID: 2118 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x00029814 File Offset: 0x00027A14
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((uint)this.targetsList.Count);
			for (int i = 0; i < this.targetsList.Count; i++)
			{
				writer.Write(this.targetsList[i]);
			}
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x00029864 File Offset: 0x00027A64
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

		// Token: 0x04000AF1 RID: 2801
		public static GameObject effectPrefab;

		// Token: 0x04000AF2 RID: 2802
		public static GameObject projectilePrefab;

		// Token: 0x04000AF3 RID: 2803
		public static float damageCoefficient = 1f;

		// Token: 0x04000AF4 RID: 2804
		public static float baseFireInterval = 0.1f;

		// Token: 0x04000AF5 RID: 2805
		public static float minSpread = 0f;

		// Token: 0x04000AF6 RID: 2806
		public static float maxSpread = 5f;

		// Token: 0x04000AF7 RID: 2807
		public static float arcAngle = 5f;

		// Token: 0x04000AF8 RID: 2808
		public List<GameObject> targetsList;

		// Token: 0x04000AF9 RID: 2809
		private Transform modelTransform;

		// Token: 0x04000AFA RID: 2810
		private AimAnimator aimAnimator;

		// Token: 0x04000AFB RID: 2811
		private float fireTimer;

		// Token: 0x04000AFC RID: 2812
		private float fireInterval;

		// Token: 0x04000AFD RID: 2813
		private int currentTargetIndex;
	}
}
