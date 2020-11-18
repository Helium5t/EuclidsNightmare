using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
    {
    [SerializeField]
    private uint bounces = 100;
    [SerializeField]
    Color rayColour = Color.red;

    // Update is called once per frame
    void Update()
        {
        castRay(transform.position, transform.forward);
        }

    void castRay(Vector3 position, Vector3 direction)
        {
		for (int i = 0; i < bounces; i++)
			{
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10, 1))
                {
                Debug.DrawLine(position, hit.point, rayColour);
                if (hit.collider.gameObject.tag == "Mirror")
                    {
                    position = hit.point;
                    direction = Vector3.Reflect(direction, hit.normal);
                    }
                else if (hit.collider.gameObject.tag == "Glass")
                    {
                    position = hit.point;
                    }
                else { break; }
                }
            else
                {
                Debug.DrawLine(position, direction * 1000, rayColour);
                break;
                }
			}
        }

    }