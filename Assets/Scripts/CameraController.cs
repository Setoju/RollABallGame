using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 originalOffset;
    private Vector3 spinCenterOffset;
    private bool isSpinning = false;

    private float spinSpeed = 90f;
    private float spinRadius;
    private float spinAngle = 0f;

    void Start()
    {
        originalOffset = transform.position - player.transform.position;
        spinRadius = originalOffset.magnitude;
    }

    void LateUpdate()
    {
        if (player == null) return;

        if (isSpinning)
        {
            spinAngle += spinSpeed * Time.deltaTime;
            float radians = spinAngle * Mathf.Deg2Rad;

            Vector3 spinOffset = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)) * spinRadius;
            spinOffset.y = originalOffset.y;

            transform.position = player.transform.position + spinOffset;
            transform.LookAt(player.transform.position);
        }
        else
        {
            transform.position = player.transform.position + originalOffset;
        }
    }

    public void StartSpinning()
    {
        isSpinning = true;
        spinAngle = 0f;
    }

    public void StopSpinning()
    {
        isSpinning = false;

        transform.position = player.transform.position + originalOffset;
        transform.LookAt(player.transform.position);
    }
}
