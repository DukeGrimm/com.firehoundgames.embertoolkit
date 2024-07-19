using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Common.Interfaces.Data
{
    public interface ISaveLoadController
    {
        void Save(string fileName);
        void Load(string fileName);
        void SaveObject<T>(T objData, string filePath);
        T LoadObject<T>(string filePath);
        List<T> LoadAllObjects<T>(string extension);
    }
}
