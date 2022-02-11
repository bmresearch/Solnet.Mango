namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents a response.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The data.
        /// </summary>
        public T Data { get; set; }
    }
}
