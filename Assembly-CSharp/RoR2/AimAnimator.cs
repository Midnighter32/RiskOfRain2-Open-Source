using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200024A RID: 586
	[RequireComponent(typeof(Animator))]
	public class AimAnimator : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000B01 RID: 2817 RVA: 0x00036A96 File Offset: 0x00034C96
		// (set) Token: 0x06000B00 RID: 2816 RVA: 0x00036A8D File Offset: 0x00034C8D
		public bool isOutsideOfRange { get; private set; }

		// Token: 0x06000B02 RID: 2818 RVA: 0x00036A9E File Offset: 0x00034C9E
		private void Awake()
		{
			this.animatorComponent = base.GetComponent<Animator>();
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00036AAC File Offset: 0x00034CAC
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

		// Token: 0x06000B04 RID: 2820 RVA: 0x00036B9C File Offset: 0x00034D9C
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

		// Token: 0x06000B05 RID: 2821 RVA: 0x00036BEC File Offset: 0x00034DEC
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

		// Token: 0x06000B06 RID: 2822 RVA: 0x00036C4B File Offset: 0x00034E4B
		private static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return outMin + (value - inMin) / (inMax - inMin) * (outMax - outMin);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00036C5B File Offset: 0x00034E5B
		private static float NormalizeAngle(float angle)
		{
			return Mathf.Repeat(angle + 180f, 360f) - 180f;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00036C74 File Offset: 0x00034E74
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

		// Token: 0x06000B09 RID: 2825 RVA: 0x00036E20 File Offset: 0x00035020
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

		// Token: 0x06000B0A RID: 2826 RVA: 0x00036F24 File Offset: 0x00035124
		private void ResetGiveup()
		{
			this.giveupTimer = this.giveupDuration;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00036F34 File Offset: 0x00035134
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

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000B0C RID: 2828 RVA: 0x00036F96 File Offset: 0x00035196
		private bool shouldGiveup
		{
			get
			{
				return this.giveupTimer <= 0f;
			}
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00036FA8 File Offset: 0x000351A8
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

		// Token: 0x06000B0E RID: 2830 RVA: 0x00037018 File Offset: 0x00035218
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

		// Token: 0x04000EEF RID: 3823
		[Tooltip("The input bank component of the character.")]
		public InputBankTest inputBank;

		// Token: 0x04000EF0 RID: 3824
		[Tooltip("The direction component of the character.")]
		public CharacterDirection directionComponent;

		// Token: 0x04000EF1 RID: 3825
		[Tooltip("The minimum pitch supplied by the aiming animation.")]
		public float pitchRangeMin;

		// Token: 0x04000EF2 RID: 3826
		[Tooltip("The maximum pitch supplied by the aiming animation.")]
		public float pitchRangeMax;

		// Token: 0x04000EF3 RID: 3827
		[Tooltip("The minimum yaw supplied by the aiming animation.")]
		public float yawRangeMin;

		// Token: 0x04000EF4 RID: 3828
		[Tooltip("The maximum yaw supplied by the aiming animation.")]
		public float yawRangeMax;

		// Token: 0x04000EF5 RID: 3829
		[Tooltip("If the pitch is this many degrees beyond the range the aiming animations support, the character will return to neutral pose after waiting the giveup duration.")]
		public float pitchGiveupRange;

		// Token: 0x04000EF6 RID: 3830
		[Tooltip("If the yaw is this many degrees beyond the range the aiming animations support, the character will return to neutral pose after waiting the giveup duration.")]
		public float yawGiveupRange;

		// Token: 0x04000EF7 RID: 3831
		[Tooltip("If the pitch or yaw exceed the range supported by the aiming animations, the character will return to neutral pose after waiting this long.")]
		public float giveupDuration;

		// Token: 0x04000EF8 RID: 3832
		[Tooltip("The speed in degrees/second to approach the desired pitch/yaw by while the weapon should be raised.")]
		public float raisedApproachSpeed = 720f;

		// Token: 0x04000EF9 RID: 3833
		[Tooltip("The speed in degrees/second to approach the desired pitch/yaw by while the weapon should be lowered.")]
		public float loweredApproachSpeed = 360f;

		// Token: 0x04000EFA RID: 3834
		[Tooltip("The smoothing time for the motion.")]
		public float smoothTime = 0.1f;

		// Token: 0x04000EFB RID: 3835
		[Tooltip("Whether or not the character can do full 360 yaw turns.")]
		public bool fullYaw;

		// Token: 0x04000EFC RID: 3836
		[Tooltip("Switches between Direct (point straight at target) or Smart (only turn when outside angle range).")]
		public AimAnimator.AimType aimType;

		// Token: 0x04000EFD RID: 3837
		[Tooltip("Assigns the weight of the aim from the center as an animator value 'aimWeight' between 0-1.")]
		public bool enableAimWeight;

		// Token: 0x04000EFF RID: 3839
		private Animator animatorComponent;

		// Token: 0x04000F00 RID: 3840
		private float pitchClipCycleEnd;

		// Token: 0x04000F01 RID: 3841
		private float yawClipCycleEnd;

		// Token: 0x04000F02 RID: 3842
		private float giveupTimer;

		// Token: 0x04000F03 RID: 3843
		private AimAnimator.AimAngles localAnglesToAimVector;

		// Token: 0x04000F04 RID: 3844
		private AimAnimator.AimAngles overshootAngles;

		// Token: 0x04000F05 RID: 3845
		private AimAnimator.AimAngles clampedLocalAnglesToAimVector;

		// Token: 0x04000F06 RID: 3846
		private AimAnimator.AimAngles currentLocalAngles;

		// Token: 0x04000F07 RID: 3847
		private AimAnimator.AimAngles smoothingVelocity;

		// Token: 0x04000F08 RID: 3848
		private static readonly int aimPitchCycleHash = Animator.StringToHash("aimPitchCycle");

		// Token: 0x04000F09 RID: 3849
		private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

		// Token: 0x04000F0A RID: 3850
		private static readonly int aimWeightHash = Animator.StringToHash("aimWeight");

		// Token: 0x0200024B RID: 587
		public enum AimType
		{
			// Token: 0x04000F0C RID: 3852
			Direct,
			// Token: 0x04000F0D RID: 3853
			Smart
		}

		// Token: 0x0200024C RID: 588
		private struct AimAngles
		{
			// Token: 0x04000F0E RID: 3854
			public float pitch;

			// Token: 0x04000F0F RID: 3855
			public float yaw;
		}
	}
}
