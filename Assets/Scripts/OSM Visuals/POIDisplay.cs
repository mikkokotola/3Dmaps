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
	public float heightAdjustment = 0.025f;

	public void DisplayPOINode(DisplayNode poiNode,Icon icon) {
        if (PositionService.IsWithinBounds(poiNode.x, poiNode.y, mapData)) {
			Vector3 nodePosition = PositionService.GetUnityPosition(poiNode, heightAdjustment, mapData);
			GenerateNodeGameObject(nodePosition, icon); 
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

    public void GenerateNodeGameObject(Vector3 nodePosition, Icon icon) {

		
        GameObject newNode = Instantiate(nodeGameObject);
		SpriteRenderer renderer = newNode.GetComponent<SpriteRenderer>();
		renderer.sprite = icon.sprite;
        newNode.transform.position = nodePosition;
        newNode.transform.SetParent(this.transform);
    }    
}
