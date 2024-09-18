using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FirstGameProg2Game
{
    public class HitNumber : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text hitText;

        public void Play(int damage, Vector3 dir)
        {
            hitText.text = damage.ToString();
            hitText.fontSize = Mathf.Lerp(125, 400, damage / 10000f);

            StartCoroutine(DisappearCoroutine(1f, dir));
        }

        private IEnumerator DisappearCoroutine(float disappearDuration, Vector3 dir)
        {
            Vector3 endPos = transform.position + 0.5f * dir;

            for (float t = 0; t < disappearDuration / 2; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, endPos, t / (disappearDuration));

                yield return null;
            }

            hitText.color = Color.red;
            Color endColor = Color.red;
            endColor.a = 0f;
            for(float t = 0; t < disappearDuration/2; t += Time.deltaTime)
            {
                hitText.color = Color.Lerp(Color.red, endColor, t/(disappearDuration/2));
                transform.position = Vector3.Lerp(transform.position, endPos,  (t + (disappearDuration/2)) / disappearDuration);

                yield return null;
            }
            hitText.color = endColor;

            Destroy(gameObject);
        }
    }

}
