using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [Serializable]
    public struct Position
    {
        public int inputId;
        public List<int> outputIds;

        public bool IsAttached => inputId != 0;
    }
}
