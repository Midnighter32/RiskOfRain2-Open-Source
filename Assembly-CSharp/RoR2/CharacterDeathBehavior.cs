using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200018A RID: 394
	public class CharacterDeathBehavior : MonoBehaviour
	{
		// Token: 0x060007F7 RID: 2039 RVA: 0x00022934 File Offset: 0x00020B34
		public void OnDeath()
		{
			if (Util.HasEffectiveAuthority(base.gameObject))
			{
				if (this.deathStateMachine)
				{
					this.deathStateMachine.SetNextState(EntityState.Instantiate(this.deathState));
				}
				EntityStateMachine[] array = this.idleStateMachine;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetNextState(new Idle());
				}
			}
			base.gameObject.layer = LayerIndex.debris.intVal;
			CharacterMotor component = base.GetComponent<CharacterMotor>();
			if (component)
			{
				component.Motor.RebuildCollidableLayers();
			}
			ILifeBehavior[] components = base.GetComponents<ILifeBehavior>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnDeathStart();
			}
			ModelLocator component2 = base.GetComponent<ModelLocator>();
			if (component2)
			{
				Transform modelTransform = component2.modelTransform;
				if (modelTransform)
				{
					components = modelTransform.GetComponents<ILifeBehavior>();
					for (int i = 0; i < components.Length; i++)
					{
						components[i].OnDeathStart();
					}
				}
			}
		}

		// Token: 0x04000869 RID: 2153
		[Tooltip("The state machine to set the state of when this character is killed.")]
		public EntityStateMachine deathStateMachine;

		// Token: 0x0400086A RID: 2154
		[Tooltip("The state to enter when this character is killed.")]
		public SerializableEntityStateType deathState;

		// Token: 0x0400086B RID: 2155
		[Tooltip("The state machine(s) to set to idle when this character is killed.")]
		public EntityStateMachine[] idleStateMachine;
	}
}
