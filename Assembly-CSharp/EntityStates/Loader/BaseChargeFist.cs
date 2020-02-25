using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007E1 RID: 2017
	public class BaseChargeFist : BaseSkillState
	{
		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06002DEC RID: 11756 RVA: 0x000C383E File Offset: 0x000C1A3E
		// (set) Token: 0x06002DED RID: 11757 RVA: 0x000C3846 File Offset: 0x000C1A46
		private protected float chargeDuration { protected get; private set; }

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06002DEE RID: 11758 RVA: 0x000C384F File Offset: 0x000C1A4F
		// (set) Token: 0x06002DEF RID: 11759 RVA: 0x000C3857 File Offset: 0x000C1A57
		private protected float charge { protected get; private set; }

		// Token: 0x06002DF0 RID: 11760 RVA: 0x000C3860 File Offset: 0x000C1A60
		public override void OnEnter()
		{
			base.OnEnter();
			this.chargeDuration = this.baseChargeDuration / this.attackSpeedStat;
			Util.PlaySound(BaseChargeFist.enterSFXString, base.gameObject);
			this.soundID = Util.PlaySound(BaseChargeFist.startChargeLoopSFXString, base.gameObject);
		}

		// Token: 0x06002DF1 RID: 11761 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06002DF2 RID: 11762 RVA: 0x000C38B0 File Offset: 0x000C1AB0
		public override void OnExit()
		{
			BaseChargeFist.ArcVisualizer arcVisualizer = this.arcVisualizer;
			if (arcVisualizer != null)
			{
				arcVisualizer.Dispose();
			}
			if (this.chargeVfxInstanceTransform)
			{
				EntityState.Destroy(this.chargeVfxInstanceTransform.gameObject);
				base.PlayAnimation("Gesture, Additive", "Empty");
				base.PlayAnimation("Gesture, Override", "Empty");
				if (this.defaultCrosshairPrefab)
				{
					base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
				}
				this.chargeVfxInstanceTransform = null;
			}
			base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
			Util.PlaySound(BaseChargeFist.endChargeLoopSFXString, base.gameObject);
			base.OnExit();
		}

		// Token: 0x06002DF3 RID: 11763 RVA: 0x000C3958 File Offset: 0x000C1B58
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.charge = Mathf.Clamp01(base.fixedAge / this.chargeDuration);
			AkSoundEngine.SetRTPCValueByPlayingID("loaderShift_chargeAmount", this.charge * 100f, this.soundID);
			base.characterBody.SetSpreadBloom(this.charge, true);
			base.characterBody.SetAimTimer(3f);
			if (this.charge >= BaseChargeFist.minChargeForChargedAttack && !this.chargeVfxInstanceTransform && BaseChargeFist.chargeVfxPrefab)
			{
				this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
				if (BaseChargeFist.crosshairOverridePrefab)
				{
					base.characterBody.crosshairPrefab = BaseChargeFist.crosshairOverridePrefab;
				}
				Transform transform = base.FindModelChild(BaseChargeFist.chargeVfxChildLocatorName);
				if (transform)
				{
					this.chargeVfxInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(BaseChargeFist.chargeVfxPrefab, transform).transform;
					ScaleParticleSystemDuration component = this.chargeVfxInstanceTransform.GetComponent<ScaleParticleSystemDuration>();
					if (component)
					{
						component.newDuration = (1f - BaseChargeFist.minChargeForChargedAttack) * this.chargeDuration;
					}
				}
				base.PlayCrossfade("Gesture, Additive", "ChargePunchIntro", "ChargePunchIntro.playbackRate", this.chargeDuration, 0.1f);
				base.PlayCrossfade("Gesture, Override", "ChargePunchIntro", "ChargePunchIntro.playbackRate", this.chargeDuration, 0.1f);
			}
			if (this.chargeVfxInstanceTransform)
			{
				base.characterMotor.walkSpeedPenaltyCoefficient = BaseChargeFist.walkSpeedCoefficient;
			}
			if (base.isAuthority)
			{
				this.AuthorityFixedUpdate();
			}
		}

		// Token: 0x06002DF4 RID: 11764 RVA: 0x000C3ADF File Offset: 0x000C1CDF
		public override void Update()
		{
			base.Update();
			Mathf.Clamp01(base.age / this.chargeDuration);
		}

		// Token: 0x06002DF5 RID: 11765 RVA: 0x000C3AFA File Offset: 0x000C1CFA
		private void AuthorityFixedUpdate()
		{
			if (!this.ShouldKeepChargingAuthority())
			{
				this.outer.SetNextState(this.GetNextStateAuthority());
			}
		}

		// Token: 0x06002DF6 RID: 11766 RVA: 0x000AE005 File Offset: 0x000AC205
		protected virtual bool ShouldKeepChargingAuthority()
		{
			return base.IsKeyDownAuthority();
		}

		// Token: 0x06002DF7 RID: 11767 RVA: 0x000C3B15 File Offset: 0x000C1D15
		protected virtual EntityState GetNextStateAuthority()
		{
			return new SwingChargedFist
			{
				charge = this.charge
			};
		}

		// Token: 0x04002AF7 RID: 10999
		public static GameObject arcVisualizerPrefab;

		// Token: 0x04002AF8 RID: 11000
		public static float arcVisualizerSimulationLength;

		// Token: 0x04002AF9 RID: 11001
		public static int arcVisualizerVertexCount;

		// Token: 0x04002AFA RID: 11002
		[SerializeField]
		public float baseChargeDuration = 1f;

		// Token: 0x04002AFB RID: 11003
		public static float minChargeForChargedAttack;

		// Token: 0x04002AFC RID: 11004
		public static GameObject chargeVfxPrefab;

		// Token: 0x04002AFD RID: 11005
		public static string chargeVfxChildLocatorName;

		// Token: 0x04002AFE RID: 11006
		public static GameObject crosshairOverridePrefab;

		// Token: 0x04002AFF RID: 11007
		public static float walkSpeedCoefficient;

		// Token: 0x04002B00 RID: 11008
		public static string startChargeLoopSFXString;

		// Token: 0x04002B01 RID: 11009
		public static string endChargeLoopSFXString;

		// Token: 0x04002B02 RID: 11010
		public static string enterSFXString;

		// Token: 0x04002B03 RID: 11011
		private GameObject defaultCrosshairPrefab;

		// Token: 0x04002B05 RID: 11013
		private Transform chargeVfxInstanceTransform;

		// Token: 0x04002B07 RID: 11015
		private BaseChargeFist.ArcVisualizer arcVisualizer;

		// Token: 0x04002B08 RID: 11016
		private bool hasBegunToCharge;

		// Token: 0x04002B09 RID: 11017
		private int gauntlet;

		// Token: 0x04002B0A RID: 11018
		private uint soundID;

		// Token: 0x020007E2 RID: 2018
		private class ArcVisualizer : IDisposable
		{
			// Token: 0x06002DF9 RID: 11769 RVA: 0x000C3B3C File Offset: 0x000C1D3C
			public ArcVisualizer(GameObject arcVisualizerPrefab, float duration, int vertexCount)
			{
				this.arcVisualizerInstance = UnityEngine.Object.Instantiate<GameObject>(arcVisualizerPrefab);
				this.lineRenderer = this.arcVisualizerInstance.GetComponent<LineRenderer>();
				this.lineRenderer.positionCount = vertexCount;
				this.points = new Vector3[vertexCount];
				this.duration = duration;
			}

			// Token: 0x06002DFA RID: 11770 RVA: 0x000C3B8B File Offset: 0x000C1D8B
			public void Dispose()
			{
				EntityState.Destroy(this.arcVisualizerInstance);
			}

			// Token: 0x06002DFB RID: 11771 RVA: 0x000C3B98 File Offset: 0x000C1D98
			public void SetParameters(Vector3 origin, Vector3 initialVelocity, float characterMaxSpeed, float characterAcceleration)
			{
				this.arcVisualizerInstance.transform.position = origin;
				if (!this.lineRenderer.useWorldSpace)
				{
					Vector3 eulerAngles = Quaternion.LookRotation(initialVelocity).eulerAngles;
					eulerAngles.x = 0f;
					eulerAngles.z = 0f;
					Quaternion rotation = Quaternion.Euler(eulerAngles);
					this.arcVisualizerInstance.transform.rotation = rotation;
					origin = Vector3.zero;
					initialVelocity = Quaternion.Inverse(rotation) * initialVelocity;
				}
				else
				{
					this.arcVisualizerInstance.transform.rotation = Quaternion.LookRotation(Vector3.Cross(initialVelocity, Vector3.up));
				}
				float y = Physics.gravity.y;
				float num = this.duration / (float)this.points.Length;
				Vector3 vector = origin;
				Vector3 vector2 = initialVelocity;
				float num2 = num;
				float num3 = y * num2;
				float maxDistanceDelta = characterAcceleration * num2;
				for (int i = 0; i < this.points.Length; i++)
				{
					this.points[i] = vector;
					Vector2 vector3 = Util.Vector3XZToVector2XY(vector2);
					vector3 = Vector2.MoveTowards(vector3, Vector3.zero, maxDistanceDelta);
					vector2.x = vector3.x;
					vector2.z = vector3.y;
					vector2.y += num3;
					vector += vector2 * num2;
				}
				this.lineRenderer.SetPositions(this.points);
			}

			// Token: 0x04002B0B RID: 11019
			private readonly Vector3[] points;

			// Token: 0x04002B0C RID: 11020
			private readonly float duration;

			// Token: 0x04002B0D RID: 11021
			private readonly GameObject arcVisualizerInstance;

			// Token: 0x04002B0E RID: 11022
			private readonly LineRenderer lineRenderer;
		}
	}
}
