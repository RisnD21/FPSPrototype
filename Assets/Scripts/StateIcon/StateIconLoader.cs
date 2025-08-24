using UnityEngine.AddressableAssets; 
using System.Threading.Tasks;   
public static class StateIconLoader          
{
    static StateIconSet _set;                          
    public static bool IsReady => _set != null;        
    public static StateIconSet Set => _set;            

    public static async Task LoadAsync(string address = "StateIcons")  
    {
        if (_set != null) return;                     

        var handle = Addressables.LoadAssetAsync<StateIconSet>(address);

        _set = await handle.Task;
    }
}