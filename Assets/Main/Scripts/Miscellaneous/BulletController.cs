using UnityEngine;

public class BulletController : MonoBehaviour//PunCallbacks, IPunObservable
{
    [SerializeField]
    private float speed = 30f;
    private string tagToIgnore;

    public string TagToIgnore
    {
       set
       {
           tagToIgnore = value;
       }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(tagToIgnore))
            Destroy(gameObject);
    }
}
