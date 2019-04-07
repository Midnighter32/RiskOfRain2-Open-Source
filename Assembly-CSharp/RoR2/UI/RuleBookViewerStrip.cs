using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200062D RID: 1581
	public class RuleBookViewerStrip : MonoBehaviour
	{
		// Token: 0x06002373 RID: 9075 RVA: 0x000A6CDC File Offset: 0x000A4EDC
		private RuleChoiceController CreateChoice()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.choicePrefab, this.choiceContainer);
			gameObject.SetActive(true);
			RuleChoiceController component = gameObject.GetComponent<RuleChoiceController>();
			component.strip = this;
			return component;
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x000A6D02 File Offset: 0x000A4F02
		private void DestroyChoice(RuleChoiceController choiceController)
		{
			UnityEngine.Object.Destroy(choiceController.gameObject);
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x000A6D10 File Offset: 0x000A4F10
		public void SetData(List<RuleChoiceDef> newChoices, int choiceIndex)
		{
			this.AllocateChoices(newChoices.Count);
			int num = this.currentDisplayChoiceIndex;
			int count = newChoices.Count;
			bool canVote = count > 1;
			for (int i = 0; i < count; i++)
			{
				this.choiceControllers[i].canVote = canVote;
				this.choiceControllers[i].SetChoice(newChoices[i]);
				if (newChoices[i].localIndex == choiceIndex)
				{
					num = i;
				}
			}
			this.currentDisplayChoiceIndex = num;
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x000A6D8C File Offset: 0x000A4F8C
		private void AllocateChoices(int desiredCount)
		{
			while (this.choiceControllers.Count > desiredCount)
			{
				int index = this.choiceControllers.Count - 1;
				this.DestroyChoice(this.choiceControllers[index]);
				this.choiceControllers.RemoveAt(index);
			}
			while (this.choiceControllers.Count < desiredCount)
			{
				this.choiceControllers.Add(this.CreateChoice());
			}
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x000A6DF8 File Offset: 0x000A4FF8
		public void Update()
		{
			if (this.choiceControllers.Count == 0)
			{
				return;
			}
			if (this.currentDisplayChoiceIndex >= this.choiceControllers.Count)
			{
				this.currentDisplayChoiceIndex = this.choiceControllers.Count - 1;
			}
			Vector3 localPosition = this.choiceControllers[this.currentDisplayChoiceIndex].transform.localPosition;
			float target = 0f;
			RectTransform.Axis axis = this.movementAxis;
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					target = -localPosition.y;
				}
			}
			else
			{
				target = -localPosition.x;
			}
			this.currentPosition = Mathf.SmoothDamp(this.currentPosition, target, ref this.velocity, this.movementDuration);
			this.UpdatePosition();
		}

		// Token: 0x06002378 RID: 9080 RVA: 0x000A6EA1 File Offset: 0x000A50A1
		private void OnEnable()
		{
			this.UpdatePosition();
		}

		// Token: 0x06002379 RID: 9081 RVA: 0x000A6EAC File Offset: 0x000A50AC
		private void UpdatePosition()
		{
			Vector3 localPosition = this.choiceContainer.localPosition;
			RectTransform.Axis axis = this.movementAxis;
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					localPosition.y = this.currentPosition;
				}
			}
			else
			{
				localPosition.x = this.currentPosition;
			}
			this.choiceContainer.localPosition = localPosition;
		}

		// Token: 0x04002668 RID: 9832
		public GameObject choicePrefab;

		// Token: 0x04002669 RID: 9833
		public RectTransform choiceContainer;

		// Token: 0x0400266A RID: 9834
		public RectTransform.Axis movementAxis = RectTransform.Axis.Vertical;

		// Token: 0x0400266B RID: 9835
		public float movementDuration = 0.1f;

		// Token: 0x0400266C RID: 9836
		private RuleDef currentRuleDef;

		// Token: 0x0400266D RID: 9837
		public readonly List<RuleChoiceController> choiceControllers = new List<RuleChoiceController>();

		// Token: 0x0400266E RID: 9838
		public int currentDisplayChoiceIndex;

		// Token: 0x0400266F RID: 9839
		private float velocity;

		// Token: 0x04002670 RID: 9840
		private float currentPosition;
	}
}
