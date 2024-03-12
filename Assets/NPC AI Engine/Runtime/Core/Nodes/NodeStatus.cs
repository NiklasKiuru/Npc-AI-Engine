using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public enum NodeStatus
    {
        /// <summary>
        /// The status is yet to be determined
        /// </summary>
        Undefined,

        /// <summary>
        /// The execution of this node is still yet to be determined
        /// </summary>
        Running,

        /// <summary>
        /// The execution succeeded
        /// </summary>
        Succes,

        /// <summary>
        /// The execution failed
        /// </summary>
        Failure,

        /// <summary>
        /// A child process somewhere along the path recieved a running status and cached itself
        /// This prevents the parents of this child from caching
        /// </summary>
        Cached,
    }


    public static class StatusExtensions
    {
        public static NodeStatus Invert(this NodeStatus status)
        {
            if (status == NodeStatus.Failure)
                return NodeStatus.Succes;
            else if (status == NodeStatus.Succes)
                return NodeStatus.Failure;
            return status;
        }
    }
}
