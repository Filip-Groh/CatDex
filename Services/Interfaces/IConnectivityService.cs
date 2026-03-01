namespace CatDex.Services.Interfaces {
    public interface IConnectivityService {
        bool IsConnected { get; }
        event EventHandler<bool> ConnectivityChanged;
    }
}
