using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001CE RID: 462
	public class DamageDisplay : MonoBehaviour
	{
		// Token: 0x060009E1 RID: 2529 RVA: 0x0002B092 File Offset: 0x00029292
		static DamageDisplay()
		{
			UICamera.onUICameraPreCull += DamageDisplay.OnUICameraPreCull;
			RoR2Application.onUpdate += DamageDisplay.UpdateAll;
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060009E2 RID: 2530 RVA: 0x0002B0CF File Offset: 0x000292CF
		public static ReadOnlyCollection<DamageDisplay> readOnlyInstancesList
		{
			get
			{
				return DamageDisplay._readOnlyInstancesList;
			}
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0002B0D8 File Offset: 0x000292D8
		private void Start()
		{
			this.velocity = Vector3.Normalize(Vector3.up + new Vector3(UnityEngine.Random.Range(-this.offset, this.offset), 0f, UnityEngine.Random.Range(-this.offset, this.offset))) * this.magnitude;
			DamageDisplay.instancesList.Add(this);
			this.internalPosition = base.transform.position;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0002B14F File Offset: 0x0002934F
		private void OnDestroy()
		{
			DamageDisplay.instancesList.Remove(this);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0002B160 File Offset: 0x00029360
		public void SetValues(GameObject victim, GameObject attacker, float damage, bool crit, DamageColorIndex damageColorIndex)
		{
			this.victimTeam = TeamIndex.Neutral;
			this.attackerTeam = TeamIndex.Neutral;
			this.scale = 1f;
			this.victim = victim;
			this.attacker = attacker;
			this.crit = crit;
			this.baseColor = DamageColor.FindColor(damageColorIndex);
			string text = Mathf.CeilToInt(Mathf.Abs(damage)).ToString();
			this.heal = (damage < 0f);
			if (this.heal)
			{
				damage = -damage;
				base.transform.parent = victim.transform;
				text = "+" + text;
				this.baseColor = DamageColor.FindColor(DamageColorIndex.Heal);
				this.baseOutlineColor = this.baseColor * Color.gray;
			}
			if (victim)
			{
				TeamComponent component = victim.GetComponent<TeamComponent>();
				if (component)
				{
					this.victimTeam = component.teamIndex;
				}
			}
			if (attacker)
			{
				TeamComponent component2 = attacker.GetComponent<TeamComponent>();
				if (component2)
				{
					this.attackerTeam = component2.teamIndex;
				}
			}
			if (crit)
			{
				text += "!";
				this.baseOutlineColor = Color.red;
			}
			this.textMeshComponent.text = text;
			this.UpdateMagnitude();
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0002B28C File Offset: 0x0002948C
		private void UpdateMagnitude()
		{
			float fontSize = this.magnitudeCurve.Evaluate(this.life / this.maxLife) * this.textMagnitude * this.scale;
			this.textMeshComponent.fontSize = fontSize;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0002B2CC File Offset: 0x000294CC
		private static void UpdateAll()
		{
			for (int i = DamageDisplay.instancesList.Count - 1; i >= 0; i--)
			{
				DamageDisplay.instancesList[i].DoUpdate();
			}
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0002B300 File Offset: 0x00029500
		private void DoUpdate()
		{
			this.UpdateMagnitude();
			this.life += Time.deltaTime;
			if (this.life >= this.maxLife)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			this.velocity += this.gravity * Vector3.down * Time.deltaTime;
			this.internalPosition += this.velocity * Time.deltaTime;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0002B38C File Offset: 0x0002958C
		private static void OnUICameraPreCull(UICamera uiCamera)
		{
			Camera camera = uiCamera.camera;
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			GameObject target = uiCamera.cameraRigController.target;
			TeamIndex targetTeamIndex = uiCamera.cameraRigController.targetTeamIndex;
			for (int i = 0; i < DamageDisplay.instancesList.Count; i++)
			{
				DamageDisplay damageDisplay = DamageDisplay.instancesList[i];
				Color b = Color.white;
				if (!damageDisplay.heal)
				{
					if (targetTeamIndex == damageDisplay.victimTeam)
					{
						b = new Color(0.5568628f, 0.29411766f, 0.6039216f);
					}
					else if (targetTeamIndex == damageDisplay.attackerTeam && target != damageDisplay.attacker)
					{
						b = Color.gray;
					}
				}
				damageDisplay.textMeshComponent.color = Color.Lerp(Color.white, damageDisplay.baseColor * b, damageDisplay.life / 0.2f);
				damageDisplay.textMeshComponent.outlineColor = Color.Lerp(Color.white, damageDisplay.baseOutlineColor * b, damageDisplay.life / 0.2f);
				Vector3 position = damageDisplay.internalPosition;
				Vector3 vector = sceneCam.WorldToScreenPoint(position);
				vector.z = ((vector.z > 0f) ? 1f : -1f);
				Vector3 position2 = camera.ScreenToWorldPoint(vector);
				damageDisplay.transform.position = position2;
			}
		}

		// Token: 0x04000A0E RID: 2574
		private static List<DamageDisplay> instancesList = new List<DamageDisplay>();

		// Token: 0x04000A0F RID: 2575
		private static ReadOnlyCollection<DamageDisplay> _readOnlyInstancesList = new ReadOnlyCollection<DamageDisplay>(DamageDisplay.instancesList);

		// Token: 0x04000A10 RID: 2576
		public TextMeshPro textMeshComponent;

		// Token: 0x04000A11 RID: 2577
		public AnimationCurve magnitudeCurve;

		// Token: 0x04000A12 RID: 2578
		public float maxLife = 3f;

		// Token: 0x04000A13 RID: 2579
		public float gravity = 9.81f;

		// Token: 0x04000A14 RID: 2580
		public float magnitude = 3f;

		// Token: 0x04000A15 RID: 2581
		public float offset = 20f;

		// Token: 0x04000A16 RID: 2582
		private Vector3 velocity;

		// Token: 0x04000A17 RID: 2583
		public float textMagnitude = 0.01f;

		// Token: 0x04000A18 RID: 2584
		private float vel;

		// Token: 0x04000A19 RID: 2585
		private float life;

		// Token: 0x04000A1A RID: 2586
		private float scale = 1f;

		// Token: 0x04000A1B RID: 2587
		[HideInInspector]
		public Color baseColor = Color.white;

		// Token: 0x04000A1C RID: 2588
		[HideInInspector]
		public Color baseOutlineColor = Color.gray;

		// Token: 0x04000A1D RID: 2589
		private GameObject victim;

		// Token: 0x04000A1E RID: 2590
		private GameObject attacker;

		// Token: 0x04000A1F RID: 2591
		private TeamIndex victimTeam;

		// Token: 0x04000A20 RID: 2592
		private TeamIndex attackerTeam;

		// Token: 0x04000A21 RID: 2593
		private bool crit;

		// Token: 0x04000A22 RID: 2594
		private bool heal;

		// Token: 0x04000A23 RID: 2595
		private Vector3 internalPosition;
	}
}
