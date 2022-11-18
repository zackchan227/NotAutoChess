using UnityEngine;

public class SpriteScaler : MonoBehaviour
{
    private void Awake()
    {
        scaleSprite();
    }

    // void LateUpdate()
    // {
    //     scaleSprite();
    // }

    private void scaleSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(
            worldScreenWidth / sr.sprite.bounds.size.x,
            worldScreenHeight / sr.sprite.bounds.size.y, 1);
        
        this.transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
    }

}