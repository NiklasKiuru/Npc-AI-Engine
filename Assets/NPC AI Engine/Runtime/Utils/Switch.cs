namespace Aikom.AIEngine.Utils
{   
    /// <summary>
    /// Toggle switch to toggle between two data structures
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Switch<T> 
    {
        private int _activeIndex;

        public T Option1 { get; private set; }
        public T Option2 { get; private set; }

        private T this[int index]
        {
            get 
            {
                return index switch
                {
                    0 => Option1,
                    1 => Option2,
                    _ => Option1,
                };
            }
        }

        public Switch(T value1, T value2)
        {
            Option1 = value1;
            Option2 = value2;
        }

        /// <summary>
        /// Flips between the active and deactive objects
        /// </summary>
        public void Flip() => _activeIndex = GetFlipIndex(_activeIndex);

        /// <summary>
        /// Gets the active object
        /// </summary>
        /// <returns></returns>
        public T GetActive() => this[_activeIndex];

        /// <summary>
        /// Gets the inactive object
        /// </summary>
        /// <returns></returns>
        public T GetInActive() => this[GetFlipIndex(_activeIndex)];    
        private int GetFlipIndex(int index) => index == 1 ? 0 : 1;
    }
}

