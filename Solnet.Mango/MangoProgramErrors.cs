namespace Solnet.Mango
{
    /// <summary>
    /// The errors that <see cref="MangoProgram"/> can throw.
    /// </summary>
    public enum MangoProgramErrors
    {
        /// <summary>
        /// 
        /// </summary>
        InvalidCache,

        /// <summary>
        /// 
        /// </summary>
        InvalidOwner,

        /// <summary>
        /// 
        /// </summary>
        InvalidGroupOwner,

        /// <summary>
        /// 
        /// </summary>
        InvalidSignerKey,

        /// <summary>
        /// 
        /// </summary>
        InvalidAdminKey,

        /// <summary>
        /// 
        /// </summary>
        InvalidVault,

        /// <summary>
        /// 
        /// </summary>
        MathError,

        /// <summary>
        /// 
        /// </summary>
        InsufficientFunds,

        /// <summary>
        /// 
        /// </summary>
        InvalidToken,

        /// <summary>
        /// 
        /// </summary>
        InvalidMarket,

        /// <summary>
        /// 
        /// </summary>
        InvalidProgramId,

        /// <summary>
        /// 
        /// </summary>
        GroupNotRentExempt,

        /// <summary>
        /// 
        /// </summary>
        OutOfSpace,

        /// <summary>
        /// 
        /// </summary>
        TooManyOpenOrders,

        /// <summary>
        /// 
        /// </summary>
        AccountNotRentExempt,

        /// <summary>
        /// 
        /// </summary>
        ClientIdNotFound,

        /// <summary>
        /// 
        /// </summary>
        InvalidNodeBank,

        /// <summary>
        /// 
        /// </summary>
        InvalidRootBank,

        /// <summary>
        /// 
        /// </summary>
        MarginBasketFull,

        /// <summary>
        /// 
        /// </summary>
        NotLiquidatable,

        /// <summary>
        /// 
        /// </summary>
        Unimplemented,

        /// <summary>
        /// 
        /// </summary>
        PostOnly,

        /// <summary>
        /// 
        /// </summary>
        Bankrupt,

        /// <summary>
        /// 
        /// </summary>
        InsufficientHealth,

        /// <summary>
        /// 
        /// </summary>
        InvalidParam,

        /// <summary>
        /// 
        /// </summary>
        InvalidAccount,

        /// <summary>
        /// 
        /// </summary>
        SignerNecessary,

        /// <summary>
        /// 
        /// </summary>
        InsufficientLiquidity,

        /// <summary>
        /// 
        /// </summary>
        InvalidOrderId,

        /// <summary>
        /// 
        /// </summary>
        InvalidOpenOrdersAccount,

        /// <summary>
        /// 
        /// </summary>
        BeingLiquidated,

        /// <summary>
        /// 
        /// </summary>
        InvalidRootBankCache,

        /// <summary>
        /// 
        /// </summary>
        InvalidPriceCache,

        /// <summary>
        ///  Cache the perp market to resolve.
        /// </summary>
        InvalidPerpetualMarketCache
    }
}