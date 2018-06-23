﻿using Breeze.Sharp.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Breeze.Sharp {

  /// <summary>
  /// Metadata information about a complex type.
  /// </summary>
  [DebuggerDisplay("{Name}")]
  public sealed class ComplexType: StructuralType, IJsonSerializable {

    public ComplexType(MetadataStore metadataStore) :base(metadataStore) {

    }

    public override void UpdateFromJNode(JNode jNode, bool isFromServer) {
      Name = this.MetadataStore.GetStructuralTypeNameFromJNode(jNode, isFromServer);
      // BaseTypeName = jnode.Get<String>("baseTypeName");
      // IsAbstract = jnode.Get<bool>("isAbstract");
      jNode.GetJNodeArray("dataProperties").ForEach(jn => {
        var dpName = GetPropertyNameFromJNode(jn);
        var dp = this.GetDataProperty(dpName);
        dp.UpdateFromJNode(jn, isFromServer);
      });
      // validators
      // custom
    }
    
    public override bool IsEntityType {
      get { return false; }
    }

    JNode IJsonSerializable.ToJNode(Object config) {
      var jo = new JNode();
      jo.AddPrimitive("shortName", this.ShortName);
      jo.AddPrimitive("namespace", this.Namespace);
      jo.AddPrimitive("isComplexType", true);
      // jo.AddProperty("baseTypeName", this.BaseTypeName);
      // jo.AddProperty("isAbstract", this.IsAbstract, false);
      jo.AddArray("dataProperties", this.DataProperties.Where(dp => dp.IsInherited == false));
      // jo.AddArrayProperty("validators", this.Validators);
      // jo.AddProperty("custom", this.Custom.ToJObject)
      return jo;
    }

  
   
  }

  /// <summary>
  /// For public use only.
  /// </summary>
  public class ComplexTypeCollection : KeyedCollection<String, ComplexType> {
    protected override String GetKeyForItem(ComplexType item) {
      return item.ShortName + ":#" + item.Namespace;
    }
  }

}
