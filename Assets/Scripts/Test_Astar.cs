using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Astar : MonoBehaviour
{
    public void GetRandomNodeByLayer(NpcController npcController, int layerNumber, string buildingType)
    {
        StartCoroutine(GetRandomNodeByLayerCoroutine(npcController, layerNumber, buildingType));
    }

    IEnumerator GetRandomNodeByLayerCoroutine(NpcController npcController, int layerNumber, string buildingType)
    {
        Node node;

        if (npcController.target != npcStartPosTransformForReturnRandomNode.position
            && !npcController.firstEntrance)
        {

            npcController.StartDidntFoundNodeCalculateCoroutine();
        }
        else
        {
            npcController.firstEntrance = false;
        }

        while (true)
        {
            int xNode = Random.Range(0, worldXSize - 1);
            int yNode = Random.Range(0, worldYSize - 1);
            node = worldNode[xNode, yNode];

            if (npcController.didntFoundNode)
            {
                npcController.didntFoundNode = false;
                npcController.target = npcStartPosTransformForReturnRandomNode.position;

                break;
            }

           
            if (node.layerNumber == layerNumber || node.buildingType == buildingType)
            {
                npcController.StopDidntFoundNodeCalculateCoroutine();

                npcController.target = node.nodePosition;

                break;
            }
            

            yield return new WaitForFixedUpdate();
        }

        npcController.endOfSetTarget = true;
    }
}
