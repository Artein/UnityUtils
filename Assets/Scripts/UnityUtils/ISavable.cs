namespace UnityUtils
{
    public interface ISavable
    {
        void Save(string saveKey);
        void Load(string saveKey);
        void DeleteSave(string saveKey);
    }
}