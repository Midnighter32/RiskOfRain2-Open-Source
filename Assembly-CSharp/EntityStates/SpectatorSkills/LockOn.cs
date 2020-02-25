using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;

namespace EntityStates.SpectatorSkills
{
	// Token: 0x02000786 RID: 1926
	public class LockOn : BaseSkillState
	{
		// Token: 0x06002C39 RID: 11321 RVA: 0x000BA9D8 File Offset: 0x000B8BD8
		public override void OnEnter()
		{
			base.OnEnter();
			RaycastHit raycastHit;
			if (base.inputBank.GetAimRaycast(float.PositiveInfinity, out raycastHit))
			{
				this.targetPoint = raycastHit.point;
			}
			else
			{
				this.outer.SetNextStateToMain();
			}
			RoR2Application.onUpdate += this.LookAtTarget;
			RoR2Application.onLateUpdate += this.LookAtTarget;
		}

		// Token: 0x06002C3A RID: 11322 RVA: 0x000BAA3B File Offset: 0x000B8C3B
		public override void OnExit()
		{
			RoR2Application.onLateUpdate -= this.LookAtTarget;
			RoR2Application.onUpdate -= this.LookAtTarget;
			base.OnExit();
		}

		// Token: 0x06002C3B RID: 11323 RVA: 0x000BAA65 File Offset: 0x000B8C65
		public override void Update()
		{
			base.Update();
			if (base.isAuthority && !base.IsKeyDownAuthority())
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002C3C RID: 11324 RVA: 0x000BAA88 File Offset: 0x000B8C88
		private void LookAtTarget()
		{
			ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				CameraRigController cameraRigController = readOnlyInstancesList[i];
				if (cameraRigController.target == base.gameObject)
				{
					cameraRigController.SetPitchYawFromLookVector(this.targetPoint - base.transform.position);
				}
			}
		}

		// Token: 0x06002C3D RID: 11325 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400283B RID: 10299
		private Vector3 targetPoint;
	}
}
