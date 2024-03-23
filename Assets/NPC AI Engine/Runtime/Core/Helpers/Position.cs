using System;
using System.Collections.Generic;

namespace Aikom.AIEngine
{   
    /// <summary>
    /// Position data for a node
    /// </summary>
    [Serializable]
    public struct Position
    {   
        /// <summary>
        /// The parent id of this node
        /// </summary>
        public int inputId;

        /// <summary>
        /// The child id's of the children of this node
        /// </summary>
        public List<int> outputIds;

        /// <summary>
        /// Is this node connected to a parent?
        /// </summary>
        public bool IsAttached => inputId != 0;
    }
}
