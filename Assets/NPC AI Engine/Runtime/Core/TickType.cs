namespace Aikom.AIEngine
{   
    /// <summary>
    /// Type of the tick used to measure time intervals in behaviour trees
    /// </summary>
    public enum TickType
    {   
        /// <summary>
        /// Same as Time.deltaTime
        /// </summary>
        DeltaTime,

        /// <summary>
        /// Same as Time.fixedDeltaTime
        /// </summary>
        FixedTime,

        /// <summary>
        /// User defined time interval between ticks
        /// </summary>
        UserDefined
    }

}
