using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibiliyCone : MonoBehaviour
{
    public float coneAngle;

    void Update()
    {
        foreach (Collider col in Physics.OverlapSphere(transform.position, 15))
        {
            if (col.tag == "Enemy")
            {

                Vector3 enemyToPlayer = col.transform.position - transform.position;
                Vector3 direction = enemyToPlayer / enemyToPlayer.magnitude;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, 20))
                {
                    if (Mathf.Abs(Vector3.Angle(transform.forward, enemyToPlayer)) <= coneAngle && hit.collider.tag == "Enemy")
                    {
                        hit.collider.transform.Find("Body").GetComponent<Renderer>().enabled = true;
                        col.transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1")
                            .Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm").Find("mixamorig:RightForeArm")
                            .Find("mixamorig:RightHand").Find("Pistol").GetComponent<Renderer>().enabled = true;
                        hit.collider.transform.Find("Health Bar").gameObject.SetActive(true);
                    }
                    else
                    {
                        col.transform.Find("Body").GetComponent<Renderer>().enabled = false;
                        col.transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1")
                            .Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm").Find("mixamorig:RightForeArm")
                            .Find("mixamorig:RightHand").Find("Pistol").GetComponent<Renderer>().enabled = false;
                        col.transform.Find("Health Bar").gameObject.SetActive(false);

                    }
                    if (Vector3.Distance(transform.position, hit.collider.transform.position) > 20)
                    {
                        col.transform.Find("Body").GetComponent<Renderer>().enabled = false;
                        col.transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1")
                            .Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm").Find("mixamorig:RightForeArm")
                            .Find("mixamorig:RightHand").Find("Pistol").GetComponent<Renderer>().enabled = false;
                        col.transform.Find("Health Bar").gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
