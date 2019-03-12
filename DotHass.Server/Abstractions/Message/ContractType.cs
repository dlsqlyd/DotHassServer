namespace DotHass.Server.Abstractions.Message
{
    /// <summary>
    /// 1-100为系统的协议id不可使用
    /// 内部协议不用签名
    /// 仅在channel中使用
    /// </summary>
    public enum ContractType
    {
        HeartBeat,
        HandShake,//握手协议,会向客户端发送一个clientid
    }
}