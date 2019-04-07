using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000398 RID: 920
	public class PressurePlateController : MonoBehaviour
	{
		// Token: 0x06001368 RID: 4968 RVA: 0x00004507 File Offset: 0x00002707
		private void Start()
		{
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0005EC08 File Offset: 0x0005CE08
		private void FixedUpdate()
		{
			if (this.enableOverlapSphere)
			{
				this.overlapSphereStopwatch += Time.fixedDeltaTime;
				if (this.overlapSphereStopwatch >= 1f / this.overlapSphereFrequency)
				{
					this.overlapSphereStopwatch -= 1f / this.overlapSphereFrequency;
					bool @switch = Physics.OverlapSphere(base.transform.position, this.overlapSphereRadius, LayerIndex.defaultLayer.mask | LayerIndex.fakeActor.mask, QueryTriggerInteraction.UseGlobal).Length != 0;
					this.SetSwitch(@switch);
				}
			}
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0005ECA7 File Offset: 0x0005CEA7
		public void EnableOverlapSphere(bool input)
		{
			this.enableOverlapSphere = input;
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0005ECB0 File Offset: 0x0005CEB0
		public void SetSwitch(bool switchIsDown)
		{
			if (switchIsDown != this.switchDown)
			{
				if (switchIsDown)
				{
					this.animationStopwatch = 0f;
					Util.PlaySound(this.switchDownSoundString, base.gameObject);
					UnityEvent onSwitchDown = this.OnSwitchDown;
					if (onSwitchDown != null)
					{
						onSwitchDown.Invoke();
					}
				}
				else
				{
					this.animationStopwatch = 0f;
					Util.PlaySound(this.switchUpSoundString, base.gameObject);
					UnityEvent onSwitchUp = this.OnSwitchUp;
					if (onSwitchUp != null)
					{
						onSwitchUp.Invoke();
					}
				}
				this.switchDown = switchIsDown;
			}
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x0005ED30 File Offset: 0x0005CF30
		private void Update()
		{
			this.animationStopwatch += Time.deltaTime;
			if (this.switchVisualTransform)
			{
				Vector3 localPosition = this.switchVisualTransform.transform.localPosition;
				bool flag = this.switchDown;
				if (flag)
				{
					if (flag)
					{
						localPosition.z = this.switchVisualPositionFromUpToDown.Evaluate(this.animationStopwatch);
					}
				}
				else
				{
					localPosition.z = this.switchVisualPositionFromDownToUp.Evaluate(this.animationStopwatch);
				}
				this.switchVisualTransform.localPosition = localPosition;
			}
		}

		// Token: 0x0600136D RID: 4973 RVA: 0x0005EDB9 File Offset: 0x0005CFB9
		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(base.transform.position, this.overlapSphereRadius);
		}

		// Token: 0x04001706 RID: 5894
		public bool enableOverlapSphere = true;

		// Token: 0x04001707 RID: 5895
		public float overlapSphereRadius;

		// Token: 0x04001708 RID: 5896
		public float overlapSphereFrequency;

		// Token: 0x04001709 RID: 5897
		public string switchDownSoundString;

		// Token: 0x0400170A RID: 5898
		public string switchUpSoundString;

		// Token: 0x0400170B RID: 5899
		public UnityEvent OnSwitchDown;

		// Token: 0x0400170C RID: 5900
		public UnityEvent OnSwitchUp;

		// Token: 0x0400170D RID: 5901
		public AnimationCurve switchVisualPositionFromUpToDown;

		// Token: 0x0400170E RID: 5902
		public AnimationCurve switchVisualPositionFromDownToUp;

		// Token: 0x0400170F RID: 5903
		public Transform switchVisualTransform;

		// Token: 0x04001710 RID: 5904
		private float overlapSphereStopwatch;

		// Token: 0x04001711 RID: 5905
		private float animationStopwatch;

		// Token: 0x04001712 RID: 5906
		private bool switchDown;
	}
}
