using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breeze.Sharp.Core;
using ConcurrentCollections;

namespace Breeze.Sharp {

  public class UnattachedChildrenMap {

    public UnattachedChildrenMap() {
      _map = new ConcurrentDictionary<EntityKey, ConcurrentHashSet<NavChildren>>();
    }

    public class NavChildren {
      public NavigationProperty NavigationProperty;
      public ConcurrentHashSet<IEntity> Children;
    }

    public ConcurrentHashSet<NavChildren> GetNavChildrenList(EntityKey entityKey, bool createIfNotFound) {
            ConcurrentHashSet<NavChildren> navChildrenList = null;

      if (_map.TryGetValue(entityKey, out navChildrenList)) {
        return navChildrenList;
      } else {
        if (createIfNotFound) {
          navChildrenList = new ConcurrentHashSet<NavChildren>();
          _map.TryAdd(entityKey, navChildrenList);
        }
      }
      return navChildrenList;
    }

    public ConcurrentHashSet<IEntity> GetNavChildren(EntityKey entityKey, NavigationProperty navProp, bool createIfNotFound) {
            ConcurrentHashSet<NavChildren> navChildrenList = GetNavChildrenList(entityKey, createIfNotFound);
      if (navChildrenList == null) return null;
      
      var navChildren = navChildrenList.FirstOrDefault(uc => uc.NavigationProperty == navProp);
      if (navChildren == null && createIfNotFound) {
        navChildren = new NavChildren() {NavigationProperty = navProp, Children = new ConcurrentHashSet<IEntity>() };
        navChildrenList.Add(navChildren);
      }

      var children = navChildren.Children;
      children.Where(entity => entity.EntityAspect.EntityState.IsDetached()).ToList().ForEach(entity => children.TryRemove(entity));

      return children;
    }

    

    public void AddChild(EntityKey parentEntityKey, NavigationProperty navProp, IEntity child) {
      var navChildren = GetNavChildren(parentEntityKey, navProp, true);
      navChildren.Add(child);
    
    }

    public void RemoveChildren(EntityKey parentEntityKey, NavigationProperty navProp) {
      var navChildrenList = GetNavChildrenList(parentEntityKey, false);
      if (navChildrenList == null) return;
      var ix = navChildrenList.FirstOrDefault(nc => nc.NavigationProperty == navProp);
      if (ix != null) return;
      navChildrenList.TryRemove(ix);
      if (navChildrenList.Count == 0) {
                ConcurrentHashSet<NavChildren> res;
        _map.TryRemove(parentEntityKey, out res);
      }
    }

    private ConcurrentDictionary<EntityKey, ConcurrentHashSet<NavChildren>> _map;
  }
}
