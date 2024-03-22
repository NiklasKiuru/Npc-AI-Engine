using System;
using System.Collections.Generic;
using Aikom.AIEngine;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class EditorHashCollision
{
    [Test]
    public void CheckNativeTypeCollisions()
    {   
        var nodes = new List<NodeBase>();
        var types = TypeCache.GetTypesDerivedFrom<NodeBase>();
        foreach (var type in types)
        {
            if(!type.IsAbstract)
                nodes.Add((NodeBase)Activator.CreateInstance(type, 0));
        }

        var hashSet = new HashSet<ContextId>();  
        foreach(var node in nodes)
        {
            var id = ContextId.Generate(nodes, node);
            Debug.Log($"Context id for node: {node.GetType().Name} is {id}");
            Debug.Assert(!hashSet.Contains(id));
            hashSet.Add(id);
        }
    }

}
