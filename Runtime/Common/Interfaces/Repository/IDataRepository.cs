﻿using System;
using System.Collections.Generic;

namespace EmberToolkit.Common.Interfaces.Repository
{
    public interface IDataRepository
    {
        void AddRepository<T>(Dictionary<Guid, T> data);
        Dictionary<Guid, T>? GetRepositoryData<T>();
        void Load(string fileName);
        void RemoveRepository<T>();
        void Save(string fileName);
        void SaveObject<T>(T objData, string filePath);
        T LoadObject<T>(string filePath);
        List<T> LoadAllObjects<T>( string extension);
    }
}