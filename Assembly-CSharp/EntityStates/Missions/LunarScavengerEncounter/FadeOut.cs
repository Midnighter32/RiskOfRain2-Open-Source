using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace EntityStates.Missions.LunarScavengerEncounter
{
	// Token: 0x020007B7 RID: 1975
	public class FadeOut : BaseState
	{
		// Token: 0x06002D21 RID: 11553 RVA: 0x000BE8C4 File Offset: 0x000BCAC4
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				this.startTime = Run.TimeStamp.now + FadeOut.delay;
			}
			this.light = base.GetComponent<ChildLocator>().FindChild("PrimaryLight").GetComponent<Light>();
			this.initialIntensity = this.light.intensity;
			this.initialAmbientIntensity = RenderSettings.ambientIntensity;
			this.initialAmbientColor = RenderSettings.ambientLight;
			this.initialFogColor = RenderSettings.fogColor;
			this.light.GetComponent<FlickerLight>().enabled = false;
			this.postProcessVolume = base.GetComponent<PostProcessVolume>();
			this.postProcessVolume.enabled = true;
		}

		// Token: 0x06002D22 RID: 11554 RVA: 0x000BE969 File Offset: 0x000BCB69
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.startTime);
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x000BE97E File Offset: 0x000BCB7E
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.startTime = reader.ReadTimeStamp();
		}

		// Token: 0x06002D24 RID: 11556 RVA: 0x000BE994 File Offset: 0x000BCB94
		public override void Update()
		{
			base.Update();
			float num = Mathf.Clamp01(this.startTime.timeSince / FadeOut.duration);
			num *= num;
			this.light.intensity = Mathf.Lerp(this.initialIntensity, 0f, num);
			RenderSettings.ambientIntensity = Mathf.Lerp(this.initialAmbientIntensity, 0f, num);
			RenderSettings.ambientLight = Color.Lerp(this.initialAmbientColor, Color.black, num);
			RenderSettings.fogColor = Color.Lerp(this.initialFogColor, Color.black, num);
			this.postProcessVolume.weight = num;
		}

		// Token: 0x06002D25 RID: 11557 RVA: 0x000BEA2C File Offset: 0x000BCC2C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
			if (this.startTime.timeSince > FadeOut.duration)
			{
				foreach (CharacterBody characterBody in CharacterBody.readOnlyInstancesList)
				{
					if (characterBody.hasEffectiveAuthority)
					{
						EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(characterBody.gameObject, "Body");
						if (entityStateMachine && !(entityStateMachine.state is Idle))
						{
							entityStateMachine.SetInterruptState(new Idle(), InterruptPriority.Frozen);
						}
					}
				}
			}
		}

		// Token: 0x06002D26 RID: 11558 RVA: 0x000BEAD4 File Offset: 0x000BCCD4
		private void FixedUpdateServer()
		{
			if ((this.startTime + FadeOut.duration).hasPassed && !this.finished)
			{
				this.finished = true;
				Run.instance.BeginGameOver(GameResultType.Unknown);
			}
		}

		// Token: 0x04002964 RID: 10596
		public static float delay;

		// Token: 0x04002965 RID: 10597
		public static float duration;

		// Token: 0x04002966 RID: 10598
		private Run.TimeStamp startTime;

		// Token: 0x04002967 RID: 10599
		private Light light;

		// Token: 0x04002968 RID: 10600
		private float initialIntensity;

		// Token: 0x04002969 RID: 10601
		private float initialAmbientIntensity;

		// Token: 0x0400296A RID: 10602
		private Color initialAmbientColor;

		// Token: 0x0400296B RID: 10603
		private Color initialFogColor;

		// Token: 0x0400296C RID: 10604
		private PostProcessVolume postProcessVolume;

		// Token: 0x0400296D RID: 10605
		private bool finished;
	}
}
