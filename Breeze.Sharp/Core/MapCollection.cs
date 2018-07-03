//using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Breeze.Sharp.Core {

  // similar to the .NET KeyedCollection class ( but different ...) 

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  public abstract class MapCollection<T,U> : ICollection<U> {

    public MapCollection() {
      
    }

    public MapCollection(IEnumerable<U> values) {
      values.ForEach(v => this.Add(v));
    }

    protected abstract T GetKeyForItem(U value);

    public virtual void Add(U value) {
      var key = GetKeyForItem(value);
      _map.TryAdd(key, value);
    }

    public ICollection<U> ReadOnlyValues {
      get { return _map.Values; }
    }

    public U this[T key] {
      get {
        U value;
        if (_map.TryGetValue(key, out value)) {
          return value;
        } else {
          return default(U);
        }
      }
      set {
        _map[key] = value;
      }
    }

    public virtual void Clear() {
      _map.Clear();
    }

    public bool Contains(U item) {
      return _map.ContainsKey(GetKeyForItem(item));
    }

    public bool ContainsKey(T key) {
      return _map.ContainsKey(key);
    }

    public void CopyTo(U[] array, int arrayIndex) {
      _map.Values.CopyTo(array, arrayIndex);
    }

    public int Count {
      get { return _map.Count; }
    }

    public bool IsReadOnly {
      get { return false; }
    }

    public virtual bool Remove(U item) {
     U itemResult;
      return _map.TryRemove(GetKeyForItem(item), out itemResult);
    }

    public virtual bool RemoveKey(T key) {

            U itemResult;
      return _map.TryRemove(key, out itemResult);
    }

    IEnumerator<U> IEnumerable<U>.GetEnumerator() {
      return _map.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return _map.Values.GetEnumerator();
    }

    private ConcurrentDictionary<T, U> _map = new ConcurrentDictionary<T, U>();

  }

}
