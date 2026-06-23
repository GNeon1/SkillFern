/*
 * HoverText.cs
 * 
 * Controls the hovertext animation on the skills page
 */

using TMPro;
using UnityEngine;

namespace SkillFern.UI
{
    public class HoverText : MonoBehaviour
    {
        public float yOffset = 10f; // how far down to start the animation
        public float flyInTime = 0.1f; // how fast the text should fly in
        public float fadeInTime = 0.05f; // how long the fade in should take
        public float fadeOutTime = 0.2f; // how long the fade out should take

        private TextMeshProUGUI textMesh; // textmesh to use for display
        private bool shown; // whether the text is currently shown

        public void Start()
        {
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
            textMesh.alpha = 0f;
            textMesh.SetText("");
            shown = false;
        }

        /*
         * Update the position and alpha of the textmesh based on it's state
         */
        public void Update() {
            if (shown)
            {
                if (textMesh.alpha < 1f)
                    textMesh.alpha +=  Time.deltaTime / fadeInTime;

                Vector2 currentPosition = textMesh.rectTransform.anchoredPosition;
                float speed = yOffset / flyInTime;

                if (currentPosition.y < 0f)
                    currentPosition.y = Mathf.MoveTowards(currentPosition.y, 0f, speed * Time.deltaTime);

                textMesh.rectTransform.anchoredPosition = currentPosition;
            }
            else
            {
                if (textMesh.alpha > 0f)
                    textMesh.alpha -= Time.deltaTime / fadeOutTime;
            }
        }

        /*
         * Set the displayed text and restart the animation
         * 
         * @param text - text to display
         */
        public void SetText(string text)
        {
            if (!shown) {
                textMesh.alpha = 0f;
                textMesh.SetText(text);
                shown = true;

                textMesh.rectTransform.anchoredPosition = new Vector2(textMesh.rectTransform.anchoredPosition.x, -yOffset);
            }
        }

        /*
         * Stop showing the displayed text
         */
        public void Clear() {
            shown = false;
        }
    }
}
