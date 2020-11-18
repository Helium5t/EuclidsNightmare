using UnityEngine;

public class ResizingTunnel : MonoBehaviour
    {

    [SerializeField]
    private GameObject rightWall, leftWall, topWall, bottomWall, entrance, exit;

    [SerializeField]
    public float scaling = .5f;

    private float length;
    private Collider tunnel_area;

    private void OnDrawGizmos()
		{
        Debug.DrawLine(entrance.transform.position, exit.transform.position);
		}

    private float distance_from_entrance_perc(Vector3 point)
        {
        Vector3 from = entrance.transform.position;
        return Vector3.Distance(point, new Vector3(point.x, point.y, from.z))/length;
        }

	void Start()
        {
        tunnel_area = GetComponent<BoxCollider>();
        length = Vector3.Distance(entrance.transform.position, exit.transform.position);
        }

    private Transform player_transform;
    private float source_scale;
    private enum Action{ SHRINK, ENLARGE};
    private Action action;

    private void OnTriggerEnter(Collider collider)
        {
        GameObject oth = collider.gameObject;
        //if (oth.tag == "Player")
            {
            action = (distance_from_entrance_perc(oth.transform.position) < .5f) ? Action.ENLARGE : Action.SHRINK;

            Debug.Log("Entered " + (action == Action.ENLARGE?"enlarge":"shrink") + " side");
            player_transform = oth.transform;
            source_scale = player_transform.localScale.x;
            }
        }

	private void OnTriggerStay(Collider collider)
        {
        GameObject oth = collider.gameObject;
        //if (oth.tag == "Player")
        if(tunnel_area.bounds.Contains(oth.transform.position))
            {
            float perc = distance_from_entrance_perc(oth.transform.position);
            float relative_scaling;

            if (action == Action.ENLARGE)
                { relative_scaling = source_scale + perc * scaling; }
            else //if(action == Action.SHRINK)
                { relative_scaling = source_scale - (1 - perc) * scaling; }

            //Debug.Log(perc + "% > " + relative_scaling);

            player_transform.localScale = new Vector3(relative_scaling, relative_scaling, relative_scaling);
            }
        }

    private void OnTriggerExit(Collider collider)
        {
        GameObject oth = collider.gameObject;
        //if (oth.tag == "Player")
            {
            Debug.Log("Left");
            }
        }
	} 