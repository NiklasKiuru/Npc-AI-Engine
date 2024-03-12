using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class AgentAttribute : Attribute
    {
        public Type Type;

        public AgentAttribute(Type type)
        {
            Type = type;
        }
    }
}

