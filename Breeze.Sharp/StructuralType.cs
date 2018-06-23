using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using System.Reflection;
using Breeze.Sharp.Core;
using System.Diagnostics;

namespace Breeze.Sharp {

  /// <summary>
  /// For public use only.
  /// </summary>
  public class StructuralTypeCollection : MapCollection<String, StructuralType> {
    protected override String GetKeyForItem(StructuralType item) {
      return item.ShortName + ":#" + item.Namespace;
    }
  }

  /// <summary>
  /// Base class for both <see cref="EntityType"/> and <see cref="ComplexType"/> classes.
  /// </summary>
  [DebuggerDisplay("{Name}")]
  public abstract class StructuralType {
    public StructuralType(MetadataStore metadataStore) {
      Warnings = new List<string>();
      MetadataStore = metadataStore;
    }

    // TODO: will be needed later when we have complexType inheritance 
    // public abstract StructuralType BaseStructuralType { get; }

    public MetadataStore MetadataStore { get; private set; }

    public String Name { 
      get { return TypeNameInfo.ToStructuralTypeName(ShortName, Namespace); }
      set {
        var parts = TypeNameInfo.FromStructuralTypeName(value);
        ShortName = parts.ShortName;
        Namespace = parts.Namespace;
        _nameOnServer = parts.ToServer(MetadataStore).StructuralTypeName;
      }
    }

    public String NameOnServer {
      get {
        return _nameOnServer;
      }
      set {
        _nameOnServer = value;
        Name = TypeNameInfo.FromStructuralTypeName(value).ToClient(MetadataStore).StructuralTypeName;
      }
    }

    public Type ClrType {
      get { return _clrType; }
      set {
        _clrType = value;
        Name = TypeNameInfo.FromClrTypeName(_clrType.FullName).StructuralTypeName;
      }  
    }

    public abstract void UpdateFromJNode(JNode jNode, bool isFromServer);


    public String GetPropertyNameFromJNode(JNode jn) {
      var dpName = jn.Get<String>("name");
      if (dpName == null) {
        var dpNameOnServer = jn.Get<String>("nameOnServer");
        dpName = MetadataStore.NamingConvention.ServerPropertyNameToClient(dpNameOnServer, this);
      }
      return dpName;
    }

    private Type _clrType;
    public String ShortName { get; private set; }
    public String Namespace { get; private set;}
    public dynamic Custom { get; set; }
    public bool IsAbstract { get; set; }
    // TODO: determine if this is  still needed;
    public bool IsAnonymous { get; set; }
    public List<String> Warnings { get; set; }
    public abstract bool IsEntityType { get;  }
    
    public virtual  IEnumerable<StructuralProperty> Properties {
      get { return _dataProperties.Cast<StructuralProperty>(); }
    }

    public ICollection<DataProperty> DataProperties {
      get { return _dataProperties.ReadOnlyValues; }
    }

    public DataProperty GetDataProperty(String dpName) {
      return _dataProperties[dpName];
    }

    public virtual StructuralProperty GetProperty(String propName) {
      return _dataProperties[propName];
    }

    public virtual DataProperty AddDataProperty(DataProperty dp) {
      dp.ParentType = this;
      _dataProperties.Add(dp);
      dp.UpdateClientServerNames();
      return dp;
    } 

    public ReadOnlyCollection<DataProperty> ComplexProperties {
      get { return _complexProperties.ReadOnlyValues; }
    }

    public ReadOnlyCollection<DataProperty> UnmappedProperties {
      get { return _unmappedProperties.ReadOnlyValues;  }
    }

    public ICollection<Validator> Validators {
      get { return _validators; }
    }

    public void UpdateComplexProperties(DataProperty dp) {
      UpdateCollection( _complexProperties, dp, dp.IsComplexProperty);
    }

    public void UpdateUnmappedProperties(DataProperty dp) {
      UpdateCollection( _unmappedProperties, dp, dp.IsUnmapped);
    }

    public String FormatDpName(String propertyName) {
      var typeLabel = this.IsEntityType ? "EntityType" : "ComplexType";
      return String.Format("Data Property: '{0}' on the {1}: '{2}'", propertyName, typeLabel, this.Name);
    }

    public String FormatNpName(String propertyName) {
      return String.Format("Navigation Property: '{0}' on the EntityType: '{1}'", propertyName, this.Name);
    }
    
    protected void UpdateCollection(SafeList<DataProperty> list, DataProperty dp, bool add) {
      var isSet = list.Contains(dp);
      if (add) {
        if (!isSet) {
          list.Add(dp);
        }
      } else {
        if (isSet) {
          list.Remove(dp);
        }
      }
    }

    private String _nameOnServer;
    public readonly DataPropertyCollection _dataProperties = new DataPropertyCollection();
    protected readonly SafeList<DataProperty> _complexProperties = new SafeList<DataProperty>();
    protected readonly SafeList<DataProperty> _unmappedProperties = new SafeList<DataProperty>();
    public  ValidatorCollection _validators = new ValidatorCollection();

  }

  


}
