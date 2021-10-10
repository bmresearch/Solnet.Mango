namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// Represents the node tags in a Mango Perpetual Market order book side.
    /// </summary>
    public enum NodeTag
    {
        /// <summary>
        /// The node is uninitialized.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// The node is an inner node.
        /// </summary>
        InnerNode,

        /// <summary>
        /// The node is a leaf node.
        /// </summary>
        LeafNode,

        /// <summary>
        /// The node is a free node.
        /// </summary>
        FreeNode,

        /// <summary>
        /// The node is the last free node.
        /// </summary>
        LastFreeNode,
    }
}