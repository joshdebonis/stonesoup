using UnityEngine;
using System.Collections;

public class RDetector : Tile
{
    public GameObject wallObj;
    BoxCollider2D bc;
    Vector2 extendSize, extendPos;
    Vector2 originalSize, originalOffset;
    void Start() {
        bc=GetComponent<BoxCollider2D>();

        originalSize=bc.size;
        originalOffset=bc.offset;

        Ray2D ray=new Ray2D((Vector2)bc.bounds.center+new Vector2(-bc.bounds.extents.x-.01f, 0), Vector2.left);
        RaycastHit2D hit=Physics2D.Raycast(ray.origin, ray.direction);
        if(!hit)
        {
            Debug.LogWarning("ray cast did not hit anything");
            return;
        }
        extendSize=new Vector2(bc.bounds.min.x-hit.point.x, bc.bounds.size.y);
        extendPos=new Vector2(hit.point.x+extendSize.x/2, bc.offset.y+transform.position.y);
        ToggleBC(true);
    }

    Coroutine toggleCoro;
    
    void ToggleBC(bool val) {
        wallObj.SetActive(val);
        wallObj.transform.localScale=new Vector3(extendSize.x, extendSize.y, 1);
        wallObj.transform.position=extendPos;
    }

	public override void tileDetected(Tile detectedTile) {
        base.tileDetected(detectedTile);
        if (toggleCoro != null)
            StopCoroutine(toggleCoro);
        toggleCoro=StartCoroutine(DelayToggleBCOn());
        ToggleBC(false);
	}
    IEnumerator DelayToggleBCOn()
    {
        yield return new WaitForSeconds(.3f);
        ToggleBC(true);
        toggleCoro=null;
    }
}