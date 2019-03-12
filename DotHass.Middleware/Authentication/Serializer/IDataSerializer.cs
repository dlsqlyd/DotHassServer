namespace DotHass.Middleware.Authentication.Serializer
{
    public interface IDataSerializer<TModel>
    {
        byte[] Serialize(TModel model);

        TModel Deserialize(byte[] data);
    }
}