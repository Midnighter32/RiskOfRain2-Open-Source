using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037D RID: 893
	public class OrbitalLaserController : MonoBehaviour
	{
		// Token: 0x060012A5 RID: 4773 RVA: 0x0005B43E File Offset: 0x0005963E
		private void Start()
		{
			this.chargeEffect.SetActive(true);
			this.chargeEffect.GetComponent<ObjectScaleCurve>().timeMax = this.chargeDuration;
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x0005B464 File Offset: 0x00059664
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			float maxSpeed = this.chargeMaxVelocity;
			switch (this.orbitalLaserState)
			{
			case OrbitalLaserController.OrbitalLaserState.Charging:
				maxSpeed = this.chargeMaxVelocity;
				if (this.stopwatch >= this.chargeDuration)
				{
					this.orbitalLaserState = OrbitalLaserController.OrbitalLaserState.Firing;
					this.stopwatch = 0f;
					this.chargeEffect.SetActive(false);
					this.fireEffect.SetActive(true);
				}
				break;
			case OrbitalLaserController.OrbitalLaserState.Firing:
				maxSpeed = this.fireMaxVelocity;
				this.bulletAttackTimer -= Time.fixedDeltaTime;
				if (this.ownerBody && this.bulletAttackTimer < 0f)
				{
					this.bulletAttackTimer += 1f / this.fireFrequency;
					new BulletAttack
					{
						owner = this.ownerBody.gameObject,
						origin = base.transform.position + Vector3.up * 600f,
						maxDistance = 1200f,
						aimVector = Vector3.down,
						minSpread = 0f,
						maxSpread = 0f,
						damage = Mathf.Lerp(this.damageCoefficientInitial, this.damageCoefficientFinal, this.stopwatch / this.fireDuration) * this.ownerBody.damage / this.fireFrequency,
						force = this.force,
						tracerEffectPrefab = this.tracerEffectPrefab,
						muzzleName = "",
						hitEffectPrefab = this.hitEffectPrefab,
						isCrit = Util.CheckRoll(this.ownerBody.crit, this.ownerBody.master),
						stopperMask = LayerIndex.world.mask,
						damageColorIndex = DamageColorIndex.Item,
						procCoefficient = this.procCoefficient / this.fireFrequency,
						radius = 2f
					}.Fire();
				}
				if (this.stopwatch >= this.fireDuration || !this.ownerBody)
				{
					this.orbitalLaserState = OrbitalLaserController.OrbitalLaserState.Decaying;
					this.stopwatch = 0f;
					this.fireEffect.SetActive(false);
				}
				break;
			case OrbitalLaserController.OrbitalLaserState.Decaying:
				maxSpeed = 0f;
				if (this.stopwatch >= this.decayDuration)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			}
			Vector3 target = base.transform.position;
			if (this.ownerBody)
			{
				this.ownerInputBank = this.ownerBody.GetComponent<InputBankTest>();
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray
				{
					direction = this.ownerInputBank.aimDirection,
					origin = this.ownerInputBank.aimOrigin
				}, out raycastHit, 900f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					target = raycastHit.point;
				}
				base.transform.position = Vector3.SmoothDamp(base.transform.position, target, ref this.refVelocity, this.smoothDampTime, maxSpeed);
			}
		}

		// Token: 0x04001652 RID: 5714
		public CharacterBody ownerBody;

		// Token: 0x04001653 RID: 5715
		public GameObject chargeEffect;

		// Token: 0x04001654 RID: 5716
		public GameObject fireEffect;

		// Token: 0x04001655 RID: 5717
		public float chargeDuration = 3f;

		// Token: 0x04001656 RID: 5718
		public float fireDuration = 6f;

		// Token: 0x04001657 RID: 5719
		public float decayDuration = 1.5f;

		// Token: 0x04001658 RID: 5720
		public float chargeMaxVelocity = 20f;

		// Token: 0x04001659 RID: 5721
		public float fireMaxVelocity = 1f;

		// Token: 0x0400165A RID: 5722
		public float smoothDampTime = 0.3f;

		// Token: 0x0400165B RID: 5723
		public float fireFrequency = 5f;

		// Token: 0x0400165C RID: 5724
		public float damageCoefficientInitial = 6f;

		// Token: 0x0400165D RID: 5725
		public float damageCoefficientFinal = 6f;

		// Token: 0x0400165E RID: 5726
		public float procCoefficient = 0.5f;

		// Token: 0x0400165F RID: 5727
		public float force;

		// Token: 0x04001660 RID: 5728
		public GameObject tracerEffectPrefab;

		// Token: 0x04001661 RID: 5729
		public GameObject hitEffectPrefab;

		// Token: 0x04001662 RID: 5730
		private float stopwatch;

		// Token: 0x04001663 RID: 5731
		private float bulletAttackTimer;

		// Token: 0x04001664 RID: 5732
		private OrbitalLaserController.OrbitalLaserState orbitalLaserState;

		// Token: 0x04001665 RID: 5733
		private Vector3 refVelocity;

		// Token: 0x04001666 RID: 5734
		private Transform chargeEffectTransform;

		// Token: 0x04001667 RID: 5735
		private InputBankTest ownerInputBank;

		// Token: 0x0200037E RID: 894
		public enum OrbitalLaserState
		{
			// Token: 0x04001669 RID: 5737
			Charging,
			// Token: 0x0400166A RID: 5738
			Firing,
			// Token: 0x0400166B RID: 5739
			Decaying
		}
	}
}
