using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200013B RID: 315
	[RequireComponent(typeof(Animator))]
	public class AimAnimator : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x000175C2 File Offset: 0x000157C2
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x000175B9 File Offset: 0x000157B9
		public bool isOutsideOfRange { get; private set; }

		// Token: 0x060005AA RID: 1450 RVA: 0x000175CA File Offset: 0x000157CA
		private void Awake()
		{
			this.animatorComponent = base.GetComponent<Animator>();
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x000175D8 File Offset: 0x000157D8
		private void Start()
		{
			int layerIndex = this.animatorComponent.GetLayerIndex("AimPitch");
			int layerIndex2 = this.animatorComponent.GetLayerIndex("AimYaw");
			this.animatorComponent.Play("PitchControl", layerIndex);
			this.animatorComponent.Play("YawControl", layerIndex2);
			this.animatorComponent.Update(0f);
			AnimatorClipInfo[] currentAnimatorClipInfo = this.animatorComponent.GetCurrentAnimatorClipInfo(layerIndex);
			AnimatorClipInfo[] currentAnimatorClipInfo2 = this.animatorComponent.GetCurrentAnimatorClipInfo(layerIndex2);
			if (currentAnimatorClipInfo.Length != 0)
			{
				AnimationClip clip = currentAnimatorClipInfo[0].clip;
				double num = (double)(clip.length * clip.frameRate);
				this.pitchClipCycleEnd = (float)((num - 1.0) / num);
			}
			if (currentAnimatorClipInfo2.Length != 0)
			{
				AnimationClip clip2 = currentAnimatorClipInfo2[0].clip;
				double num2 = (double)(clip2.length * clip2.frameRate);
				this.yawClipCycleEnd = (float)((num2 - 1.0) / num2);
			}
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x000176C8 File Offset: 0x000158C8
		private void Update()
		{
			if (Time.deltaTime <= 0f)
			{
				return;
			}
			this.UpdateLocalAnglesToAimVector();
			this.UpdateGiveup();
			this.ApproachDesiredAngles();
			this.UpdateAnimatorParameters(this.animatorComponent, this.pitchRangeMin, this.pitchRangeMax, this.yawRangeMin, this.yawRangeMax);
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00017718 File Offset: 0x00015918
		public void OnDeathStart()
		{
			base.enabled = false;
			this.currentLocalAngles = new AimAnimator.AimAngles
			{
				pitch = 0f,
				yaw = 0f
			};
			this.UpdateAnimatorParameters(this.animatorComponent, this.pitchRangeMin, this.pitchRangeMax, this.yawRangeMin, this.yawRangeMax);
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00017777 File Offset: 0x00015977
		private static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return outMin + (value - inMin) / (inMax - inMin) * (outMax - outMin);
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x00017787 File Offset: 0x00015987
		private static float NormalizeAngle(float angle)
		{
			return Mathf.Repeat(angle + 180f, 360f) - 180f;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x000177A0 File Offset: 0x000159A0
		private void UpdateLocalAnglesToAimVector()
		{
			Vector3 vector = this.inputBank ? this.inputBank.aimDirection : base.transform.forward;
			float y = this.directionComponent ? this.directionComponent.yaw : base.transform.eulerAngles.y;
			Vector3 eulerAngles = Util.QuaternionSafeLookRotation(vector, Vector3.up).eulerAngles;
			Vector3 vector2 = new Vector3(0f, y, 0f);
			Vector3 vector3 = vector;
			vector3.y = 0f;
			this.localAnglesToAimVector = new AimAnimator.AimAngles
			{
				pitch = -Mathf.Atan2(vector.y, vector3.magnitude) * 57.29578f,
				yaw = AimAnimator.NormalizeAngle(eulerAngles.y - vector2.y)
			};
			this.overshootAngles = new AimAnimator.AimAngles
			{
				pitch = Mathf.Max(this.pitchRangeMin - this.localAnglesToAimVector.pitch, this.localAnglesToAimVector.pitch - this.pitchRangeMax),
				yaw = Mathf.Max(Mathf.DeltaAngle(this.localAnglesToAimVector.yaw, this.yawRangeMin), Mathf.DeltaAngle(this.yawRangeMax, this.localAnglesToAimVector.yaw))
			};
			this.clampedLocalAnglesToAimVector = new AimAnimator.AimAngles
			{
				pitch = Mathf.Clamp(this.localAnglesToAimVector.pitch, this.pitchRangeMin, this.pitchRangeMax),
				yaw = Mathf.Clamp(this.localAnglesToAimVector.yaw, this.yawRangeMin, this.yawRangeMax)
			};
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0001794C File Offset: 0x00015B4C
		private void ApproachDesiredAngles()
		{
			AimAnimator.AimAngles aimAngles;
			float maxSpeed;
			if (this.shouldGiveup)
			{
				aimAngles = new AimAnimator.AimAngles
				{
					pitch = 0f,
					yaw = 0f
				};
				maxSpeed = this.loweredApproachSpeed;
			}
			else
			{
				aimAngles = this.clampedLocalAnglesToAimVector;
				maxSpeed = this.raisedApproachSpeed;
			}
			float yaw;
			if (this.fullYaw)
			{
				yaw = AimAnimator.NormalizeAngle(Mathf.SmoothDampAngle(this.currentLocalAngles.yaw, aimAngles.yaw, ref this.smoothingVelocity.yaw, this.smoothTime, maxSpeed, Time.deltaTime));
			}
			else
			{
				yaw = Mathf.SmoothDamp(this.currentLocalAngles.yaw, aimAngles.yaw, ref this.smoothingVelocity.yaw, this.smoothTime, maxSpeed, Time.deltaTime);
			}
			this.currentLocalAngles = new AimAnimator.AimAngles
			{
				pitch = Mathf.SmoothDampAngle(this.currentLocalAngles.pitch, aimAngles.pitch, ref this.smoothingVelocity.pitch, this.smoothTime, maxSpeed, Time.deltaTime),
				yaw = yaw
			};
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00017A50 File Offset: 0x00015C50
		private void ResetGiveup()
		{
			this.giveupTimer = this.giveupDuration;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00017A60 File Offset: 0x00015C60
		private void UpdateGiveup()
		{
			if (this.overshootAngles.pitch > this.pitchGiveupRange || (!this.fullYaw && this.overshootAngles.yaw > this.yawGiveupRange))
			{
				this.giveupTimer -= Time.deltaTime;
				this.isOutsideOfRange = true;
				return;
			}
			this.isOutsideOfRange = false;
			this.ResetGiveup();
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x00017AC2 File Offset: 0x00015CC2
		private bool shouldGiveup
		{
			get
			{
				return this.giveupTimer <= 0f;
			}
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x00017AD4 File Offset: 0x00015CD4
		public void AimImmediate()
		{
			this.UpdateLocalAnglesToAimVector();
			this.ResetGiveup();
			this.currentLocalAngles = this.clampedLocalAnglesToAimVector;
			this.smoothingVelocity = new AimAnimator.AimAngles
			{
				pitch = 0f,
				yaw = 0f
			};
			this.UpdateAnimatorParameters(this.animatorComponent, this.pitchRangeMin, this.pitchRangeMax, this.yawRangeMin, this.yawRangeMax);
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x00017B44 File Offset: 0x00015D44
		public void UpdateAnimatorParameters(Animator animator, float pitchRangeMin, float pitchRangeMax, float yawRangeMin, float yawRangeMax)
		{
			float num = 1f;
			if (this.enableAimWeight)
			{
				num = this.animatorComponent.GetFloat(AimAnimator.aimWeightHash);
			}
			animator.SetFloat(AimAnimator.aimPitchCycleHash, AimAnimator.Remap(this.currentLocalAngles.pitch * num, pitchRangeMin, pitchRangeMax, this.pitchClipCycleEnd, 0f));
			animator.SetFloat(AimAnimator.aimYawCycleHash, AimAnimator.Remap(this.currentLocalAngles.yaw * num, yawRangeMin, yawRangeMax, 0f, this.yawClipCycleEnd));
		}

		// Token: 0x0400060C RID: 1548
		[Tooltip("The input bank component of the character.")]
		public InputBankTest inputBank;

		// Token: 0x0400060D RID: 1549
		[Tooltip("The direction component of the character.")]
		public CharacterDirection directionComponent;

		// Token: 0x0400060E RID: 1550
		[Tooltip("The minimum pitch supplied by the aiming animation.")]
		public float pitchRangeMin;

		// Token: 0x0400060F RID: 1551
		[Tooltip("The maximum pitch supplied by the aiming animation.")]
		public float pitchRangeMax;

		// Token: 0x04000610 RID: 1552
		[Tooltip("The minimum yaw supplied by the aiming animation.")]
		public float yawRangeMin;

		// Token: 0x04000611 RID: 1553
		[Tooltip("The maximum yaw supplied by the aiming animation.")]
		public float yawRangeMax;

		// Token: 0x04000612 RID: 1554
		[Tooltip("If the pitch is this many degrees beyond the range the aiming animations support, the character will return to neutral pose after waiting the giveup duration.")]
		public float pitchGiveupRange;

		// Token: 0x04000613 RID: 1555
		[Tooltip("If the yaw is this many degrees beyond the range the aiming animations support, the character will return to neutral pose after waiting the giveup duration.")]
		public float yawGiveupRange;

		// Token: 0x04000614 RID: 1556
		[Tooltip("If the pitch or yaw exceed the range supported by the aiming animations, the character will return to neutral pose after waiting this long.")]
		public float giveupDuration;

		// Token: 0x04000615 RID: 1557
		[Tooltip("The speed in degrees/second to approach the desired pitch/yaw by while the weapon should be raised.")]
		public float raisedApproachSpeed = 720f;

		// Token: 0x04000616 RID: 1558
		[Tooltip("The speed in degrees/second to approach the desired pitch/yaw by while the weapon should be lowered.")]
		public float loweredApproachSpeed = 360f;

		// Token: 0x04000617 RID: 1559
		[Tooltip("The smoothing time for the motion.")]
		public float smoothTime = 0.1f;

		// Token: 0x04000618 RID: 1560
		[Tooltip("Whether or not the character can do full 360 yaw turns.")]
		public bool fullYaw;

		// Token: 0x04000619 RID: 1561
		[Tooltip("Switches between Direct (point straight at target) or Smart (only turn when outside angle range).")]
		public AimAnimator.AimType aimType;

		// Token: 0x0400061A RID: 1562
		[Tooltip("Assigns the weight of the aim from the center as an animator value 'aimWeight' between 0-1.")]
		public bool enableAimWeight;

		// Token: 0x0400061C RID: 1564
		private Animator animatorComponent;

		// Token: 0x0400061D RID: 1565
		private float pitchClipCycleEnd;

		// Token: 0x0400061E RID: 1566
		private float yawClipCycleEnd;

		// Token: 0x0400061F RID: 1567
		private float giveupTimer;

		// Token: 0x04000620 RID: 1568
		private AimAnimator.AimAngles localAnglesToAimVector;

		// Token: 0x04000621 RID: 1569
		private AimAnimator.AimAngles overshootAngles;

		// Token: 0x04000622 RID: 1570
		private AimAnimator.AimAngles clampedLocalAnglesToAimVector;

		// Token: 0x04000623 RID: 1571
		private AimAnimator.AimAngles currentLocalAngles;

		// Token: 0x04000624 RID: 1572
		private AimAnimator.AimAngles smoothingVelocity;

		// Token: 0x04000625 RID: 1573
		private static readonly int aimPitchCycleHash = Animator.StringToHash("aimPitchCycle");

		// Token: 0x04000626 RID: 1574
		private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

		// Token: 0x04000627 RID: 1575
		private static readonly int aimWeightHash = Animator.StringToHash("aimWeight");

		// Token: 0x0200013C RID: 316
		public enum AimType
		{
			// Token: 0x04000629 RID: 1577
			Direct,
			// Token: 0x0400062A RID: 1578
			Smart
		}

		// Token: 0x0200013D RID: 317
		private struct AimAngles
		{
			// Token: 0x0400062B RID: 1579
			public float pitch;

			// Token: 0x0400062C RID: 1580
			public float yaw;
		}
	}
}
