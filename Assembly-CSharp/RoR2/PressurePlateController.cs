using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002D5 RID: 725
	public class PressurePlateController : MonoBehaviour
	{
		// Token: 0x06001086 RID: 4230 RVA: 0x0000409B File Offset: 0x0000229B
		private void Start()
		{
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x000486D0 File Offset: 0x000468D0
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

		// Token: 0x06001088 RID: 4232 RVA: 0x0004876F File Offset: 0x0004696F
		public void EnableOverlapSphere(bool input)
		{
			this.enableOverlapSphere = input;
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00048778 File Offset: 0x00046978
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

		// Token: 0x0600108A RID: 4234 RVA: 0x000487F8 File Offset: 0x000469F8
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

		// Token: 0x0600108B RID: 4235 RVA: 0x00048881 File Offset: 0x00046A81
		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(base.transform.position, this.overlapSphereRadius);
		}

		// Token: 0x04000FD6 RID: 4054
		public bool enableOverlapSphere = true;

		// Token: 0x04000FD7 RID: 4055
		public float overlapSphereRadius;

		// Token: 0x04000FD8 RID: 4056
		public float overlapSphereFrequency;

		// Token: 0x04000FD9 RID: 4057
		public string switchDownSoundString;

		// Token: 0x04000FDA RID: 4058
		public string switchUpSoundString;

		// Token: 0x04000FDB RID: 4059
		public UnityEvent OnSwitchDown;

		// Token: 0x04000FDC RID: 4060
		public UnityEvent OnSwitchUp;

		// Token: 0x04000FDD RID: 4061
		public AnimationCurve switchVisualPositionFromUpToDown;

		// Token: 0x04000FDE RID: 4062
		public AnimationCurve switchVisualPositionFromDownToUp;

		// Token: 0x04000FDF RID: 4063
		public Transform switchVisualTransform;

		// Token: 0x04000FE0 RID: 4064
		private float overlapSphereStopwatch;

		// Token: 0x04000FE1 RID: 4065
		private float animationStopwatch;

		// Token: 0x04000FE2 RID: 4066
		private bool switchDown;
	}
}
