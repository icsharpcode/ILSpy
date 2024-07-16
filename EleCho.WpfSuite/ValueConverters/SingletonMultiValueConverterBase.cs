namespace EleCho.WpfSuite
{
    /// <summary>
    /// Base class of singleton multi value converter
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public abstract class SingletonMultiValueConverterBase<TSelf> : MultiValueConverterBase<TSelf>
        where TSelf : SingletonMultiValueConverterBase<TSelf>, new()
    {
        private static TSelf? _instance = null;

        /// <summary>
        /// Get an instance of <typeparamref name="TSelf"/>
        /// </summary>
        public static TSelf Instance => _instance ?? new();
    }
}
