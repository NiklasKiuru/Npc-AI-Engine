using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Aikom.AIEngine.Editor
{
    public class BTEdgeConnectorDropoutListener : IEdgeConnectorListener
    {   
        private TreeGraphView _graphView;

        public BTEdgeConnectorDropoutListener(TreeGraphView graphView)
        {
            _graphView = graphView;
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            //BTEdgeConnectorParentListener.Update(edge);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            _graphView.OnDropCreate(edge, position);
        }
    }

}
