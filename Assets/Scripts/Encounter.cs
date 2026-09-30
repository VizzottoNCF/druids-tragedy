using UnityEngine;

public class Encounter : MonoBehaviour
{
    public EnemyWave wave;
    public bool available = false;
    public bool complete = false;
    public Encounter dependancy = null;
    private Collider2D collider;
    private SpriteRenderer sr;
    private LineRenderer lr;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        CheckAvailability();
    }
    private void CheckAvailability()
    {
        // for encounters without dependancies
        if (dependancy != null)
        {
            // when dependancy has been completed, makes it available
            if (dependancy.complete)
            {
                sr.color = Color.white;

                // check if its already completed, if not, make it available
                if (!complete)
                {
                    available = true;
                }
                else
                {
                    available = false;
                }
            }
        }
        else // if there is no dependancy
        {
            // check if its already completed, if not, make it available
            if (!complete)
            {
                available = true;
            }
            else
            {
                available = false;
            }
        }
    }

    private void OnMouseDown()
    {
        if (available)
        {
            MatchData.Instance.PickNewEncounter(wave);
            Complete();
            Debug.Log($"{gameObject.name} has been complete");
        }
        else
        {
            Debug.Log($"{gameObject.name} is not available");
        }
    }

    private void Complete()
    {
        complete = true;

        // create gradient
        var gradient = new Gradient();
        var colors = new GradientColorKey[1];
        var alphas = new GradientAlphaKey[1];
        colors[0] = new GradientColorKey(Color.white, 1.0f);
        alphas[0] = new GradientAlphaKey(1.0f, 1.0f);
        gradient.SetKeys(colors, alphas);

        lr.colorGradient = gradient;
    }
}
