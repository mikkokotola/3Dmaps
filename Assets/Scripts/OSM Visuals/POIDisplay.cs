using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Produces a representation of a point of interest by positioning each node 
/// to the map created from a MapData by creating a new gameObject.
/// </summary>

public class POIDisplay : MonoBehaviour {

	public MapData mapData;
	public GameObject nodeGameObject;    
    public Vector3 nodePosition;
	public float heightAdjustment = 0.005f;
  
    public Material material;
    	
        public void DisplayPOINode(DisplayNode poiNode) {
        
        if (IsWithinBounds(poiNode.x, poiNode.y)) {
			GenerateNodeGameObject(GenerateNode(poiNode)); 
		}           
    }
    

    public Vector3 GenerateNode (DisplayNode node) {

		float height = mapData.GetNormalized (node.x, node.y);

		float xFromCenter = node.x - mapData.GetWidth() / 2;
		float yFromCenter = (mapData.GetHeight() / 2) - node.y;

		float scale = 1 / ((float) Mathf.Max (mapData.GetWidth(), mapData.GetHeight()) - 1);

		Vector3 nodePosition = new Vector3 ((float) xFromCenter * scale, height + heightAdjustment, (float) yFromCenter * scale);
        return nodePosition;
    }

    public void GenerateNodeGameObject(Vector3 nodePosition) {

        GameObject newNode = Instantiate(nodeGameObject);
        newNode.transform.position = nodePosition;
        newNode.transform.SetParent(this.transform);
    }    


	public bool IsWithinBounds(int rawX, int rawY) {
		if (rawX < 0 || rawX > mapData.GetWidth() - 1) {
			return false;		
		}

		if (rawY < 0 || rawY > mapData.GetHeight() - 1) {
			return false;		
		}

		return true;
	}
}
