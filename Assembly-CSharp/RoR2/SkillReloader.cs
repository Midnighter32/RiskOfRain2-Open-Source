using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E5 RID: 997
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillReloader : MonoBehaviour
	{
		// Token: 0x060015BB RID: 5563 RVA: 0x00068181 File Offset: 0x00066381
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x0006818F File Offset: 0x0006638F
		private void Start()
		{
			this.timer = 0f;
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x0006819C File Offset: 0x0006639C
		private void FixedUpdate()
		{
			if (Util.HasEffectiveAuthority(this.networkIdentity))
			{
				bool flag = this.stateMachine.state.GetType() == typeof(Idle) && !this.stateMachine.HasPendingState();
				if (this.skill.stock < this.skill.maxStock && flag)
				{
					this.timer += Time.fixedDeltaTime;
				}
				else
				{
					this.timer = 0f;
				}
				if (this.timer >= this.reloadDelay || (this.skill.stock == 0 && flag))
				{
					this.stateMachine.SetNextState(EntityState.Instantiate(this.reloadState));
				}
			}
		}

		// Token: 0x0400191C RID: 6428
		private NetworkIdentity networkIdentity;

		// Token: 0x0400191D RID: 6429
		public GenericSkill skill;

		// Token: 0x0400191E RID: 6430
		public EntityStateMachine stateMachine;

		// Token: 0x0400191F RID: 6431
		public SerializableEntityStateType reloadState;

		// Token: 0x04001920 RID: 6432
		public float reloadDelay = 0.2f;

		// Token: 0x04001921 RID: 6433
		private float timer;
	}
}
