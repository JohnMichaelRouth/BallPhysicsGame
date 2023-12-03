using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    public float floatUpSpeed = 1f;
    public float fadeOutTime = 1f;
    private SpriteRenderer spriteRenderer;
    private float startTime;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startTime = Time.time;
    }

    void Update()
    {
        // Float up
        transform.position += new Vector3(0, floatUpSpeed * Time.deltaTime, 0);

        // Fade out
        float elapsed = Time.time - startTime;
        if (elapsed < fadeOutTime)
        {
            float alpha = Mathf.Clamp01(1 - (elapsed / fadeOutTime));
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }
        else
        {
            Destroy(gameObject); // Destroy the object after fade out
        }
    }

    public void SetScoreSprite(Sprite scoreSprite)
    {
        spriteRenderer.sprite = scoreSprite;
    }
}
