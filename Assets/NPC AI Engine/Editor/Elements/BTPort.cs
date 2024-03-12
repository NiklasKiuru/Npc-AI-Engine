using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// https://forum.unity.com/threads/callback-on-edge-connection-in-graphview.796290/ solution mainly from David

namespace Aikom.AIEngine.Editor
{
    public class BTPort : Port
    {
        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);
                    }
                }
                if (edge.output.capacity == Port.Capacity.Single)
                {
                    foreach (Edge connection in edge.output.connections)
                    {
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);
                    }
                }
                if (m_EdgesToDelete.Count > 0)
                    graphView.DeleteElements(m_EdgesToDelete);
                List<Edge> edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                foreach (Edge edge1 in edgesToCreate)
                {
                    graphView.AddElement((GraphElement)edge1);
                    edge.input.Connect(edge1);
                    edge.output.Connect(edge1);
                }
            }
        }

        public Action<Port> OnConnect;
        public Action<Port> OnDisconnect;

        private BTNode _parent => node as BTNode;

        protected BTPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            _parent?.UpdateValidityByConnections();
            OnDisconnect?.Invoke(this);
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            _parent?.UpdateValidityByConnections();
            OnConnect?.Invoke(this);
        }

        public override void DisconnectAll()
        {
            base.DisconnectAll();
            _parent?.UpdateValidityByConnections();
            OnDisconnect?.Invoke(this);
        }

        public new static BTPort Create<TEdge>(Orientation orientation, Direction direction, Capacity capacity, Type type)
            where TEdge : Edge, new()
        {
            var listener = new DefaultEdgeConnectorListener();
            var ele = new BTPort(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            ele.AddManipulator(ele.m_EdgeConnector);
            return ele;
        }
    }

}
