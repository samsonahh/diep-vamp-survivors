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

        public void Play(int damage)
        {
            hitText.text = damage.ToString();
            hitText.fontSize = Mathf.Lerp(125, 400, damage / 10000f);

            StartCoroutine(DisappearCoroutine(1f));
        }

        private IEnumerator DisappearCoroutine(float disappearDuration)
        {
            Vector3 endPos = transform.position + 0.5f * new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f).normalized;

            for (float t = 0; t < disappearDuration / 2; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, endPos, t / (disappearDuration));

                yield return null;
            }

            hitText.color = Color.red;
            for(float t = 0; t < disappearDuration/2; t += Time.deltaTime)
            {
                hitText.color = Color.Lerp(Color.red, Color.clear, t/(disappearDuration/2));
                transform.position = Vector3.Lerp(transform.position, endPos,  (t + (disappearDuration/2)) / disappearDuration);

                yield return null;
            }
            hitText.color = Color.clear;

            Destroy(gameObject);
        }
    }

}
