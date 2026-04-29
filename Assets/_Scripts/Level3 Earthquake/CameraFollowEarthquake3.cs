using UnityEngine;

public class CameraFollowEarthquake3 : MonoBehaviour
{
    public void ZoomToPlayer(Transform target)
    {
        StartCoroutine(Zoom(target));
    }

    System.Collections.IEnumerator Zoom(Transform target)
    {
        Camera cam = Camera.main;
        float t = 0;

        Vector3 startPos = cam.transform.position;
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10);

        float startSize = cam.orthographicSize;
        float targetSize = 3f;

        while (t < 1)
        {
            t += Time.deltaTime * 2f;

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }
    }
}
