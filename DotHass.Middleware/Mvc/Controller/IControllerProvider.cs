namespace DotHass.Middleware.Mvc.Controller
{
    public interface IControllerProvider
    {
        void Initialize();

        ControllerDescriptor GetControllerDescriptor(int contractID);
    }
}