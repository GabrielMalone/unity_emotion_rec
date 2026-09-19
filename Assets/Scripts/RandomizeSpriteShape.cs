using UnityEngine;
using UnityEngine.U2D;

public class RandomizeSpriteShape : MonoBehaviour
{
    private SpriteShapeController controller;

    void Start()
    {
        controller = GetComponent<SpriteShapeController>();

        RandomizeSprites();
    }

    void RandomizeSprites()
    {
        Spline spline = controller.spline;

        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            // Change 5 to however many sprites
            // you put in your Sprite Shape list
            int randomSprite = Random.Range(0, 5);

            spline.SetSpriteIndex(i, randomSprite);
        }

        controller.RefreshSpriteShape();
    }
}