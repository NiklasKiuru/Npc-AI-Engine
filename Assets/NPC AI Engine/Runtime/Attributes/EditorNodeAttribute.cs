using System;
namespace Aikom.AIEngine
{   
    /// <summary>
    /// Attribute for nodes to display information in editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorNodeAttribute : Attribute
    {   
        /// <summary>
        /// Default description for the node
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Subcategory this node belongs to
        /// </summary>
        public string SubCategory { get; private set; }

        public EditorNodeAttribute(string defaultDescription = "", string subCategory = "")
        {
            Description = defaultDescription;
            SubCategory = subCategory;
        }
    }

}
