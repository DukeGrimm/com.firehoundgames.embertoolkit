using EmberToolkit.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace EmberToolkit.Common.Struct.Collections
{
    [Serializable]
    public struct CountedCollection<T> where T : IEmberObject
    {
        public List<CountedElement<T>> Collection;

        public int Count => Collection?.Count ?? 0;

        public void AddCountToElement(T element, int count)
        {
            if (Collection == null)
            {
                Collection = new List<CountedElement<T>>();
            }
            if (Collection.Where(x => x.element.Id == element.Id).Any())
            {
                Collection.Where(x => x.element.Id == element.Id).FirstOrDefault().AddToCount(count);
            }
            else
            {
                Collection.Add(new CountedElement<T> { element = element, elementCount = count });
            }
        }
        public bool Contains(T element)
        {
            return Collection?.Exists(listElement => listElement.element.Id == element.Id) ?? false;
        }
        public int GetCountOfElement(T element)
        {
            CountedElement<T> pairFound = Collection.Where(x => x.element.Id == element.Id).FirstOrDefault();
            if (pairFound.element == null) return -1;
            return pairFound.elementCount;
        }
    }
    [Serializable]
    public struct CountedElement<T> where T : IEmberObject
    {
        public T element;
        public int elementCount;

        public void AddToCount(int input) => elementCount += input;
    }
}
