namespace DotHass.Repository.Collection
{
    /// <summary>
    /// 加载状态
    /// </summary>
    public enum LoadingStatus
    {
        /// <summary>
        /// 未同步库的状态，包括新增的
        /// </summary>
        None = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success,

        /// <summary>
        /// 加载出错
        /// </summary>
        Error
    }
}